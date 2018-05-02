namespace FutureWonder.Exercises.Configuration
{
    public enum ValueType
    {
        ValueNone,
        ValueInt,
        ValueDouble,
        ValueString
    }

    public struct ConfigValue
    {
        public string Value { get; set; }
        public ValueType ValueType { get; set; }
        public User User { get; set; }

        public override bool Equals(object obj)
        {
            if (null != obj && obj is ConfigValue)
            {
                ConfigValue configValueObj = (ConfigValue)obj;
                if (!configValueObj.Value.Equals(Value))
                    return false;
                if (!configValueObj.ValueType.Equals(ValueType))
                    return false;
                if (User != null && configValueObj.User != null)
                    return User.Equals(configValueObj.User);
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}