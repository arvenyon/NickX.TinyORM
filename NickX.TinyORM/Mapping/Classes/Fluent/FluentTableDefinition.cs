using NickX.TinyORM.Mapping.Classes.Common;
using NickX.TinyORM.Mapping.Enums;
using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Mapping.MappingUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NickX.TinyORM.Mapping.Classes.Fluent
{
    public class FluentTableDefinition<T> : ITableDefinition where T : class
    {
        #region ITableDefinition
        public IReadOnlyCollection<IColumnDefinition> Columns => _columns;
        public Type Type { get; private set; }
        public string TableName { get; private set; }
        public IColumnDefinition PrimaryKey { get; private set; }
        public IReadOnlyCollection<IForeignKeyDefinition> ForeignKeys => _foreignKeys;
        #endregion

        #region Fields
        private List<IForeignKeyDefinition> _foreignKeys;
        private List<FluentColumnDefinition> _columns;
        private FluentMapping _mapping;
        private List<PropertyInfo> _ignoredProperties;
        #endregion

        #region Ctor
        public FluentTableDefinition(string tableName, FluentMapping mapping)
        {
            this.TableName = tableName;
            this.Type = typeof(T);
            _mapping = mapping;

            _columns = new List<FluentColumnDefinition>();
            _foreignKeys = new List<IForeignKeyDefinition>();
            _ignoredProperties = new List<PropertyInfo>();
        }
        #endregion

        #region Misc
        public FluentMapping PackUp()
        {
            // get all not mapped properties which are not ignored
            var remainingProperties = new List<PropertyInfo>();

            foreach (var property in this.Type.GetProperties())
            {
                if (_columns.Any(c => c.Property.Name == property.Name))
                    continue;
                if (_ignoredProperties.Any(c => c.PropertyType.Name == property.Name))
                    continue;
                if (this.PrimaryKey.Property.Name == property.Name)
                    continue;

                remainingProperties.Add(property);
            }

            //var remainingProperties = this.Type.GetProperties()
            //    .Where(
            //    p => 
            //        !_columns.Any(c => c.Property == p)  // only if is not mapped manually
            //        && !_ignoredProperties.Contains(p)  // only if not set as ignored
            //        && (this.PrimaryKey != null && this.PrimaryKey.Property != p)).ToList(); // only if is not primary key

            // map them with default values
            foreach (var property in remainingProperties)
            {
                var column = new FluentColumnDefinition(property, property.Name, true, default, DefaultValues.None);
                _columns.Add(column);
            }

            // return mapping for further table mappings
            return _mapping;
        }

        public FluentTableDefinition<T> IgnoreProperty(Expression<Func<T, object>> propertyExpression)
        {
            var property = propertyExpression.ToProperty();
            if (_ignoredProperties.Contains(property))
                throw new InvalidOperationException("The Property {0} is already ignored.");

            _ignoredProperties.Add(property);
            return this;
        }
        #endregion

        #region Constraints
        public FluentTableDefinition<T> SetPrimaryKey(Expression<Func<T, object>> propertyExpression, string columnName = null, int length = default, DefaultValues defaultValue = DefaultValues.AutoIncrement, object customDefaultValue = null)
        {
            if (this.PrimaryKey != null)
                throw new InvalidOperationException("Only a single property can be set as primary key!");

            var property = propertyExpression.ToProperty();

            // check if already mapped
            if (_columns.Any(c => c.Property == property))
                throw new InvalidOperationException("PrimaryKeys should not be mapped, simple set them via SetPrimaryKey(...).");

            if (columnName == null)
                columnName = property.Name;

            var colDef = new FluentColumnDefinition(property, columnName, false, length, defaultValue, customDefaultValue);

            this.PrimaryKey = colDef;
            return this;
        }

        public FluentTableDefinition<T> AddForeignKey<TRef>(Expression<Func<T, object>> propertyExpression, Expression<Func<TRef, object>> referencedPropertyExpression) where TRef : class
        {
            var property = propertyExpression.ToProperty();
            var referencedProperty = referencedPropertyExpression.ToProperty();

            if (_foreignKeys.Any(fk => fk.BoundProperty == property && fk.ReferencedProperty == referencedProperty))
                throw new InvalidOperationException(string.Format("A foreign key definition from Property {0} to Type {1} with Property {2} already exists.", property.Name, typeof(TRef).Name, referencedProperty.Name));
            
            var foreignKey = new ForeignKeyDefinition(propertyExpression.ToProperty(), typeof(TRef), referencedPropertyExpression.ToProperty());
            _foreignKeys.Add(foreignKey);
            return this;
        }
        #endregion

        #region MapColumn
        public FluentTableDefinition<T> MapColumn
            (
            Expression<Func<T, object>> propertyExpression, 
            string columnName,
            bool allowsNull,
            int length,
            object customDefaultValue)
        {
            var property = propertyExpression.ToProperty();

            // throw if is set as primary key
            if (PrimaryKey != null && property == PrimaryKey.Property)
                throw new Exception(string.Format("The Property {0} is set as PrimaryKey, it doesn't need to be mapped via MapColumn(...)."));

            // throw if already mapped
            if (_columns.Any(c => c.Property == property))
                throw new InvalidOperationException(string.Format("Property {0} has already been mapped.", property.Name));

            // create & add
            var column = new FluentColumnDefinition(property, columnName, allowsNull, length, DefaultValues.Custom, customDefaultValue);
            _columns.Add(column);

            // return self for further property mappings
            return this;
        }

        public FluentTableDefinition<T> MapColumn(
            Expression<Func<T, object>> propertyExpression,
            string columnName,
            bool allowsNull,
            int length,
            DefaultValues defaultValue,
            object customDefaultValue)
        {
            var property = propertyExpression.ToProperty();

            // throw if is set as primary key
            if (PrimaryKey != null && property == PrimaryKey.Property)
                throw new Exception(string.Format("The Property {0} is set as PrimaryKey, it doesn't need to be mapped via MapColumn(...)."));

            // throw if already mapped
            if (_columns.Any(c => c.Property == property))
                throw new InvalidOperationException(string.Format("Property {0} has already been mapped.", property.Name));

            // create & add
            var column = new FluentColumnDefinition(property, columnName, allowsNull, length, defaultValue, customDefaultValue);
            _columns.Add(column);

            // return self for further property mappings
            return this;
        }

        public FluentTableDefinition<T> MapColumn(Expression<Func<T, object>> propertyExpression, string columnName)
        {
            var property = propertyExpression.ToProperty();

            // throw if is set as primary key
            if (PrimaryKey != null && property == PrimaryKey.Property)
                throw new Exception(string.Format("The Property {0} is set as PrimaryKey, it doesn't need to be mapped via MapColumn(...)."));

            // throw if already mapped
            if (_columns.Any(c => c.Property == property))
                throw new InvalidOperationException(string.Format("Property {0} has already been mapped.", property.Name));

            // create & add
            var column = new FluentColumnDefinition(property, columnName, true, default, DefaultValues.None);
            _columns.Add(column);

            // return self for further property mappings
            return this;
        }

        public FluentTableDefinition<T> MapColumn(
            Expression<Func<T, object>> propertyExpression, 
            bool allowsNull, 
            int length = default, 
            DefaultValues defaultValue = DefaultValues.None,
            object customDefaultValue = null)
        {
            var property = propertyExpression.ToProperty();

            // throw if is set as primary key
            if (PrimaryKey != null && property == PrimaryKey.Property)
                throw new Exception(string.Format("The Property {0} is set as PrimaryKey, it doesn't need to be mapped via MapColumn(...).", property.Name));

            // throw if already mapped
            if (_columns.Any(c => c.Property == property))
                throw new InvalidOperationException(string.Format("Property {0} has already been mapped.", property.Name));

            // create & add
            var column = new FluentColumnDefinition(property, property.Name, allowsNull, length, defaultValue, customDefaultValue);
            _columns.Add(column);

            // return self for further property mappings
            return this;
        }
        #endregion
    }
}
