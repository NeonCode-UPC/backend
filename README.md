# Medical SMARTBOX — Backend API

Backend Web API de la plataforma **Medical SMARTBOX**, orientada al monitoreo en tiempo real, supervisión de telemetría IoT y trazabilidad de la cadena de custodia en el transporte médico asistencial. Proyecto desarrollado por el equipo **NeonCode** para el curso de **Aplicaciones Web (1ASI0730)** de la Universidad Peruana de Ciencias Aplicadas (UPC).

Construido sobre **ASP.NET Core (.NET 10)** implementando una arquitectura orientada a **Domain-Driven Design (DDD)**.

---

## Tecnologías Utilizadas

- **.NET 10 / C# 14**: Plataforma y lenguaje base para el desarrollo de servicios RESTful.
- **ASP.NET Core Web API**: Framework para la construcción de controladores y endpoints HTTP.
- **Entity Framework Core 10**: ORM para persistencia y mapeo objeto-relacional.
- **MySQL (MySql.EntityFrameworkCore)**: Motor de base de datos relacional.
- **Scalar / OpenAPI**: Documentación y exploración interactiva de endpoints.
- **Humanizer**: Normalización y convenciones de nomenclatura.

---

## Bounded Contexts

El sistema está estructurado bajo principios de Domain-Driven Design (DDD) dividido en los siguientes contextos delimitados:

1. **Smart Container & Telemetry Monitoring** (`telemetry`)
2. **Medical Transport Planning & Dispatching** (`transport`)
3. **Critical Alerting & Incident Response** (`alerting`)
4. **Chain of Custody & Traceability** (`custody`)
5. **Identity & Access Management** (`iam`)

---

## Estructura de la Solución

```text
backend/
├── MedicalSmartBox.sln
├── global.json
├── .gitignore
├── README.md
├── CHANGELOG.md
├── LICENSE.md
└── MedicalSmartBox.API/
    ├── MedicalSmartBox.API.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── MedicalSmartBox.API.http
    └── Properties/
        └── launchSettings.json
```
