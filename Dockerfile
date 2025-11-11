# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR C:\app\src

COPY Program.cs ./
COPY config.yml ./

# Create temporary project to build
RUN dotnet new web -n UltrakillApp --force && move Program.cs UltrakillApp\ && cd UltrakillApp && dotnet publish -c Release -o C:\app\publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022
WORKDIR C:\app

COPY --from=build C:\app\publish\ .\
COPY ULTRAKILL.exe .\
COPY config.yml .\

EXPOSE 8080
ENV START_SECRET=supersecret

ENTRYPOINT ["dotnet", "UltrakillApp.dll"]
