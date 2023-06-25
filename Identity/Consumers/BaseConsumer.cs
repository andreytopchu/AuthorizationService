using System;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Extensions;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Identity.Consumers
{
    public abstract class BaseConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        private ConsumeContext<TMessage>? _context;
        protected ILogger Logger { get; }

        protected BaseConsumer(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            _context = context;

            try
            {
                await Process(context);
            }
            catch (DeferConsumerException)
            {
                // ignore
            }
            catch (Exception e)
            {
                LogError(context, e);
                throw;
            }
        }

        protected abstract Task Process(ConsumeContext<TMessage> context);

        /// <summary>
        /// Прерывает текущее исполнение путем выброса DeferConsumerException.
        /// Отправляет сообщение в delay_exchange на указанный интервал.
        /// </summary>
        /// <param name="delay"></param>
        /// <exception cref="BaseConsumer{TMessage}.DeferConsumerException"></exception>
        protected async Task Defer(TimeSpan delay)
        {
            await _context.Defer(delay);
            throw new DeferConsumerException();
        }

        private void LogError(ConsumeContext<TMessage> context, Exception e)
        {
            var friendlyJson = context.Message.ToFriendlyJson().ToString();
            Logger.LogError(e, "Consumer process failed. {MessageData}", friendlyJson[..Math.Min(4096, friendlyJson.Length - 1)]);
        }

        private sealed class DeferConsumerException : Exception
        {
        }
    }
}