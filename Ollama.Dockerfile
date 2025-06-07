FROM ollama/ollama:latest

ENV OLLAMA_CPU_OVERRIDE=1 \
    OLLAMA_NUM_PARALLEL=1 \
    OLLAMA_LOAD_TIMEOUT=20m \
    OLLAMA_MAX_LOADED_MODELS=1

RUN ollama serve

# Предзагружаем модели при сборке образа
RUN ollama pull nomic-embed-text && \
    ollama pull qwen3:1.7b