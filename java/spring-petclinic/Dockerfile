# Use Eclipse Temurin 17 base image (official OpenJDK distribution)
FROM eclipse-temurin:17-jre

# Set working directory
WORKDIR /app

# Copy the jar file
COPY target/spring-petclinic-3.5.0-SNAPSHOT.jar app.jar

# Expose port 8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["java", "-jar", "app.jar"]
