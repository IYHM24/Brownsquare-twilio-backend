//using Brownsquare_twilio_backend.Middlewares;
using Brownsquare_twilio_backend.Filters;
using Brownsquare_twilio_backend.Hnadlers;
using Brownsquare_twilio_backend.Middlewares;
using Brownsquare_twilio_backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
using Utils;


//Habilitar soporte para HTTP/2 sin cifrado (gRPC)
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); // ← AÑADIR ESTA LÍNEA para habilitar testeo en HTTP


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Origenes permitidos
string[] origenes_permitidos = new string[]
{
    //Entorno local
    "127.0.0.1", "localhost"

    //Entorno frontend - expansion

};

//Politica de corsAddSingleton
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors_policy", policy =>
    {
        policy
            .WithOrigins(origenes_permitidos) // ← tus orígenes
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // si usas cookies o auth
    });
});

// Configuración Swagger para exponer el ApiKey en la UI (Authorize)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Brownsquare Backend", Version = "v1.0.0" });

    var apiKeyScheme = new OpenApiSecurityScheme
    {
        Description = "Introduce la API Key en el header: 'X-API-KEY'",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = ApiKeyAuthenticationHandler.SchemeName
        }
    };

    c.AddSecurityDefinition(ApiKeyAuthenticationHandler.SchemeName, apiKeyScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { apiKeyScheme, new List<string>() }
    });
});

// Configuracion de autenticacion - ApiKey
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthenticationHandler.SchemeName;
    options.DefaultChallengeScheme = ApiKeyAuthenticationHandler.SchemeName;
})
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
    ApiKeyAuthenticationHandler.SchemeName, null);

//Configuracion de singletones - instancia global única
//Cliente gRPC de WhatsApp
builder.Services.AddSingleton<WhatsAppGrpcClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var grpcAddress = configuration["GrpcSettings:WhatsAppService:Address"] ?? "http://localhost:50051";
    return new WhatsAppGrpcClient(grpcAddress);
});

//Configuracion de scoped - instancia por solicitud
builder.Services.AddScoped<AuthTwilioFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    //Configurar Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

// Activar Autenticacion y Autorizacion
app.UseAuthentication();
app.UseAuthorization();

//aplicar politica de CORS - Frontend
app.UseCors("cors_policy");

//Middlewares (Reglas para cuando entra una solicitud, nivel global) personalizados - se usa Filtros para reglas a solo ciertos controladores o acciones
//app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
