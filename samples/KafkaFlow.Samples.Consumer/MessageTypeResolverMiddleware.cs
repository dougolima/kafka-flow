namespace KafkaFlow.Samples.Consumer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using KafkaFlow;
    using KafkaFlow.Samples.Common;

    public class MessageTypeResolverMiddleware : IMessageMiddleware
    {
        private readonly Dictionary<string, Type> messageTypes = new Dictionary<string, Type>()
        {
            [typeof(TestMessage).FullName] = typeof(TestMessage)
        };

        public Task Invoke(IMessageContext context, MessageDelegate next)
        {
            if (context.Message.Headers.TryGetValue("Message-Type", out var messageTypeNameHeader))
            {
                var messageTypeName = Encoding.UTF8.GetString(messageTypeNameHeader);

                if (this.messageTypes.TryGetValue(messageTypeName, out var messageType))
                {
                    context.MessageType = messageType;
                }
            }

            return next();
        }
    }
}
