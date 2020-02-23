namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly IEnumerable<ConfigurableDefinition<IMessageMiddleware>> middlewares;
        private readonly IServiceProvider serviceProvider;

        public MiddlewareExecutor(IEnumerable<ConfigurableDefinition<IMessageMiddleware>> middlewares, IServiceProvider serviceProvider)
        {
            this.middlewares = middlewares;
            this.serviceProvider = serviceProvider;
        }

        public async Task Execute(MessageContext context, Func<MessageContext, Task> nextOperation)
        {
            using (var enumerator = this.middlewares.GetEnumerator())
            {
                await this.ExecuteDefinition(enumerator, context, nextOperation).ConfigureAwait(false);
            }
        }

        private Task ExecuteDefinition(
            IEnumerator<ConfigurableDefinition<IMessageMiddleware>> enumerator,
            MessageContext context,
            Func<MessageContext, Task> nextOperation)
        {
            if (!enumerator.MoveNext() || enumerator.Current is null)
            {
                return nextOperation(context);
            }

            var definition = enumerator.Current;

            var middleware = (IMessageMiddleware)this.serviceProvider.GetRequiredService(definition.Type);

            definition.Configurator(middleware, this.serviceProvider);

            return middleware.Invoke(context, () => this.ExecuteDefinition(enumerator, context, nextOperation));
        }
    }
}
