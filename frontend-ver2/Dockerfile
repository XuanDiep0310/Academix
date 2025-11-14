# ---------------------
# Development stage
# ---------------------
FROM node:20-alpine AS development
WORKDIR /app
COPY package*.json ./
RUN npm install --legacy-peer-deps
COPY . .
# Port 3000 (CRA) hoặc 5173 (Vite) là phổ biến, chỉnh lại nếu cần
EXPOSE 5173
CMD ["npm", "run", "dev"]

# ---------------------
# Dependencies stage (for production build)
# ---------------------
FROM node:20-alpine AS deps
WORKDIR /app
COPY package*.json ./
RUN npm ci --legacy-peer-deps

# ---------------------
# Build stage
# ---------------------
FROM node:20-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .

# --- Thêm vào để nhận biến môi trường lúc build ---
# Tên ARG (VITE_PUBLIC_API_URL) phải khớp với tên trong docker-compose.yml
ARG VITE_PUBLIC_API_URL
# Tên ENV phải khớp với tên mà app React của bạn sử dụng (ví dụ: .env.production)
ENV VITE_PUBLIC_API_URL=${VITE_PUBLIC_API_URL}
# !!! Nếu dùng Create React App, hãy đổi VITE_PUBLIC_API_URL thành REACT_APP_API_URL ở cả 2 dòng trên
# --- Hết ---

# Lệnh này sẽ tạo ra thư mục 'dist' (Vite) hoặc 'build' (CRA)
RUN npm run build

# ---------------------
# Production runtime stage
# ---------------------
FROM nginx:alpine AS production
WORKDIR /usr/share/nginx/html

# Copy Nginx config tùy chỉnh để hỗ trợ React Router (SPA)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy các file static đã build từ stage 'build'
COPY --from=build /app/dist .

# Nginx chạy trên port 80
EXPOSE 80
# CMD mặc định của Nginx sẽ tự khởi chạy server