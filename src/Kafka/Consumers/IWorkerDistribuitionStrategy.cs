namespace Kafka.Consumers
{
    public interface IWorkerDistribuitionStrategy
    {
        int Distribute(byte[] data, int max);
    }
}