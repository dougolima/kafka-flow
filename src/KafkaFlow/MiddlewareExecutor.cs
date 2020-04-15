namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
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
            using (var enumerator = this.configuration.Factories.GetEnumerator())
            {
                await this.ExecuteDefinition(
                        enumerator,
                        context,
                        nextOperation)
                    .ConfigureAwait(false);
            }
        }

        private Task ExecuteDefinition(
            IEnumerator<Factory<IMessageMiddleware>> enumerator,
            MessageContext context,
            Func<MessageContext, Task> nextOperation)
        {
            if (!enumerator.MoveNext() || enumerator.Current is null)
            {
                return nextOperation(context);
            }

            var middleware = enumerator.Current(this.serviceProvider);

            return middleware.Invoke(
                context,
                () => this.ExecuteDefinition(
                    enumerator,
                    context,
                    nextOperation));
        }
    }
}
