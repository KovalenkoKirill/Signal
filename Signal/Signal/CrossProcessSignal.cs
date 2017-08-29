using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Signal
{
	internal class CrossProcessSignal<T> : ISignal<T>
	{
		private volatile object sync = new object();

		private bool isDisposabled = false;

		private volatile bool isLocalSignalFlag = false;

		private Signal<T> localSignal = new Signal<T>();

		private EventWaitHandle handle;

		private IBuffer<T> buffer;

		private string mutexName;

		public CrossProcessSignal(string signalName)
		{
			handle = CreateSignal(signalName);

			buffer = new SharedBuffer<T>(mutexName);

			this.mutexName = signalName;

			Task.Factory.StartNew(() =>
			{
				CrossProcessWaiter();
			});
		}

		~CrossProcessSignal()
		{
			if (!isDisposabled)
			{
				Dispose();
			}
		}

		public T Receive()
		{
			return localSignal.Receive();
		}

		public T Receive(int timeOut)
		{
			return localSignal.Receive(timeOut);
		}

		public void Send(T signal)
		{
			buffer.SetBuffer(signal);
			isLocalSignalFlag = true;
			handle.Set();
		}

		public void Dispose()
		{
			localSignal.Dispose();
			handle.Dispose();
			isDisposabled = true;
		}

		private EventWaitHandle CreateSignal(string name)
		{
			EventWaitHandle result = null;

			if (EventWaitHandle.TryOpenExisting(name, out result))
			{
				return result;
			}
			else
			{
				// code from https://stackoverflow.com/questions/2590334/creating-a-cross-process-eventwaithandle
				// user https://stackoverflow.com/users/241462/dean-harding
				var users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
				var rule = new EventWaitHandleAccessRule(users, EventWaitHandleRights.Synchronize | EventWaitHandleRights.Modify,
										  AccessControlType.Allow);
				var security = new EventWaitHandleSecurity();
				security.AddAccessRule(rule);

				bool created;
				var wh = new EventWaitHandle(false, EventResetMode.ManualReset, name, out created, security);
				return wh;
			}
		}

		private void CrossProcessWaiter()
		{
			while (!isDisposabled)
			{
				if (handle.WaitOne())
				{
					T entity = buffer.GetBuffer();
					localSignal.Send(entity);
				}
				if (isLocalSignalFlag)
				{
					Thread.Sleep(0);
					handle.Reset();
					isLocalSignalFlag = false;
				}
			}
		}
	}
}