# Sistema de Préstamo de Equipos de Laboratorio

## Proyectos
- EquiposAPI: Web API (.NET 8, EF Core, AutoMapper, Swagger)
- EquiposMVC: cliente web MVC que consume la API

## Requisitos
.NET 8.0 SDK, Visual Studio 2022 o superior, SQL Server y SSMS

## Configuración (cada integrante)
1. Clonar el repositorio y abrir Equipos/Equipos.slnx.
2. En EquiposAPI: clic derecho, Administrar secretos de usuario, y agregar
   ConnectionStrings:DefaultConnection y Jwt (Key, Issuer, Audience).
3. En la Consola del Administrador de paquetes (proyecto EquiposAPI): Update-Database
4. En EquiposMVC/appsettings.json, poner en ApiBaseUrl el puerto de la API (termina en /api/).
5. Alternativa sin migraciones: ejecutar script.sql en SSMS.

## Orden de arranque
SQL Server, EquiposAPI, EquiposMVC, EquiposReportes

## Reglas de negocio
- Solo se prestan equipos Disponibles (409 si no).
- Fecha de vencimiento entre hoy y 7 días (400 si no).
- Préstamo y cambio de estado del equipo en una sola transacción.
- Devolver un préstamo deja el equipo Disponible.
- No se elimina un equipo con préstamos ni una categoría con equipos (409).
- Concurrencia optimista con RowVersion al editar equipos (409).

## Integrantes 
- Susana Nicole Valle Méndez
- Alessandra Guadalupe González Burgos
