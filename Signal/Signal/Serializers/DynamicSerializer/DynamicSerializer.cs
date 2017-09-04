using Signal.Serializers.DynamicSerializer.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static Signal.Serializers.DynamicSerializer.DynamicBuffer;

namespace Signal.Serializers.DynamicSerializer
{
	internal class DynamicSerializer
	{
		#region static instancesBlock

		private static Dictionary<Type, DynamicSerializer> _instances = new Dictionary<Type, DynamicSerializer>();

		public static DynamicSerializer Instance(Type type)
		{
			if (!_instances.ContainsKey(type))
			{
				_instances.Add(type, new DynamicSerializer(type));
			}
			return _instances[type];
		}

		#endregion static instancesBlock

		private static byte[] nullPtrBytres = BitConverter.GetBytes(((short)-1));

		private Func<object, byte[]> _Serialize;

		private List<MemberInfo> _memberInfo;

		private Type _type;

		internal int _size = 0;

		private bool? _isHasReference = null;

		internal bool isHasReference
		{
			get
			{
				if(_isHasReference ==null)
				{
					_isHasReference = FindReferenceType();
				}
				return _isHasReference.Value;
			}
		}

		#region serialize/Desirialize method

		public object Deserialize(byte[] buffer)
		{
			throw new NotImplementedException();
		}

		public byte[] Serialize(object entity)
		{
			if (_Serialize != null)
			{
				return _Serialize(entity);
			}
			else
			{
				return ReferenceTypeSerialize(entity);
			}
		}

		#endregion serialize/Desirialize method

		#region constructor

		private DynamicSerializer(Type type)
		{
			_type = type;
			if (!_type.IsPrimitive)
			{
				_memberInfo = _type.GetMembers().ToList();
				_memberInfo = _type.GetMembers(
						 BindingFlags.NonPublic |
						 BindingFlags.Public |
						 BindingFlags.Instance)
						 .Where(x => x.MemberType == MemberTypes.Field).ToList();
			}

			_size = GetSize();

			if (_type.IsValueType &&
			(_type.IsPrimitive || !isHasReference))
			{
				_Serialize = (buffer) =>
				{
					BitSerializer bitSerializer = BitSerializer.GetInstanse(_type);
					return bitSerializer.Serialize(buffer);
				};
			}
			else
			{
				_Serialize = (entity) =>
				{
					return ReferenceTypeSerialize(entity);
				};
			}
		}

		private int GetSize()
		{
			if(_type.IsPrimitive)
			{
				return _type.SizeOfPrimitive();
			}
			int size = 0;
			foreach (var innerMember in _memberInfo)
			{
				var innerMemberType = GetMemberType(innerMember);
				if (innerMemberType.IsPrimitive)
				{
					size += innerMemberType.SizeOfPrimitive();
				}
				else if (innerMemberType.IsValueType)
				{
					if (DynamicSerializer.Instance(innerMemberType).isHasReference)
					{
						size += sizeof(short);
					}
					else
					{
						size += Marshal.SizeOf(innerMemberType);
					}
				}
				else
				{
					size += sizeof(short);
				}
			}
			return size;
		}

		#endregion constructor

		#region Reflection Helpers

		private bool FindReferenceType()
		{
			var members =  _memberInfo;
			foreach (var member in members)
			{
				Type memberType = GetMemberType(member);

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
						if (Instance(memberType).isHasReference)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private Type GetMemberType(MemberInfo member)
		{
			Type memberType = null;
			if (member.MemberType == MemberTypes.Field)
			{
				memberType = ((FieldInfo)member).FieldType;
			}
			else if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				memberType = propertyInfo.PropertyType;
			}
			return memberType;
		}

		private object GetValue(object entity, MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Field)
			{
				FieldInfo field = (FieldInfo)member;
				return field.GetValue(entity);
			}
			else if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				return propertyInfo.GetValue(entity);
			}
			return null;
		}

		#endregion Reflection Helpers

		private BufferPtr ReferenceSerizlize(object Entity, DynamicBuffer buffer, Dictionary<object, BufferPtr> referenceMaping)
		{
			int size = _size;
			byte[] current = new byte[size];
			var ptr = buffer.Alloc(size);
			if (!_type.IsValueType)
			{
				referenceMaping.Add(Entity, ptr);
			}
			int currentPadding = 0;
			foreach (var member in _memberInfo)
			{
				Type memberType = GetMemberType(member);
				object value = GetValue(Entity, member);
				if (value == null) //null ptr
				{
					Array.Copy(nullPtrBytres, 0, current, currentPadding, nullPtrBytres.Length);
					currentPadding += nullPtrBytres.Length;
				}
				else if (referenceMaping.ContainsKey(value))
				{
					var objectptr = referenceMaping[value];
					var memberBytes = BitConverter.GetBytes(objectptr.position);
					Array.Copy(memberBytes, 0, current, currentPadding, memberBytes.Length);
					currentPadding += memberBytes.Length;
					continue;
				}
				if (memberType.IsValueType &&
				(memberType.IsPrimitive ||  !Instance(memberType).isHasReference))
				{
					BitSerializer BitSerializer = BitSerializer.GetInstanse(memberType);
					var memberBytes = BitSerializer.Serialize(GetValue(Entity, member));
					Array.Copy(memberBytes, 0, current, currentPadding, memberBytes.Length);
					currentPadding += memberBytes.Length;
					continue;
				}
				else
				{
					var objectptr = Instance(memberType).ReferenceSerizlize(value, buffer, referenceMaping);
					var memberBytes = BitConverter.GetBytes(objectptr.position);
					Array.Copy(memberBytes, 0, current, currentPadding, memberBytes.Length);
					currentPadding += memberBytes.Length;
					continue;
				}
			}
			ptr.Set(current);
			return ptr;
		}

		private byte[] ReferenceTypeSerialize(object Entity)
		{
			var referenceMaping = new Dictionary<object, BufferPtr>();

			var buffer = new DynamicBuffer(100);
			ReferenceSerizlize(Entity, buffer, referenceMaping);
			return buffer.Buffer;
		}
	}
}