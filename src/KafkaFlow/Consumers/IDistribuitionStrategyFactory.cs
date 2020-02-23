namespace KafkaFlow.Consumers
{
    using KafkaFlow.Consumers.DistribuitionStrategies;

    public interface IDistribuitionStrategyFactory
    {
        IDistribuitionStrategy Create();
    }
}