using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Swagger
{
    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        public ConfigureSwaggerOptions()
        {
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

            // auth
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT/Reference",
                    In = ParameterLocation.Header,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
            options.OperationFilter<AuthenticationRequirementsOperationFilter>();

            var baseDirectoryPath = AppContext.BaseDirectory;
            foreach (var fileName in Directory.GetFiles(baseDirectoryPath, "*.xml"))
            {
                options.IncludeXmlComments(fileName);
            }
        }
    }
}