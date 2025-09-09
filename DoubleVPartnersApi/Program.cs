using Microsoft.OpenApi.Models;
using DoubleVPartnersApi.Security;
using DoubleVPartnersApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DoubleVPartnersApi", Version = "v1" });
});

// Otros servicios
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddApplicationServices();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        builder =>
        {
            builder.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// ===== CONSTRUCCIÓN DE LA APLICACIÓN =====
var app = builder.Build();

// ===== CONFIGURACIÓN DEL PIPELINE HTTP =====

// Swagger (solo en desarrollo)
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DoubleVPartnersApi v1");
        c.RoutePrefix = string.Empty; // Esto redirige la raíz a Swagger
    });
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DoubleVPartnersApi v1");
        c.RoutePrefix = "swagger";
    });
}

// Middleware de infraestructura
app.UseHttpsRedirection();

// Configuración de CORS
app.UseCors("AllowWebApp");

// Middleware personalizados
app.UseMiddleware<PaginationMiddleware>();

// Mapeo de controladores
app.MapControllers();

// ===== EJECUCIÓN =====

app.Run();
