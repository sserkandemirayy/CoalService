# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution
COPY ../Domain/ ../Domain/
COPY ../Application/ ../Application/
COPY ../Infrastructure/ ../Infrastructure/
COPY ./ ./Api/

RUN dotnet restore ./Api/Api.csproj
RUN dotnet publish ./Api/Api.csproj -c Release -o /app/publish

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Api.dll"]
