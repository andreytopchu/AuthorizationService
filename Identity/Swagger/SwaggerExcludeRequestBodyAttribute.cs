using System;

namespace Identity.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerExcludeRequestBodyAttribute : Attribute
    {
    }
}