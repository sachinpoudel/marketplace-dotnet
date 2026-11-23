FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/MarketPlace.API/MarketPlace.API.csproj src/MarketPlace.API/
COPY src/MarketPlace.Application/MarketPlace.Application.csproj src/MarketPlace.Application/
COPY src/MarketPlace.Domain/MarketPlace.Domain.csproj src/MarketPlace.Domain/
COPY src/MarketPlace.Infrastructure/MarketPlace.Infrastructure.csproj src/MarketPlace.Infrastructure/

RUN dotnet restore src/MarketPlace.API/MarketPlace.API.csproj

COPY src/ src/
WORKDIR /src/src/MarketPlace.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "MarketPlace.API.dll"]
