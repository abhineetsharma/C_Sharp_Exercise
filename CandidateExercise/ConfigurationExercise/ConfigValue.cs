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
        public App App { get; set; }
        public override bool Equals(object obj)
        {
            if (null != obj && obj is ConfigValue)
            {

                ConfigValue configValueObj = (ConfigValue)obj;
                if (
                    configValueObj.Value.Equals(Value) &&
                    configValueObj.ValueType.Equals(ValueType) &&
                    (App == null && configValueObj.App == null) || App.Equals(configValueObj.App) &&
                    (User == null && configValueObj.User == null) || User.Equals(configValueObj.User)
                    )
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