using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signal;
using System.Diagnostics;
using System.Threading;

namespace ProcessForTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var signal = SignalFactory.GetInstanse<string>("TestSignal");

			signal.Send($"TestSignal from {Process.GetCurrentProcess().Id}");

			Console.WriteLine("Waiting message...");

			Thread.Sleep(100);

			string message = signal.Receive();

			Console.WriteLine($"Current Process {Process.GetCurrentProcess().Id}, Message = {message}");

			Console.ReadKey();

		}
	}
}
