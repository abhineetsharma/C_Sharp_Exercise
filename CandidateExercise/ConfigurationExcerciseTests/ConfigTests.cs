using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace FutureWonder.Exercises.Configuration
{
    using KVPList = IList<KeyValuePair<string, ConfigValue>>;
    using KVP = KeyValuePair<string, ConfigValue>;
    [TestClass]
    public class ConfigTests
    {
        private readonly ILog _log = LogManager.GetLogger(nameof(ConfigTests));
        private readonly MockRepository _repository = new MockRepository(MockBehavior.Loose);
        private Mock<IPersistSource> _persistSource;
        private Config _config;

        public ConfigTests()
        {
            XmlConfigurator.Configure();
        }

        [TestInitialize]
        public void TestFrameworkInitialization()
        {
            _log.Info("Initializing Framework");
            _persistSource = _repository.Create<IPersistSource>();
            _config = new Config(_persistSource.Object);

        }

        [TestMethod]
        public void InitializeTest()
        {
            _log.Info("InitializeTest");

        }

        [TestMethod]
        public void LoadValueTest()
        {
            KVPList list = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>("Key1", new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString}),
                new KeyValuePair<string, ConfigValue>("Key2", new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt}),
                new KeyValuePair<string, ConfigValue>("Key3", new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString}),
            };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(list);

            var convertedConfigValue = _config.GetValue("Key1");
            Assert.AreEqual("1", convertedConfigValue.Value);
        }

        [TestMethod]
        public void SaveValueTest()
        {
            _log.Info("SaveValueTest");

            var stringConfigValue = new ConfigValue
            {
                Value = "Googles",
                ValueType = ValueType.ValueString
            };
            var kvp = new KeyValuePair<string, ConfigValue>("Bing", stringConfigValue);
            _config.SaveValue(kvp);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list[0].Key.Equals("Bing"))), Times.Once);
        }
       
    }
}
