namespace KafkaFlow.IntegrationTests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BasicHandlerTests
    {
        private IServiceProvider provider;

        [TestInitialize]
        public void Setup()
        {
            this.provider = Bootstrapper.GetServiceProvider();
        }
        
        [TestMethod]
        public void InitializeLastOffset_InitializeTheValue_Success()
        {

        }
    }
}
