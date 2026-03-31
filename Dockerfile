# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HealthCareBlog_Backend/HealthCareBlog_Backend.csproj", "HealthCareBlog_Backend/"]
RUN dotnet restore "HealthCareBlog_Backend/HealthCareBlog_Backend.csproj"

COPY . .
WORKDIR "/src/HealthCareBlog_Backend"
RUN dotnet publish "HealthCareBlog_Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HealthCareBlog_Backend.dll"]
