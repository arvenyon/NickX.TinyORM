using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NickX.TinyORM.Mapping.Interfaces
{
    public interface IMapping
    {
        IReadOnlyCollection<ITableDefinition> Tables { get; }
        string ResolveTableName<T>() where T : class, new();
        string ResolveColumnName<T>(Expression<Func<T, object>> propertyExpression) where T : class, new();
    }
}
