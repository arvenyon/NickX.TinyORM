using NickX.TinyORM.Mapping.Interfaces;
using NickX.TinyORM.Mapping.MappingUtils;
using NickX.TinyORM.Persistence.Attributes;
using NickX.TinyORM.Persistence.PersistenceUtils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NickX.TinyORM.Persistence.Queries
{
    public class QueryConditionBuilder<T> where T : class, new()
    {
        public string Query { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();  

        private IMapping _mapping;
        private int _paramIndex = 1;

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

            //// enum handling
            //var property = propertyExpression.ToProperty();
            //if (property.PropertyType.IsEnum && value.GetType() != typeof(int))
            //    value = (int)value;

            value = value.ConvertValueForSql();
            switch (queryOperator)
            {
                case QueryOperators.Contains:
                case QueryOperators.NotContains:
                    value = string.Format(@"%{0}%", value);
                    break;
                case QueryOperators.StartsWith:
                case QueryOperators.NotStartsWith:
                    value = string.Format(@"{0}%", value);
                    break;
                case QueryOperators.EndsWith:
                case QueryOperators.NotEndsWith:
                    value = string.Format(@"%{0}", value);
                    break;
                default:
                    value = string.Format(@"{0}", value);
                    break;
            }

            // create param name from index
            var param = "@p" + _paramIndex;
            
            // up ze index of ze params
            _paramIndex++;

            // cache param name & its value in parameters dict
            this.Parameters.Add(param, value);

            // build query & return
            return string.Format("[{0}] {1} {2}", columnName, queryOperatorSqlValue, param);
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
