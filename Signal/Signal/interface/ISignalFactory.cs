using System.Collections;

namespace Signal
{
	public interface ISignalFactory<T>
	{
		ISignal<T> GetInstanse();
	}
}