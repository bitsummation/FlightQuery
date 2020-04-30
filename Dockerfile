FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY . ./
RUN export CURRENT_TIMESTAMP=$(date '+%s') ; \
    sed -e "s/version_replace/${CURRENT_TIMESTAMP}/g" \
    FlightQuery.Web/Views/Shared/_Layout.cshtml > FlightQuery.Web/Views/Shared/_Layout.cshtml.tmp \
    && mv FlightQuery.Web/Views/Shared/_Layout.cshtml.tmp FlightQuery.Web/Views/Shared/_Layout.cshtml
RUN dotnet build
RUN dotnet test --no-build --logger "trx;LogFileName=results.trx"

RUN dotnet publish FlightQuery.Web/ -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FlightQuery.Web.dll"]
