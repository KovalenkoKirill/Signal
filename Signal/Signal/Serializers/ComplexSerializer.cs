using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Serializers
{
	public class ComplexSerializer<T> : ISerializer<T>
	{
		static ISerializer<T> serializer;
		static ISerializer<string> stringSerializer;

		static ComplexSerializer()
		{
			if (typeof(T) == typeof(string))
			{
				stringSerializer = new StringSerializer<string>();
			}
			else if (typeof(T).IsValueType)
			{
				if (FindReferenceType(typeof(T)))
				{
					serializer = new BinarySerializer<T>();
				}
				else
				{
					serializer = new BitSerializer<T>();
				}
			}
			else
			{
				serializer = new BinarySerializer<T>();
			}
		}

		private static bool FindReferenceType(Type type)
		{
			var members = type.GetMembers();
			foreach (var member in members)
			{
				Type memberType = null;
				if(member.MemberType == MemberTypes.Field)
				{
					memberType = member.DeclaringType;
				}
				else if(member.MemberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = (PropertyInfo)member;
					memberType = propertyInfo.PropertyType;
				}

				if (memberType != null)
				{
					if (memberType.IsValueType && memberType.IsPrimitive)
						continue;
					if (!memberType.IsValueType)
					{
						return true;
					}
					else
					{
						if(FindReferenceType(memberType))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public T Deserialize(byte[] buffer)
		{
			if(stringSerializer == null)
			{
				return serializer.Deserialize(buffer);
			}
			else
			{
				return (T)Convert.ChangeType(stringSerializer.Deserialize(buffer), typeof(string));
			}
		}

		public byte[] Serialize(T entity)
		{
			if (stringSerializer == null)
			{
				return serializer.Serialize(entity);
			}
			else
			{
				return stringSerializer.Serialize(entity.ToString());
			}
		}
	}
}
