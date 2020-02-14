namespace KafkaFlow.Samples.Producer
{
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow;

    public class MessageTypeNameHeaderMiddleware : IMessageMiddleware
    {
        public Task Invoke(MessageContext context, MessageDelegate next)
        {
            context.Message.Headers["Message-Type"] = Encoding.UTF8.GetBytes(context.MessageType.FullName);

            return next();
        }
    }
}
