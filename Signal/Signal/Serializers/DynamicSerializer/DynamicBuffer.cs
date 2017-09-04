using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Serializers.DynamicSerializer
{
	public class DynamicBuffer
	{
		byte[] bytes;

		int size = 0;

		public int CurrentPoint
		{
			get
			{
				return size;
			}
		}


		public BufferPtr Alloc(int Count)
		{
			int currentPtr = size;
			size += Count;
			TryResize(Count);
			return new BufferPtr(this, currentPtr, Count);
		}

		public int Copy(byte[] buffer)
		{
			return Copy(buffer, size);
		}

		private void TryResize(int lenghtNeeded)
		{
			if (bytes.Length < lenghtNeeded)
			{
				byte[] nextBuffer = new byte[lenghtNeeded * 2];
				Array.Copy(bytes, nextBuffer, bytes.Length);
				bytes = nextBuffer;
				size = lenghtNeeded;
			}
		}

		public int Copy(byte[] buffer,int point)
		{
			int lenghtNeeded = point + buffer.Length;
			TryResize(lenghtNeeded);
			Array.Copy(buffer, 0, bytes, point, buffer.Length);
			return point;
		}

		public byte[] Buffer
		{
			get
			{
				byte[] result = new byte[size];
				Array.Copy(bytes, result, size);
				return result;
			}
		}

		public DynamicBuffer(int startSize)
		{
			bytes = new byte[startSize];
		}

		public class BufferPtr
		{
			public short position { get;  }

			public int lenght { get;  }

			private readonly DynamicBuffer buffer;

			internal BufferPtr(DynamicBuffer buffer,int position,int lenght)
			{
				this.buffer = buffer;
				this.position = (short)position;
				this.lenght = lenght;
			}

			public void Set(byte[] buffer)
			{
				Array.Copy(buffer, 0, this.buffer.bytes, position, lenght);
			}
		}
	}
}
