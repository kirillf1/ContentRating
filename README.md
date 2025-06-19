# ContentRating - Система Оценки Контента (Документация сгенерирована при помощи ИИ, могут быть ошибки)

ContentRating - это веб-приложение для создания и групповой оценки контента. Система позволяет создавать топы контента, приглашать участников для совместной оценки и анализировать результаты.

## 🚀 Основные возможности

### 📋 Редакторы топов
- **Создание топов контента** - создавайте списки для последующей оценки
- **Управление редакторами** - приглашайте и удаляйте редакторов
- **Загрузка файлов** - поддержка различных типов медиа-контента
- **Импорт из YouTube** - автоматический импорт плейлистов из YouTube
- **Коллаборативное редактирование** - работа в режиме реального времени через SignalR

### 🎯 Групповая оценка
- **Комнаты для оценки** - создание пространств для совместного оценивания
- **Приглашение участников** - добавление оценщиков в комнаты
- **Настраиваемая шкала оценок** - гибкие диапазоны рейтингов (0-10)
- **Mock-пользователи** - возможность тестирования с виртуальными участниками
- **Анализ результатов** - визуализация и статистика оценок

### 🔧 Административные функции
- **Управление пользователями** - контроль доступа и ролей
- **Удаление контента** - модерация и управление списками
- **Настройки комнат** - изменение параметров оценки
- **Мониторинг активности** - отслеживание действий пользователей

## 🛠 Технологии

### Backend (.NET 9.0)
- **ASP.NET Core 9.0** - основной веб-фреймворк
- **MongoDB** - NoSQL база данных с replica set
- **SignalR** - real-time коммуникация
- **MediatR** - CQRS и медиатор паттерн
- **JWT Authentication** - авторизация с токенами
- **Google OAuth 2.0** - аутентификация через Google
- **FluentValidation** - валидация данных
- **Ardalis.Result** - результирующие типы
- **OpenTelemetry** - телеметрия и мониторинг
- **Serilog** - структурированное логирование

### Frontend (Blazor WebAssembly)
- **Blazor WebAssembly** - клиентский фреймворк
- **MudBlazor** - UI компоненты Material Design
- **SignalR Client** - real-time обновления
- **JWT Tokens** - клиентская авторизация
- **PWA** - прогрессивное веб-приложение

### Инфраструктура
- **Docker & Docker Compose** - контейнеризация
- **MongoDB Replica Set** - высокодоступная БД
- **FFmpeg** - обработка медиа-файлов
- **Grafana Loki** - агрегация логов
- **OTLP** - экспорт телеметрии

## 🔐 Google OAuth 2.0 Аутентификация

