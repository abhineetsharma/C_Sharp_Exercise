using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace FutureWonder.Exercises.Configuration
{
    using KVPList = IList<KeyValuePair<string, ConfigValue>>;
    using KVP = KeyValuePair<string, ConfigValue>;
    using KList = IList<string>;
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
            var kvp = new KeyValuePair<string, ConfigValue>("Search", stringConfigValue);
            _config.SaveValue(kvp);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list[0].Key.Equals("Search"))), Times.Once);
        }

        [TestMethod]
        public void SaveValuesTest()
        {

            _log.Info("SaveValuesTest");
            List<KeyValuePair<string, ConfigValue>> kvpList = null;
            for (int i = 0; i < 3; i++)
            {
                var stringConfigValue = new ConfigValue
                {
                    Value = "Google" + i,
                    ValueType = ValueType.ValueString
                };
                var kvp = new KeyValuePair<string, ConfigValue>("Search" + i, stringConfigValue);
                if (kvpList == null)
                    kvpList = new List<KeyValuePair<string, ConfigValue>>();
                kvpList.Add(kvp);

            }
            _config.SaveValues(kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list[0].Key.Contains("Search") && list.Count == 3)), Times.Once);
        }

        [TestMethod]
        public void GetValueTest()
        {
            // _log.Info("GetValueTest");
            String key = "Search";
            var configValue = new ConfigValue()
            {
                User = null,
                Value = "Google",
                ValueType = ValueType.ValueString
            };

            KVPList kvpList = new List<KVP> { new KVP(key, configValue) };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(kvpList);

            var configVal = _config.GetValue(key);

            Assert.AreEqual(kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetValuesTest()
        {
            List<String> keyList = new List<string>() { "Key1", "Key2", "Key3" };
            KVPList list = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString}),
                new KeyValuePair<string, ConfigValue>(keyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt}),
                new KeyValuePair<string, ConfigValue>(keyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString}),
            };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(list);

            var convertedConfigValues = _config.GetValues(keyList);
            Assert.IsTrue(convertedConfigValues.Intersect(list).Count() == 3);
        }


        [TestMethod]
        public void SaveValuesTestUser()
        {

            List<String> keyList = new List<string>() { "Key1", "Key2", "Key3" };
            User user = new User() { Username = "Abhineet" };
            KVPList kvpList = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString,User=user}),
                new KeyValuePair<string, ConfigValue>(keyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,User=user}),
                new KeyValuePair<string, ConfigValue>(keyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,User=user}),
            };
            _config.SaveValues(user, kvpList);

            // _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list.Where(e=>e.Value.User.Equals(user))), Times.Once);
            _persistSource.Verify(
               e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 3 && klist.Intersect(kvpList).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void SaveValueTestUser()
        {

            List<String> keyList = new List<string>() { "Key1" };
            User user = new User() { Username = "Abhineet" };
            KVPList kvpList = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString,User=user}),
            };
            _config.SaveValues(user, kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 1 && klist.Intersect(kvpList).Count() == 1)), Times.Once);
        }

        [TestMethod]
        public void GetUserValueTest()
        {
            // _log.Info("GetValueTest");
            String key = "Search";
            User user = new User() { Username = "Abhineet" };
            var configValue = new ConfigValue()
            {
                User = user,
                Value = "Google",
                ValueType = ValueType.ValueString
            };

            KVPList kvpList = new List<KVP> { new KVP(key, configValue) };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(kvpList);

            var configVal = _config.GetValue(user, key);

            Assert.AreEqual(kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetUserValuesTest()
        {
            List<String> keyList = new List<string>() { "Key1", "Key2", "Key3" };
            User user = new User() { Username = "Abhineet" };
            KVPList list = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString,User=user}),
                new KeyValuePair<string, ConfigValue>(keyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,User=user}),
                new KeyValuePair<string, ConfigValue>(keyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,User=user}),
            };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(list);

            var convertedConfigValues = _config.GetValues(user,keyList);
            Assert.IsTrue(convertedConfigValues.Intersect(list).Count() == 3);
        }
    }
}
