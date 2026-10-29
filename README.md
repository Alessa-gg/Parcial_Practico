# Sistema de Préstamo de Equipos de Laboratorio

Tres componentes que comparten una sola base de datos SQL Server (`EquiposDb`):

| Proyecto | Descripción |
|---|---|
| EquiposAPI | ASP.NET Core Web API (.NET 8), EF Core código primero, JWT, AutoMapper, Swagger |
| EquiposMVC | Cliente de operación (MVC + HttpClient + Session). Toda escritura pasa por la API |
| EquiposReportes | Módulo de reportes (MVC + EF Core base de datos primero). Solo lectura, usuario con `db_datareader` |

## Requisitos
.NET 8.0 SDK, Visual Studio 2022 o superior, SQL Server y SSMS.

## Configuración (cada integrante)
1. Clonar el repositorio y abrir `Equipos/Equipos.slnx`.
2. **Secretos de EquiposAPI** (clic derecho en el proyecto, *Administrar secretos de usuario*), o con `dotnet user-secrets set`:

```json
{
  "ConnectionStrings:DefaultConnection": "Server=...;Database=EquiposDb;Trusted_Connection=True;TrustServerCertificate=True",
  "Jwt:Key": "una-clave-de-al-menos-32-caracteres-xxxxxxxx",
  "Jwt:Issuer": "EquiposAPI",
  "Jwt:Audience": "EquiposClientes",
  "Jwt:ExpireMinutes": "60",
  "AdminSeed:Username": "admin",
  "AdminSeed:Email": "admin@equipos.local",
  "AdminSeed:Password": "<contraseña del Admin inicial>"
}
```
   `Jwt:Key` debe tener al menos 32 caracteres; la API no arranca si falta o es más corta. Para probar la expiración del token (caso 11) use `Jwt:ExpireMinutes` = `1`.
3. Crear la base de datos. En la Consola del Administrador de paquetes (proyecto EquiposAPI): `Update-Database`. Alternativa sin migraciones: ejecutar `Equipos/script.sql` en SSMS.
4. Crear el usuario de solo lectura: ejecutar `Equipos/reportes-usuario.sql` en SSMS (cambie la contraseña por una propia).
5. **Secretos de EquiposReportes:**

```json
{
  "ConnectionStrings:ReportesDb": "Server=...;Database=EquiposDb;User Id=reportes;Password=<la del paso 4>;TrustServerCertificate=True"
}
```
6. En `EquiposMVC/appsettings.json` y `EquiposReportes/appsettings.json`, poner en `ApiBaseUrl` la URL de la API (termina en `/api/`).

El Admin inicial se crea solo al arrancar la API cuando la tabla `Usuarios` está vacía (usa `AdminSeed:Password`).

### Cómo se generó el contexto de EquiposReportes
Base de datos primero, desde la consola del Administrador de paquetes (proyecto EquiposReportes), leyendo la cadena desde User Secrets para que no quede en `OnConfiguring`:

```powershell
Scaffold-DbContext "Name=ConnectionStrings:ReportesDb" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -ContextDir Data -Context EquiposDbContext -Tables Categorias,Equipos,Prestamos,Usuarios -Force
```

## Orden de arranque
SQL Server, EquiposAPI, EquiposMVC, EquiposReportes.

## Roles
| Acción | Admin | Docente | Estudiante |
|---|---|---|---|
| CRUD de categorías y equipos | Sí | No | No |
| Registrar usuarios | Sí | No | No |
| Registrar préstamo y devolución | Sí | Sí | No |
| Ver equipos disponibles | Sí | Sí | Sí |
| Ver préstamos | Todos | Todos | Solo los propios (Id tomado del token) |
| Reportes | Sí | No | No |

## Reglas de negocio
- Solo se prestan equipos Disponibles (409 si no).
- Fecha de vencimiento entre hoy y 7 días (400 si no).
- Préstamo y cambio de estado del equipo en una sola transacción.
- Devolver un préstamo deja el equipo Disponible.
- No se elimina un equipo con préstamos ni una categoría con equipos (409).
- Concurrencia optimista con RowVersion al editar equipos (409).
- Username repetido al registrar usuarios devuelve 409.

## Pruebas
- Colección de Postman: `Equipos/Equipos.postman_collection.json` (defina las variables `adminPass` y `estudiantePass` antes de ejecutar).
- Evidencias de los 12 casos: carpeta `Evidencias/`.

## Integrantes 
- Susana Nicole Valle Méndez
- Alessandra Guadalupe González Burgos
