"use client";

import React, { useMemo, useState } from "react";
import styles from "@/assets/styles/ExamCreation.module.scss";
import {
  Card,
  Form,
  Input,
  Select,
  InputNumber,
  DatePicker,
  Button,
  List,
  Tag,
  Radio,
  message,
} from "antd";
import { Plus, Save, Eye, Radio as RadioIcon } from "lucide-react";
import dayjs from "dayjs";
import { mockClasses, mockQuestions } from "@/data/seed";

export default function ExamCreationPage() {
  const [form] = Form.useForm();
  const [selected, setSelected] = useState<string[]>([]);
  const [showCreate, setShowCreate] = useState(false);
  const [qForm] = Form.useForm();

  const questions = mockQuestions;
  const chosen = useMemo(
    () => questions.filter((q) => selected.includes(q.id)),
    [questions, selected]
  );
  const totalPoints = chosen.reduce((s, q) => s + q.points, 0);

  const togglePick = (id: string) => {
    setSelected((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  };

  const onSaveExam = async () => {
    try {
      const v = await form.validateFields();
      if (!v.startTime) return message.error("Chọn thời gian bắt đầu");
      message.success("Đã tạo bài kiểm tra (mock)");
      form.resetFields();
      setSelected([]);
    } catch {}
  };

  const onSaveQuestion = async () => {
    const v = await qForm.validateFields();
    if (
      v.type === "multiple-choice" &&
      (!v.options || v.options.some((t: string) => !t))
    ) {
      return message.error("Nhập đầy đủ đáp án");
    }
    message.success("Đã lưu câu hỏi vào ngân hàng (mock)");
    qForm.resetFields();
    setShowCreate(false);
  };

  const examTypeMap = {
    "multiple-choice": "Trắc nghiệm",
    essay: "Tự luận",
    mixed: "Hỗn hợp",
  };
  const displayedValue = examTypeMap[form.getFieldValue("type") ?? "mixed"];

  return (
    <div className={styles.wrap}>
      <div>
        <h1>Tạo bài kiểm tra</h1>
        <p className={styles.muted}>Tạo bài kiểm tra mới cho lớp học</p>
      </div>

      <div className={styles.grid}>
        {/* Left */}
        <div className={styles.left}>
          <Card className={styles.card} title="Thông tin bài kiểm tra">
            <Form
              form={form}
              layout="vertical"
              initialValues={{
                classId: mockClasses[0]?.id,
                type: "mixed",
                duration: 60,
              }}
            >
              <Form.Item
                name="title"
                label="Tiêu đề"
                rules={[{ required: true }]}
              >
                <Input placeholder="Ví dụ: Kiểm tra giữa kỳ - Đại số" />
              </Form.Item>
              <div className={styles.row2}>
                <Form.Item name="classId" label="Lớp">
                  <Select
                    options={mockClasses.map((c) => ({
                      value: c.id,
                      label: c.name,
                    }))}
                  />
                </Form.Item>
                <Form.Item name="type" label="Loại">
                  <Select
                    options={[
                      { value: "multiple-choice", label: "Trắc nghiệm" },
                      { value: "essay", label: "Tự luận" },
                      { value: "mixed", label: "Hỗn hợp" },
                    ]}
                  />
                </Form.Item>
              </div>
              <div className={styles.row2}>
                <Form.Item name="duration" label="Thời gian (phút)">
                  <InputNumber min={5} style={{ width: "100%" }} />
                </Form.Item>
                <Form.Item name="startTime" label="Thời gian bắt đầu">
                  <DatePicker
                    showTime
                    style={{ width: "100%" }}
                    disabledDate={(d) =>
                      d && d.isBefore(dayjs().startOf("day"))
                    }
                  />
                </Form.Item>
              </div>
            </Form>
          </Card>

          <Card
            className={styles.card}
            title="Chọn câu hỏi từ ngân hàng"
            extra={
              <Button
                icon={<Plus size={16} />}
                onClick={() => setShowCreate((v) => !v)}
              >
                {showCreate ? "Đóng" : "Tạo câu hỏi mới"}
              </Button>
            }
          >
            {showCreate && (
              <Card className={styles.subCard} title="Tạo câu hỏi mới">
                <Form
                  form={qForm}
                  layout="vertical"
                  initialValues={{
                    type: "multiple-choice",
                    points: 5,
                    options: ["", "", "", ""],
                    correctAnswer: 0,
                  }}
                >
                  <Form.Item name="type" label="Loại câu hỏi">
                    <Radio.Group>
                      <Radio value="multiple-choice">Trắc nghiệm</Radio>
                      <Radio value="essay">Tự luận</Radio>
                      <Radio value="fill-blank">Điền khuyết</Radio>
                    </Radio.Group>
                  </Form.Item>
                  <Form.Item
                    name="content"
                    label="Nội dung"
                    rules={[{ required: true }]}
                  >
                    <Input.TextArea rows={3} />
                  </Form.Item>
                  <Form.Item shouldUpdate noStyle>
                    {() =>
                      qForm.getFieldValue("type") === "multiple-choice" && (
                        <div className={styles.mcWrap}>
                          {["A", "B", "C", "D"].map((l, idx) => (
                            <div key={idx} className={styles.mcRow}>
                              <Radio
                                checked={
                                  qForm.getFieldValue("correctAnswer") === idx
                                }
                                onChange={() =>
                                  qForm.setFieldValue("correctAnswer", idx)
                                }
                              />
                              <span className={styles.mcLabel}>{l}.</span>
                              <Form.Item name={["options", idx]} noStyle>
                                <Input placeholder={`Đáp án ${l}`} />
                              </Form.Item>
                            </div>
                          ))}
                        </div>
                      )
                    }
                  </Form.Item>
                  <Form.Item name="points" label="Điểm số">
                    <InputNumber min={1} />
                  </Form.Item>
                  <Button type="primary" onClick={onSaveQuestion}>
                    Lưu câu hỏi
                  </Button>
                </Form>
              </Card>
            )}

            <List
              dataSource={questions}
              renderItem={(q) => (
                <List.Item
                  className={`${styles.qItem} ${
                    selected.includes(q.id) ? styles.qPicked : ""
                  }`}
                  onClick={() => togglePick(q.id)}
                >
                  <div className={styles.qMain}>
                    <div className={styles.qContent}>{q.content}</div>
                    <div className={styles.qTags}>
                      <Tag>
                        {q.type === "multiple-choice"
                          ? "Trắc nghiệm"
                          : q.type === "essay"
                          ? "Tự luận"
                          : "Điền khuyết"}
                      </Tag>
                      <Tag color="blue">{q.points} điểm</Tag>
                    </div>
                  </div>
                  {selected.includes(q.id) && (
                    <Tag color="processing">Đã chọn</Tag>
                  )}
                </List.Item>
              )}
            />
          </Card>
        </div>

        {/* Right */}
        <div className={styles.right}>
          <Card className={styles.card} title="Tổng quan">
            <div className={styles.kv}>
              <span>Số câu hỏi:</span>
              <b>{selected.length}</b>
            </div>
            <div className={styles.kv}>
              <span>Tổng điểm:</span>
              <b>{totalPoints}</b>
            </div>
            <Form form={form} component={false}>
              <div className={styles.kv}>
                <span>Thời gian:</span>
                <b>{form.getFieldValue("duration") ?? 60} phút</b>
              </div>
              <div className={styles.kv}>
                <span>Loại:</span>
                <b>{displayedValue}</b>
              </div>
            </Form>
            <div className={styles.actionsCol}>
              <Button
                type="primary"
                icon={<Save size={16} />}
                onClick={onSaveExam}
              >
                Lưu bài kiểm tra
              </Button>
              <Button icon={<Eye size={16} />}>Xem trước</Button>
            </div>
          </Card>

          {selected.length > 0 && (
            <Card className={styles.card} title="Câu hỏi đã chọn">
              <List
                dataSource={chosen}
                renderItem={(q, idx) => (
                  <List.Item className={styles.selRow}>
                    <span>Câu {idx + 1}</span>
                    <Tag color="blue">{q.points} điểm</Tag>
                  </List.Item>
                )}
              />
            </Card>
          )}
        </div>
      </div>
    </div>
  );
}
