using System;
using System.Runtime.InteropServices;

namespace Signal
{
	public class BitSerializer<T> : ISerializer<T>
	{
		public T Deserialize(byte[] buffer)
		{
			T str = Activator.CreateInstance<T>();

			int size = Marshal.SizeOf(str);
			IntPtr ptr = Marshal.AllocHGlobal(size);

			Marshal.Copy(buffer, 0, ptr, size);

			str = (T)Marshal.PtrToStructure(ptr, str.GetType());
			Marshal.FreeHGlobal(ptr);

			return str;
		}

		public byte[] Serialize(T entity)
		{
			int size = Marshal.SizeOf(entity);
			byte[] arr = new byte[size];

			IntPtr ptr = Marshal.AllocHGlobal(size);
			Marshal.StructureToPtr(entity, ptr, true);
			Marshal.Copy(ptr, arr, 0, size);
			Marshal.FreeHGlobal(ptr);
			return arr;
		}
	}
}