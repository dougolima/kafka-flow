namespace Kafka
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kafka.Configuration;

    public class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly IEnumerable<MiddlewareDefinition> middlewares;
        private readonly IServiceProvider serviceProvider;

        public MiddlewareExecutor(IEnumerable<MiddlewareDefinition> middlewares, IServiceProvider serviceProvider)
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
            IEnumerator<MiddlewareDefinition> enumerator,
            MessageContext context,
            Func<MessageContext, Task> nextOperation)
        {
            if (!enumerator.MoveNext() || enumerator.Current is null)
            {
                return nextOperation(context);
            }

            var definition = enumerator.Current;

            var middleware = (IMessageMiddleware)this.serviceProvider.GetService(definition.MiddlewareType);

            definition.Configurator(middleware);

            return middleware.Invoke(context, () => this.ExecuteDefinition(enumerator, context, nextOperation));
        }
    }
}
