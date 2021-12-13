using System;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Attributes
{
    public class ForeignKeyAttribute : Attribute 
    {
        public Type ReferencedType { get; set; }
        public PropertyInfo ReferencedProperty { get; set; }

        public ForeignKeyAttribute(Type referencedType, string referencedPropertyName)
        {
            this.ReferencedType = referencedType;

            var prop = referencedType.GetProperty(referencedPropertyName);
            if (prop == null)
                throw new InvalidOperationException("A property with given name does not exist in the referenced type.");
            this.ReferencedProperty = prop;
        }
    }
}
