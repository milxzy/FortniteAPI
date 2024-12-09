


# Use the official .NET SDK image as the base image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Set the working directory inside the Docker container
WORKDIR /src
COPY *.sln .
# Copy the project file and restore dependencies
COPY ["FortniteAPI.csproj", "./"]
RUN dotnet restore "FortniteAPI.csproj"
# Copy the entire project into the container
COPY . .
# Publish the application to the /app directory
RUN dotnet publish "FortniteAPI.csproj" -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Expose the dynamic port
EXPOSE $PORT
# Copy the published output from the build stage
COPY --from=build /app/publish .
# Set the entry point for the application to dynamically use the PORT environment variable
CMD ["dotnet", "FortniteAPI.dll"]



