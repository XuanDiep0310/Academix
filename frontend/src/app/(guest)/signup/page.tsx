"use client";
import React, { useState } from "react";
import { Form, Input, Button, Checkbox, Divider, Row, Col } from "antd";
import {
  UserOutlined,
  MailOutlined,
  LockOutlined,
  GoogleOutlined,
  FacebookOutlined,
} from "@ant-design/icons";
import Link from "next/link";
import "@/assets/styles/signUp.scss";

interface IInfoSignUp {
  token: string;
  refreshToken: string;
  expiresAt: string;
  user: {
    userId: 0;
    email: string;
    displayName: string;
    avatarUrl: string;
    organizationId: 0;
    roles: [string];
    permissions: [string];
  };
}

const SignupPage = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const onFinish = async (values: IInfoSignUp) => {
    setLoading(true);
    console.log("Form values:", values);

    setTimeout(() => {
      setLoading(false);
    }, 1000);
  };

  const handleSocialLogin = (provider: string) => {
    console.log(`Login with ${provider}`);
  };

  return (
    <div className="signup-container">
      <Row className="signup-row">
        <Col xs={24} md={12} className="logo-section">
          <div className="logo-circle">
            <div className="panda-icon">
              <div className="panda-face">
                <div className="ear ear-left"></div>
                <div className="ear ear-right"></div>
                <div className="graduation-cap">
                  <div className="cap-top"></div>
                  <div className="tassel"></div>
                </div>
                <div className="eyes">
                  <div className="eye eye-left">
                    <div className="eye-patch"></div>
                  </div>
                  <div className="eye eye-right">
                    <div className="eye-patch"></div>
                  </div>
                </div>
                <div className="nose"></div>
                <div className="mouth"></div>
                <div className="cheek cheek-left"></div>
                <div className="cheek cheek-right"></div>
              </div>
            </div>
            <h1 className="brand-name">ACADEMIX</h1>
          </div>
        </Col>

        <Col xs={24} md={12} className="form-section">
          <div className="form-card">
            <h2 className="title">Sign Up</h2>
            <p className="subtitle">
              Create an account to start your learning journey
            </p>

            <Form
              form={form}
              name="signup"
              onFinish={onFinish}
              layout="vertical"
              className="signup-form"
            >
              <Form.Item
                label="Full Name"
                name="fullName"
                rules={[
                  { required: true, message: "Please input your full name!" },
                ]}
              >
                <Input
                  prefix={<UserOutlined />}
                  placeholder="John Doe"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                label="Email"
                name="email"
                rules={[
                  { required: true, message: "Please input your email!" },
                  { type: "email", message: "Please enter a valid email!" },
                ]}
              >
                <Input
                  prefix={<MailOutlined />}
                  placeholder="student@university.edu"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                label="Password"
                name="password"
                rules={[
                  { required: true, message: "Please input your password!" },
                  {
                    min: 8,
                    message: "Password must be at least 8 characters!",
                  },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Create a strong password"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                label="Confirm Password"
                name="confirmPassword"
                dependencies={["password"]}
                rules={[
                  { required: true, message: "Please confirm your password!" },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue("password") === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(
                        new Error("Passwords do not match!")
                      );
                    },
                  }),
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Confirm your password"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="agreement"
                valuePropName="checked"
                rules={[
                  {
                    validator: (_, value) =>
                      value
                        ? Promise.resolve()
                        : Promise.reject(new Error("Please accept the terms")),
                  },
                ]}
              >
                <Checkbox>
                  I agree to the{" "}
                  <Link href="/terms" className="link">
                    Terms of Service
                  </Link>{" "}
                  and{" "}
                  <Link href="/privacy" className="link">
                    Privacy Policy
                  </Link>
                </Checkbox>
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  size="large"
                  loading={loading}
                  block
                  className="submit-button"
                >
                  Create Account
                </Button>
              </Form.Item>
            </Form>

            <Divider plain className="divider">
              OR CONTINUE WITH
            </Divider>

            <Row gutter={12} className="social-buttons">
              <Col span={12}>
                <Button
                  icon={<GoogleOutlined />}
                  size="large"
                  onClick={() => handleSocialLogin("Google")}
                  block
                  className="social-button"
                >
                  Google
                </Button>
              </Col>
              <Col span={12}>
                <Button
                  icon={<FacebookOutlined />}
                  size="large"
                  onClick={() => handleSocialLogin("Facebook")}
                  block
                  className="social-button"
                >
                  Facebook
                </Button>
              </Col>
            </Row>

            <p className="signin-link">
              Already have an account?{" "}
              <Link href="/signin" className="link">
                Sign in
              </Link>
            </p>
          </div>
        </Col>
      </Row>
    </div>
  );
};

export default SignupPage;
