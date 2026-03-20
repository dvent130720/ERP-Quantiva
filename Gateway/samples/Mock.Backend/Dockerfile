FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Gateway/samples/Mock.Backend/Mock.Backend.csproj ./Gateway/samples/Mock.Backend/
RUN dotnet restore Gateway/samples/Mock.Backend/Mock.Backend.csproj
COPY Gateway/samples/Mock.Backend/. ./Gateway/samples/Mock.Backend/
RUN dotnet publish Gateway/samples/Mock.Backend/Mock.Backend.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mock.Backend.dll"]
