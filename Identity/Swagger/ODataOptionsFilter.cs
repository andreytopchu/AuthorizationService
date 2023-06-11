using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity.Swagger
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ODataOptionsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.MethodInfo.IsDefined(typeof(SwaggerExcludeRequestBodyAttribute), true))
            {
                operation.RequestBody = null;
            }
        }
    }
}