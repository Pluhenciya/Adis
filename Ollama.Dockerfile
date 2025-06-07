FROM ollama/ollama:latest

# Предзагружаем модели при сборке образа
RUN ollama pull nomic-embed-text && \
    ollama pull qwen3:1.7b