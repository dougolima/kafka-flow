namespace KafkaFlow.Consumers
{
    using System;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration.Consumers.Raw;
    using Microsoft.Extensions.DependencyInjection;

    public class RawConsumer : IMessageConsumer
    {
        private readonly RawConsumerConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public RawConsumer(
            RawConsumerConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }

        public MessageContext CreateMessageContext(ConsumerMessage message, IOffsetManager offsetManager)
        {
            return new MessageContext(
                message,
                offsetManager,
                null,
                null);
        }

        public async Task Cosume(MessageContext context)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var handler =
                    (IMessageHandler<byte[]>)scope.ServiceProvider.GetService(this.configuration
                        .HandlerType);

                await handler
                    .Handle(context, context.Message.Value)
                    .ConfigureAwait(false);
            }
        }
    }
}
