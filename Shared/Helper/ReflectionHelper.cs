using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SoftwareMind.Shared.Helper
{
    public static class ReflectionHelper
    {
        public static MethodInfo GetMethodInfo(Expression<Action> lamda)
        {
            var body = lamda.Body;
            if (body.NodeType != ExpressionType.Call)
            {
                throw new ArgumentException("Invalid expression provided");
            }

            var call = (MethodCallExpression)body;

            return call.Method;
        }
    }
}
