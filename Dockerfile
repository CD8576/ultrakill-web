# Step 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY MyWebApp.csproj .
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# Step 2: Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy published app including the .exe
COPY --from=build /app/publish .

# Expose port (Koyeb will map to it)
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_HOST_PATH=/usr/bin/dotnet

# Start the app
ENTRYPOINT ["dotnet", "MyWebApp.dll"]
