# Sistema de Registro y Análisis de Gastos Personales
Proyecto final desarrollado para la asignatura de Programación II. Es una aplicación web para el control de finanzas personales, presupuestos mensuales y generación de reportes, construida con ASP.NET Core Web API, Entity Framework Core y un frontend web integrado.
---
## Arquitectura del Proyecto
El sistema está organizado en una arquitectura de 4 capas para separar las responsabilidades:
1. **GastosPersonales.Domain:** Contiene las entidades principales (Usuario, Gasto, Categoria, MetodoPago, Presupuesto) y las interfaces de los repositorios. No depende de ninguna otra capa.
2. **GastosPersonales.Application:** Contiene la lógica de negocio, los DTOs de entrada y salida, los servicios (AuthService, GastoService, ReporteService, etc.) y las estrategias de exportación.
3. **GastosPersonales.Infrastructure:** Implementa el acceso a datos con Entity Framework Core (ApplicationDbContext), las consultas a SQL Server, la encriptación de contraseñas con BCrypt, la generación de tokens JWT y la lectura de archivos CSV.
4. **GastosPersonales.API:** Es el punto de entrada de la aplicación. Contiene los controladores REST, el middleware de manejo de errores, la configuración de inyección de dependencias y los archivos web del frontend en la carpeta `wwwroot`.
---
## Patrones de Diseño Utilizados
- **Patrón Strategy:** Se utilizó para la exportación de reportes (`CsvExportStrategy`, `TxtExportStrategy`, `JsonExportStrategy` implementando `IExportStrategy`). Permite cambiar el formato de exportación de forma dinámica sin modificar los controladores.
- **Patrón Factory:** Implementado en `ExportStrategyFactory` para resolver la estrategia de exportación correcta según el formato solicitado por el usuario (CSV, TXT o JSON).
- **Patrón Repository:** Utilizado para desacoplar el acceso a la base de datos de la lógica de negocio mediante interfaces como `IGastoRepository`, `IUsuarioRepository`, etc.
- **Inyección de Dependencias (IoC):** Todos los servicios, repositorios y utilidades están registrados en el contenedor de dependencias en `Program.cs` usando `AddScoped`.
---
## Funcionalidades Principales
- **Autenticación y Seguridad:** Registro y login con contraseñas encriptadas con BCrypt y autenticación mediante tokens JWT.
- **Seeding Automático:** Al registrar un usuario, se crean automáticamente 5 categorías por defecto (Comida, Transporte, Servicios, Entretenimiento, Otros) y 3 métodos de pago (Efectivo, Tarjeta, Transferencia).
- **Control de Gastos y Presupuestos:** Registro de gastos diarios y asignación de presupuestos mensuales por categoría con alertas automáticas al alcanzar el 50%, 80% y 100% del límite.
- **Importación Masiva:** Carga de gastos mediante archivos CSV utilizando `StreamReader` nativo de C#.
- **Exportación de Reportes:** Descarga de reportes en formatos Excel (CSV), TXT y JSON.
- **Manejo Global de Excepciones:** `ExceptionMiddleware` para capturar errores no controlados y responder con formato JSON estandarizado (error 500).
- **Frontend SPA:** Interfaz web en HTML, CSS y JavaScript ubicada en `wwwroot`, con gráficos de gastos usando Chart.js y consumo de la API con `fetch`.
---
## Estructura de la Base de Datos
- **Usuarios:** Id, Nombre, Email, PasswordHash, FechaCreacion.
- **Categorias:** Id, UsuarioId, Nombre, Descripcion, EsActivo.
- **MetodosPago:** Id, UsuarioId, Nombre, Icono, EsActivo.
- **Gastos:** Id, UsuarioId, CategoriaId, MetodoPagoId, Monto, Fecha, Descripcion.
- **Presupuestos:** Id, UsuarioId, CategoriaId, MontoLimite, Mes, Anio.
---
## Endpoints de la API
### Autenticación
- `POST /api/Auth/register` - Registro de usuario.
- `POST /api/Auth/login` - Inicio de sesión (retorna token JWT).
- `GET /api/Auth/profile` - Obtener datos del perfil.
- `PUT /api/Auth/profile` - Actualizar nombre y contraseña.
### Gastos
- `GET /api/Gastos` - Listar gastos con filtros de fecha y categoría.
- `POST /api/Gastos` - Registrar un gasto.
- `DELETE /api/Gastos/{id}` - Eliminar un gasto.
- `POST /api/Gastos/importar-excel` - Importar gastos desde archivo CSV.
### Presupuestos
- `GET /api/Presupuestos` - Listar presupuestos del usuario.
- `POST /api/Presupuestos` - Crear presupuesto mensual por categoría.
- `DELETE /api/Presupuestos/{id}` - Eliminar presupuesto.
### Categorías y Métodos de Pago
- `GET /api/Categorias` y `POST /api/Categorias` - Gestión de categorías.
- `DELETE /api/Categorias/{id}` - Eliminar categoría.
- `GET /api/MetodosPago` y `POST /api/MetodosPago` - Gestión de métodos de pago.
- `DELETE /api/MetodosPago/{id}` - Eliminar método de pago.
### Reportes
- `GET /api/Reportes/mensual` - Reporte mensual y comparativa con el mes anterior.
- `GET /api/Reportes/alertas-presupuesto` - Estado de presupuestos y alertas (50/80/100%).
- `GET /api/Reportes/exportar/{formato}` - Exportar reporte en formato `csv`, `txt` o `json`.
---
## Instrucciones para Ejecutar el Proyecto
1. Abrir la terminal en la carpeta de la solución.
2. Aplicar las migraciones para crear la base de datos en SQL Server:
   ```bash
   dotnet ef database update --project GastosPersonales.Infrastructure --startup-project GastosPersonales.API