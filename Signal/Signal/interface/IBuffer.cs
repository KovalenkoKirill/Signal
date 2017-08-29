namespace Signal
{
	internal interface IBuffer<T>
	{
		void SetBuffer(T entity);

		T GetBuffer();
	}
}