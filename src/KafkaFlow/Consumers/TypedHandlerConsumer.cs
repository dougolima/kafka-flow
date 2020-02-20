namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration.Consumers.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;

    public class TypedHandlerConsumer : IMessageConsumer
    {
        private readonly TypedHandlerConsumerConfiguration configuration;
        private readonly ILogHandler logHandler;
        private readonly IServiceProvider serviceProvider;

        public TypedHandlerConsumer(
            TypedHandlerConsumerConfiguration configuration,
            ILogHandler logHandler,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.logHandler = logHandler;
            this.serviceProvider = serviceProvider;
        }

        public MessageContext CreateMessageContext(
            ConsumerMessage message,
            IOffsetManager offsetManager,
            int workerId)
        {
            return new MessageContext(
                message,
                offsetManager,
                workerId,
                this.configuration.Serializer,
                this.configuration.Compressor);
        }

        public async Task Consume(MessageContext context)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var handlerType = this.configuration.HandlerMapping.GetHandlerType(context.MessageType);

                if (handlerType == null)
                {
                    this.logHandler.Info("No handler found for message type", new { context.MessageType });
                    return;
                }

                if (context.Message.Value != null)
                {
                    var compressor = (IMessageCompressor)this.serviceProvider.GetService(context.Compressor);
                    var serializer = (IMessageSerializer)this.serviceProvider.GetService(context.Serializer);

                    var decompressedMessage = compressor.Decompress(context.Message.Value);

                    context.MessageObject = serializer.Desserialize(decompressedMessage, context.MessageType);
                }

                dynamic handler = scope.ServiceProvider.GetService(handlerType);

                var handleTask = (Task)handler.Handle(context, (dynamic)context.MessageObject);

                await handleTask.ConfigureAwait(false);
            }
        }
    }
}
