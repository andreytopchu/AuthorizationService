using System.Collections;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Identity.ExceptionFilter
{
    public record ErrorResponse
    {
        public string Type { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string? StackTrace { get; set; }
        public IDictionary? Data { get; set; }
    }
}