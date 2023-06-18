using System;
using Common.Logging;
using Common.Logging.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;

namespace Identity.Logger
{
    public static class LoggerConfigurator
    {
        public static IHostBuilder ConfigureLogger(this IHostBuilder builder)
        {
            return builder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
                configuration.Enrich.FromLogContext();
                configuration.Enrich.WithSpan();
                configuration.Enrich.WithProperty("AppName", AppDomain.CurrentDomain.FriendlyName);
                LogManager.Adapter = new SerilogFactoryAdapter();
            });
        }

        public static void UseRequestLogger(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
            app.UseSerilogRequestLogging(SerilogRequestLoggerHelper.SetupSerilogEnrichAction);
        }
    }
}