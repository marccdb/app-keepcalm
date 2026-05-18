FROM node:22-alpine AS production
WORKDIR /app

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

COPY package.json ./
COPY build ./build

EXPOSE 5173
ENV PORT=5173

CMD ["node", "build/index.js"]
