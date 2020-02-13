namespace Kafka
{
    using System.Threading.Tasks;

    public interface IMessageMiddleware
    {
        Task Invoke(MessageContext context, MessageDelegate next);
    }
}
