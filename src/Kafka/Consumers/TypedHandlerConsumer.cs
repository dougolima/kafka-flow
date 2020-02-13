namespace Kafka.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Kafka.Configuration.Consumers.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;

    public class TypedHandlerConsumer : IMessageConsumer
    {
        private readonly TypedHandlerConsumerConfiguration configuration;
        private readonly ILogHandler logHandler;
        private readonly IServiceProvider serviceProvider;
        private readonly MiddlewareExecutor middlewareExecutor;

        public TypedHandlerConsumer(
            TypedHandlerConsumerConfiguration configuration,
            ILogHandler logHandler,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.serviceProvider = serviceProvider;
            this.middlewareExecutor = new MiddlewareExecutor(this.configuration.Middlewares, this.serviceProvider);
        }

        public Task Cosume(ConsumerMessage message)
        {
            return this.middlewareExecutor
                .Execute(
                    new MessageContext(
                        message,
                        this.configuration.Serializer,
                        this.configuration.Compressor),
                    async context =>
                    {
                        using (var scope = this.serviceProvider.CreateScope())
                        {
                            var handlerType = this.configuration.HandlerMapping.GetHandlerType(context.MessageType);

                            if (handlerType == null)
                            {
                                this.logHandler.Info("No handler found for message type", message);
                                return;
                            }

                            dynamic messageObject;

                            if (message.Value == null)
                            {
                                messageObject = null;
                            }
                            else
                            {
                                var compressor = (IMessageCompressor)this.serviceProvider.GetService(context.Compressor);
                                var serializer = (IMessageSerializer)this.serviceProvider.GetService(context.Serializer);

                                var decompressedMessage = compressor.Decompress(message.Value);

                                messageObject = serializer.Desserialize(decompressedMessage, context.MessageType);
                            }

                            dynamic handler = scope.ServiceProvider.GetService(handlerType);

                            var handleTask = (Task)handler.Handle(messageObject, context);

                            await handleTask.ConfigureAwait(false);
                        }
                    });
        }
    }
}
