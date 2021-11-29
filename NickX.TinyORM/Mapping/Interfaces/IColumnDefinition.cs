using NickX.TinyORM.Mapping.Enums;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Interfaces
{
    public interface IColumnDefinition
    {
        PropertyInfo Property { get; }
        string ColumnName { get; }
        DefaultValues DefaultValue { get; }
        object CustomDefaultValue { get; }
        int ColumnLength { get; }
        bool AllowsNull { get; }
    }
}
