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

        public MessageContext CreateMessageContext(ConsumerMessage message, IOffsetManager offsetManager)
        {
            return new MessageContext(
                message,
                offsetManager,
                this.configuration.Serializer,
                this.configuration.Compressor);
        }

        public async Task Cosume(MessageContext context)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var handlerType = this.configuration.HandlerMapping.GetHandlerType(context.MessageType);

                if (handlerType == null)
                {
                    this.logHandler.Info("No handler found for message type", new { context.MessageType });
                    return;
                }

                dynamic messageObject = null;

                if (context.Message.Value != null)
                {
                    var compressor = (IMessageCompressor)this.serviceProvider.GetService(context.Compressor);
                    var serializer = (IMessageSerializer)this.serviceProvider.GetService(context.Serializer);

                    var decompressedMessage = compressor.Decompress(context.Message.Value);

                    messageObject = serializer.Desserialize(decompressedMessage, context.MessageType);
                }

                dynamic handler = scope.ServiceProvider.GetService(handlerType);

                var handleTask = (Task)handler.Handle(context, messageObject);

                await handleTask.ConfigureAwait(false);
            }
        }
    }
}
