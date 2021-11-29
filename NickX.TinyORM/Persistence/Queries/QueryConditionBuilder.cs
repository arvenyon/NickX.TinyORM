using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Mapping.MappingUtils;
using NickX.TinyORM.Persistence.Attributes;
using NickX.TinyORM.Persistence.PersistenceUtils;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NickX.TinyORM.Persistence.Queries
{
    public class QueryConditionBuilder<T> where T : class, new()
    {
        public string Query { get; private set; }

        private IMapping _mapping;

        public QueryConditionBuilder(IMapping mapping)
        {
            _mapping = mapping;
        }

        public QueryConditionBuilder<T> Start(Expression<Func<T, object>> propertyExpression, QueryOperators queryOperator, object value)
        {
            Query += BuildQuery(propertyExpression, queryOperator, value);
            return this;
        }

        public QueryConditionBuilder<T> And(Expression<Func<T, object>> propertyExpression, QueryOperators queryOperator, object value)
        {
            Query += " AND " + BuildQuery(propertyExpression, queryOperator, value);
            return this;
        }

        public QueryConditionBuilder<T> Or(Expression<Func<T, object>> propertyExpression, QueryOperators queryOperator, object value)
        {
            Query += " OR " + BuildQuery(propertyExpression, queryOperator, value);
            return this;
        }


        private string BuildQuery(Expression<Func<T, object>> propertyExpression, QueryOperators queryOperator, object value)
        {
            var queryOperatorSqlValue = queryOperator.GetAttribute<QueryOperatorAttribute>().SqlValue;
            var columnName = _mapping.ResolveColumnName(propertyExpression);
            value = value.ConvertValueForSql();
            switch (queryOperator)
            {
                case QueryOperators.Contains:
                case QueryOperators.NotContains:
                    value = string.Format(@"'%{0}%'", value);
                    break;
                case QueryOperators.StartsWith:
                case QueryOperators.NotStartsWith:
                    value = string.Format(@"'{0}%'", value);
                    break;
                case QueryOperators.EndsWith:
                case QueryOperators.NotEndsWith:
                    value = string.Format(@"'%{0}'", value);
                    break;
                default:
                    value = string.Format(@"'{0}'", value);
                    break;
            }

            return string.Format("[{0}] {1} {2}", columnName, queryOperatorSqlValue, value);
        }

    }

    public enum QueryOperators
    {
        [QueryOperator("=")]
        Equals,

        [QueryOperator("<>")]
        NotEquals,

        [QueryOperator("like")]
        Contains,

        [QueryOperator("not like")]
        NotContains,

        [QueryOperator("like")]
        StartsWith,

        [QueryOperator("not like")]
        NotStartsWith,

        [QueryOperator("like")]
        EndsWith,

        [QueryOperator("not like")]
        NotEndsWith,

        [QueryOperator(">")]
        GreaterThan,

        [QueryOperator("<")]
        LessThan,

        [QueryOperator(">=")]
        GreaterThanOrEqual,

        [QueryOperator("<=")]
        LessThanOrEqual
    }
}
