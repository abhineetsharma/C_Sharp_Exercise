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
        public App(string AppnameI)
        {
            Appname = AppnameI;
        }

        public string Appname { get; set; }

        public override bool Equals(object obj)
        {
            var app = obj as App;
            return app != null &&
                   Appname.Equals(app.Appname);
        }

        public override string ToString()
        {
            return Appname;
        }
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
        KVPList GetValues(App app, KList keys);
        void SaveValue(App app, KVP kvp);
        void SaveValues(App app, KVPList kvps);

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
        private Exception catchAndThrowException(String message, Exception ex, User user = null, App app = null)
        {
            _log.Error($"{message} {(app != null ? $"for App {app} " : String.Empty)} {(user != null ? $"for User {user} " : String.Empty)}: {ex.Message}");
            return ex;
        }
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
                KVPList kvList = GetValues(new List<String>() { key });
                if (kvList == null)
                    throw new PersistException("cannot get the value " + key, null);
                return kvList[0].Value;
            }
            catch (PersistException ex)
            {
                throw catchAndThrowException("Persist Exception in  GetValue", ex);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in  GetValue", ex);
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
                throw catchAndThrowException("Persist Exception in  SaveValue", ex);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in  SaveValue", ex);
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
                throw catchAndThrowException("Persist Exception in  SaveValues", ex);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in  SaveValues", ex);
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
                        throw new PersistException("cannot get the values for the keys " + keys.ToString(), null);

                }
                catch (PersistException ex)
                {
                    throw catchAndThrowException("Persist Exception in  GetValues", ex);

                }
                catch (Exception ex)
                {
                    throw catchAndThrowException("Exception in  GetValues", ex);
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
                return kvpList[0].Value;
            }
            catch (PersistException ex)
            {
                throw catchAndThrowException("Persist Exception in  GetValues for User", ex, user: user);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in  GetValues for User", ex, user: user);
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
                    throw catchAndThrowException("Persist Exception in  GetValues for User", ex, user: user);
                }
                catch (Exception ex)
                {
                    throw catchAndThrowException("Exception in  GetValues for User", ex, user: user);
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
                throw catchAndThrowException("Persist Exception in SaveValue for User", ex, user: user);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in SaveValue for User ", ex, user: user);
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
                throw catchAndThrowException("Persist Exception in SaveValues for User", ex, user: user);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in SaveValues for User", ex, user: user);
            }

        }

        public ConfigValue GetValue(App app, string key)
        {
            _log.Info("In GetValue for App " + app);

            try
            {
                var kvpList = GetValues(new List<String>() { key }).Where(e => e.Value.App.Equals(app)).ToList();
                return kvpList[0].Value;
            }
            catch (PersistException ex)
            {
                throw catchAndThrowException("Persist Exception in  GetValues for App", ex, app: app);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in  GetValues for App", ex, app: app);
            }
        }

        public KVPList GetValues(App app, KList keys)
        {
            _log.Info("In GetValues for App " + app);
            KVPList kvpList = null;
            var storage = _persistSource;
            if (null != storage)
            {
                try
                {
                    kvpList = GetValues(keys).Where(e => e.Value.App.Equals(app)).ToList();
                }
                catch (PersistException ex)
                {
                    throw catchAndThrowException("Persist Exception in  GetValues for App", ex, app: app);
                }
                catch (Exception ex)
                {
                    throw catchAndThrowException("Exception in  GetValues for App", ex, app: app);
                }
            }
            return kvpList;
        }

        public void SaveValue(App app, KVP kvp)
        {
            _log.Info("in SaveValue for App " + app);
            try
            {
                SaveValues(app, new List<KVP> { kvp });
            }
            catch (PersistException ex)
            {
                throw catchAndThrowException("Persist Exception in SaveValue for App", ex, app: app);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in SaveValue for App ", ex, app: app);
            }
        }

        public void SaveValues(App app, KVPList kvps)
        {
            _log.Info("in SaveValues for App " + app);
            try
            {
                var kvpList = kvps.Where(e => e.Value.App != null && e.Value.App.Equals(app)).ToList();
                SaveValues(kvpList);
            }
            catch (PersistException ex)
            {
                throw catchAndThrowException("Persist Exception in SaveValues for App", ex, app: app);
            }
            catch (Exception ex)
            {
                throw catchAndThrowException("Exception in SaveValues for App", ex, app: app);
            }
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