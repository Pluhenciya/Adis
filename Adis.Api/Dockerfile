# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Adis.Api/Adis.Api.csproj", "Adis.Api/"]
COPY ["Adis.Bll/Adis.Bll.csproj", "Adis.Bll/"]
COPY ["Adis.Dal/Adis.Dal.csproj", "Adis.Dal/"]
COPY ["Adis.Dm/Adis.Dm.csproj", "Adis.Dm/"] 
COPY ["Adis.Tests/Adis.Tests.csproj", "Adis.Tests/"]
RUN dotnet restore "./Adis.Api/Adis.Api.csproj"
RUN dotnet restore "Adis.Tests/Adis.Tests.csproj"
COPY . .

# Запуск тестов
RUN dotnet test "Adis.Tests/Adis.Tests.csproj"

WORKDIR "/src/Adis.Api"
RUN dotnet build "./Adis.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Adis.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["chmod", "-R", "777", "."]
ENTRYPOINT ["dotnet", "Adis.Api.dll"]