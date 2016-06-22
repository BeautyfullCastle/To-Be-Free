using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToBeFree
{
    static public class EnumConvert<T> where T : IConvertible
    {
        static public T ToEnum(string enumName)
        {
            return (T)Enum.Parse(typeof(T), enumName);
        }

        static public string ToString(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }
    }
}
