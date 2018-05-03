using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FutureWonder.Exercises.Configuration
{
    using KList = IList<string>;
    using KVP = KeyValuePair<string, ConfigValue>;
    using KVPList = IList<KeyValuePair<string, ConfigValue>>;

    // Strongly named exceptions
    public class ConfigErrorException : Exception
    {
        public ConfigErrorException(String message, PersistException exception) : base(message, exception)
        {
            Console.WriteLine(message);
        }
    }

    public class User
    {
        public string Username { get; set; }

        public override bool Equals(object obj)
        {
            User user = obj as User;
            if (null != user)
            {
                return user.Username.Equals(Username);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Username;
        }
    }

    public class App
    {
        public string Appname { get; set; }
    }

    public interface IConfig
    {
        void Initialize();
        void Shutdown(); // if needed

        #region 1. Basic operations

        ConfigValue GetValue(string key);
        void SaveValue(KVP kvp);
        void SaveValues(KVPList kvps);
        KVPList GetValues(KList keys);

        #endregion 1. Basic operations

        #region 2. User operations

        ConfigValue GetValue(User user, string key);
        KVPList GetValues(User user, KList keys);
        void SaveValue(User user, KVP kvp);
        void SaveValues(User user, KVPList kvps);

        #endregion 2. User operations

        #region 3a. App operations

        ConfigValue GetValue(App app, string key);
        KVPList GetValues(App user, KList keys);
        void SaveValue(App user, KVP kvp);
        void SaveValues(App user, KVPList kvps);

        #endregion 3a. App operations

        #region 3b. App / User operations

        ConfigValue GetValue(App app, User user, string key);
        KVPList GetValues(App app, User user, KList keys);
        void SaveValue(App app, User user, KVP kvp);
        void SaveValues(App app, User user, KVPList kvps);

        #endregion 3b. App / User operations
    }

    public class Config : IConfig
    {
        private readonly ILog _log = LogManager.GetLogger(nameof(Config));
        private IPersistSource _persistSource;

        public Config(IPersistSource storage)
        {
            _persistSource = storage;
        }

        public void Initialize()
        {
            _log.Info("Initializing Config");
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public ConfigValue GetValue(string key)
        {
            _log.Info("In GetValue");
            try
            {
                var persistenceStorage = _persistSource;
                KVPList kvList = GetValues(new List<String>() { key });
                if (kvList == null)
                    throw new PersistException("cannon get the value " + key, null);
                return kvList[0].Value;
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in  GetValue : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in  GetValue : " + ex.Message);
                throw ex;
            }
        }

        public void SaveValue(KVP kvp)
        {
            _log.Info("In SaveValue");
            try
            {
                var list = new List<KVP>() { kvp };
                SaveValues(list);
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in  SaveValue : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in  SaveValue : " + ex.Message);
                throw ex;
            }
        }

        public void SaveValues(KVPList kvps)
        {
            _log.Info("In SaveValues");
            try
            {
                _persistSource.PersistValues(kvps);
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in  SaveValues : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in  SaveValues : " + ex.Message);
                throw ex;
            }

        }

        public KVPList GetValues(KList keys)
        {
            _log.Info("In GetValues");
            KVPList kvpList = null;
            var storage = _persistSource;
            if (null != storage)
            {
                try
                {
                    kvpList = storage.LoadValues(keys);
                    if (kvpList == null)
                        throw new PersistException("cannon get the values for the keys " + keys.ToString(), null);

                }
                catch (PersistException ex)
                {
                    _log.Info("Persist Exception in  GetValues : " + ex.Message);
                    throw ex;
                }
                catch (Exception ex)
                {
                    _log.Info("Persist Exception in  GetValues : " + ex.Message);
                    throw ex;
                }
            }
            return kvpList;
        }

        public ConfigValue GetValue(User user, string key)
        {
            _log.Info("In GetValue for User " + user);
            try
            {
                var kvpList = GetValues(new List<String>() { key }).Where(e => e.Value.User.Equals(user)).ToList();
                if (kvpList == null)
                    throw new PersistException("cannon get the value for the user " + user + " for key " + key, null);
                return kvpList[0].Value;
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in  GetValues for User " + user + " : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in  GetValues for User " + user + " : " + ex.Message);
                throw ex;
            }
        }

        public KVPList GetValues(User user, KList keys)
        {
            _log.Info("In GetValues for User " + user);
            KVPList kvpList = null;
            var storage = _persistSource;
            if (null != storage)
            {
                try
                {

                    kvpList = GetValues(keys).Where(e => e.Value.User.Equals(user)).ToList();
                }
                catch (PersistException ex)
                {
                    _log.Info("Persist Exception in  GetValues for User " + user + " : " + ex.Message);
                    throw ex;
                }
                catch (Exception ex)
                {
                    _log.Info("Exception in  GetValues for User " + user + " : " + ex.Message);
                    throw ex;
                }
            }
            return kvpList;
        }

        public void SaveValue(User user, KVP kvp)
        {
            _log.Info("in SaveValue for User " + user);
            try
            {
                SaveValues(user, new List<KVP> { kvp });
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in SaveValue for User " + user + " : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Exception in SaveValue for User " + user + " : " + ex.Message);
                throw ex;
            }
        }

        public void SaveValues(User user, KVPList kvps)
        {
            _log.Info("in SaveValues for User " + user);
            try
            {
                var kvpList = kvps.Where(e => e.Value.User != null && e.Value.User.Equals(user)).ToList();
                SaveValues(kvpList);
            }
            catch (PersistException ex)
            {
                _log.Info("Persist Exception in SaveValues for User " + user + " : " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                _log.Info("Persist Exception in SaveValues for User " + user + " : " + ex.Message);
                throw ex;
            }

        }

        public ConfigValue GetValue(App app, string key)
        {
            throw new NotImplementedException();
        }

        public KVPList GetValues(App user, KList keys)
        {
            throw new NotImplementedException();
        }

        public void SaveValue(App user, KVP kvp)
        {
            throw new NotImplementedException();
        }

        public void SaveValues(App user, KVPList kvps)
        {
            throw new NotImplementedException();
        }

        public ConfigValue GetValue(App app, User user, string key)
        {
            throw new NotImplementedException();
        }

        public KVPList GetValues(App app, User user, KList keys)
        {
            throw new NotImplementedException();
        }

        public void SaveValue(App app, User user, KVP kvp)
        {
            throw new NotImplementedException();
        }

        public void SaveValues(App app, User user, KVPList kvps)
        {
            throw new NotImplementedException();
        }
    }
}