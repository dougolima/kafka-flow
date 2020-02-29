namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    internal interface IMiddlewareExecutor
    {
        Task Execute(MessageContext context, Func<MessageContext, Task> nextOperation);
    }
}
