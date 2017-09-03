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
		public void PingPong()
		{
			var signal = SignalFactory.GetInstanse<string>();
			var task = Task.Factory.StartNew(() => { 
				for(int i = 0;i<10;i++)
				{
					Debug.WriteLine(signal.Receive());
					Thread.Sleep(30);
					signal.Send("pong");
				}
			});
			Task.Factory.StartNew(() => {
				Thread.Sleep(10);
				for (int i = 0; i < 10; i++)
				{
					signal.Send("ping");
					Debug.WriteLine(signal.Receive());
					Thread.Sleep(30);
				}
			});

			task.Wait();
		}

		[TestMethod]
		public void ExampleTest()
		{
			var signal = SignalFactory.GetInstanse<string>();
			var task1 = Task.Factory.StartNew(() => // старт потока
			{
				Thread.Sleep(1000);
				signal.Send("Some message");
			});
			// блокировка текущего потока
			string message = signal.Receive();
			Debug.WriteLine(message);
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