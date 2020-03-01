namespace KafkaFlow.Serializer
{
    using System.Threading.Tasks;

    public class SerializerProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageSerializer serializer;
        private readonly IMessageTypeResolver typeResolver;

        public SerializerProducerMiddleware(
            IMessageSerializer serializer,
            IMessageTypeResolver typeResolver)
        {
            this.serializer = serializer;
            this.typeResolver = typeResolver;
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            this.typeResolver.OnProduce(context);

            context.TransformMessage(this.serializer.Serialize(context.Message));

            return next();
        }
    }
}
