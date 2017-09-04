using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Serializers.DynamicSerializer.Extentions
{
	internal static class BaseTypeSizeHelper
	{
		public static int SizeOfPrimitive(this Type type)
		{
			if (type == typeof(bool))
			{
				return sizeof(bool);
			}
			else if (type == typeof(char))
			{
				return sizeof(char);
			}
			else if (type == typeof(sbyte))
			{
				return sizeof(sbyte);
			}
			else if (type == typeof(short))
			{
				return sizeof(short);
			}
			else if (type == typeof(int))
			{
				return sizeof(int);
			}
			else if (type == typeof(long))
			{
				return sizeof(long);
			}
			else if (type == typeof(byte))
			{
				return sizeof(byte);
			}
			else if (type == typeof(ushort))
			{
				return sizeof(ushort);
			}
			else if (type == typeof(uint))
			{
				return sizeof(uint);
			}
			else if (type == typeof(ulong))
			{
				return sizeof(ulong);
			}
			else if (type == typeof(float))
			{
				return sizeof(float);
			}
			else if (type == typeof(double))
			{
				return sizeof(double);
			}
			else if (type == typeof(decimal))
			{
				return sizeof(decimal);
			}
			throw new ArgumentException($"Incorect type for SizeOfPrimitive {type.Name}. Use it for only primitive types");
		}
	}
}
