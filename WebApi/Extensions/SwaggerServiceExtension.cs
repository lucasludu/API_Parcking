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
                // 1. Common "All" Doc
                variable.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Completa",
                    Description = "Sistema de Estacionamiento",
                    Version = "v1",
                    TermsOfService = new Uri("https://localhost:7042/api-docs/index.html")
                });

                // 2. Specific Docs
                variable.SwaggerDoc("authentication", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
                variable.SwaggerDoc("users", new OpenApiInfo { Title = "Users API", Version = "v1" });
                variable.SwaggerDoc("cocheras", new OpenApiInfo { Title = "Cocheras API", Version = "v1" });
                variable.SwaggerDoc("lugares", new OpenApiInfo { Title = "Lugares API", Version = "v1" });
                variable.SwaggerDoc("tickets", new OpenApiInfo { Title = "Tickets API", Version = "v1" });

                // 3. Inclusion Logic
                variable.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (docName == "v1") return true; // Show all in v1
                    if (string.IsNullOrEmpty(apiDesc.GroupName)) return false;
                    return apiDesc.GroupName == docName;
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
