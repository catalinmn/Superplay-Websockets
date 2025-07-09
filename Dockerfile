# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ONLY required .csproj files
COPY ["src/Server/Server.csproj", "src/Server/"]
COPY ["src/Core/Core.csproj", "src/Core/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

# Restore only the Server project (it will restore dependencies)
RUN dotnet restore "src/Server/Server.csproj"

# Copy ONLY the source code directories (not entire solution)
COPY ["src/Server/", "src/Server/"]
COPY ["src/Core/", "src/Core/"]
COPY ["src/Infrastructure/", "src/Infrastructure/"]

# Build and publish
WORKDIR "/src/src/Server"
RUN dotnet build "Server.csproj" -c Release --no-restore -o /app/build
RUN dotnet publish "Server.csproj" -c Release --no-restore -o /app/publish


# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]