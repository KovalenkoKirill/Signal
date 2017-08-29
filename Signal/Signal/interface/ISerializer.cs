namespace Signal
{
	public interface ISerializer<T>
	{
		byte[] Serialize(T entity);

		T Deserialize(byte[] buffer);
	}
}