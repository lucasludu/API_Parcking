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
        variable.SwaggerEndpoint("/swagger/v1/swagger.json", "API Completa");
        variable.SwaggerEndpoint("/swagger/v1/auth/swagger.json", "Auth");
        variable.SwaggerEndpoint("/swagger/v1/users/swagger.json", "Users");
        variable.SwaggerEndpoint("/swagger/v1/cocheras/swagger.json", "Cocheras");
        variable.SwaggerEndpoint("/swagger/v1/lugares/swagger.json", "Lugares");
        variable.SwaggerEndpoint("/swagger/v1/tickets/swagger.json", "Tickets");

        variable.DefaultModelsExpandDepth(-1);
    });
}
app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
