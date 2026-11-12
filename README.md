# Medical SMARTBOX — Backend RESTful API & IoT Platform

Backend RESTful de la plataforma **Medical SMARTBOX**, orientada al monitoreo en tiempo real, supervisión de telemetría IoT, planificación de transporte médico asistencial y trazabilidad de la cadena de custodia en el traslado de órganos, tejidos y componentes biológicos. Proyecto desarrollado por el equipo **NeonCode** para el curso de **Aplicaciones Web (1ASI0730)** de la **Universidad Peruana de Ciencias Aplicadas (UPC)**.

La aplicación está construida sobre **.NET 10** y **C# 13**, implementando una arquitectura basada en **Domain-Driven Design (DDD)** con separación estricta de responsabilidades por Bounded Contexts, persistencia relacional transaccional mediante **Entity Framework Core** sobre **MySQL / TiDB Serverless**, y documentación interactiva mediante **OpenAPI** y **Scalar**.

---

## Tecnologías Utilizadas

- **.NET 10 & C# 13:** Plataforma de desarrollo y lenguaje moderno con soporte de registros (`record`), tipos fuertemente tipados y operaciones asíncronas de alto rendimiento.
- **ASP.NET Core:** Framework web para la exposición de endpoints RESTful con serialización JSON uniforme y convención de rutas kebab-case.
- **Entity Framework Core 9:** Mapeador objeto-relacional (ORM) para la persistencia de agregados de dominio y ejecución de migraciones automáticas.
- **MySQL / TiDB Serverless:** Motor de base de datos relacional compatible con MySQL para almacenamiento persistente y transaccional en la nube.
- **Scalar & OpenAPI:** Generación de especificación OpenAPI v3 y catálogo interactivo de documentación para pruebas de endpoints directamente desde el navegador.
- **Docker:** Contenerización multi-stage para compilación reproducible y generación de imágenes de producción ligeras.
- **Render:** Plataforma de alojamiento en la nube con despliegue automatizado basado en infraestructura como código (`render.yaml`).
- **TomTom Routing API:** Integración con servicio externo de geolocalización para el cálculo de distancias, rutas polilínea y tiempo estimado de llegada (ETA) con tráfico en tiempo real.

---

## Arquitectura del Proyecto

El código fuente (`MedicalSmartBox.API/`) se encuentra estructurado bajo principios de **Domain-Driven Design (DDD)**, desacoplando la lógica de negocio en Bounded Contexts autónomos. Cada contexto implementa un esquema de 4 capas:

```text
MedicalSmartBox.API/
├── Alerting/                 # Bounded Context: Detección y respuesta ante incidentes críticos
│   ├── Domain/               # Agregados, entidades, value objects, enums y contratos de repositorios
│   ├── Application/          # Servicios de comando y consulta (orquestación de casos de uso)
│   ├── Infrastructure/       # Persistencia con EF Core y mapeo relacional
│   └── Interfaces/REST/      # Controladores API, DTOs (Resources) y Assemblers
├── Custody/                  # Bounded Context: Cadena de custodia y trazabilidad digital
│   ├── Domain/               # Agregados de transferencias, lecturas térmicas y manifiestos de auditoría
│   ├── Application/          # Lógica de emisión/validación de OTP y sellado criptográfico
│   ├── Infrastructure/       # Repositorio MySQL y generación de reportes oficiales en PDF
│   └── Interfaces/Rest/      # Endpoints de transferencias, manifiestos y eventos de auditoría
├── SmartContainers/          # Bounded Context: Inventario y gestión de contenedores IoT
│   ├── Domain/               # Entidades de SmartContainer, calibración térmica y pesos tara/neto
│   ├── Application/          # Comandos de emparejamiento con ambulancias y movimientos de peso
│   ├── Infrastructure/       # Repositorio relacional y mapeo de estados operativos
│   └── Interfaces/Rest/      # Controladores de inventario, telemetría en agregados y vinculación
├── Telemetry/                # Bounded Context: Ingestión de telemetría IoT de alta frecuencia
│   ├── Domain/               # Modelos de series de tiempo de sensores ESP32
│   ├── Application/          # Servicios de persistencia y consultas cronológicas
│   ├── Infrastructure/       # Repositorio optimizado para lecturas continuas
│   └── Interfaces/Rest/      # Endpoints de ingesta de snapshots y consultas por contenedor/viaje
├── Transport/                # Bounded Context: Planificación de transporte y despacho
│   ├── Domain/               # Órdenes de transporte, viajes de despacho y reglas de isquemia
│   ├── Application/          # Asignación de recursos, control de pre-enfriamiento y geocercas
│   ├── Infrastructure/       # Repositorios EF Core y cliente HTTP de tráfico TomTom
│   └── Interfaces/Rest/      # Endpoints de órdenes clínicas y control de viaje
└── Shared/                   # Kernel compartido
    ├── Domain/               # Interfaces base de repositorios y unidad de trabajo (Unit of Work)
    └── Infrastructure/       # AppDbContext, convenciones kebab-case, manejo global de errores y CORS
```

