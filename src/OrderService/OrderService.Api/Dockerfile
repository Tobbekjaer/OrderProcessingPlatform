FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OrderService/OrderService.Api/OrderService.Api.csproj", "src/OrderService/OrderService.Api/"]
COPY ["src/OrderService/OrderService.Application/OrderService.Application.csproj", "src/OrderService/OrderService.Application/"]
COPY ["src/OrderService/OrderService.Domain/OrderService.Domain.csproj", "src/OrderService/OrderService.Domain/"]
COPY ["src/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/Shared/Shared.Contracts/"]
COPY ["src/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/OrderService/OrderService.Infrastructure/"]
RUN dotnet restore "src/OrderService/OrderService.Api/OrderService.Api.csproj"
COPY . .
WORKDIR "/src/src/OrderService/OrderService.Api"
RUN dotnet build "./OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OrderService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Api.dll"]
