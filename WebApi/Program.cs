using Application;
using Persistence;
using Shared;
using WebApi.Extensions;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceInfraestructure(builder.Configuration);
builder.Services.AddSharedInfraestructure(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
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
}
app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