---

## Bounded Contexts

- **Medical Transport Planning & Dispatching (`Transport/`):** Creación de órdenes de transporte médico con hospitales de origen y destino, clasificación clínica de carga biológica, umbrales de isquemia fría, asignación de recursos operativos (ambulancia, contenedor, conductor, paramédico), verificación de pre-enfriamiento y confirmación de llegada dentro de la geocerca hospitalaria de 100 metros.
- **Smart Container Management (`SmartContainers/`):** Inventario de cajas isotérmicas inteligentes, registro por número de serie de hardware, vinculación y desvinculación a placas de ambulancias activas, y auditoría de movimientos de peso neto al introducir o retirar unidades biológicas.
- **IoT Telemetry Monitoring (`Telemetry/`):** Ingestión y persistencia de series de tiempo transmitidas por dispositivos ESP32, registrando temperatura de cámara, temperatura exterior, peso, nivel de batería, alimentación auxiliar vehicular de 12V, potencia Peltier, estado de cerradura electromecánica, coordenadas GPS y firma de firmware.
- **Critical Alerting & Incident Response (`Alerting/`):** Monitoreo continuo y detección automática de excursiones térmicas fuera del rango crítico (2.0°C a 8.0°C), pérdida de energía vehicular y sospechas de manipulación. Enrutamiento de acuse de recibo de operadores, escalamiento a directores médicos y registro de mitigación con firma de calidad.
- **Chain of Custody & Traceability (`Custody/`):** Trazabilidad inmutable de extremo a extremo, registro de participantes, bitácora de lecturas térmicas en tránsito, generación y validación de tokens OTP efímeros con HMAC-SHA256, y generación de actas de auditoría con sellado digital SHA-256 según norma técnica del MINSA.
- **Shared Infrastructure (`Shared/`):** Kernel transversal que provee la conexión a base de datos compartida, migraciones automáticas al iniciar, enrutamiento kebab-case, middleware de errores RFC 9110, CORS permisivo para clientes web y redirección de raíz hacia la documentación Scalar.

---

## Requisitos Previos

- **.NET SDK:** Versión 10.0 o superior instalada localmente.
- **Base de Datos:** Instancia activa de MySQL 8.0+ o TiDB Serverless accesible.
- **Git:** Versión 2.30 o superior para clonación y control de versiones.
- **Docker (opcional):** Para compilación y ejecución mediante contenedores locales.

---

## Instalación y Puesta en Marcha

### 1. Clonar el repositorio y posicionarse en la rama develop
```bash
git clone https://github.com/NeonCode-UPC/backend.git
cd backend
git checkout develop
```

### 2. Restaurar dependencias del proyecto
```bash
dotnet restore
```

### 3. Configurar la cadena de conexión
En entorno de desarrollo local, configure la cadena de conexión mediante .NET User Secrets:

```bash
cd MedicalSmartBox.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MedicalSmartBox" "server=127.0.0.1;port=3306;database=medical_smartbox;user=root;password=tu_password;"
cd ..
```

*Alternativamente, puede establecer la variable de entorno del sistema `ConnectionStrings__MedicalSmartBox`.*

### 4. Compilar la solución
```bash
dotnet build
```

### 5. Iniciar la API
```bash
dotnet run --project MedicalSmartBox.API
```

Al iniciar la aplicación, Entity Framework Core aplicará automáticamente las migraciones pendientes en la base de datos MySQL configurada.

