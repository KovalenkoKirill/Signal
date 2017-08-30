using Signal.Serializers;
using System.IO.MemoryMappedFiles;

namespace Signal
{
	internal class SharedBuffer<T> : IBuffer<T>
	{
		private string bufferName;

		private ISerializer<T> serializer;

		public SharedBuffer(string bufferName)
		{
			this.bufferName = $"Signal/{bufferName}";
			serializer = new ComplexSerializer<T>();
		}

		public T GetBuffer()
		{
			try
			{
				MemoryMappedFile file = MemoryMappedFile.OpenExisting(bufferName);
				using (var stream = file.CreateViewStream())
				{
					byte[] buffer = new byte[stream.Length];
					stream.Read(buffer, 0, (int)stream.Length);
					return serializer.Deserialize(buffer);
				}
			}
			catch { }
			return default(T);
		}

		public void SetBuffer(T entity)
		{
			byte[] entityBytes = serializer.Serialize(entity);
			MemoryMappedFile file = MemoryMappedFile.CreateOrOpen(bufferName, entityBytes.Length, MemoryMappedFileAccess.ReadWrite);
			using (var stream = file.CreateViewStream())
			{
				stream.Write(entityBytes, 0, entityBytes.Length);
			}
		}

	}
}