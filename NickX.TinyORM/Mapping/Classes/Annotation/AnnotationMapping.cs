using Humanizer;
using NickX.TinyORM.Mapping.Attributes;
using NickX.TinyORM.Mapping.Classes.Common;
using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Mapping.MappingUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Classes.Annotation
{
    public class AnnotationMapping : IMapping
    {
        #region IMapping
        public IReadOnlyCollection<ITableDefinition> Tables => _tables;

        public string ResolveColumnName<T>(Expression<Func<T, object>> propertyExpression) where T : class, new()
        {
            var property = propertyExpression.ToProperty();
            var attr = property.GetCustomAttribute<ColumnAttribute>();
            if (attr != null)
                return attr.ColumnName;
            return property.Name;
        }

        public string ResolveTableName<T>() where T : class, new()
        {
            return ResolveTableName(typeof(T));
        }

        public string ResolveTableName(Type type)
        {
            var attr = type.GetCustomAttribute<TableAttribute>(false);
            if (attr != null)
                return attr.TableName;
            return type.Name.Pluralize();
        }
        #endregion


        #region Fields
        private List<ITableDefinition> _tables = new List<ITableDefinition>(); 
        #endregion


        public void RegisterTypes(params Type[] types)
        {
            foreach (var type in types)
            {
                RegisterType(type);
            }
        }

        public void RegisterType<T>() where T : class, new()
        {
            RegisterType(typeof(T));
        }

        public void RegisterType(Type type)
        {
            var table = new AnnotationTableDefinition()
            {
                TableName = ResolveTableName(type),
                Type = type
            };

            // validate table
            if (this.Tables.Any(t => t.TableName == table.TableName))
                throw new InvalidOperationException("There's already a class mapped with given table name!");

            foreach (var property in table.Type.GetProperties())
            {
                // cancel out if property is write or read only, this currently only supports read- & writable properties
                if (!property.CanRead || !property.CanWrite)
                    continue;

                // cancel out if property has ignore attribute
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                // get attribute if exists
                var attr = property.GetCustomAttribute<ColumnAttribute>();
                var columnName = property.Name;
                object customDefaultValue = null;
                var defaultValue = DefaultValues.None;
                var length = default(int);
                var allowsNull = true;

                if (attr != null)
                {
                    columnName = attr.ColumnName;
                    defaultValue = attr.DefaultValue;
                    customDefaultValue = attr.CustomDefaultValue;
                    length = attr.ColumnLength;
                    allowsNull = attr.AllowsNull;
                }

                // validate input
                if (table.Columns.Any(c => c.ColumnName == columnName))
                    throw new InvalidOperationException("There's already a property mapped with given column name!");

                var column = new AnnotationColumnDefinition()
                {
                    ColumnName = columnName,
                    ColumnLength = length,
                    AllowsNull = allowsNull,
                    CustomDefaultValue = customDefaultValue,
                    DefaultValue = defaultValue,
                    Property = property
                };
                // set primary key if attribute is set, else add as default mapped column
                // PrimaryKey definition cannot be mapped as normal column due to database generation structure
                if (property.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                    table.PrimaryKey = column;
                else
                    table.AddColumn(column);


                // add foreign key if attribute is set
                var fKAttr = property.GetCustomAttribute<ForeignKeyAttribute>();
                if (fKAttr != null)
                {
                    // throw an invalidoperationexception if the referenced type is not mapped
                    if (!this.Tables.Any(t => t.Type.FullName == fKAttr.ReferencedType.FullName))
                        throw new InvalidOperationException("The referenced type is not mapped! Regarding foreign key references -> Make sure you register your types int the correct order.");

                    table.AddForeignKey(new ForeignKeyDefinition(property, fKAttr.ReferencedType, fKAttr.ReferencedProperty));
                }
            }

            _tables.Add(table);
        }
    }
}
