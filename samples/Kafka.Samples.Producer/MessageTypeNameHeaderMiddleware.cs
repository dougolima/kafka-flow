namespace Kafka.Samples.Producer
{
    using System.Text;
    using System.Threading.Tasks;

    public class MessageTypeNameHeaderMiddleware : IMessageMiddleware
    {
        public Task Invoke(MessageContext context, MessageDelegate next)
        {
            context.Message.Headers["Message-Type"] = Encoding.UTF8.GetBytes(context.MessageType.FullName);

            return next();
        }
    }
}
