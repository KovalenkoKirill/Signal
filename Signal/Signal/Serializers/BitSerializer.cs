using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Signal
{
	public class BitSerializer<T> :ISerializer<T>
	{
		static BitSerializer innerSerializer;

		static BitSerializer()
		{
			innerSerializer = BitSerializer.GetInstanse(typeof(T));
		}

		public T Deserialize(byte[] buffer)
		{
			return (T)innerSerializer.Deserialize(buffer);
		}

		public byte[] Serialize(T entity)
		{
			return innerSerializer.Serialize(entity);
		}
	}
	public class BitSerializer
	{
		static Dictionary<Type, BitSerializer> innerLIst = new Dictionary<Type, BitSerializer>();

		public static BitSerializer GetInstanse(Type t)
		{
			if(!innerLIst.ContainsKey(t))
			{
				innerLIst.Add(t,new BitSerializer(t));
			}
			return innerLIst[t];
		}

		internal Func<byte[], object> Deserialize;
		internal Func<object, byte[]> Serialize;

		private BitSerializer(Type type)
		{
			#region PrimitiveTypes
			if (type == typeof(bool))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToBoolean(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((bool)Convert.ChangeType(entity, typeof(bool)));
				};
			}
			else if (type == typeof(char))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToChar(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((char)Convert.ChangeType(entity, typeof(char)));
				};
			}
			else if (type == typeof(sbyte))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType((sbyte)buffer.First(), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((sbyte)Convert.ChangeType(entity, typeof(sbyte)));
				};
			}
			else if (type == typeof(short))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToInt16(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((short)Convert.ChangeType(entity, typeof(short)));
				};
			}
			else if (type == typeof(int))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToInt32(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((int)Convert.ChangeType(entity, typeof(int)));
				};
			}
			else if (type == typeof(long))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToInt64(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((long)Convert.ChangeType(entity, typeof(long)));
				};
			}
			else if (type == typeof(byte))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(buffer.First(), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((byte)Convert.ChangeType(entity, typeof(byte)));
				};
			}
			else if (type == typeof(ushort))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToUInt16(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((ushort)Convert.ChangeType(entity, typeof(ushort)));
				};

			}
			else if (type == typeof(uint))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToUInt32(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((uint)Convert.ChangeType(entity, typeof(uint)));
				};
			}
			else if (type == typeof(ulong))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToUInt64(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((ulong)Convert.ChangeType(entity, typeof(ulong)));
				};
			}
			else if (type == typeof(float))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToSingle(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((float)Convert.ChangeType(entity, typeof(float)));
				};
			}
			else if (type == typeof(double))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToDouble(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((double)Convert.ChangeType(entity, typeof(double)));
				};
			}
			else if (type == typeof(decimal))
			{
				Deserialize = (buffer) =>
				{
					return Convert.ChangeType(BitConverter.ToInt32(buffer, 0), type);
				};
				Serialize = (entity) => {
					return BitConverter.GetBytes((int)Convert.ChangeType(entity, typeof(decimal)));
				};
			}
			#endregion
			else
			{
				Deserialize = (buffer) => {
					object str = Activator.CreateInstance(type);

					int size = Marshal.SizeOf(str);
					IntPtr ptr = Marshal.AllocHGlobal(size);

					Marshal.Copy(buffer, 0, ptr, size);

					str = Marshal.PtrToStructure(ptr, str.GetType());
					Marshal.FreeHGlobal(ptr);

					return str;
				};
				Serialize = (entity) =>
				{
					int size = Marshal.SizeOf(entity);
					byte[] arr = new byte[size];

					IntPtr ptr = Marshal.AllocHGlobal(size);
					Marshal.StructureToPtr(entity, ptr, true);
					Marshal.Copy(ptr, arr, 0, size);
					Marshal.FreeHGlobal(ptr);
					return arr;
				};
			}
		}
	}
}