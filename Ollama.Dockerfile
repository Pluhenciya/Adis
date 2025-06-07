FROM ollama/ollama:latest

# Устанавливаем необходимые утилиты
RUN apk add --no-cache curl

# Устанавливаем переменные окружения для CPU-режима
ENV OLLAMA_CPU_OVERRIDE=1 \
    OLLAMA_NUM_PARALLEL=1 \
    OLLAMA_LOAD_TIMEOUT=20m \
    OLLAMA_MAX_LOADED_MODELS=1

# Создаем скрипт для загрузки моделей с запущенным сервером
RUN echo $'#!/bin/sh\n\
set -e\n\
\n\
# Запускаем сервер в фоновом режиме\n\
ollama serve &\n\
SERVER_PID=$!\n\
\n\
# Ждем пока сервер станет доступен\n\
until curl -sSf http://localhost:11434 > /dev/null; do\n\
  sleep 1\n\
done\n\
\n\
# Загружаем модели\n\
ollama pull nomic-embed-text\n\
ollama pull qwen3:1.7b\n\
\n\
# Останавливаем сервер\n\
kill $SERVER_PID\n\
wait $SERVER_PID' > /pull-models.sh \
    && chmod +x /pull-models.sh

# Запускаем скрипт загрузки моделей
RUN /pull-models.sh