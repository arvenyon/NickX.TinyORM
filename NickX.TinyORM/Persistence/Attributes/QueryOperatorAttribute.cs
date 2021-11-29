using System;

namespace NickX.TinyORM.Persistence.Attributes
{
    public class QueryOperatorAttribute : Attribute
    {
        public string SqlValue { get; private set; }

        public QueryOperatorAttribute(string sqlValue)
        {
            this.SqlValue = sqlValue;
        }

    }
}
