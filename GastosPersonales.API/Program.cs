using Microsoft.EntityFrameworkCore;
using GastosPersonales.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GastosPersonales.Domain.Repositories;  
using GastosPersonales.Infrastructure.Repositories;
using GastosPersonales.Application.Services;
using GastosPersonales.Infrastructure.Services;
using Scalar.AspNetCore;
using GastosPersonales.API.Middlewares;
using GastosPersonales.Application.ExportStrategies;


var builder = WebApplication.CreateBuilder(args);
// Configuracion de la base de datos SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configuracion de  Autenticación con JWT Bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var keyStr = jwtSettings["Key"] ?? "ClaveSuperSecretaYMuyLarga1234567890!";
var key = Encoding.UTF8.GetBytes(keyStr);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "GastosPersonalesApp",
        ValidAudience = jwtSettings["Audience"] ?? "GastosPersonalesUsers",
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
// Registrar Repositorios (Inyección de Dependencias)
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();
builder.Services.AddScoped<IGastoRepository, GastoRepository>();
//  Registrar Servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
// Registro de Patrones para Exportaciones
builder.Services.AddScoped<IExportStrategy, CsvExportStrategy>();
builder.Services.AddScoped<IExportStrategy, TxtExportStrategy>();
builder.Services.AddScoped<IExportStrategy, JsonExportStrategy>();
builder.Services.AddScoped<IExportStrategyFactory, ExportStrategyFactory>();
//  Registrar Utilidades de Infraestructura
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
//  Registrar controladores y OpenAPI/Scalar
builder.Services.AddControllers();
builder.Services.AddOpenApi();
var app = builder.Build();

// Registra el middleware de errores global al inicio del pipeline
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
// Comentado para evitar errores SSL localmente al usar el puerto http://localhost:5056  lo pongo aqui para tenerlo a mano
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();