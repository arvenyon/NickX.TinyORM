using NickX.TinyORM.Mapping.Classes.Common;
using NickX.TinyORM.Mapping.Interfaces;
using System;
using System.Collections.Generic;

namespace NickX.TinyORM.Mapping.Classes.Annotation
{
    public class AnnotationTableDefinition : ITableDefinition
    {
        #region ITableDefinition
        public IReadOnlyCollection<IColumnDefinition> Columns => _columns;
        public Type Type { get; set; }
        public string TableName { get; set; }
        public IColumnDefinition PrimaryKey { get; set; }
        public IReadOnlyCollection<IForeignKeyDefinition> ForeignKeys => _foreignKeys;
        #endregion

        private List<AnnotationColumnDefinition> _columns = new List<AnnotationColumnDefinition>();
        private List<ForeignKeyDefinition> _foreignKeys = new List<ForeignKeyDefinition>();

        public void AddColumn(AnnotationColumnDefinition column)
        {
            _columns.Add(column);
        }

        public void AddForeignKey(ForeignKeyDefinition foreignKey)
        {
            _foreignKeys.Add(foreignKey);
        }
    }
}
