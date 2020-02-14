namespace KafkaFlow.Consumers
{
    public interface IWorkerDistribuitionStrategy
    {
        int Distribute(byte[] data, int max);
    }
}