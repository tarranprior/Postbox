FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Postbox.csproj ./ 
RUN dotnet restore "./Postbox.csproj"

COPY src/ .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

COPY .env .env
ENV DOTNET_ENV_PATH="/app/.env"
ENV POSTBOX_KEYS_PATH="/app/keys"

ENTRYPOINT ["dotnet", "Postbox.dll"]
