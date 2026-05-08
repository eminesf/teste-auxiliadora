# Estágio 1 — build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia a solution e todos os .csproj juntos
COPY RentalPipeline.slnx .
COPY src/RentalPipeline.Domain/RentalPipeline.Domain.csproj src/RentalPipeline.Domain/
COPY src/RentalPipeline.Application/RentalPipeline.Application.csproj src/RentalPipeline.Application/
COPY src/RentalPipeline.Infrastructure/RentalPipeline.Infrastructure.csproj src/RentalPipeline.Infrastructure/
COPY src/RentalPipeline.API/RentalPipeline.API.csproj src/RentalPipeline.API/
COPY tests/RentalPipeline.Tests/RentalPipeline.Tests.csproj tests/RentalPipeline.Tests/

# Restaura todos os projetos da solution de uma vez
RUN dotnet restore RentalPipeline.slnx

# Copia o restante do código e publica
COPY src/ src/
RUN dotnet publish src/RentalPipeline.API/RentalPipeline.API.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RentalPipeline.API.dll"]