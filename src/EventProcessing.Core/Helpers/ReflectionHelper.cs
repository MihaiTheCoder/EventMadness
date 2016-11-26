using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventProcessing.Core.Helpers
{
    public static class ReflectionHelper
    {
        public static bool IsSameOrChildOf(this object obj, Type t)
        {
            var objType = obj is Type ? (Type)obj : obj.GetType();
            return IsSameTypeOrChildOf(t, objType);
        }

        private static bool IsSameTypeOrChildOf(Type t, Type objType)
        {
            return objType == t || objType.GetTypeInfo().IsSubclassOf(t);
        }
    }
}
