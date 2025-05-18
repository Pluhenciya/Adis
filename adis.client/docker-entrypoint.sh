#!/bin/sh
set -e

echo "Processing environment variables..."
CONFIG='{"yandexMapsApiKey": "'"${YANDEX_MAPS_API_KEY}"'"}'

# Исправленная команда find
find /usr/share/nginx/html -name '*.js' -exec sed -i "s|APP_CONFIG = {};|APP_CONFIG = ${CONFIG};|g" {} \;

exec "$@"