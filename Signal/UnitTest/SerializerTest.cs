using Microsoft.VisualStudio.TestTools.UnitTesting;
using Signal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
	[TestClass]
	public class SerializerTest
	{
		[TestMethod]
		public void PerformanceTest()
		{
			ISerializer<int> binarySerializer = new BinarySerializer<int>();
			ISerializer<int> bitSerializer = new BitSerializer<int>();

			Stopwatch watch = new Stopwatch();

			watch.Start();

			for(int i = 0;i<1000;i++)
			{
				byte[] resultBytes = binarySerializer.Serialize(i);
				int result = binarySerializer.Deserialize(resultBytes);
			}

			watch.Stop();
			Debug.WriteLine($"binarySerializer result {watch.ElapsedMilliseconds} ms.");
			watch.Reset();
			watch.Start();
			for (int i = 0; i < 1000; i++)
			{
				byte[] resultBytes = bitSerializer.Serialize(i);
				int result = bitSerializer.Deserialize(resultBytes);
			}
			watch.Stop();
			Debug.WriteLine($"bitSerializer result {watch.ElapsedMilliseconds} ms.");
		}
	}
}
