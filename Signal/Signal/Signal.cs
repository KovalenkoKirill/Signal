using System.Collections.Generic;
using System.Threading;

namespace Signal
{
	internal class Signal<T> : ISignal<T>
	{
		private T buffer;

		Dictionary<int,AutoResetEvent> events = new Dictionary<int, AutoResetEvent>();

		private volatile object sync = new object();

		private bool isDisposabled = false;

		~Signal()
		{
			if (!isDisposabled)
			{
				Dispose();
			}
		}

		public T Receive()
		{
			var waiter = GetEvents();
			waiter.WaitOne();
			waiter.Reset();
			return buffer;
		}

		public T Receive(int timeOut)
		{
			var waiter = GetEvents();
			waiter.WaitOne(timeOut);
			waiter.Reset();
			return buffer;
		}

		public void Send(T signal)
		{
			lock (sync)
			{
				buffer = signal;
				foreach(var autoResetEvent in events.Values)
				{
					autoResetEvent.Set();
				}
			}
		}

		private AutoResetEvent GetEvents()
		{
			var threadId = Thread.CurrentThread.ManagedThreadId;
			AutoResetEvent autoResetEvent;
			if (!events.ContainsKey(threadId))
			{
				autoResetEvent = new AutoResetEvent(false);
				events.Add(threadId, autoResetEvent);
			}
			else
			{
				autoResetEvent = events[threadId];
			}
			return autoResetEvent;
		}

		public void Dispose()
		{
			foreach(var resetEvent in events.Values)
			{
				resetEvent.Dispose();
			}
			isDisposabled = true;
		}
	}
}