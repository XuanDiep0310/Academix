"use client";

import React, { useMemo, useState } from "react";
import styles from "@/assets/styles/QuestionBank.module.scss";
import {
  Card,
  Input,
  Select,
  Tabs,
  List,
  Tag,
  Button,
  Modal,
  Form,
  Radio,
  message,
} from "antd";
import { Plus, Edit, Trash2 } from "lucide-react";
import { mockQuestions } from "@/data/seed";

const QuestionBankPage = () => {
  const [q, setQ] = useState("");
  const [cat, setCat] = useState<"all" | string>("all");
  const [open, setOpen] = useState(false);
  const [form] = Form.useForm();

  const categories = Array.from(
    new Set(mockQuestions.map((x) => x.category ?? "Khác"))
  );
  const filtered = mockQuestions.filter(
    (x) =>
      x.content.toLowerCase().includes(q.toLowerCase()) &&
      (cat === "all" || (x.category ?? "Khác") === cat)
  );

  const byType = useMemo(
    () => ({
      all: filtered,
      "multiple-choice": filtered.filter((x) => x.type === "multiple-choice"),
      essay: filtered.filter((x) => x.type === "essay"),
      "fill-blank": filtered.filter((x) => x.type === "fill-blank"),
    }),
    [filtered]
  );

  const onCreate = async () => {
    const v = await form.validateFields();
    if (
      v.type === "multiple-choice" &&
      (!v.options || v.options.some((t: string) => !t))
    )
      return message.error("Nhập đủ đáp án A, B, C, D");
    message.success("Đã tạo câu hỏi (mock)");
    setOpen(false);
    form.resetFields();
  };

  const QuestionItem = ({ item }: { item: any }) => (
    <Card className={styles.qCard}>
      <div className={styles.qTop}>
        <div className={styles.qContent}>{item.content}</div>
        <div className={styles.qActions}>
          <Button type="text" icon={<Edit size={16} />} />
          <Button type="text" danger icon={<Trash2 size={16} />} />
        </div>
      </div>
      <div className={styles.qTags}>
        <Tag>
          {item.type === "multiple-choice"
            ? "Trắc nghiệm"
            : item.type === "essay"
            ? "Tự luận"
            : "Điền khuyết"}
        </Tag>
        <Tag color="blue">{item.points} điểm</Tag>
        <Tag color="default">{item.category ?? "Khác"}</Tag>
      </div>
      {item.type === "multiple-choice" && item.options && (
        <div className={styles.options}>
          {item.options.map((op: string, i: number) => (
            <div
              key={i}
              className={
                i === item.correctAnswer ? styles.optCorrect : styles.opt
              }
            >
              {String.fromCharCode(65 + i)}. {op}
              {i === item.correctAnswer ? " ✓" : ""}
            </div>
          ))}
        </div>
      )}
    </Card>
  );

  return (
    <div className={styles.wrap}>
      <div className={styles.head}>
        <h1>Ngân hàng câu hỏi</h1>
        <Button
          type="primary"
          icon={<Plus size={16} />}
          onClick={() => setOpen(true)}
        >
          Tạo câu hỏi mới
        </Button>
      </div>

      <Card className={styles.card}>
        <div className={styles.filters}>
          <Input
            placeholder="Tìm kiếm câu hỏi…"
            value={q}
            onChange={(e) => setQ(e.target.value)}
          />
          <Select
            value={cat}
            onChange={setCat}
            options={[
              { value: "all", label: "Tất cả danh mục" },
              ...categories.map((c) => ({ value: c, label: c })),
            ]}
            style={{ minWidth: 220 }}
          />
        </div>
      </Card>

      <Tabs
        items={[
          {
            key: "all",
            label: `Tất cả (${byType.all.length})`,
            children: (
              <List
                dataSource={byType.all}
                renderItem={(it: any) => <QuestionItem item={it} />}
              />
            ),
          },
          {
            key: "multiple-choice",
            label: `Trắc nghiệm (${byType["multiple-choice"].length})`,
            children: (
              <List
                dataSource={byType["multiple-choice"]}
                renderItem={(it: any) => <QuestionItem item={it} />}
              />
            ),
          },
          {
            key: "essay",
            label: `Tự luận (${byType.essay.length})`,
            children: (
              <List
                dataSource={byType.essay}
                renderItem={(it: any) => <QuestionItem item={it} />}
              />
            ),
          },
          {
            key: "fill-blank",
            label: `Điền khuyết (${byType["fill-blank"].length})`,
            children: (
              <List
                dataSource={byType["fill-blank"]}
                renderItem={(it: any) => <QuestionItem item={it} />}
              />
            ),
          },
        ]}
      />

      <Modal
        title="Tạo câu hỏi mới"
        open={open}
        onCancel={() => setOpen(false)}
        onOk={onCreate}
        okText="Lưu câu hỏi"
        width={720}
      >
        <Form
          form={form}
          layout="vertical"
          initialValues={{
            type: "multiple-choice",
            points: 5,
            correctAnswer: 0,
            options: ["", "", "", ""],
            category: "Đại số",
          }}
        >
          <Form.Item label="Loại câu hỏi" name="type">
            <Radio.Group>
              <Radio value="multiple-choice">Trắc nghiệm</Radio>
              <Radio value="essay">Tự luận</Radio>
              <Radio value="fill-blank">Điền khuyết</Radio>
            </Radio.Group>
          </Form.Item>
          <Form.Item label="Danh mục" name="category">
            <Input placeholder="Đại số, Hình học…" />
          </Form.Item>
          <Form.Item
            label="Nội dung"
            name="content"
            rules={[{ required: true }]}
          >
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item noStyle shouldUpdate>
            {() =>
              form.getFieldValue("type") === "multiple-choice" && (
                <div className={styles.mcWrap}>
                  {["A", "B", "C", "D"].map((l, i) => (
                    <div key={i} className={styles.mcRow}>
                      <Radio
                        checked={form.getFieldValue("correctAnswer") === i}
                        onChange={() => form.setFieldValue("correctAnswer", i)}
                      />
                      <span className={styles.mcLabel}>{l}.</span>
                      <Form.Item name={["options", i]} noStyle>
                        <Input placeholder={`Đáp án ${l}`} />
                      </Form.Item>
                    </div>
                  ))}
                  <div className={styles.miniNote}>
                    Đáp án đúng:{" "}
                    <b>
                      {String.fromCharCode(
                        65 + (form.getFieldValue("correctAnswer") ?? 0)
                      )}
                    </b>
                  </div>
                </div>
              )
            }
          </Form.Item>
          <Form.Item label="Điểm số" name="points">
            <Input type="number" min={1} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
export default QuestionBankPage;
