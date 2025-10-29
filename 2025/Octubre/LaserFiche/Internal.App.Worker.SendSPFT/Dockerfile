FROM daviregistry.daviviendacr.com:32003/davivienda/framework/container/image/dotnet/sdk:8.0.410-alpine3.21-amd64 AS build
COPY . .
RUN dotnet restore "Internal.App.Worker.SendSPFT/Internal.App.Worker.SendSPFT.csproj" --disable-parallel --configfile ./nuget.config
RUN dotnet build "Internal.App.Worker.SendSPFT/Internal.App.Worker.SendSPFT.csproj" -c Release -o /app/build --no-restore
RUN dotnet publish "Internal.App.Worker.SendSPFT/Internal.App.Worker.SendSPFT.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM daviregistry.daviviendacr.com:32003/dotnet/aspnet:8.0.406-alpine3.21-amd64 AS final
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
COPY --from=build /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Internal.App.Worker.SendSPFT.dll"]
