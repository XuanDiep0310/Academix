# frontend/Dockerfile

# ---------------------
# Development stage
# ---------------------
FROM node:20-alpine AS development
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 3000
CMD ["npm", "run", "dev"]

# ---------------------
# Dependencies stage (for production build)
# ---------------------
FROM node:20-alpine AS deps
WORKDIR /app
COPY package*.json ./
RUN npm ci

# ---------------------
# Build stage
# ---------------------
FROM node:20-alpine AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
RUN npm run build

# ---------------------
# Production runtime stage
# ---------------------
FROM node:20-alpine AS production
WORKDIR /app

# copy production artifacts
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
COPY package*.json ./
COPY --from=deps /app/node_modules ./node_modules

# prune development deps to keep image small
RUN npm prune --production

# install curl for healthcheck
RUN apk add --no-cache curl

EXPOSE 3000
ENV NODE_ENV=production

CMD ["npm", "run", "start"]
