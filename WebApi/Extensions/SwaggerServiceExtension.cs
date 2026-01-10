using System.Reflection;
using Microsoft.OpenApi;

namespace WebApi.Extensions
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(variable =>
            {
                variable.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API_Universidad",
                    Description = "Sistema Universitario",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "LucasLudu",
                        Email = "lucas@gmail.com",
                        Url = new Uri("https://www.google.com.ar/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                variable.IncludeXmlComments(xmlPath);

                variable.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                variable.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer"),
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}
