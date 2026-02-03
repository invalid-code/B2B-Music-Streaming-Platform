# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "B2BMusicStreamingPlatformwebapi.csproj" -c Release -o /app/publish

# Use the ASP.NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Command to run the application
ENTRYPOINT ["dotnet", "B2BMusicStreamingPlatformwebapi.dll"]
