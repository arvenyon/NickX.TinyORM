using System;
using System.Reflection;

namespace NickX.TinyORM.Persistence.PersistenceUtils
{
    public static class EnumHelper
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttribute<TAttribute>();
        }
    }
}
