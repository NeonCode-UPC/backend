# Critical Alerting & Incident Response: primer avance

Este avance implementa el registro y la consulta de incidencias. Utiliza el modelo de dominio, servicios de aplicacion y persistencia con Entity Framework Core y MySQL.

## Alcance actual

- Crear un incidente con identificadores de traslado y contenedor, tipo, severidad y mediciones opcionales.
- Consultar un incidente por identificador y listar los incidentes activos.
- Validar identificadores, clasificaciones y porcentaje de bateria.
- Registrar fechas de auditoria y mantener una migracion inicial para la tabla de incidentes.

La integracion con el frontend todavia no esta implementada. Tampoco se han implementado el acuse de recibo, la resolucion, el historial completo, reglas de umbrales, el despacho de notificaciones, autenticacion ni autorizacion. No usar esta API para datos sensibles o operaciones reales sin implementar y validar esos controles.

## Configuracion local

Se requiere el SDK indicado en `global.json`, un servidor MySQL y una base de datos de desarrollo. Configurar la cadena de conexion mediante user secrets o una variable de entorno; no subir credenciales al repositorio. En PowerShell, para la sesion actual:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=localhost;Port=3306;Database=medicalsmartbox;User=<usuario>;Password=<clave>;'
```

Con una version compatible de `dotnet-ef` instalada, desde la raiz del repositorio se puede aplicar la migracion a una base de desarrollo:

```powershell
dotnet ef database update --project .\MedicalSmartBox.API\MedicalSmartBox.API.csproj
dotnet run --project .\MedicalSmartBox.API\MedicalSmartBox.API.csproj
```

Revisar primero la base y la cadena de conexion; este comando modifica el esquema de la base configurada. La migracion no fue aplicada automaticamente por el proyecto.

## Endpoints iniciales

- `POST /api/v1/incidents`: registra un incidente y devuelve `201 Created`.
- `GET /api/v1/incidents/{id}`: devuelve el incidente o `404` si no existe.
- `GET /api/v1/incidents/active`: lista incidentes cuyo estado no es `resolved`.

Ejemplo de solicitud para crear un incidente:

```json
{
  "tripId": "8dab92a3-c580-44ba-b182-008966042b83",
  "containerId": "33a26df8-e1b3-489b-ae9e-0840c39f254d",
  "incidentType": "coldChainBreachHigh",
  "severity": "criticalEmergency",
  "triggerTemperatureCelsius": 9.5,
  "triggerBatteryPercentage": 70
}
```

Los nombres y estados siguen el diagrama de dominio del informe. El frontend actual emplea otros identificadores y nombres de tipo/estado, por lo que se necesitara acordar un contrato y adaptar la integracion.

## Pruebas

```powershell
dotnet test .\MedicalSmartBox.sln
```

Las pruebas de persistencia y de la API usan SQLite en memoria; no sustituyen una prueba de integracion contra MySQL.