namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly MiddlewareConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public MiddlewareExecutor(
            MiddlewareConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }

        public async Task Execute(MessageContext context, Func<MessageContext, Task> nextOperation)
        {
            await this.ExecuteDefinition(
                    0,
                    context,
                    nextOperation)
                .ConfigureAwait(false);
        }

        private Task ExecuteDefinition(
            int index,
            MessageContext context,
            Func<MessageContext, Task> nextOperation)
        {
            if (this.configuration.Factories.Count == index)
            {
                return nextOperation(context);
            }

            var middleware = this.configuration.Factories[index](this.serviceProvider);

            return middleware.Invoke(
                context,
                () => this.ExecuteDefinition(
                    index + 1,
                    context,
                    nextOperation));
        }
    }
}
