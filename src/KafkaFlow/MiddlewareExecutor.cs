namespace KafkaFlow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class MiddlewareExecutor : IMiddlewareExecutor
    {
        private readonly IEnumerable<Factory<IMessageMiddleware>> middlewares;
        private readonly IServiceProvider serviceProvider;

        public MiddlewareExecutor(
            IEnumerable<Factory<IMessageMiddleware>> middlewares,
            IServiceProvider serviceProvider)
        {
            this.middlewares = middlewares;
            this.serviceProvider = serviceProvider;
        }

        public async Task Execute(MessageContext context, Func<MessageContext, Task> nextOperation)
        {
            using (var enumerator = this.middlewares.GetEnumerator())
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
