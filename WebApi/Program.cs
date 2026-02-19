using Application;
using Application.Interfaces;
using Persistence;
using Shared;
using System.Threading.RateLimiting;
using WebApi;
using WebApi.Extensions;
using WebApi.Hubs;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfraestructure(builder.Configuration);
builder.Services.AddSharedInfraestructure(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotifier, SignalRNotifier>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("https://localhost:7200", "http://localhost:5275", "https://localhost:7042") // Frontend URL & Swagger fallback
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddRateLimiter(ops =>
{
    ops.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    ops.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100, // 100 peticiones
                Window = TimeSpan.FromMinutes(1) // por minuto
            }));
});

var app = builder.Build();

// Aplicar migraciones automáticamente al iniciar (útil para Docker)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Persistence.Contexts.ApplicationDbContext>();
        Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.Migrate(context.Database);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al migrar la base de datos.");
    }
}

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI(variable =>
{
    // Esta SÍ lleva v1 porque el documento se llama "v1"
    variable.SwaggerEndpoint("/swagger/v1/swagger.json", "API Completa");

    // CORRECCIÓN: Quitamos el "/v1/" de todas estas rutas:
    variable.SwaggerEndpoint("/swagger/authentication/swagger.json", "Authentication");
    variable.SwaggerEndpoint("/swagger/users/swagger.json", "Users");
    variable.SwaggerEndpoint("/swagger/cocheras/swagger.json", "Cocheras");
    variable.SwaggerEndpoint("/swagger/lugares/swagger.json", "Lugares");
    variable.SwaggerEndpoint("/swagger/tickets/swagger.json", "Tickets");

    variable.DefaultModelsExpandDepth(-1);
});
app.UseReDoc(options =>
{
    options.DocumentTitle = "Parking API Docs";
    options.SpecUrl = "/swagger/v1/swagger.json";
});

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AllowClient");

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    Console.WriteLine($"[DEBUG] Authorization Header: {authHeader ?? "NULL"}");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ParkingHub>("/hubs/parking"); // <--- ESTA ES LA URL QUE USARÁ EL FRONTEND

app.MapControllers();

app.Run();
