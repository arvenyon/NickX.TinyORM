using System;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Interfaces
{
    public interface IForeignKeyDefinition
    {
        PropertyInfo BoundProperty { get; }
        PropertyInfo ReferencedProperty { get; }
        Type ReferencedType { get; }
    }
}
