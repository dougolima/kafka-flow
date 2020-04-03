namespace KafkaFlow.TypedHandler
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class TypedHandlerMiddleware : IMessageMiddleware
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TypedHandlerConfiguration configuration;

        public TypedHandlerMiddleware(
            IServiceProvider serviceProvider,
            TypedHandlerConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var handlerType = this.configuration.HandlerMapping.GetHandlerType(context.MessageType);

                if (handlerType == null)
                {
                    return;
                }

                var handler = scope.ServiceProvider.GetService(handlerType);

                await HandlerExecutor
                    .GetExecutor(context.MessageType)
                    .Execute(
                        handler,
                        context,
                        context.Message)
                    .ConfigureAwait(false);
            }

            await next();
        }
    }
}
