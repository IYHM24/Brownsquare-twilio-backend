# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar los archivos .csproj de todos los proyectos
COPY ["Web/Brownsquare-twilio-backend.csproj", "Web/"]
COPY ["Bussiness/Bussiness/Bussiness.csproj", "Bussiness/Bussiness/"]
COPY ["Shared/Dto/Dto.csproj", "Shared/Dto/"]
COPY ["Utils/Utils/Utils.csproj", "Utils/Utils/"]

# Restaurar dependencias
RUN dotnet restore "Web/Brownsquare-twilio-backend.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar el proyecto
WORKDIR "/src/Web"
RUN dotnet build "Brownsquare-twilio-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Brownsquare-twilio-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
USER root
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Brownsquare-twilio-backend.dll"]