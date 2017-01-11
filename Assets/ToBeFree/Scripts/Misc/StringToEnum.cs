using System;
using ToBeFree;

static public class EnumConvert<T> where T : IConvertible
{
	static public T ToEnum(string enumName)
	{
		if(Enum.IsDefined(typeof(T), enumName) == false)
		{
			return default(T);
		}
		return (T)Enum.Parse(typeof(T), enumName);
	}

	static public string ToString(T enumValue)
	{
		if (Enum.IsDefined(typeof(T), enumValue) == false)
		{
			return string.Empty;
		}
		return Enum.GetName(typeof(T), enumValue);
	}
}
