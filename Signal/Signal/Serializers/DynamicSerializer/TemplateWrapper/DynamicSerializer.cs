namespace Signal.Serializers.DynamicSerializer
{
	public class DynamicSerializer<T> : ISerializer<T>
	{
		private static DynamicSerializer _serializer;

		static DynamicSerializer()
		{
			_serializer = DynamicSerializer.Instance(typeof(T));
		}

		public T Deserialize(byte[] buffer)
		{
			return (T)_serializer.Deserialize(buffer);
		}

		public byte[] Serialize(T entity)
		{
			return _serializer.Serialize(entity);
		}
	}
}