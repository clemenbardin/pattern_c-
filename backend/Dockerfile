# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie des fichiers projet et restauration
COPY ["MonApp.API.csproj", "."]
RUN dotnet restore "MonApp.API.csproj"

# Copie du reste du code et build
COPY . .
RUN dotnet publish "MonApp.API.csproj" -c Release -o /app/publish

# Étape d'exécution
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copie depuis l'étape de build
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MonApp.API.dll"]
