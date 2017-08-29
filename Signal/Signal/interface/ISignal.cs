using System;

namespace Signal
{
	public interface ISignal<T> : IDisposable
	{
		void Send(T signal);

		T Receive();

		T Receive(int timeOut);
	}
}