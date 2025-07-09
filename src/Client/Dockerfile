FROM node:18-alpine AS builder
WORKDIR /usr/src/app

# Copy package files first for caching
COPY package.json package-lock.json ./
RUN npm install

# Copy application source
COPY . .

# Build production assets - Vite outputs to 'dist' directory
RUN npm run build

# Verify build output
RUN ls -la dist

FROM node:18-alpine
RUN npm install -g serve
WORKDIR /usr/src/app

# Copy from Vite's output directory
COPY --from=builder /usr/src/app/dist ./dist

# Start production server
CMD ["serve", "-s", "dist", "-l", "4200", "--single"]