namespace KafkaFlow
{
    using System;
    using System.Threading.Tasks;

    public interface IMiddlewareExecutor
    {
        Task Execute(MessageContext context, Func<MessageContext, Task> nextOperation);
    }
}
