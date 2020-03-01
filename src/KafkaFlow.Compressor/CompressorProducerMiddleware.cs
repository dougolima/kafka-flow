namespace KafkaFlow.Compressor
{
    using System;
    using System.Threading.Tasks;

    public class CompressorProducerMiddleware : IMessageMiddleware
    {
        private readonly IMessageCompressor compressor;

        public CompressorProducerMiddleware(IMessageCompressor compressor)
        {
            this.compressor = compressor;
        }

        public Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            if (!(context.Message is byte[] rawData))
            {
                throw new InvalidOperationException($"{nameof(context.Message)} must be a byte array to be compressed and it is '{context.Message.GetType().FullName}'");
            }

            context.TransformMessage(this.compressor.Compress(rawData));

            return next();
        }
    }
}
