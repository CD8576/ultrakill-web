# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MyWebApp.csproj", "./"]
RUN dotnet restore "MyWebApp.csproj"

# Copy everything else
COPY . .

# Publish the app
RUN dotnet publish "MyWebApp.csproj" -c Release -o /app/publish

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copy published app including the exe
COPY --from=build /app/publish ./

# Expose port
EXPOSE 80

# Run the app
ENTRYPOINT ["dotnet", "MyWebApp.dll"]
