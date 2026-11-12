FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY MedicalSmartBox.API/MedicalSmartBox.API.csproj MedicalSmartBox.API/
RUN dotnet restore ./MedicalSmartBox.API/MedicalSmartBox.API.csproj
COPY . .
RUN dotnet publish ./MedicalSmartBox.API/MedicalSmartBox.API.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 10000
ENTRYPOINT ["dotnet", "MedicalSmartBox.API.dll"]
