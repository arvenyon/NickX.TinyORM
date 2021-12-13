using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Classes.Annotation
{
    public class AnnotationColumnDefinition : IColumnDefinition
    {
        public PropertyInfo Property { get; set; }

        public string ColumnName { get; set; }

        public DefaultValues DefaultValue { get; set; }

        public object CustomDefaultValue { get; set; }

        public int ColumnLength { get; set; }

        public bool AllowsNull { get; set; }
    }
}
