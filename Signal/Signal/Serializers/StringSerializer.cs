using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Serializers
{
	public class StringSerializer<T> : ISerializer<T> where T: class
	{
		public T Deserialize(byte[] buffer)
		{
			return Encoding.Unicode.GetString(buffer) as T;
		}

		public byte[] Serialize(T entity)
		{
			return Encoding.Unicode.GetBytes(entity as string);
		}
	}
}
