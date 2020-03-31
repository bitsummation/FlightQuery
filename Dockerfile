FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet build
RUN dotnet test --no-build --logger "trx;LogFileName=results.trx"

RUN dotnet publish FlightQuery.Web/ -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FlightQuery.Web.dll"]
