namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    public delegate Task MiddlewareDelegate(IMessageContext context);

    public delegate T Factory<out T>(IServiceProvider provider);
}
