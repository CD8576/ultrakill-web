# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy entire project
COPY . .

# Restore and publish
RUN dotnet restore "MyWebApp.csproj"
RUN dotnet publish "MyWebApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port for Koyeb
EXPOSE 8080

# Environment variables for ASP.NET Core
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Run the app
ENTRYPOINT ["dotnet", "MyWebApp.dll"]
