using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Swagger
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AuthenticationRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var allowAnonymous = context.MethodInfo.GetCustomAttribute(typeof(AllowAnonymousAttribute), true) != null
                                 || context.MethodInfo.DeclaringType?.GetCustomAttribute(
                                     typeof(AllowAnonymousAttribute), true) != null;

            if (allowAnonymous) return;

            var authAttr = context.MethodInfo.GetCustomAttribute(typeof(AuthorizeAttribute), true) != null ||
                           context.MethodInfo.DeclaringType?.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                               .Length > 0;

            if (!authAttr) return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
            };
            operation.Security.Add(new OpenApiSecurityRequirement { [scheme] = new List<string>() });
        }
    }
}