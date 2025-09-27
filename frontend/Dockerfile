# frontend/Dockerfile

# Stage 1: Build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package.json và cài dependencies
COPY package*.json ./
RUN npm ci

# Copy toàn bộ source code và build
COPY . .
RUN npm run build

# Stage 2: Production
FROM node:20-alpine AS production
WORKDIR /app

# Copy package.json và cài production dependencies
COPY package*.json ./
RUN npm ci --only=production && npm cache clean --force

# Copy built application từ build stage
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
COPY --from=build /app/next.config.js ./ 
COPY --from=build /app/package.json ./

# Tạo user non-root để chạy ứng dụng
RUN addgroup -g 1001 -S nodejs \
  && adduser -S nextjs -u 1001 \
  && chown -R nextjs:nodejs /app

USER nextjs

EXPOSE 3000

ENV NODE_ENV=production

CMD ["npm", "start"]