### Настройка Google Console
1. Перейдите в [Google Cloud Console](https://console.cloud.google.com/)
2. Создайте новый проект или выберите существующий
3. Включите Google+ API
4. Создайте OAuth 2.0 Client ID:
   - **Application type**: Web application
   - **Authorized redirect URIs**: `{ваш адрес}/accounts/signin-google`
   - **Authorized JavaScript origins**: `{ваш адрес}`

### Переменные окружения
```bash
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
JWT_ISSUER=ContentRating
JWT_AUDIENCE=ContentRatingUsers
```

### Процесс аутентификации
1. **Инициация входа** - пользователь нажимает "Войти через Google"
2. **Перенаправление** - система перенаправляет на Google OAuth
3. **Авторизация** - пользователь подтверждает доступ к профилю и YouTube
4. **Обратный вызов** - Google возвращает код авторизации
5. **Обмен токенов** - система обменивает код на access_token
6. **Создание JWT** - генерируется внутренний JWT токен
7. **Сохранение данных** - пользователь сохраняется в MongoDB

### Области доступа (Scopes)
- `profile` - базовая информация профиля
- `email` - адрес электронной почты
- `https://www.googleapis.com/auth/youtube.readonly` - чтение YouTube плейлистов

### Архитектура безопасности
```
[Frontend] --JWT--> [Backend API] --Access Token--> [YouTube API]
    |                     |
    ↓                     ↓
[Secure Storage]    [MongoDB Users]
```

## 🐳 Развертывание

### Быстрый старт с Docker Compose

1. **Клонируйте репозиторий:**
```bash
git clone <repository-url>
cd ContentRating
```

2. **Создайте файл `.env`:**
```bash
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
JWT_ISSUER=ContentRating
JWT_AUDIENCE=ContentRatingUsers
```

3. **Создайте директории и ключи:**
```bash
mkdir -p keys uploads mongo_data
# Создайте RSA ключ для JWT
openssl genrsa -out keys/jwtKey 2048
```

4. **Запустите контейнеры:**
```bash
docker-compose up -d
```

5. **Проверьте состояние:**
```bash
docker-compose ps
```

Приложение будет доступно по адресу: `http://localhost:8080/content-rating/`

### Локальная разработка

1. **Установите зависимости:**
   - .NET 9.0 SDK
   - MongoDB (replica set)
   - FFmpeg

2. **Настройте MongoDB replica set:**
```bash
mongod --replSet rs0 --port 27017
mongo --eval "rs.initiate()"
```

3. **Настройте конфигурацию:**
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your_client_id",
      "ClientSecret": "your_client_secret"
    },
    "JWT": {
      "AsymmetricKeyPath": "path/to/jwtKey",
      "Issuer": "ContentRating",
      "Audience": "ContentRatingUsers"
    }
  }
}
```

4. **Запустите проект:**
```bash
dotnet run --project src/ContentRatingAPI
```

## 📊 Мониторинг и наблюдаемость

### Логирование
- **Serilog** с структурированными логами
- **Grafana Loki** для агрегации
- **Контекстные логи** для запросов

### Метрики
- **OpenTelemetry** инструментация
- **ASP.NET Core** метрики
- **MongoDB** драйвер метрики
- **Пользовательские счетчики**

### Трассировка
- **Distributed tracing** через OpenTelemetry
- **HTTP** запросы трассировка
- **SignalR** соединения мониторинг

## 🏗 Архитектура

### Доменная архитектура (DDD)
```
src/
├── ContentRatingAPI/           # Web API + SignalR Hubs
├── ContentRating.Domain/       # Доменная логика
├── ContentRating.Web.UI/       # Blazor WASM Frontend
└── ContentRating.Web.Contracts/ # Контракты API
```

### Агрегаты
- **ContentEstimationListEditor** - управление топами контента
- **ContentPartyEstimationRoom** - комнаты для групповой оценки  
- **ContentPartyRating** - рейтинги и оценки

### Паттерны
- **CQRS** через MediatR
- **Domain Events** для межагрегатной коммуникации
- **Repository Pattern** для доступа к данным
- **Specification Pattern** для бизнес-правил

## 🔄 Real-time возможности

### SignalR Hubs
- **ContentEstimationListEditorHub** - коллаборативное редактирование
- **ContentPartyEstimationHub** - групповая оценка в реальном времени

### События в реальном времени
- Добавление/изменение/удаление контента
- Приглашение/исключение участников
- Обновление оценок
- Завершение оценивания

## 🔧 Консоль управления

### Возможности для администраторов
1. **Управление топами:**
   - Создание новых топов контента
   - Редактирование существующих списков
   - Приглашение и исключение редакторов
   - Загрузка файлов различных форматов

2. **Управление комнатами оценки:**
   - Создание комнат для групповой оценки
   - Настройка диапазонов рейтингов (0-10)
   - Приглашение оценщиков
   - Контроль mock-пользователей

3. **Мониторинг активности:**
   - Просмотр всех созданных топов
   - Отслеживание активных комнат оценки
   - Статистика участников
   - Анализ результатов оценки

### Панель управления редакторами
- **Приглашение новых редакторов** по email
- **Удаление редакторов** из топов (только создатель)
- **Просмотр активности** редакторов
- **Управление правами доступа**

### Управление контентом
- **Загрузка файлов:** поддержка видео, аудио, изображений
- **YouTube интеграция:** импорт плейлистов через YouTube API
- **Валидация контента:** автоматическая проверка ссылок и файлов
- **Удаление контента:** с подтверждением и уведомлениями

## 📈 API Endpoints

### Аутентификация
- `GET /accounts/login-google` - инициация входа через Google
- `GET /accounts/signin-google` - обработка callback от Google
- `POST /accounts/refresh-token` - обновление JWT токена

### Топы контента
- `GET /api/content-estimation-list-editor` - список топов пользователя
- `POST /api/content-estimation-list-editor` - создание нового топа
- `GET /api/content-estimation-list-editor/{id}` - получение топа
- `POST /api/content-estimation-list-editor/{id}/content` - добавление контента
- `POST /api/content-estimation-list-editor/{id}/editor` - приглашение редактора

### Групповая оценка
- `GET /api/content-party-estimation-room` - список комнат
- `POST /api/content-party-estimation-room` - создание комнаты
- `GET /api/content-party-estimation-room/{id}` - получение комнаты
- `POST /api/content-party-estimation-room/{id}/estimate` - отправка оценки
- `PUT /api/content-party-estimation-room/{id}/complete-estimation` - завершение оценки

## 🤝 Вклад в проект

1. Fork репозиторий
2. Создайте feature branch
3. Внесите изменения
4. Добавьте тесты
5. Создайте Pull Request

## 📄 Лицензия

Проект распространяется под лицензией MIT. См. файл [LICENSE.txt](LICENSE.txt) для подробностей.