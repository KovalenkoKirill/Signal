using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Signal
{
	public class BinarySerializer<T> : ISerializer<T>
	{
		public T Deserialize(byte[] buffer)
		{
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				BinaryFormatter formatter = new BinaryFormatter();

				return (T)formatter.Deserialize(stream);
			}
		}

		public byte[] Serialize(T entity)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();

				formatter.Serialize(stream, entity);

				return stream.ToArray();
			}
		}
	}
}