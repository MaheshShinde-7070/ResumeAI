# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["ResumeAI.API/ResumeAI/ResumeAI.csproj", "ResumeAI.API/ResumeAI/"]

RUN dotnet restore "ResumeAI.API/ResumeAI/ResumeAI.csproj"

COPY . .

WORKDIR "/src/ResumeAI.API/ResumeAI"

RUN dotnet publish "ResumeAI.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet ResumeAI.dll --urls http://0.0.0.0:${PORT:-10000}"]