# Evidencias de los casos de la Persona B

Cada captura muestra la petición enviada (curl), el código de respuesta y el cuerpo devuelto por EquiposAPI.

| Nº | Caso | Resultado esperado | Resultado obtenido | Evidencia |
|---|---|---|---|---|
| 1 | Login correcto devuelve token con rol | 200 con token JWT; el payload incluye el rol y `exp` | 200. Devolvió token JWT, `expira`, `usuarioId: 1`, `username: admin` y `rol: Admin` | Caso1.png |
| 2 | Login con contraseña incorrecta | 401 | 401 con el mensaje "Usuario o contraseña incorrectos." | Caso2.png |
| 3 | Endpoint protegido sin token (`POST /api/Equipos` sin cabecera Authorization) | 401 | 401 con `www-authenticate: Bearer` | Caso3.png |
| 4 | Estudiante intenta crear un equipo | 403 | 403 al enviar el token de `estudiante1` a `POST /api/Equipos` | Caso4.png |
| 5 | Registrar usuario con Username repetido | 409 con mensaje claro | PENDIENTE: repetir `POST /api/auth/register` con `estudiante1` y capturar el 409 | Caso5.png |
| 10 | Estudiante solo ve sus propios préstamos aunque cambie parámetros en la URL | 200 y todos los préstamos son suyos (`?usuarioId=1` se ignora) | PENDIENTE: `GET /api/Prestamos` con token del estudiante (debe salir 1 préstamo) | Caso10.png |
| 11 | Token expirado en el cliente MVC | Redirige al login con mensaje de sesión expirada | Redirigió a `/Auth/Login` con el aviso "Tu sesión expiró. Inicia sesión de nuevo." (token configurado a 1 minuto) | Caso11.png |

## Cómo se probó el caso 11
1. `Jwt:ExpireMinutes` se configuró en `1` en los secretos de EquiposAPI.
2. Se inició sesión en EquiposMVC y se esperó más de un minuto.
3. Al navegar a otra pantalla, la API respondió 401; el cliente limpió la sesión y mostró el login con el aviso.
4. `Jwt:ExpireMinutes` se restauró a `60`.

## Carpeta Extras
Capturas de la preparación de datos de prueba (registro de `estudiante1`, creación del equipo LAP-001 y préstamos de ejemplo).
