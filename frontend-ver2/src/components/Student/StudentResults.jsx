// src/components/Student/StudentResults.jsx
import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Card,
  Typography,
  Tag,
  Progress,
  Collapse,
  Spin,
  message,
  Empty,
  Select,
  Input,
} from "antd";
import { CheckCircle, XCircle, Clock, FileText } from "lucide-react";
import styles from "../../assets/styles/StudentResults.module.scss";
import {
  callStudentGetExamHistoryAPI,
  callListMyClassesAPI,
} from "../../services/api.service";

const { Title, Text } = Typography;
const { Panel } = Collapse;

function formatDateTime(dt) {
  const d = new Date(dt);
  if (Number.isNaN(d.getTime())) return "-";
  return d.toLocaleString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function calculateScore10(totalScore, totalMarks) {
  if (!totalMarks) return Number(totalScore || 0).toFixed(1);
  return ((totalScore / totalMarks) * 10).toFixed(1);
}

function calculateAccuracy(correctAnswers, totalQuestions) {
  if (!totalQuestions) return 0;
  return Math.round((correctAnswers / totalQuestions) * 100);
}

function getStatusLabel(status) {
  const statusMap = {
    Graded: "Đã chấm điểm",
    Pending: "Chờ chấm điểm",
    InProgress: "Đang làm",
    Submitted: "Đã nộp",
    Draft: "Bản nháp",
  };
  return statusMap[status] || status;
}

export function StudentResults() {
  const [results, setResults] = useState([]);
  const [classes, setClasses] = useState([]);
  const [selectedClassId, setSelectedClassId] = useState(null);
  const [searchText, setSearchText] = useState("");
  const [loadingClasses, setLoadingClasses] = useState(false);
  const [loadingHistory, setLoadingHistory] = useState(false);

  const fetchHistory = useCallback(
    async (classIdValue, providedClasses) => {
      setLoadingHistory(true);
      try {
        const sourceClasses = providedClasses ?? classes;
        const availableClassIds = sourceClasses
          .map((cls) => Number(cls?.classId ?? cls?.id))
          .filter((id) => Number.isFinite(id));

        if (!availableClassIds.length) {
          setResults([]);
          return;
        }

        let targetClassIds = [];

        if (classIdValue === "all") {
          targetClassIds = availableClassIds;
        } else {
          const classId = Number(classIdValue);
          if (Number.isFinite(classId)) {
            targetClassIds = [classId];
          } else {
            setResults([]);
            return;
          }
        }

        // Tạo map classId -> className để map vào history data
        const classInfoMap = new Map();
        sourceClasses.forEach((cls) => {
          const id = Number(cls?.classId ?? cls?.id);
          if (Number.isFinite(id)) {
            classInfoMap.set(id, {
              classId: id,
              className:
                cls?.className ??
                cls?.name ??
                cls?.title ??
                cls?.code ??
                `Lớp ${id}`,
            });
          }
        });

        const settled = await Promise.allSettled(
          targetClassIds.map((id) => callStudentGetExamHistoryAPI(id))
        );

        const merged = [];
        let hadError = false;

        settled.forEach((item, idx) => {
          if (item.status === "fulfilled") {
            const historyData =
              item.value?.data?.data || item.value?.data || [];
            if (Array.isArray(historyData)) {
              // Map thêm className và classId vào mỗi item
              const classId = targetClassIds[idx];
              const classInfo = classInfoMap.get(classId);
              const mappedData = historyData.map((result) => ({
                ...result,
                classId: classId,
                className: classInfo?.className || `Lớp ${classId}`,
              }));
              merged.push(...mappedData);
            }
          } else {
            hadError = true;
            console.error(
              `fetchHistory error for class ${targetClassIds[idx]}`,
              item.reason
            );
          }
        });

        setResults(merged);

        if (hadError && merged.length === 0) {
          message.error("Không thể tải lịch sử bài kiểm tra");
        } else if (hadError) {
          message.warning(
            "Một số lớp chưa tải được lịch sử. Vui lòng thử lại sau."
          );
        }
      } catch (err) {
        console.error("fetchHistory error:", err);
        message.error("Không thể tải lịch sử bài kiểm tra");
        setResults([]);
      } finally {
        setLoadingHistory(false);
      }
    },
    [classes]
  );

  useEffect(() => {
    const fetchClasses = async () => {
      setLoadingClasses(true);
      try {
        const classesRes = await callListMyClassesAPI();
        setClasses(classesRes.data);

        if (!classesRes.data.length) {
          setSelectedClassId(null);
          setResults([]);
          return;
        }

        const defaultSelection =
          classesRes.data.length > 1
            ? "all"
            : String(
                classesRes.data[0]?.classId ?? classesRes.data[0]?.id ?? ""
              );

        setSelectedClassId(defaultSelection);
        await fetchHistory(defaultSelection, classesRes.data);
      } catch (e) {
        console.warn("Error fetching classes:", e);
        message.error("Không thể tải danh sách lớp");
        setClasses([]);
        setResults([]);
      } finally {
        setLoadingClasses(false);
      }
    };

    fetchClasses();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleClassChange = (value) => {
    setSelectedClassId(value);
    fetchHistory(value);
  };

  const loading = loadingClasses || loadingHistory;

  const filteredResults = useMemo(() => {
    const keyword = searchText.trim().toLowerCase();
    const parsedClassId = Number(selectedClassId);
    const normalizedClassId =
      !selectedClassId ||
      selectedClassId === "all" ||
      Number.isNaN(parsedClassId)
        ? null
        : parsedClassId;

    return results.filter((result) => {
      if (normalizedClassId !== null) {
        const resultClassId = Number(
          result?.classId ??
            result?.class?.classId ??
            result?.class?.id ??
            result?.class?.ClassId
        );
        if (resultClassId !== normalizedClassId) {
          return false;
        }
      }

      if (!keyword) return true;

      // Sử dụng className đã được map vào result
      const classLabel = result?.className ?? "";

      const fields = [
        result?.examTitle,
        getStatusLabel(result?.status),
        classLabel,
        result?.studentName,
      ].filter(Boolean);

      return fields.some((field) =>
        field.toString().toLowerCase().includes(keyword)
      );
    });
  }, [results, selectedClassId, searchText]);

  const classOptions = useMemo(() => {
    if (!classes?.length) return [];

    const dynamic =
      classes
        ?.map((cls) => {
          const raw = cls?.classId ?? cls?.id;
          if (raw === undefined || raw === null) return null;
          return {
            value: String(raw),
            label:
              cls?.className ??
              cls?.name ??
              cls?.title ??
              cls?.code ??
              "Không tên",
          };
        })
        .filter(Boolean) ?? [];

    return [{ value: "all", label: "Tất cả lớp" }, ...dynamic];
  }, [classes]);

  const getClassLabel = (result) => {
    // Sử dụng className đã được map vào result
    return result?.className ?? "Không rõ lớp";
  };

  return (
    <div className={styles.wrap}>
      {/* Header */}
      <div className={styles.header}>
        <Title level={2} className={styles.title}>
          <FileText size={24} style={{ display: "inline", marginRight: 8 }} />
          Kết quả bài kiểm tra
        </Title>
        <Text type="secondary">
          Xem điểm số và đáp án chi tiết cho các bài kiểm tra bạn đã làm
        </Text>
      </div>

      {/* Filters */}
      <div className={styles.filters}>
        <div className={styles.filterControl}>
          <label className={styles.filterLabel}>Lọc theo lớp</label>
          <Select
            value={selectedClassId}
            onChange={handleClassChange}
            options={classOptions}
            placeholder="Chọn lớp"
            disabled={loading}
            style={{ width: "100%" }}
          />
        </div>

        <div className={styles.filterControl}>
          <label className={styles.filterLabel}>Tìm kiếm</label>
          <Input
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            placeholder="Tìm theo tên bài kiểm tra, lớp, trạng thái..."
          />
        </div>
      </div>

      {/* KPIs Summary */}
      {filteredResults.length > 0 && (
        <div className={styles.kpis}>
          <Card className={styles.kpiCard}>
            <Text type="secondary">Tổng số bài kiểm tra</Text>
            <div className={styles.kpiValue}>{filteredResults.length}</div>
            <Text className={styles.inlineMuted}>
              <FileText size={14} />
              Đã hoàn thành
            </Text>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Điểm trung bình</Text>
            <div className={styles.kpiValue}>
              {filteredResults.length > 0
                ? (
                    filteredResults.reduce((acc, r) => {
                      return (
                        acc +
                        parseFloat(calculateScore10(r.totalScore, r.totalMarks))
                      );
                    }, 0) / filteredResults.length
                  ).toFixed(1)
                : "0.0"}
              /10
            </div>
            <Text className={styles.inlineMuted}>
              <CheckCircle size={14} />
              Thang điểm 10
            </Text>
          </Card>

          <Card className={styles.kpiCard}>
            <Text type="secondary">Tỷ lệ chính xác</Text>
            <div className={styles.kpiValue}>
              {filteredResults.length > 0
                ? Math.round(
                    filteredResults.reduce((acc, r) => {
                      return (
                        acc +
                        calculateAccuracy(r.correctAnswers, r.totalQuestions)
                      );
                    }, 0) / filteredResults.length
                  )
                : 0}
              %
            </div>
            <Text className={styles.inlineMuted}>
              <Clock size={14} />
              Trung bình
            </Text>
          </Card>
        </div>
      )}

      {/* Results List */}
      <Spin spinning={loading}>
        {filteredResults.length === 0 ? (
          <Empty description="Không có kết quả nào" />
        ) : (
          <div className={styles.list}>
            {filteredResults.map((result) => {
              const score10 = calculateScore10(
                result.totalScore,
                result.totalMarks
              );
              const accuracyPercent = calculateAccuracy(
                result.correctAnswers,
                result.totalQuestions
              );
              const percentage = result.totalMarks
                ? Math.round((result.totalScore / result.totalMarks) * 100)
                : 0;

              return (
                <Card
                  key={result.examSubmissionId || result.id}
                  className={styles.item}
                >
                  {/* Header */}
                  <div className={styles.itemHeader}>
                    <div className={styles.itemMeta}>
                      <Title level={4} className={styles.itemTitle}>
                        {result.examTitle || "Bài kiểm tra"}
                      </Title>
                      <div className={styles.itemDesc}>
                        <Tag color="blue">{getStatusLabel(result.status)}</Tag>
                        <Tag color="green">{getClassLabel(result)}</Tag>
                      </div>
                      <div className={styles.time}>
                        Bắt đầu: {formatDateTime(result.startTime)} • Nộp lúc{" "}
                        {formatDateTime(result.submitTime)}
                      </div>
                    </div>

                    <div className={styles.scoreBox}>
                      <div className={styles.score}>
                        {result.totalScore}
                        <span className={styles.scoreDenom}>
                          /{result.totalMarks}
                        </span>
                      </div>
                      <div className={styles.sub}>
                        {result.correctAnswers}/{result.totalQuestions} câu đúng
                      </div>
                      <Progress
                        type="circle"
                        percent={percentage}
                        size={80}
                        strokeWidth={8}
                        strokeColor={{
                          "0%": "#108ee9",
                          "100%": "#87d068",
                        }}
                        style={{ marginTop: 8 }}
                      />
                    </div>
                  </div>

                  {/* KPIs Row */}
                  <div
                    style={{
                      marginTop: 16,
                      display: "grid",
                      gridTemplateColumns: "1fr 1fr 1fr",
                      gap: 12,
                    }}
                  >
                    <div>
                      <Text
                        type="secondary"
                        style={{ fontSize: 14, fontWeight: 500 }}
                      >
                        Điểm quy đổi (thang 10)
                      </Text>
                      <div
                        style={{ fontSize: 24, fontWeight: 700, marginTop: 4 }}
                      >
                        {score10}/10
                      </div>
                    </div>
                    <div>
                      <Text
                        type="secondary"
                        style={{ fontSize: 14, fontWeight: 500 }}
                      >
                        Tỷ lệ chính xác
                      </Text>
                      <div
                        style={{ fontSize: 24, fontWeight: 700, marginTop: 4 }}
                      >
                        {accuracyPercent}%
                      </div>
                    </div>
                    <div>
                      <Text
                        type="secondary"
                        style={{ fontSize: 14, fontWeight: 500 }}
                      >
                        Điểm số
                      </Text>
                      <div style={{ marginTop: 8 }}>
                        <Progress
                          percent={percentage}
                          status={percentage >= 50 ? "success" : "exception"}
                          strokeWidth={8}
                        />
                      </div>
                    </div>
                  </div>

                  {/* Details */}
                  {Array.isArray(result.answers) &&
                    result.answers.length > 0 && (
                      <Collapse
                        className={styles.details}
                        ghost
                        expandIconPosition="end"
                      >
                        <Panel header="Xem chi tiết câu hỏi" key="1">
                          <div className={styles.detailList}>
                            {result.answers.map((ans, idx) => (
                              <div
                                key={ans.questionId || idx}
                                className={`${styles.detailItem} ${
                                  ans.isCorrect
                                    ? styles.correct
                                    : styles.incorrect
                                }`}
                              >
                                <div className={styles.detailIcon}>
                                  {ans.isCorrect ? (
                                    <CheckCircle size={20} />
                                  ) : (
                                    <XCircle size={20} />
                                  )}
                                </div>

                                <div className={styles.detailContent}>
                                  <div className={styles.question}>
                                    Câu {idx + 1}: {ans.questionText}
                                  </div>

                                  <div className={styles.answers}>
                                    <div className={styles.answerRow}>
                                      <span className={styles.muted}>
                                        Câu trả lời:{" "}
                                      </span>
                                      <span
                                        className={
                                          ans.isCorrect
                                            ? styles.ansOk
                                            : styles.ansBad
                                        }
                                      >
                                        {ans.selectedOptionText || "—"}
                                      </span>
                                    </div>

                                    {!ans.isCorrect && (
                                      <div className={styles.answerRow}>
                                        <span className={styles.muted}>
                                          Đáp án đúng:{" "}
                                        </span>
                                        <span className={styles.ansOk}>
                                          {ans.correctOptionText || "—"}
                                        </span>
                                      </div>
                                    )}

                                    <div className={styles.answerRow}>
                                      <span className={styles.muted}>
                                        Điểm:{" "}
                                      </span>
                                      <strong>
                                        {ans.marksObtained}/{ans.totalMarks}
                                      </strong>
                                    </div>
                                  </div>
                                </div>
                              </div>
                            ))}
                          </div>
                        </Panel>
                      </Collapse>
                    )}
                </Card>
              );
            })}
          </div>
        )}
      </Spin>
    </div>
  );
}
