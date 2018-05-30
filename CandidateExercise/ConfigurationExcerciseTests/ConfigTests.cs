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
        #region variables

        private readonly ILog _log = LogManager.GetLogger(nameof(ConfigTests));
        private readonly MockRepository _repository = new MockRepository(MockBehavior.Loose);
        private Mock<IPersistSource> _persistSource;
        private Config _config;

        private List<String> KeyList { get { return new List<string>() { "Key1", "Key2", "Key3" }; } }
        private KVPList Kvplist
        {
            get
            {
                return
                    new List<KeyValuePair<string, ConfigValue>>()
                    {
                        new KeyValuePair<string, ConfigValue>(KeyList[0], new ConfigValue(){
                            Value = "1",
                            ValueType = ValueType.ValueString
                        }),
                        new KeyValuePair<string, ConfigValue>(KeyList[1], new ConfigValue(){ Value = 2.ToString()}),
                        new KeyValuePair<string, ConfigValue>(KeyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString}),
                    };
            }
        }
        private KVPList KvpUserlist
        {
            get
            {
                return
                    new List<KeyValuePair<string, ConfigValue>>()
                    {
                        new KeyValuePair<string, ConfigValue>(KeyList[0], new ConfigValue(){
                            Value = "1",
                            ValueType = ValueType.ValueString,
                            User = UserTestObject
                        }),
                        new KeyValuePair<string, ConfigValue>(KeyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,User=UserTestObject}),
                        new KeyValuePair<string, ConfigValue>(KeyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,User=UserTestObject}),
                    };
            }
        }

        private KVPList KvpAppList
        {
            get
            {
                return
                    new List<KeyValuePair<string, ConfigValue>>()
                    {
                        new KeyValuePair<string, ConfigValue>(KeyList[0], new ConfigValue(){
                            Value = "1",
                            ValueType = ValueType.ValueString,
                            App = AppTestObject
                        }),
                        new KeyValuePair<string, ConfigValue>(KeyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,App=AppTestObject}),
                        new KeyValuePair<string, ConfigValue>(KeyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,App=AppTestObject }),
                    };
            }
        }
        private KVPList KvpAppListNull
        {
            get
            {
                return
                    new List<KeyValuePair<string, ConfigValue>>()
                    {
                        new KeyValuePair<string, ConfigValue>(KeyList[0], new ConfigValue(){
                            Value = null,
                            ValueType = ValueType.ValueString,
                            App = AppTestObject
                        }),
                        new KeyValuePair<string, ConfigValue>(KeyList[1], new ConfigValue(){ Value =null,ValueType = ValueType.ValueInt}),
                        new KeyValuePair<string, ConfigValue>(KeyList[2], new ConfigValue(){ Value = null,ValueType = ValueType.ValueString }),
                    };
            }
        }

        private KVPList KvpAppWithUserList
        {
            get
            {
                return
                    new List<KeyValuePair<string, ConfigValue>>()
                    {
                        new KeyValuePair<string, ConfigValue>(KeyList[0], new ConfigValue(){
                            Value = "1",
                            ValueType = ValueType.ValueString,
                            App = AppTestObject,
                            User = UserTestObject
                        }),
                        new KeyValuePair<string, ConfigValue>(KeyList[1], new ConfigValue(){ Value = 2.ToString(),ValueType = ValueType.ValueInt,App=AppTestObject,User = UserTestObject}),
                        new KeyValuePair<string, ConfigValue>(KeyList[2], new ConfigValue(){ Value = "3",ValueType = ValueType.ValueString,App=AppTestObject,User = UserTestObject}),
                    };
            }
        }
        private User UserTestObject
        {
            get
            {
                return new User()
                { Username = "Abhineet" };
            }
        }

        private App AppTestObject
        {
            get
            {
                return new App("Visual Studio");
            }
        }


        #endregion variables


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
            _log.Info("Load Value Test");
            _persistSource.Setup(e => e.LoadValues(It.IsAny<List<string>>())).Returns(Kvplist);

            var convertedConfigValue = _config.GetValue(KeyList[0]);
            Assert.AreEqual(Kvplist[0].Value.Value, convertedConfigValue.Value);
        }
        #region basic key value store Tests
        [TestMethod]
        public void SaveValue_ShouldSaveAndVerifyTheValueToPersistentSource()
        {
            _log.Info("Save Value Test");

            _config.SaveValue(Kvplist[0]);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(f => f[0].Key.Equals(KeyList[0]))), Times.Once);
        }

        [TestMethod]
        public void SaveValues_ShouldSaveAndVerifyTheValuesToPersistentSource()
        {

            _log.Info("Save Values Test");

            _config.SaveValues(Kvplist);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(list => list.Intersect(Kvplist).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void GetValue_GetsAndVerifyTheValueFromPersistentSource()
        {
            _log.Info("Get Value Test");


            KVPList kvpList = Kvplist.Where(e => e.Key.Equals(KeyList[0])).ToList();

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(kvpList);

            var configVal = _config.GetValue(KeyList[0]);

            Assert.AreEqual(kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetValues_GetsAndVerifyTheValuesFromPersistentSource()
        {

            _log.Info("Get User Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(Kvplist);

            var convertedConfigValues = _config.GetValues(KeyList);
            Assert.IsTrue(convertedConfigValues.Intersect(Kvplist).Count() == 3);
        }
        #endregion
        #region keys with User Tests
        [TestMethod]
        public void SaveUserValue_ShouldSaveAndVerifyTheValueForAUserToPersistentSource()
        {

            _log.Info("Save User Value Test");

            _config.SaveValues(UserTestObject, KvpUserlist);
            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(k => k.Intersect(KvpUserlist).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void SaveUserValues_ShouldSaveAndVerifyTheValuesForAUserToPersistentSource()
        {
            _log.Info("Save User Values Test");

            KVPList kvpList = KvpUserlist.Where(e => e.Key.Equals(KeyList[0])).ToList();
            _config.SaveValues(UserTestObject, kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 1 && klist.Intersect(kvpList).Count() == 1)), Times.Once);
        }

        [TestMethod]
        public void GetUserValue_GetsAndVerifyTheValueForAUserFromPersistentSource()
        {
            _log.Info("Get User Value Test");

            KVPList _kvpList = KvpUserlist.Where(e => e.Key.Equals(KeyList[0])).ToList();

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(_kvpList);

            var configVal = _config.GetValue(UserTestObject, KeyList[0]);

            Assert.AreEqual(_kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetUserValues_GetsAndVerifyTheValuesForAUserFromPersistentSource()
        {
            _log.Info("Get User Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(KvpUserlist);

            var convertedConfigValues = _config.GetValues(UserTestObject, KeyList);
            Assert.IsTrue(convertedConfigValues.Intersect(KvpUserlist).Count() == 3);
        }
        #endregion
        #region Keys of App Tests
        [TestMethod]
        public void SaveAppValue_ShouldSaveAndVerifyTheValueForAAppToPersistentSource()
        {

            _log.Info("Save App Value Test");

            _config.SaveValues(AppTestObject, KvpAppList);
            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(k => k.Intersect(KvpAppList).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void SaveAppValues_ShouldSaveAndVerifyTheValuesForAAppToPersistentSource()
        {
            _log.Info("Save App Values Test");

            KVPList kvpList = KvpAppList.Where(e => e.Key.Equals(KeyList[0])).ToList();
            _config.SaveValues(AppTestObject, kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 1 && klist.Intersect(kvpList).Count() == 1)), Times.Once);
        }

        [TestMethod]
        public void GetAppValue_GetsAndVerifyTheValueForAAppFromPersistentSource()
        {
            _log.Info("Get App Value Test");

            KVPList _kvpList = KvpAppList.Where(e => e.Key.Equals(KeyList[0])).ToList();

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(_kvpList);

            var configVal = _config.GetValue(AppTestObject, KeyList[0]);

            Assert.AreEqual(_kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetAppValues_GetsAndVerifyTheValuesForAAppFromPersistentSource()
        {
            _log.Info("Get App Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(KvpAppList);

            var convertedConfigValues = _config.GetValues(AppTestObject, KeyList);
            Assert.IsTrue(convertedConfigValues.Intersect(KvpAppList).Count() == 3);
        }
        #endregion
        #region Keys of App with User Tests
        [TestMethod]
        public void SaveAppUserValue_ShouldSaveAndVerifyTheValueForAnAppWithUserToPersistentSource()
        {

            _log.Info("Save App with User Value Test");

            _config.SaveValues(AppTestObject, UserTestObject, KvpAppWithUserList);
            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(k => k.Intersect(KvpAppWithUserList).Count() == 3)), Times.Once);
        }

        [TestMethod]
        public void SaveAppUserValues_ShouldSaveAndVerifyTheValuesForAAppToPersistentSource()
        {
            _log.Info("Save App with User Values Test");

            KVPList kvpList = KvpAppWithUserList.Where(e => e.Key.Equals(KeyList[0])).ToList();
            _config.SaveValues(AppTestObject, UserTestObject, kvpList);

            _persistSource.Verify(e => e.PersistValues(It.Is<KVPList>(klist => klist.Count == 1 && klist.Intersect(kvpList).Count() == 1)), Times.Once);
        }

        [TestMethod]
        public void GetAppUserValue_GetsAndVerifyTheValueForAAppFromPersistentSource()
        {
            _log.Info("Get App with User Value Test");

            KVPList _kvpList = KvpAppWithUserList.Where(e => e.Key.Equals(KeyList[0])).ToList();

            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<KList>())).Returns(_kvpList);

            var configVal = _config.GetValue(AppTestObject, UserTestObject, KeyList[0]);

            Assert.AreEqual(_kvpList[0].Value, configVal);
        }

        [TestMethod]
        public void GetAppUserValues_GetsAndVerifyTheValuesForAAppFromPersistentSource()
        {
            _log.Info("Get App with User Values Test");
            _persistSource.Setup(ps => ps.LoadValues(It.IsAny<List<string>>())).Returns(KvpAppWithUserList);

            var convertedConfigValues = _config.GetValues(AppTestObject, UserTestObject, KeyList);
            Assert.IsTrue(convertedConfigValues.Intersect(KvpAppWithUserList).Count() == 3);
        }
        #endregion
    }
}
