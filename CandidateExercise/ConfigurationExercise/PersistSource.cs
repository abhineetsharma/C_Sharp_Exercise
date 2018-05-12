using System;
using System.Collections.Generic;

namespace FutureWonder.Exercises.Configuration
{
    using KVP = KeyValuePair<string, ConfigValue>;
    using KVPList = IList<KeyValuePair<string, ConfigValue>>;
    using KList = IList<string>;

    public class PersistException : Exception
    { 
        public PersistException(String message, PersistException exception) : base(message, exception) { }

    }

    public interface IPersistSource
    {
        void PersistValues(KVPList values);
        KVPList LoadValues(KList keys);
    }

    // Note no PersistStore needed for this exercise
}