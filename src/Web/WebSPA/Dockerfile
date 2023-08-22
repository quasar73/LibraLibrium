FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM node:16.20 as node-build
WORKDIR /web/src
COPY src/Web/WebSPA/Client/package.json .
COPY src/Web/WebSPA/Client/package-lock.json .
RUN npm install -g npm@8.5.5
RUN npm install
COPY src/Web/WebSPA/Client .
RUN npm run build:prod

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "LibraLibrium.sln" "LibraLibrium.sln"

COPY "src/Services/Books/Books.API/Books.API.csproj" "src/Services/Books/Books.API/Books.API.csproj"
COPY "src/Services/Catalog/Catalog.API/Catalog.API.csproj" "src/Services/Catalog/Catalog.API/Catalog.API.csproj"
COPY "src/Services/Catalog/Catalog.Core/Catalog.Core.csproj" "src/Services/Catalog/Catalog.Core/Catalog.Core.csproj"
COPY "src/Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj" "src/Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj"
COPY "src/Services/Identity/Identity.API/Identity.API.csproj" "src/Services/Identity/Identity.API/Identity.API.csproj"
COPY "src/Services/Trading/Trading.API/Trading.API.csproj" "src/Services/Trading/Trading.API/Trading.API.csproj"
COPY "src/Services/Trading/Trading.Domain/Trading.Domain.csproj" "src/Services/Trading/Trading.Domain/Trading.Domain.csproj"
COPY "src/Services/Trading/Trading.Infrastructure/Trading.Infrastructure.csproj" "src/Services/Trading/Trading.Infrastructure/Trading.Infrastructure.csproj"
COPY "src/Web/WebStatus/WebStatus.csproj" "src/Web/WebStatus/WebStatus.csproj"
COPY "src/Web/WebSPA/WebSPA.csproj" "src/Web/WebSPA/WebSPA.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "LibraLibrium.sln"

COPY . .
COPY --from=node-build /web/wwwroot /src/src/Web/WebSPA/wwwroot/
WORKDIR /src/src/Web/WebSPA
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSPA.dll"]