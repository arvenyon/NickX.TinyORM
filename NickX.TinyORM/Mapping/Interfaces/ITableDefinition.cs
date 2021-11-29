using System;
using System.Collections.Generic;

namespace NickX.TinyORM.Mapping.Interfaces
{
    public interface ITableDefinition
    {
        IReadOnlyCollection<IColumnDefinition> Columns { get; }
        Type Type { get; }
        string TableName { get; }
        IColumnDefinition PrimaryKey { get; }
        IReadOnlyCollection<IForeignKeyDefinition> ForeignKeys { get; }
    }
}
