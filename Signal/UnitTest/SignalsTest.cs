using Microsoft.VisualStudio.TestTools.UnitTesting;
using Signal;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest
{
	[TestClass]
	public class SignalsTest
	{
		[TestMethod]
		public void PingPongTest()
		{
			SendTest();
		}

		[TestMethod]
		public void PingPongCrossProcessMutexTest()
		{
			SendTest("Test");
		}

		private void SendTest(string name = "")
		{
			ISignal<string> signal;
			if (string.IsNullOrEmpty(name))
			{
				 signal = SignalFactory.GetInstanse<string>(); // создаем локальный сигнал
			}
			else
			{
				signal = SignalFactory.GetInstanse<string>(name);
			}

			var task1 = Task.Factory.StartNew(() => // старт потока
			{
				for (int i = 0; i < 10; i++)
				{
					// блокировка потока, ожидание сигнала
					var message = signal.Receive();
					Debug.WriteLine($"Thread 1 {message}");
				}
			});
			var task2 = Task.Factory.StartNew(() => // старт потока
			{
				for (int i = 0; i < 10; i++)
				{
					// блокировка потока, ожидание сигнала
					var message = signal.Receive();
					Debug.WriteLine($"Thread 2 {message}");
				}
			});

			for (int i = 0; i < 10; i++)
			{
				// отправка сигнала ожидающим потокам.
				signal.Send($"Ping {i}");
				Thread.Sleep(50);
			}

		}

	}
}