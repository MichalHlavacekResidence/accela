FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

COPY "accela.csproj" .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "accela.dll"]

EXPOSE 80 443
