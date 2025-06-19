# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Install GStreamer runtime packages
RUN apt-get update -y && apt-get install -y --no-install-recommends \
    gstreamer1.0-tools \
    gstreamer1.0-plugins-base \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad \
    gstreamer1.0-plugins-ugly \
    gstreamer1.0-libav \
    libgstreamer1.0-0 \
    libgstreamer-plugins-base1.0-0 && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update -y
RUN apt-get install -y python3
RUN dotnet workload install wasm-tools

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ContentRatingAPI/ContentRatingAPI.csproj", "src/ContentRatingAPI/"]
COPY ["src/ContentRating.Domain/ContentRating.Domain.csproj", "src/ContentRating.Domain/"]
COPY ["src/ContentRating.Web.Contracts/ContentRating.Web.Contracts.csproj", "src/ContentRating.Web.Contracts/"]
COPY ["src/ContentRating.Web.UI/ContentRating.Web.UI.csproj", "src/ContentRating.Web.UI/"]
RUN dotnet restore "./src/ContentRatingAPI/ContentRatingAPI.csproj"
COPY . .
WORKDIR "/src/src/ContentRatingAPI"
RUN dotnet build "./ContentRatingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ContentRatingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContentRatingAPI.dll"]