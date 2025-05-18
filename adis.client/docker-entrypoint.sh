#!/bin/sh
set -e

echo "Processing environment variables..."
CONFIG='{"yandexMapsApiKey": "'"${YANDEX_MAPS_API_KEY}"'"}'

# Ищем JavaScript файлы для замены конфигурации
find /usr/share/nginx/html -name '*.js' -exec sed -i "s|APP_CONFIG = {};|APP_CONFIG = ${CONFIG};|g" {} +

exec "$@"