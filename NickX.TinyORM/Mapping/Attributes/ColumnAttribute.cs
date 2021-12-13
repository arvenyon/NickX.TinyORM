using NickX.TinyORM.Mapping.Enums;
using System;

namespace NickX.TinyORM.Mapping.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName, bool allowsNull = true, DefaultValues defaultValue = DefaultValues.None, object customDefaultValue = null, int columnLength = default)
        {
            ColumnName = columnName;
            DefaultValue = defaultValue;
            CustomDefaultValue = customDefaultValue;
            AllowsNull = allowsNull;
            ColumnLength = columnLength;
        }

        public string ColumnName { get; set; }
        public DefaultValues DefaultValue { get; set; }
        public object CustomDefaultValue { get; set; }
        public bool AllowsNull { get; set; }
        public int ColumnLength { get; set; }
    }
}
