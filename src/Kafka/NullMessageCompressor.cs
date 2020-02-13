namespace Kafka
{
    public class NullMessageCompressor : IMessageCompressor
    {
        public byte[] Compress(byte[] data)
        {
            return data;
        }

        public byte[] Decompress(byte[] data)
        {
            return data;
        }
    }
}
