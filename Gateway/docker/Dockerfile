FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Gateway/Directory.Build.props ./Gateway/
COPY Gateway/External.Web.Api.Gateway.sln ./Gateway/
COPY Gateway/src/External.Web.Api.Gateway.Domain/External.Web.Api.Gateway.Domain.csproj ./Gateway/src/External.Web.Api.Gateway.Domain/
COPY Gateway/src/External.Web.Api.Gateway.Application/External.Web.Api.Gateway.Application.csproj ./Gateway/src/External.Web.Api.Gateway.Application/
COPY Gateway/src/External.Web.Api.Gateway.Infrastructure/External.Web.Api.Gateway.Infrastructure.csproj ./Gateway/src/External.Web.Api.Gateway.Infrastructure/
COPY Gateway/src/External.Web.Api.Gateway.Api/External.Web.Api.Gateway.Api.csproj ./Gateway/src/External.Web.Api.Gateway.Api/
RUN dotnet restore Gateway/External.Web.Api.Gateway.sln
COPY Gateway/. ./Gateway/
RUN dotnet publish Gateway/src/External.Web.Api.Gateway.Api/External.Web.Api.Gateway.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "External.Web.Api.Gateway.Api.dll"]
