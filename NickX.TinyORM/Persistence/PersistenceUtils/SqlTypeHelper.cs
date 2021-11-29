using System;
using System.Collections.Generic;
using System.Reflection;

namespace NickX.TinyORM.Persistence.PersistenceUtils
{
    public static class SqlTypeHelper
    {
        public static string ToSqlType(this PropertyInfo property)
        {
            var type = property.PropertyType;

            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            if (!_typesMapping.ContainsKey(type))
                throw new NotSupportedException(string.Format("The Type {0} is not supported (yet?)!", type.Name));

            return _typesMapping[type];
        }

        private static Dictionary<Type, string> _typesMapping = new Dictionary<Type, string>()
        {
            { typeof(long), "bigint" },
            { typeof(bool), "bit" },
            { typeof(DateTime), "datetime" },
            { typeof(DateTimeOffset), "datetimeoffset" },
            { typeof(decimal), "decimal" },
            { typeof(double), "float" },
            { typeof(int), "int" },
            { typeof(string), "nvarchar" },
            { typeof(short), "smallint" },
            { typeof(Guid), "uniqueidentifier" }
        };
    }
}
