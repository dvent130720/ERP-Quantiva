FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore ERPSystem.API/ERPSystem.API.csproj
RUN dotnet publish ERPSystem.API/ERPSystem.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5012

COPY --from=build /app/publish .

EXPOSE 5012

ENTRYPOINT ["dotnet", "ERPSystem.API.dll"]
