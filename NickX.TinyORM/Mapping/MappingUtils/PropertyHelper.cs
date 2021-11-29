using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NickX.TinyORM.Mapping.MappingUtils
{
    public static class PropertyHelper
    {
        public static PropertyInfo ToProperty<TType>(this Expression<Func<TType, object>> propertyLambda) where TType : class
        {
            MemberExpression expression = null;

            if (propertyLambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)propertyLambda.Body;
                if (unaryExpression.Operand is MemberExpression)
                {
                    expression = (MemberExpression)unaryExpression.Operand;
                }
                else
                    throw new ArgumentException();
            }
            else if (propertyLambda.Body is MemberExpression)
            {
                expression = (MemberExpression)propertyLambda.Body;
            }
            else
            {
                throw new ArgumentException();
            }

            return (PropertyInfo)expression.Member;
        }
    }
}
