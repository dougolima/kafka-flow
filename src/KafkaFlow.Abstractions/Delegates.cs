namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    public delegate Task MiddlewareDelegate();

    public delegate T Factory<out T>(IServiceProvider provider);
}
