# ADIS 🏗️
**Информационная система управления проектами в строительстве**

[![.NET](https://img.shields.io/badge/.NET_9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23_13.0-purple?style=for-the-badge&logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-blueviolet?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/web-api/)
[![EF Core](https://img.shields.io/badge/EF_Core_9.0-512BD4?style=for-the-badge&logo=entity-framework&logoColor=white)](https://learn.microsoft.com/ef/core/)
[![MySQL](https://img.shields.io/badge/MySQL_8.4-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Angular](https://img.shields.io/badge/Angular_19-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.io/)
[![Angular Material](https://img.shields.io/badge/Angular_Material_19-green?style=for-the-badge&logo=materialdesign&logoColor=white)](https://material.angular.io/)
[![JWT](https://img.shields.io/badge/Auth-JWT_Bearer-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=for-the-badge&logo=swagger&logoColor=white)](https://swagger.io/)
[![Yandex Maps](https://img.shields.io/badge/Yandex_Maps-API-5B4BD5?style=for-the-badge&logo=yandex&logoColor=white)](https://yandex.ru/dev/maps/)
[![Ollama](https://img.shields.io/badge/LLM-Ollama-2C3E50?style=for-the-badge&logo=ollama&logoColor=white)](https://ollama.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![GitHub Actions](https://img.shields.io/badge/CI%2FCD-GitHub_Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white)](https://github.com/features/actions)
[![License](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)](LICENSE.txt)

---

## 📋 О проекте

Веб-система для управления жизненным циклом строительных проектов: от этапа проектирования до сдачи объекта. Проекты привязаны к объектам работ на карте (Яндекс.Карты), по ним распределяются задачи между исполнителями и проверяющими, ведётся документооборот и контроль исполнения.

**Изюминка проекта** — встроенный **Нейропроводник** 🧠: RAG-ассистент, который отвечает на вопросы по нормативным документам проекта с опорой на загруженные PDF/DOCX и ссылками на источники. Модель крутится локально через **Ollama** — без внешних API и затрат на токены.

**Ключевые возможности:**
- ✅ Полный жизненный цикл проекта: проектирование → поиск подрядчика → исполнение → завершён
- ✅ Геопривязка проектов к объектам работ (GeoJSON + Яндекс.Карты)
- ✅ Kanban-доска задач со статусами `ToDo → Doing → Checking → Completed`
- ✅ Назначение исполнителей и проверяющих, результаты и сроки задач
- ✅ Документооборот: загрузка PDF/DOCX, скачивание, выгрузка архивом ZIP
- ✅ Комментарии к задачам
- ✅ Контроль исполнения по секциям объекта (инспектор)
- ✅ Ролевая модель: Admin, ProjectManager, Projecter, Inspector
- ✅ JWT-аутентификация с refresh-токенами
- ✅ Нейропроводник — локальный LLM-ассистент по нормативным документам
- ✅ Полное покрытие инфраструктурой: Docker Compose + CI/CD и деплой на сервер

---

## 🏗️ Архитектура

```
┌──────────────────────────────────────────────────────────────────┐
│                         Docker Compose                            │
│  ┌─────────────┐  ┌──────────────┐  ┌────────────┐  ┌─────────┐ │
│  │   Backend   │  │   Frontend   │  │   MySQL    │  │ Ollama  │ │
│  │ Adis.Api    │  │ adis.client  │  │    8.4     │  │ LLM+EMB │ │
│  │ :8080/8081  │  │     :80      │  │   :3306    │  │ :11434  │ │
│  └──────┬──────┘  └──────┬───────┘  └─────┬──────┘  └────┬────┘ │
│         └────────────────┴────────────────┴───────────────┘      │
│                         сеть app-network (bridge)                 │
└──────────────────────────────────────────────────────────────────┘
         ▲                            ▲
         │  /docker (nginx proxy)     │  Swagger UI
    ┌────┴─────┐                  ┌───┴────────┐
    │ Пользова-│                  │ REST API   │
    │ тель     │                  │ /api/*     │
    │(браузер) │                  │ (JWT)      │
    └──────────┘                  └────────────┘
```

**Многослойная архитектура (.NET 9 + Angular 19):**

```
Adis.sln
│
├── 🧱 Adis.Dm          # Domain Models (сущности)
├── 🗄️  Adis.Dal        # Data Access Layer (EF Core, репозитории, спецификации)
├── ⚙️  Adis.Bll        # Business Logic Layer (сервисы, DTO, AutoMapper, RAG)
├── 🌐 Adis.Api         # ASP.NET Core Web API (REST + Swagger)
├── 🧪 Adis.Tests       # Интеграционные тесты
└── 🖥️  adis.client      # Angular SPA (Material UI + Яндекс.Карты)
```

---

## 🧱 Adis.Dm — Domain Models

Основные сущности предметной области:

| Сущность               | Назначение                                             |
|------------------------|--------------------------------------------------------|
| `Project`              | Проект: название, даты проектирования/исполнения, статус |
| `ProjectStatus`        | `Designing` → `ContractorSearch` → `InExecution` → `Completed` |
| `WorkObject`           | Объект работ с **геометрией** (`NetTopologySuite`)      |
| `WorkObjectSection`    | Секция объекта (для контроля исполнения)               |
| `Contractor`           | Подрядчик, выполняющий проект                           |
| `ProjectTask`          | Задача: исполнители, проверяющие, статус, результат     |
| `Status`               | Статус задачи: `ToDo / Doing / Checking / Completed`    |
| `ExecutionTask`        | Исполнительная задача по секции объекта                 |
| `Document`             | Документ (PDF/DOCX), привязанный к задаче               |
| `DocumentType`         | Тип документа                                           |
| `Comment`              | Комментарий к задаче                                    |
| `User` / `AppRole`     | Пользователи и роли (ASP.NET Identity)                  |
| `RefreshToken`         | Refresh-токены для JWT                                  |

**Роли пользователей:**

| Роль             | Права                                                                 |
|------------------|------------------------------------------------------------------------|
| 👑 `Admin`        | Всё: пользователи, документы (в т.ч. нормативные для нейропроводника) |
| 🧭 `ProjectManager` | Управление проектами, задачами, подрядчиками                        |
| ✏️ `Projecter`    | Доска задач, работа с результатами и статусами                        |
| 🔍 `Inspector`    | Контроль исполнения проекта (завершение исполнения)                   |

---

## 🗄️ Adis.Dal — Data Access Layer

**Паттерны:**
- **Generic repository** (`EFGenericRepository<T>` + `IRepository<T>`) для базовых CRUD-операций
- **Specification pattern** — переиспользуемые спецификации запросов: `ProjectFilterSpecification`, `TaskDetailsByIdSpecification`, `UserByEmailSpecification`, `DocumentsWithoutTaskSpecification` и др.
- **`AppDbContext`** — EF Core + MySQL 8.4, `UseNetTopologySuite()` для пространственных данных (GeoJSON)

**24 миграции** — полная эволюция схемы: от `InitialCreate` до `AddActualEndDate...` и `AddCreatedAt...`.

---

## ⚙️ Adis.Bll — Business Logic Layer

- **Сервисы:** `AuthService`, `UserService`, `ProjectService`, `TaskService`, `CommentService`, `DocumentService`, `ExecutionTaskService`, `WorkObjectSectionService`, `NeuralGuideService`
- **DTO** по каждому контексту: `Auth/`, `Project/`, `Task/`, `User/`, `Comment/`, а также `PaginatedResult<T>`, `ProjectsResponseDto`, `NeuralRequest`
- **AutoMapper-профили** — маппинг сущностей ↔ DTO
- **Конфигурации:** `JwtSettings`, `AdminSettings`, `OllamaSetting` (URL, эмбеддинг-модель, LLM-модель)
- **`AdminInitializer`** — создание админа и ролей при первом запуске

### 🧠 NeuralGuideService — RAG-ассистент

Локальный вопросно-ответный сервис по нормативным документам проекта:

```
Загруженные PDF/DOCX
        │
        ▼
 Text Splitter (chunks 512/128) ──► Embeddings (nomic-embed-text) ──► InMemory Vector DB
        │                                                                      ▲
        │                                                          Семантический поиск
        ▼                                                                  (top-5, score ≥ 0.7)
 Вопрос пользователя ───────────────────────────────────────────────────────┘
        │                          ┌─────────────────────────────┐
        │                          │ Слияние результатов:        │
        └─────────────────────────►│ семантика + keyword-поиск   │
                                   └──────────────┬──────────────┘
                                                  ▼
                              Prompt (контекст + источники) ──► Ollama (qwen3:1.7b)
                                                  ▼
                       Ответ в формате: Вывод / Обоснование / Источники
```

**Фишки:**
- 🔎 Гибридный поиск: семантический (векторный) + keyword-поиск по совпадению терминов
- 📚 Ответ всегда сопровождается списком источников (`[Источник: файл|id:документ]`)
- 🧹 Очистка юридического текста перед индексацией
- ⚡ Кэширование индекса в `IMemoryCache` (TTL 1 час)
- 🪄 Стриминговая генерация ответа через `OllamaChatModel`

---

## 🌐 Adis.Api — Web API

**Публичные эндпоинты:**

| Метод  | Эндпоинт                                 | Доступ                  | Описание                              |
|--------|-------------------------------------------|-------------------------|---------------------------------------|
| POST   | `/api/auth/login`                          | ✳️ Все                  | Вход, выдача access + refresh токенов |
| POST   | `/api/auth/refresh-token`                  | ✳️ Все                  | Обновление пары токенов               |
| GET    | `/api/users`                               | 👑 Admin                | Список пользователей                  |
| POST   | `/api/users`                               | 👑 Admin                | Создание пользователя                 |
| GET    | `/api/users/{id}`                          | 👑 + 🧭                 | Пользователь по id                    |
| GET    | `/api/users/{role}/{partialFullName}`      | 👑 + 🧭                 | Поиск по ФИО и роли (автодополнение)  |
| GET    | `/api/projects`                            | 🔐 Авториз.             | Проекты с фильтрами/сортировкой/пагинацией |
| POST   | `/api/projects`                            | 👑 + 🧭                 | Создание проекта                      |
| GET    | `/api/projects/{id}`                       | 🔐 Авториз.             | Детали проекта с задачами             |
| PUT    | `/api/projects`                            | 👑 + 🧭                 | Обновление проекта                    |
| DELETE | `/api/projects/{id}`                       | 👑 + 🧭                 | Удаление проекта                      |
| GET    | `/api/projects/{id}/complete/{idEstimate}` | 👑 + 🧭                 | Завершение проектирования             |
| PATCH  | `/api/projects/{id}/complete-contractor-search` | 👑 + 🧭           | Завершение поиска подрядчика          |
| PATCH  | `/api/projects/{id}/complete-execution`    | 👑 + 🔍                 | Завершение исполнения                 |
| GET    | `/api/tasks`                               | ✏️ Projecter            | Задачи проектировщика                 |
| GET    | `/api/tasks/{id}`                          | 🔐 Авториз.             | Детали задачи                         |
| POST   | `/api/tasks`                               | 👑 + 🧭                 | Создание задачи                       |
| PUT    | `/api/tasks`                               | 👑 + 🧭                 | Обновление задачи                     |
| GET    | `/api/tasks/{id}/{status}`                 | ✏️ + 👑                 | Смена статуса задачи                  |
| PUT    | `/api/tasks/{idTask}`                      | ✏️ + 👑                 | Сохранение результата задачи          |
| POST   | `/api/comments`                            | 👑 + 🧭 + ✏️            | Добавление комментария                |
| POST   | `/api/documents/upload`                    | 👑 + ✏️                 | Загрузка документа                    |
| GET    | `/api/documents/{id}/download`             | 🔐 Авториз.             | Скачивание документа                  |
| GET    | `/api/documents/{idProject}`               | 🔐 Авториз.             | Документы проекта                     |
| GET    | `/api/documents/download-zip?ids=1,2,3`    | 🔐 Авториз.             | Выгрузка документов ZIP-архивом       |
| GET    | `/api/documents/guide`                     | 👑 Admin                | Нормативные документы (для RAG)       |
| PATCH  | `/api/executiontasks/{id}/status`          | 🔐 Авториз.             | Выполнение исполнительной задачи      |
| POST   | `/api/neuralguide`                         | 🔐 Авториз.             | Вопрос к Нейропроводнику              |

**Особенности:**
- 📖 Swagger UI + XML-документация (полные примеры запросов в описаниях)
- 🔐 JWT Bearer (`SymmetricSecurityKey`), валидация issuer/audience/lifetime
- 🌍 CORS, поддержка GeoJSON через `NetTopologySuite` (конвертер `GeoJsonConverterFactory`)
- ⚙️ Автоприменение миграций и инициализация админа + индекса RAG при старте

---

## 🖥️ adis.client — Веб-интерфейс (Angular 19)

**Страницы:**
| Маршрут           | Страница                                    |
|-------------------|---------------------------------------------|
| `/login`          | Авторизация                                 |
| `/projects`       | Список проектов (фильтры, сортировка, поиск) |
| `/projects/:id`   | Детали проекта с задачами и картой          |
| `/tasks`          | Kanban-доска задач (`Projecter`)            |
| `/documents`      | Документы проекта (админ)                   |
| `/admin/users`    | Управление пользователями                   |
| `/neuro-guide`    | Чат с Нейропроводником 🤖                   |
| `/forbidden`      | Ошибка 403                                  |

**Стек фронтенда:**
- 🎨 **Angular Material 19** (azure-blue тема) + custom SCSS
- 🗺️ **Яндекс.Карты** (`yandex-maps`, GeoJSON-объекты) — карта привязана к проектам
- 📝 **Marked** — рендеринг markdown-ответов Нейропроводника
- 🔐 **jwt-decode** + интерцепторы + guards (`AuthGuard`, `RoleGuard`, `RoleBasedRedirectGuard`)
- 📅 **date-fns** — работа с датами
- 🧩 Компоненты: `add-task-dialog`, `task-card`, `task-details-dialog`, `task-result-dialog`, `task-return-dialog`, `project-form`, `upload-document-dialog`, `user-form`, `filter-menu`, `sort-menu` и др.
- 🔧 Настройка API-ключа Яндекс.Карт через runtime-конфиг (`APP_CONFIG` в `docker-entrypoint.sh`)

---

## 📦 Docker Compose

| Сервис    | Образ                       | Порты                | Назначение                          |
|-----------|-----------------------------|----------------------|--------------------------------------|
| `backend` | `ghcr.io/pluhenciya/adis-backend` | 8080 (HTTP), 8081 | ASP.NET Core API                    |
| `frontend`| `ghcr.io/pluhenciya/adis-frontend`| 80 → nginx         | Angular SPA (proxy `/docker` → API) |
| `mysql`   | `mysql:8.4`                 | 3306                 | База данных + healthcheck           |
| `ollama`  | `ollama/ollama:latest`      | 11434                | Локальные LLM и эмбеддинг-модели    |

Всё в сети `app-network`, данные — в именованных volume (`mysql-data`, `ollama-data`, `documents-data`).

---

## 🚀 Запуск

### Docker Compose (рекомендуется)

```bash
# 1. Подготовить переменные окружения
cp environment/default.env_example environment/default.env
cp environment/mysql.env_example environment/mysql.env
cp environment/frontend.env_example environment/frontend.env

# 2. Указать ключ Яндекс.Карт в frontend.env (иначе карты не будут грузиться)
#    YANDEX_MAPS_API_KEY=ваш_ключ

# 3. Запустить
docker compose up -d --build

# 4. Открыть в браузере
start http://localhost            # Веб-интерфейс
start http://localhost:8080/swagger  # Swagger API
```

> **Админ по умолчанию:** задаётся в `environment/default.env` (`AdminSettings__Email` / `AdminSettings__Password`).

> ⚠️ Первый запуск подтянет модели для Нейропроводника (`ollama pull nomic-embed-text qwen3:1.7b`) — потребуется интернет и немного времени.

### Без Docker

```bash
# Backend (требуется .NET 9 SDK + MySQL)
set ConnectionStrings__DefaultConnection="server=localhost;port=3306;user=root;password=pass;database=adis"
set JWT__Key="Супер_секретный_ключ_минимум_32_символа"
set Ollama__OllamaUrl="http://localhost:11434/api"
dotnet run --project Adis.Api

# Frontend (требуется Node 20+)
cd adis.client
npm install
ng serve --host=127.0.0.1
```

---

## ⚙️ Переменные окружения

**`environment/default.env`:**
```env
ConnectionStrings__DefaultConnection=server=mysql;port=3306;user=root;password=...;database=...
JWT__Issuer=localhost:5221
JWT__Audience=localhost:5221
JWT__Key=SuperSecretKey_...
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
AdminSettings__Email=admin@example.com
AdminSettings__Password=Admin1234!
Ollama__OllamaUrl=http://ollama:11434/api
Ollama__EmbeddingModel=nomic-embed-text
Ollama__LlmModel=qwen3:1.7b
```

**`environment/mysql.env`:**
```env
MYSQL_ROOT_PASSWORD=...
MYSQL_DATABASE=...
MYSQL_USER=...
MYSQL_PASSWORD=...
```

**`environment/frontend.env`:**
```env
YANDEX_MAPS_API_KEY=ваш_ключ_яндекс_карт
```

---

## 🚢 CI/CD (GitHub Actions)

`ci-cd.yml` — два job'а:

1. **build-and-test** (push в `dev`/`master` или PR): сборка Docker-образов backend + frontend, прогон интеграционных тестов в контейнере (`dotnet test` внутри Dockerfile), поднятие окружения через `docker-compose.ci.yml`.
2. **deploy-production** (PR в `master`): сборка и публикация образов в **GHCR**, копирование исходников и **автодеплой на продакшен-сервер** по SSH (URL окружения: `http://92.255.104.183/`).

Секреты: `DEV_*`, `PROD_*`, `JWT__Key`, `YANDEX_MAPS_API_KEY`, SSH-ключи и т.д.

---

## 🧪 Тестирование

Интеграционные тесты на `CustomWebApplicationFactory` (InMemory DB):
- ✅ `AuthControllerIntegrationTests` — вход и refresh-токены
- ✅ `ProjectsControllerIntegrationTests` — CRUD проектов, ролевой доступ
- ✅ `UsersControllerIntegrationTests` — CRUD пользователей

```bash
dotnet test Adis.Tests
```

---

## 🧰 Технологический стек

| Категория            | Технология                                                                  |
|----------------------|-----------------------------------------------------------------------------|
| **Язык**             | [![C#](https://img.shields.io/badge/C%23_13-purple?style=for-the-badge&logo=csharp)](https://learn.microsoft.com/dotnet/csharp/) [![TypeScript](https://img.shields.io/badge/TypeScript_5.7-3178C6?style=for-the-badge&logo=typescript)](https://www.typescriptlang.org/) |
| **Backend**          | [![.NET 9](https://img.shields.io/badge/.NET_9-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/) ASP.NET Core Web API |
| **ORM**              | [![EF Core 9](https://img.shields.io/badge/EF_Core_9-512BD4?style=for-the-badge&logo=entity-framework)](https://learn.microsoft.com/ef/core/) |
| **СУБД**             | [![MySQL 8.4](https://img.shields.io/badge/MySQL_8.4-4479A1?style=for-the-badge&logo=mysql)](https://www.mysql.com/) + NetTopologySuite |
| **Аутентификация**   | [![ASP.NET Identity](https://img.shields.io/badge/ASP.NET_Identity-blueviolet?style=for-the-badge&logo=dotnet)](https://learn.microsoft.com/aspnet/core/security/) JWT + Refresh токены |
| **Frontend**         | [![Angular 19](https://img.shields.io/badge/Angular_19-DD0031?style=for-the-badge&logo=angular)](https://angular.io/) [![Angular Material](https://img.shields.io/badge/Angular_Material-green?style=for-the-badge&logo=materialdesign)](https://material.angular.io/) |
| **Карты**            | [![Yandex Maps](https://img.shields.io/badge/Yandex_Maps-5B4BD5?style=for-the-badge&logo=yandex)](https://yandex.ru/dev/maps/) |
| **AI / RAG**         | [![Ollama](https://img.shields.io/badge/Ollama-2C3E50?style=for-the-badge&logo=ollama)](https://ollama.com/) [![LangChain](https://img.shields.io/badge/LangChain_for_.NET-1C3C3C?style=for-the-badge&logo=langchain)](https://github.com/tryAGI/LangChain) |
| **Маппинг**          | [![AutoMapper](https://img.shields.io/badge/AutoMapper-FF2D20?style=for-the-badge&logo=dotnet)](https://automapper.org/) |
| **Документация API** | [![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/) |
| **Тесты**            | [![xUnit](https://img.shields.io/badge/xUnit-3-512BD4?style=for-the-badge)](https://xunit.net/) |
| **CI/CD**            | [![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-2088FF?style=for-the-badge&logo=githubactions)](https://github.com/features/actions) |
| **Контейнеризация**  | [![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/) |

---

## 📁 Структура решения

```
Adis/
├── 📄 Adis.sln
├── 📄 docker-compose.yml / docker-compose.ci.yml
├── 📄 LICENSE.txt
│
├── 📁 environment/                        # Переменные окружения
│   ├── default.env_example
│   ├── mysql.env_example
│   └── frontend.env_example
│
├── 📁 Adis.Dm/                            # 🧱 Domain Models
│   ├── Project.cs / ProjectStatus.cs
│   ├── ProjectTask.cs / Status.cs / ExecutionTask.cs
│   ├── WorkObject.cs / WorkObjectSection.cs / Contractor.cs
│   ├── Document.cs / DocumentType.cs / Comment.cs
│   ├── User.cs / AppRole.cs / Role.cs / RefreshToken.cs
│
├── 📁 Adis.Dal/                           # 🗄️ Data Access Layer
│   ├── Data/AppDbContext.cs, InitialData.cs
│   ├── Interfaces/ + Repositories/        # Generic + специфичные репозитории
│   ├── Specifications/                    # Specification pattern (12 шт.)
│   └── Migrations/                        # 24 миграции
│
├── 📁 Adis.Bll/                           # ⚙️ Business Logic Layer
│   ├── Services/                          # 9 сервисов (вкл. NeuralGuideService)
│   ├── Interfaces/                        # Контракты сервисов
│   ├── Dtos/                              # Auth / Project / Task / User / Comment / ...
│   ├── Profiles/                          # AutoMapper-профили
│   ├── Configurations/                    # JWT / Admin / Ollama
│   └── Initializers/AdminInitializer.cs
│
├── 📁 Adis.Api/                           # 🌐 Web API
│   ├── Controllers/                       # 8 контроллеров
│   ├── Program.cs
│   ├── Dockerfile
│   └── Properties/launchSettings.json
│
├── 📁 Adis.Tests/                         # 🧪 Интеграционные тесты
│   ├── AuthControllerIntegrationTests.cs
│   ├── ProjectsControllerIntegrationTests.cs
│   ├── UsersControllerIntegrationTests.cs
│   └── Helpers/CustomWebApplicationFactory.cs
│
├── 📁 adis.client/                        # 🖥️ Angular 19 SPA
│   ├── src/app/
│   │   ├── pages/                         # 8 страниц (проекты, задачи, нейропроводник…)
│   │   ├── components/                    # 13 диалогов и компонентов
│   │   ├── services/                      # 9 сервисов (auth, project, task, map…)
│   │   ├── models/ + core/guards/ + interceptors/ + directives/
│   │   └── environments/
│   ├── nginx.conf                         # SPA + proxy /docker → backend:8080
│   ├── docker-entrypoint.sh               # Runtime-конфиг Яндекс.Карт
│   └── Dockerfile
│
└── 📁 .github/workflows/                  # 🚢 CI/CD
    └── ci-cd.yml
```

---

<div align="center">
    <br>
    <sub>© 2026 — Информационная система управления проектами в строительстве</sub>
    <br>
    <sub>Made with ❤️, .NET 9, Angular и локальным LLM</sub>
</div>

