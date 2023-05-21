using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Exceptions;
using Identity.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Identity.ExceptionFilter
{
    /// <summary>
    /// Глобальный перехват и логирование ошибок
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string ResponseContentType = "application/json";
        private const string ErrorPath = "errors";

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context, IOptions<JsonOptions> options)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var response = new ErrorResponse
                {
                    Message = exception.Message,
                    Type = $"{ErrorPath}.{ResolveTypeName(exception)}",
                    Data = exception.Data
                };

                var result = new ContentResult
                {
                    StatusCode = ResolveHttpStatusCode(exception),
                    ContentType = ResponseContentType,
                };

                if (result.StatusCode == Status500InternalServerError || exception is ArgumentException)
                {
                    var isStackTrace = exception is not (NotImplementedException or NotSupportedException
                        or OperationCanceledException);
                    response.StackTrace = isStackTrace ? exception.StackTrace : null;
                    _logger.LogError(exception, "Request failed");
                }
                else
                {
                    _logger.LogWarning("Request complete with warning. {@Detail}", response);
                }

                context.Response.StatusCode = result.StatusCode.Value;
                context.Response.ContentType = result.ContentType;
                await context.Response.WriteAsync(response.ToFriendlyJson());
            }
        }

        private static string ResolveTypeName(Exception exception)
        {
            var type = exception.GetType().Name.Replace("`1", "", StringComparison.InvariantCulture);
            if (exception.Data["Type"] != null)
            {
                type = $"{type}.{exception.Data["Type"]}";
            }

            return type;
        }

        private static int ResolveHttpStatusCode(Exception exception)
        {
            return exception switch
            {
                // 499 отмена операции, System.OperationCanceledException
                OperationCanceledException _ => 499,

                // 409
                DbUpdateConcurrencyException _ => Status409Conflict,
                AlreadyExistException _ => Status409Conflict,

                // 408
                RetryLimitExceededException _ => Status408RequestTimeout,
                TimeoutException _ => Status408RequestTimeout,

                // 404
                FileNotFoundException _ => Status404NotFound,

                // 400
                FormatException _ => Status400BadRequest,
                ValidationException _ => Status400BadRequest,
                ArgumentException _ => Status400BadRequest,

                NotImplementedException _ => Status501NotImplemented,
                NotSupportedException _ => Status510NotExtended,

                _ => Status500InternalServerError
            };
        }
    }
}