namespace Kafka.Consumers
{
    using System;
    using System.Threading.Tasks;
    using Kafka.Configuration.Consumers.Raw;
    using Microsoft.Extensions.DependencyInjection;

    public class RawConsumer : IMessageConsumer
    {
        private readonly RawConsumerConfiguration configuration;
        private readonly IServiceProvider serviceProvider;
        private readonly MiddlewareExecutor middlewareExecutor;

        public RawConsumer(
            RawConsumerConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;

            this.middlewareExecutor = new MiddlewareExecutor(this.configuration.Middlewares, serviceProvider);
        }

        public Task Cosume(ConsumerMessage message)
        {
            return this.middlewareExecutor
                .Execute(
                    new MessageContext(message, null, null),
                    async context =>
                    {
                        using (var scope = this.serviceProvider.CreateScope())
                        {
                            var handler =
                                (IMessageHandler<byte[]>)scope.ServiceProvider.GetService(this.configuration
                                    .HandlerType);

                            await handler
                                .Handle(message.Value, new MessageContext(message, null, null))
                                .ConfigureAwait(false);
                        }
                    });
        }
    }
}
