# Этап сборки
FROM node:20 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod

# Этап запуска
FROM nginx:alpine

# Копируем скрипт для обработки переменных окружения
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Копируем собранное приложение и конфиг nginx
COPY --from=build /app/dist/adis.client/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Устанавливаем точку входа для обработки переменных окружения
ENTRYPOINT ["/docker-entrypoint.sh"]

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]