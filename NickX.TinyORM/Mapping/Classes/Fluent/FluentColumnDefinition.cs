using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Classes.Fluent
{
    public class FluentColumnDefinition : IColumnDefinition
    {
        #region IColumnDefinition
        public PropertyInfo Property { get; private set; }
        public string ColumnName { get; private set; }
        public DefaultValues DefaultValue { get; private set; }
        public object CustomDefaultValue { get; private set; }
        public int ColumnLength { get; private set; }
        public bool AllowsNull { get; private set; }
        #endregion

        #region Ctor
        public FluentColumnDefinition(PropertyInfo property, string columnName, bool allowsNull, int length, DefaultValues defaultValue, object customDefaultValue = null)
        {
            this.Property = property;
            this.AllowsNull = allowsNull;
            this.ColumnName = columnName;
            this.DefaultValue = defaultValue;
            this.CustomDefaultValue = customDefaultValue;
            this.ColumnLength = length;
        }
        #endregion
    }
}
