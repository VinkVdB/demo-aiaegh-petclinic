# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project file and restore dependencies
COPY PetClinic/*.csproj ./
RUN dotnet restore

# Copy source code and build the application
COPY PetClinic/ ./
RUN dotnet publish -c Release -o out

# Use the official .NET 8 runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build-env /app/out .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5172

# Expose port 5172
EXPOSE 5172

# Create directory for SQLite database with proper permissions
RUN mkdir -p /app/data && chmod 755 /app/data

# Set the entry point
ENTRYPOINT ["dotnet", "PetClinic.dll"]
