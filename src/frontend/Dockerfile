FROM node:22-alpine AS build
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
WORKDIR /app

COPY package.json ./
RUN npm install

COPY . ./
RUN npm run build

FROM node:22-alpine AS production
WORKDIR /app

COPY --from=build /app/package.json ./
COPY --from=build /app/node_modules ./node_modules
COPY --from=build /app/build ./build
COPY --from=build /app/.svelte-kit ./.svelte-kit

EXPOSE 5173
ENV PORT=5173

CMD ["node", "build"]