La API y su documentación interactiva estarán disponibles en:
- **Documentación Interactiva Scalar:** `http://localhost:5000/scalar` (o `/scalar/v1`)
- **Health Check:** `http://localhost:5000/health`
- **Especificación OpenAPI:** `http://localhost:5000/openapi/v1.json`

---

## Scripts del Proyecto

| Comando | Descripción |
| :--- | :--- |
| `dotnet restore` | Restaura los paquetes NuGet y dependencias del proyecto. |
| `dotnet build` | Compila la solución completa y verifica la ausencia de errores o advertencias. |
| `dotnet run --project MedicalSmartBox.API` | Inicia el servidor Kestrel en modo desarrollo local. |
| `dotnet publish ./MedicalSmartBox.API -c Release -o out` | Genera los binarios compilados y optimizados para producción. |
| `docker build -t medical-smartbox-api .` | Construye la imagen Docker multi-stage de producción. |
| `docker run -p 10000:10000 medical-smartbox-api` | Ejecuta el contenedor Docker localmente exponiendo el puerto 10000. |

---

## Variables de Entorno

| Variable | Descripción | Valor por Defecto / Ejemplo |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Define el entorno de ejecución (`Development`, `Production`). | `Production` en Render |
| `PORT` | Puerto HTTP en el que la aplicación escucha peticiones. | `10000` (Render) o `5000` (local) |
| `OpenApi__ExposeInProduction` | Habilita la exposición de Scalar y OpenAPI en entornos de producción. | `"true"` |
| `Custody__OtpHmacKey` | Clave secreta para la generación y validación de hashes OTP con HMAC-SHA256. | Generada por Render o clave local |
| `Custody__ExposeOtpForTesting` | Expone el código OTP en la respuesta de recepción para pruebas interactivas. | `"true"` |
| `ConnectionStrings__MedicalSmartBox` | Cadena de conexión hacia el servidor MySQL o clúster TiDB Serverless. | *(Requerida)* |
| `Cors__AllowedOrigins` | Orígenes HTTP/HTTPS autorizados para realizar solicitudes CORS. | Permisivo por defecto si no se especifica |

---

## Despliegue en Render

El repositorio incluye el manifiesto de infraestructura como código [render.yaml](./render.yaml) configurado para el plan gratuito de Render con despliegue Docker automático:

- **Servicio Web:** `medical-smartbox-api`
- **Runtime:** `docker` (utiliza el [Dockerfile](./Dockerfile) multi-stage)
- **Ruta de Health Check:** `/health`
- **URL de Producción:** `https://medical-smartbox-api.onrender.com`
- **Documentación en Producción:** `https://medical-smartbox-api.onrender.com/scalar`

---

## Flujo de Trabajo y Convenciones

### GitFlow
El equipo trabaja bajo el modelo de ramificación GitFlow:

- `main`: Rama de producción que almacena versiones estables y liberadas.
- `develop`: Rama central de integración continua.
- `feature/<nombre>`: Ramas de trabajo creadas a partir de `develop` para la implementación de cada historia de usuario o componente. Una vez culminadas, se integran mediante Pull Request con revisión de código hacia `develop`.

### Conventional Commits
Los mensajes de confirmación siguen el estándar de Conventional Commits con el formato `<tipo>(<alcance>): <descripción>`:

- **Tipos:** `feat`, `fix`, `docs`, `style`, `refactor`, `chore`.
- **Alcances sugeridos:** `transport`, `smartcontainers`, `telemetry`, `alerting`, `custody`, `shared`, `deploy`.
- **Ejemplos:**
  - `feat(transport): implement hospital arrival geofence validation`
  - `fix(alerting): attach string enum converters to incident resource`
  - `docs(scalar): enrich openapi introduction and tag descriptions`

---

## Información del Equipo

- **Equipo:** NeonCode
- **Curso:** Aplicaciones Web (1ASI0730) — NRC 8150
- **Docente:** Velásquez Núñez, Ángel Augusto
- **Periodo:** 2026-2

### Integrantes

| Integrante | Código |
| :--- | :--- |
| Espinoza Flores, Aaron André | u202222859 |
| Gargate Paredes, Santiago | u20211b556 |
| Jaramillo Mayta, Jhon Jordy | u202520310 |
| Munayco Apolaya, Maria Luisa | u20231c995 |
| Santos Minaya, Renzo Piero | u202114790 |
