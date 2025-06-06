# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Customers.Management.Consumer/Customers.Management.Consumer.csproj", "src/Customers.Management.Consumer/"]
COPY ["src/Customers.Management.Domain/Customers.Management.Domain.csproj", "src/Customers.Management.Domain/"]
COPY ["src/Customers.Management.Infra/Customers.Management.Infra.csproj", "src/Customers.Management.Infra/"]
RUN dotnet restore "./src/Customers.Management.Consumer/Customers.Management.Consumer.csproj"
COPY . .
WORKDIR "/src/src/Customers.Management.Consumer"
RUN dotnet build "./Customers.Management.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Customers.Management.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customers.Management.Consumer.dll"]