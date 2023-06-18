using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;
using AutoMapper;
using Dex.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog.AspNetCore;

namespace Identity.Logger;

public static class SerilogRequestLoggerHelper
{
    private const string HeaderApplicationGrpc = "application/grpc";
    private const string HeaderApplicationJson = "application/json";

    private static readonly MapperConfiguration MapperConfiguration = new(cfg =>
    {
        cfg.CreateMap<HttpRequest, HttpRequestInfo>()
            .ForMember(info => info.Action, expression => expression.MapFrom(request => request.RouteValues["action"]!.ToString()))
            .ForMember(info => info.Controller, expression => expression.MapFrom(request => request.RouteValues["controller"]!.ToString()))
            .ForMember(info => info.QueryString, expression => expression.MapFrom(request => request.QueryString.ToString()))
            .ForMember(info => info.RequestUrl, expression => expression.MapFrom(request => request.GetDisplayUrl()));
    });

    private static IMapper GetMapper() => MapperConfiguration.CreateMapper();

    /// <summary>
    /// Наименования атрибутов значения которых будут заменены и не будут сохранены в лог
    /// </summary>
    public static HashSet<string> MaskedFieldNames { get; } = new() {"authorization", "token", "password"};

    /// <summary>
    /// Устанавливает делегат обогащающий IDiagnosticContext
    /// для каждого запроса записываем (headers, cookies, queries, forms, payloads)
    /// маскируем значения запрещенных полей
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetupSerilogEnrichAction(RequestLoggingOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        const string masked = "**masked**";

        options.EnrichDiagnosticContext = (diagnosticsContext, httpContext) =>
        {
            var request = httpContext.Request;
            if (request.Headers.Count > 0)
            {
                MaskValues("headers_", request.Headers);
            }

            if (request.Cookies.Count > 0)
            {
                MaskValues("cookies_", request.Cookies);
            }

            if (request.Query.Count > 0)
            {
                MaskValues("queries_", request.Query);
            }

            if (request.HasFormContentType && request.Form.Count > 0)
            {
                MaskValues("forms_", request.Form);
            }

            if (request.ContentType?.Contains(HeaderApplicationJson, StringComparison.OrdinalIgnoreCase) == true)
            {
                var payloadString = ReadPayloadAsString(httpContext);
                if (!string.IsNullOrEmpty(payloadString))
                {
                    try
                    {
                        var json = JsonNode.Parse(payloadString);
                        if (json != null)
                        {
                            try
                            {
                                MaskValues("payloads_", json.AsObject());
                            }
                            catch (InvalidOperationException)
                            {
                                diagnosticsContext.Set("payload_unparsed", json);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        diagnosticsContext.Set("payload_raw_body", payloadString);
                        diagnosticsContext.Set("payload_parse_error", e);
                    }
                }
            }

            var requestInfo = GetHttpRequestInfo(httpContext);
            if (requestInfo is null) return;

            requestInfo.AsArray().ForEach(x => diagnosticsContext.Set(x.Key, x.Value));

            void MaskValues<TV>(string prefix, IEnumerable<KeyValuePair<string, TV>> valuePairs)
            {
                foreach (var (key, value) in valuePairs)
                {
                    diagnosticsContext.Set(prefix + key,
                        MaskedFieldNames.Contains(key.ToLowerInvariant())
                            ? masked
                            : value);
                }
            }
        };

        string ReadPayloadAsString(HttpContext httpContext)
        {
            httpContext.Request.Body.Position = 0;
            using var reader = new StreamReader(httpContext.Request.Body);
            var body = reader.ReadToEnd();
            return body;
        }
    }

    private static HttpRequestInfo? GetHttpRequestInfo(HttpContext httpContext)
    {
        var request = httpContext.Request;
        return request.ContentType?.Equals(HeaderApplicationGrpc, StringComparison.OrdinalIgnoreCase) != true
            ? GetMapper().Map<HttpRequestInfo>(request)
            : null;
    }

    private record HttpRequestInfo
    {
        public string? Controller { get; set; }

        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request scheme.
        /// </summary>
        /// <returns>The HTTP request scheme.</returns>
        public string? Scheme { get; set; }

        public string? RequestUrl { get; set; }

        /// <summary>
        /// Gets or sets the raw query string used to create the query collection in Request.Query.
        /// </summary>
        /// <returns>The raw query string.</returns>
        public string? QueryString { get; set; }

        /// <summary>
        /// Gets or sets the request protocol (e.g. HTTP/1.1).
        /// </summary>
        /// <returns>The request protocol.</returns>
        public string? Protocol { get; set; }

        /// <summary>
        /// Gets or sets the Content-Length header.
        /// </summary>
        /// <returns>The value of the Content-Length header, if any.</returns>
        public long? ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the Content-Type header.
        /// </summary>
        /// <returns>The Content-Type header.</returns>
        public string? ContentType { get; set; }

        public IEnumerable<KeyValuePair<string, object>> AsArray()
        {
            yield return new KeyValuePair<string, object>(nameof(Controller), Controller!);
            yield return new KeyValuePair<string, object>(nameof(Action), Action!);
            yield return new KeyValuePair<string, object>(nameof(Scheme), Scheme!);
            yield return new KeyValuePair<string, object>(nameof(RequestUrl), RequestUrl!);
            yield return new KeyValuePair<string, object>(nameof(QueryString), QueryString!);
            yield return new KeyValuePair<string, object>(nameof(Protocol), Protocol!);
            yield return new KeyValuePair<string, object>(nameof(ContentLength), ContentLength!);
            yield return new KeyValuePair<string, object>(nameof(ContentType), ContentType!);
        }
    }
}