using FastMember;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace NickX.TinyORM.Persistence.PersistenceUtils
{
    public static class CrudHelper
    {
        public static T ConvertToObject<T>(this SqlDataReader reader) where T : class, new()
        {
            Type type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (!reader.IsDBNull(i))
                {
                    string fieldName = reader.GetName(i);

                    var member = members.FirstOrDefault(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase));
                    if (member != null)
                    {
                        var value = reader.GetValue(i);
                        value = ConvertValueFromSql(value, member.Type);
                        accessor[t, fieldName] = value;
                    }
                }
            }
            return t;
        }

        public static string ConvertValueForSql(this object value)
        {
            if (value == null)
                return "";

            string retVal = value.ToString();
            if (value.GetType() == typeof(bool))
            {
                retVal = (bool)value == true ? "1" : "0";
            }
            return retVal;
        }

        //public static object GetValueForSql(this PropertyInfo property, object entity)
        //{
        //    object retVal = null;
        //    if (property.PropertyType.IsEnum)
        //    {
        //        retVal = (int)property.
        //    }
        //}

        public static object ConvertValueFromSql(this object value, Type targetType)
        {
            object retVal = value;
            if (targetType == typeof(bool))
            {
                var iVal = value.ToString();
                retVal = iVal == "1";
            }
            return retVal;
        }
    }
}
