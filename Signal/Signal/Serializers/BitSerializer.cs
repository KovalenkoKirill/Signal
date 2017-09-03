using System;
using System.Runtime.InteropServices;
using System.Linq;

namespace Signal
{
	public class BitSerializer<T> :ISerializer<T>
	{
		static Func<byte[], T> _Deserialize;
		static Func<T, byte[]> _Serialize;

		static BitSerializer()
		{
			Type type = typeof(T);
			#region PrimitiveTypes
			if (type == typeof(bool))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToBoolean(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((bool)Convert.ChangeType(entity, typeof(bool)));
				};
			}
			else if (type == typeof(char))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToChar(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((char)Convert.ChangeType(entity, typeof(char)));
				};
			}
			else if (type == typeof(sbyte))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType((sbyte)buffer.First(), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((sbyte)Convert.ChangeType(entity, typeof(sbyte)));
				};
			}
			else if (type == typeof(short))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToInt16(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((short)Convert.ChangeType(entity, typeof(short)));
				};
			}
			else if (type == typeof(int))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToInt32(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((int)Convert.ChangeType(entity, typeof(int)));
				};
			}
			else if (type == typeof(long))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToInt64(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((long)Convert.ChangeType(entity, typeof(long)));
				};
			}
			else if (type == typeof(byte))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(buffer.First(), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((byte)Convert.ChangeType(entity, typeof(byte)));
				};
			}
			else if (type == typeof(ushort))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToUInt16(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((ushort)Convert.ChangeType(entity, typeof(ushort)));
				};

			}
			else if (type == typeof(uint))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToUInt32(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((uint)Convert.ChangeType(entity, typeof(uint)));
				};
			}
			else if (type == typeof(ulong))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToUInt64(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((ulong)Convert.ChangeType(entity, typeof(ulong)));
				};
			}
			else if (type == typeof(float))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToSingle(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((float)Convert.ChangeType(entity, typeof(float)));
				};
			}
			else if (type == typeof(double))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToDouble(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((double)Convert.ChangeType(entity, typeof(double)));
				};
			}
			else if (type == typeof(decimal))
			{
				_Deserialize = (buffer) =>
				{
					return (T)Convert.ChangeType(BitConverter.ToInt32(buffer, 0), typeof(T));
				};
				_Serialize = (entity) => {
					return BitConverter.GetBytes((int)Convert.ChangeType(entity, typeof(decimal)));
				};
			}
			#endregion
			else
			{
				_Deserialize = (buffer) => {
					T str = Activator.CreateInstance<T>();

					int size = Marshal.SizeOf(str);
					IntPtr ptr = Marshal.AllocHGlobal(size);

					Marshal.Copy(buffer, 0, ptr, size);

					str = (T)Marshal.PtrToStructure(ptr, str.GetType());
					Marshal.FreeHGlobal(ptr);

					return str;
				};
				_Serialize = (entity) =>
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

		public T Deserialize(byte[] buffer)
		{
			return _Deserialize(buffer);
		}

		public byte[] Serialize(T entity)
		{
			return _Serialize(entity);
		}
	}
}