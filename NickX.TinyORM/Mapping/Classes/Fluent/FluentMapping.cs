using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Mapping.MappingUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NickX.TinyORM.Mapping.Classes.Fluent
{
    public class FluentMapping : IMapping
    {
        #region IMapping
        public IReadOnlyCollection<ITableDefinition> Tables => _tables;
        #endregion

        #region Fields
        private List<ITableDefinition> _tables;
        #endregion

        #region Ctor
        public FluentMapping()
        {
            _tables = new List<ITableDefinition>();
        }
        #endregion

        #region MapTable<T>
        public FluentTableDefinition<T> MapTable<T>() where T : class
        {
            // if no name is defined, revert to default type name
            var name = typeof(T).Name;
            return MapTable<T>(name);
        }

        public FluentTableDefinition<T> MapTable<T>(string tableName) where T : class
        {
            var table = new FluentTableDefinition<T>(tableName, this);

            // check if already mapped
            if (_tables.Any(t => t.Type == typeof(T)))
                throw new InvalidOperationException(string.Format("Type {0} is already mapped.", typeof(T).Name));

            // add table definition
            _tables.Add(table);

            // return self
            return table;
        }

        public string ResolveTableName<T>() where T : class, new()
        {
            var table = this.Tables.SingleOrDefault(t => t.Type == typeof(T));
            if (table == null)
                throw new InvalidOperationException("Requested Type is not mapped!");

            return table.TableName;
        }

        public string ResolveColumnName<T>(Expression<Func<T, object>> propertyExpression) where T : class, new()
        {
            var table = this.Tables.SingleOrDefault(t => t.Type == typeof(T));
            if (table == null)
                throw new InvalidOperationException("Requested Type is not mapped!");

            var property = propertyExpression.ToProperty();

            if (table.PrimaryKey.Property == property)
                return table.PrimaryKey.ColumnName;

            var column = table.Columns.SingleOrDefault(c => c.Property == property);
            if (column == null)
                throw new InvalidOperationException("Requested Property is not mapped!");
            return column.ColumnName;
        }
        #endregion
    }
}
