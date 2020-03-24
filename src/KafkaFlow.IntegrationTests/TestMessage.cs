namespace KafkaFlow.IntegrationTests
{
    using System.Runtime.Serialization;

    [DataContract]
    public class TestMessage1 : ITestMessage
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }
    }

    [DataContract]
    public class TestMessage2 : ITestMessage
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }
    }

    public interface ITestMessage
    {
        string Value { get; set; }
    }
}
