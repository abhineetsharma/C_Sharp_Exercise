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


        List<String> keyList;
        KVPList kvplist;
        User user;
        String key;
        ConfigValue configValue;
       [TestInitialize]
        public void TestFrameworkInitialization()
        {
            _log.Info("Initializing Framework");
            _persistSource = _repository.Create<IPersistSource>();
            _config = new Config(_persistSource.Object);
            keyList = new List<string>() { "Key1", "Key2", "Key3" };
            user = new User() { Username = "Abhineet" };
            kvplist = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){
                    Value = "1",
                    ValueType = ValueType.ValueString,
                    User = user
                }),
                new KeyValuePair<string, ConfigValue>(keyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,User=user}),
                new KeyValuePair<string, ConfigValue>(keyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,User=user}),
            };
            key = "Search";
            configValue = new ConfigValue()
            {
                User = user,
                Value = "Google",
                ValueType = ValueType.ValueString
            };


        }

        [TestMethod]
        public void InitializeTest()
        {
            _log.Info("InitializeTest");

        }

        [TestMethod]
        public void LoadValueTest()
        {
            _log.Info("Load Value Test");
            _persistSource.Setup(e => e.LoadValues(It.IsAny<List<string>>())).Returns(kvplist);

            var convertedConfigValue = _config.GetValue("Key1");
            Assert.AreEqual("1", convertedConfigValue.Value);
        }

        [TestMethod]
        public void SaveValue_ShouldSaveAndVerifyTheValueToPersistentSource()
        {
            _log.Info("Save Value Test");

            var stringConfigValue = new ConfigValue
            {
                Value = "Googles",
                ValueType = ValueType.ValueString
            };

            var kvp = new KeyValuePair<string, ConfigValue>("Search", stringConfigValue);
            _config.SaveValue(kvp);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(f => f[0].Key.Equals("Search"))), Times.Once);
        }

        [TestMethod]
        public void SaveValues_ShouldSaveAndVerifyTheValuesToPersistentSource()
        {

            _log.Info("Save Values Test");

            _config.SaveValues(kvplist);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list.Intersect(kvplist).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void GetValue_GetsAndVerifyTheValueFromPersistentSource()
        {
             _log.Info("Get Value Test");
           

            KVPList kvpList = new List<KVP> { new KVP(key, configValue) };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(kvpList);

            var configVal = _config.GetValue(key);

            Assert.AreEqual(kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetValues_GetsAndVerifyTheValuesFromPersistentSource()
        {

            _log.Info("Get User Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(kvplist);

            var convertedConfigValues = _config.GetValues(keyList);
            Assert.IsTrue(convertedConfigValues.Intersect(kvplist).Count() == 3);
        }


        [TestMethod]
        public void SaveUserValue_ShouldSaveAndVerifyTheValueForAUserToPersistentSource()
        {

            _log.Info("Save User Value Test");

            _config.SaveValues(user, kvplist);

            //_persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(e => e.Where(e=>e.Value.User.Equals(user))).ToList()+-+, Times.Once);
            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(k => k.Intersect(kvplist).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void SaveUserValues_ShouldSaveAndVerifyTheValuesForAUserToPersistentSource()
        {
            _log.Info("Save User Values Test");
            List<String> keyList = new List<string>() { "Key1" };

            KVPList kvpList = new List<KeyValuePair<string, ConfigValue>>()
            {
                new KeyValuePair<string, ConfigValue>(keyList[0], new ConfigValue(){ Value = "1",ValueType = ValueType.ValueString,User=user}),
            };
            _config.SaveValues(user, kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 1 && klist.Intersect(kvpList).Count() == 1)), Times.Once);
        }

        [TestMethod]
        public void GetUserValue_GetsAndVerifyTheValueForAUserFromPersistentSource()
        {
             _log.Info("Get User Value Test");
            
            KVPList _kvpList = new List<KVP> { new KVP(key, configValue) };

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(_kvpList);

            var configVal = _config.GetValue(user, key);

            Assert.AreEqual(_kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetUserValues_GetsAndVerifyTheValuesForAUserFromPersistentSource()
        {
            _log.Info("Get User Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(kvplist);

            var convertedConfigValues = _config.GetValues(user, keyList);
            Assert.IsTrue(convertedConfigValues.Intersect(kvplist).Count() == 3);
        }
    }
}
