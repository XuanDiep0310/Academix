"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { Send, Bot, User, StopCircle, Trash2, Copy } from "lucide-react";
import { Card, Input, Button, App } from "antd";
import styles from "@/assets/styles/Chatbox.module.scss";
import { SEED_BOT_RESPONSES, SEED_CHAT_MESSAGES } from "@/data/seed";

type Message = {
  id: string;
  content: string;
  sender: "user" | "bot";
  timestamp: string; // ISO string để dễ lưu localStorage
};

const STORAGE_KEY = "chat_messages_v1";

export default function Chatbot() {
  const { message: toast } = App.useApp();

  // khởi tạo từ localStorage, nếu trống thì dùng seed
  const [messages, setMessages] = useState<Message[]>(() => {
    if (typeof window === "undefined") return [];
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) return JSON.parse(raw) as Message[];
    } catch {}
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return (SEED_CHAT_MESSAGES as any[]).map((m) => ({
      ...m,
      timestamp:
        m.timestamp instanceof Date
          ? m.timestamp.toISOString()
          : String(m.timestamp),
    }));
  });

  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [aborter, setAborter] = useState<AbortController | null>(null);

  // scroll refs
  const scrollerRef = useRef<HTMLDivElement>(null);
  const endRef = useRef<HTMLDivElement>(null);
  const shouldAutoScroll = useRef(true);

  // lưu lịch sử
  useEffect(() => {
    if (typeof window === "undefined") return;
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(messages));
    } catch {}
  }, [messages]);

  // auto-scroll nếu đang ở đáy
  const scrollToBottom = () =>
    endRef.current?.scrollIntoView({ behavior: "smooth" });

  const handleScroll = () => {
    const el = scrollerRef.current;
    if (!el) return;
    const threshold = 80; // px
    const distanceToBottom = el.scrollHeight - el.scrollTop - el.clientHeight;
    shouldAutoScroll.current = distanceToBottom < threshold;
  };

  useEffect(() => {
    if (shouldAutoScroll.current) scrollToBottom();
  }, [messages]);

  const prettyTime = (iso: string) =>
    new Date(iso).toLocaleTimeString("vi-VN", {
      hour: "2-digit",
      minute: "2-digit",
    });

  const handleCopy = async (text: string) => {
    try {
      await navigator.clipboard.writeText(text);
      toast.success("Đã sao chép tin nhắn");
    } catch {
      toast.error("Không sao chép được");
    }
  };

  const sendUserMessage = (text: string) => {
    const userMsg: Message = {
      id: `u_${Date.now()}`,
      content: text,
      sender: "user",
      timestamp: new Date().toISOString(),
    };
    setMessages((prev) => [...prev, userMsg]);
  };

  const simulateBot = async (text: string, signal: AbortSignal) => {
    // mô phỏng “sinh” từng đoạn (stream)
    const lower = text.toLowerCase();
    let response = SEED_BOT_RESPONSES.default.replace("{query}", text);
    for (const [key, value] of Object.entries(SEED_BOT_RESPONSES)) {
      if (key !== "default" && lower.includes(key)) {
        response = value;
        break;
      }
    }

    // tạo khung tin nhắn bot rỗng để stream
    const botId = `b_${Date.now() + 1}`;
    const base: Message = {
      id: botId,
      content: "",
      sender: "bot",
      timestamp: new Date().toISOString(),
    };
    setMessages((prev) => [...prev, base]);

    // stream từng chunk
    const chunks = response.split(/(\s+)/); // giữ khoảng trắng
    for (const chunk of chunks) {
      if (signal.aborted) throw new DOMException("Aborted", "AbortError");
      await new Promise((r) => setTimeout(r, 15)); // tốc độ stream
      setMessages((prev) =>
        prev.map((m) =>
          m.id === botId ? { ...m, content: m.content + chunk } : m
        )
      );
    }
  };

  const handleSend = async () => {
    const text = input.trim();
    if (!text || isLoading) return;

    setInput("");
    setIsLoading(true);

    const ctl = new AbortController();
    setAborter(ctl);

    try {
      sendUserMessage(text);
      await simulateBot(text, ctl.signal);
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (e: any) {
      if (e?.name !== "AbortError") {
        toast.error("Bot gặp lỗi khi phản hồi");
      }
    } finally {
      setIsLoading(false);
      setAborter(null);
    }
  };

  const handleKeyDown: React.KeyboardEventHandler<HTMLTextAreaElement> = (
    e
  ) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  };

  const handleStop = () => {
    aborter?.abort();
  };

  const handleClear = () => {
    setMessages([]);
    if (typeof window !== "undefined") localStorage.removeItem(STORAGE_KEY);
  };

  const headerActions = useMemo(
    () => (
      <div className={styles.headerActions}>
        <Button
          className={styles.iconBtn}
          onClick={handleClear}
          icon={<Trash2 size={16} />}
        >
          Xóa lịch sử
        </Button>
        <Button
          className={styles.iconBtn}
          disabled={!isLoading}
          onClick={handleStop}
          icon={<StopCircle size={16} />}
        >
          Dừng
        </Button>
      </div>
    ),
    [isLoading]
  );

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div className={styles.titleWrap}>
          <h1>Trợ lý AI</h1>
          <p>Hỏi đáp, giải thích, hướng dẫn nhanh — như ChatGPT</p>
        </div>
        {headerActions}
      </div>

      <Card
        className={styles.chatCard}
        title={
          <div className={styles.cardTitle}>
            <Bot className={styles.icon} />
            <span>Phiên trò chuyện</span>
          </div>
        }
      >
        <div className={styles.cardContent}>
          <div
            className={styles.scrollArea}
            ref={scrollerRef}
            onScroll={handleScroll}
          >
            <div className={styles.messagesContainer}>
              {messages.map((m) => (
                <div
                  key={m.id}
                  className={`${styles.message} ${
                    m.sender === "user" ? styles.userMessage : styles.botMessage
                  }`}
                >
                  <div className={styles.avatar}>
                    {m.sender === "bot" ? <Bot /> : <User />}
                  </div>

                  <div className={styles.bubbleWrap}>
                    <div className={styles.messageContent}>
                      {/* đơn giản: giữ xuống dòng; nếu cần Markdown có thể thêm lib sau */}
                      {m.content.split("\n").map((line, i) => (
                        <p key={i}>{line}</p>
                      ))}
                    </div>

                    <div className={styles.metaBar}>
                      <span className={styles.timestamp}>
                        {prettyTime(m.timestamp)}
                      </span>
                      <button
                        className={styles.copyBtn}
                        onClick={() => handleCopy(m.content)}
                        title="Sao chép"
                      >
                        <Copy size={14} />
                      </button>
                    </div>
                  </div>
                </div>
              ))}
              <div ref={endRef} />
            </div>
          </div>

          <div className={styles.inputArea}>
            <Input.TextArea
              placeholder="Nhập nội dung…  (Enter: gửi • Shift+Enter: xuống dòng)"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={handleKeyDown}
              autoSize={{ minRows: 1, maxRows: 8 }}
              disabled={isLoading}
              className={styles.ta}
            />
            <Button
              type="primary"
              onClick={handleSend}
              loading={isLoading}
              icon={<Send size={16} />}
            >
              Gửi
            </Button>
          </div>
        </div>
      </Card>
    </div>
  );
}
