using NickX.TinyORM.Mapping.Interfaces;
using System;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Classes.Common
{
    public class ForeignKeyDefinition : IForeignKeyDefinition
    {
        public PropertyInfo BoundProperty { get; private set; }
        public PropertyInfo ReferencedProperty { get; private set; }
        public Type ReferencedType { get; private set; }

        public ForeignKeyDefinition(PropertyInfo boundProperty, Type referencedType, PropertyInfo referencedProperty)
        {
            this.BoundProperty = boundProperty;
            this.ReferencedType = referencedType;
            this.ReferencedProperty = referencedProperty;
        }
    }
}
