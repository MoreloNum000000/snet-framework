using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SNET.Framework.Api.DependencyConfig;
using SNET.Framework.Api.Metricas;
using SNET.Framework.Domain.Audit;
using SNET.Framework.Domain.Notifications.Email;
using SNET.Framework.Infrastructure.Middlewares;
using SNET.Framework.Infrastructure.Notifications.Email;
using SNET.Framework.Persistence;
using SNET.Framework.Persistence.Audit;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddCarter();
builder.AddRepositories();
builder.AddEmailSettings();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resuorse =>
    {
        resuorse.AddService("FrameworkApi");
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        metrics.AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();
        tracing.AddOtlpExporter();
    });

builder.Logging.AddOpenTelemetry(loggin => loggin.AddOtlpExporter());

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssembly(typeof(SNET.Framework.Features.AssemblyReference).Assembly);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(c =>
{
    c.Title = "Framework Api";
    c.Version = "v1";
});

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApiDbContext"));
}, ServiceLifetime.Scoped);

builder.Services.AddValidatorsFromAssembly(typeof(SNET.Framework.Features.AssemblyReference).Assembly, ServiceLifetime.Scoped);
builder.Services.AddScoped<IEmailNotifications, SmtpNotifications>();
builder.Services.AddScoped<IAuditService, AuditService>(); // Auditoría

var app = builder.Build();

// Configuración de middlewares
app.UseOpenApi();
app.UseSwaggerUi(settings => { settings.Path = "/docs"; });
app.UseReDoc(settings =>
{
    settings.Path = "/redoc";
    settings.DocumentPath = "/swagger/v1/swagger.json";
});

app.UseMiddleware<AuditMiddleware>(); // Middleware de auditoría

app.MapCarter();

app.MapGet("/{name}", (ILogger<Program> logger, string name) =>
{
    ContarNombres.Contar.Add(1, new KeyValuePair<string, object>("nombre", name));
    logger.LogInformation("Su nombre es {name}", name);
    return name;
});

app.Run();
