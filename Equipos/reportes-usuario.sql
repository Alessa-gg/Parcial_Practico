-- Inicio de sesión y usuario de SOLO LECTURA para EquiposReportes.
-- Ejecutar en SSMS con una cuenta administradora. Reemplace <CONTRASEÑA_SEGURA> por una
-- contraseña propia y guárdela únicamente en User Secrets de EquiposReportes (nunca en el repositorio).
-- Si su base de datos no se llama EquiposDb, cambie el nombre en las dos líneas indicadas.

USE [master];
GO
CREATE LOGIN [reportes] WITH PASSWORD = N'<CONTRASEÑA_SEGURA>', CHECK_POLICY = ON;
GO

USE [EquiposDb];   -- <- nombre de su base de datos
GO
CREATE USER [reportes] FOR LOGIN [reportes];
ALTER ROLE [db_datareader] ADD MEMBER [reportes];
GO

-- Verificación: debe devolver únicamente db_datareader
SELECT r.name AS rol
FROM sys.database_role_members m
JOIN sys.database_principals r ON r.principal_id = m.role_principal_id
JOIN sys.database_principals u ON u.principal_id = m.member_principal_id
WHERE u.name = N'reportes';
GO
