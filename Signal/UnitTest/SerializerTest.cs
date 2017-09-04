using Microsoft.VisualStudio.TestTools.UnitTesting;
using Signal;
using Signal.Serializers;
using Signal.Serializers.DynamicSerializer;
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
		const int iterationCount = 1000000;
		
		[TestMethod]
		public void PerformanceTest()
		{
			ISerializer<int> binarySerializer = new BinarySerializer<int>();
			ISerializer<int> bitSerializer = new BitSerializer<int>();

			Stopwatch watch = new Stopwatch();

			watch.Start();

			for(int i = 0;i< iterationCount; i++)
			{
				byte[] resultBytes = binarySerializer.Serialize(i);
				int result = binarySerializer.Deserialize(resultBytes);
			}

			watch.Stop();
			Debug.WriteLine($"binarySerializer result {watch.ElapsedMilliseconds} ms.");
			watch.Reset();
			watch.Start();
			for (int i = 0; i < iterationCount; i++)
			{
				byte[] resultBytes = bitSerializer.Serialize(i);
				int result = bitSerializer.Deserialize(resultBytes);
			}
			watch.Stop();
			Debug.WriteLine($"bitSerializer result {watch.ElapsedMilliseconds} ms.");
		}

		[TestMethod]
		public void DynamicSerializerTest()
		{
			var dynamicSerializer = new DynamicSerializer<ClassForTest>();
			var testEntity = new ClassForTest();
			var watch = Stopwatch.StartNew();
			for (int i = 0; i < iterationCount; i++)
			{
				var buffer = dynamicSerializer.Serialize(testEntity);
			}
			watch.Stop();
			Debug.WriteLine($"DynamicSerialize result {watch.ElapsedMilliseconds} ms.");

			var binary = new BinarySerializer<ClassForTest>();
			watch = Stopwatch.StartNew();
			for (int i = 0; i < iterationCount; i++)
			{
				var buffer = binary.Serialize(testEntity);
			}
			watch.Stop();
			Debug.WriteLine($"BinarySerializer result {watch.ElapsedMilliseconds} ms.");
		}

		[Serializable]
		public class ClassForTest
		{
			int i = 0;
			public int z = 50;
			public int azaza { get; set; }

			public int[] _Array;

			private TestStruct str;

			private ArrayItem[] arrayClass;

			public ClassForTest()
			{
				_Array = new int[]
				{
					10,0,9,1
				};

				str = new TestStruct()
				{
					B = 10,
					G = 10,
					R = 10
				};
				arrayClass = new[]
				{
					new ArrayItem(),
					new ArrayItem(),
					new ArrayItem()
				};
			}
		}

		[Serializable]
		public class ArrayItem
		{
			public int abc = 99;

			public int dfg = 50;
		}

		[TestMethod]
		public void ComplexSerializerTest()
		{
			var stringSerializer = new ComplexSerializer<string>();
			var testSting = "TestSting";
			var serializeResult = stringSerializer.Serialize(testSting);
			var resultString = stringSerializer.Deserialize(serializeResult);
			Assert.AreEqual(testSting, resultString);

			var valueTypeSerializer = new ComplexSerializer<int>();
			int valueType = 10;
			serializeResult = valueTypeSerializer.Serialize(valueType);
			var valueTypeResult = valueTypeSerializer.Deserialize(serializeResult);
			Assert.AreEqual(valueTypeResult, valueType);

			var stuctSerializer = new ComplexSerializer<TestStruct>();
			var testStruct = new TestStruct()
			{
				R = 10,
				G = 50,
				B = 40
			};
			serializeResult = stuctSerializer.Serialize(testStruct);
			var stuctResult = stuctSerializer.Deserialize(serializeResult);
			Assert.AreEqual(stuctResult, testStruct);

			var stuctWithReferenceSerializer = new ComplexSerializer<TestStructWithReference>();
			var testStructWithReference = new TestStructWithReference()
			{
				R = 10,
				G = 50,
				B = 40,
				List = new List<int>()
			};
			serializeResult = stuctWithReferenceSerializer.Serialize(testStructWithReference);
			var stuctWithResult = stuctWithReferenceSerializer.Deserialize(serializeResult);
			Assert.AreNotEqual(testStructWithReference, stuctWithResult);
		}

		private void TestPerformanceIteration<T>(ISerializer<T> serializer,T testValue)
		{
			for(int i = 0;i< iterationCount;i++)
			{
				var serializeResult = serializer.Serialize(testValue);
				var result = serializer.Deserialize(serializeResult);
			}
		}

		[TestMethod]
		public void ComplexSerialzerPerfomenseTest()
		{
			Stopwatch watch = new Stopwatch();

			watch.Start();
			var stringSerializer = new ComplexSerializer<string>();
			var testSting = "TestSting";
			TestPerformanceIteration(stringSerializer, testSting);
			watch.Stop();
			Debug.WriteLine($"StringSerializer result = {watch.ElapsedMilliseconds}");
			watch.Reset();

			watch.Start();
			var valueTypeSerializer = new ComplexSerializer<int>();
			int valueType = 10;
			TestPerformanceIteration(valueTypeSerializer, valueType);
			watch.Stop();
			Debug.WriteLine($"Primitive ValueType result = {watch.ElapsedMilliseconds}");
			watch.Reset();

			watch.Start();
			var stuctSerializer = new ComplexSerializer<TestStruct>();
			var testStruct = new TestStruct()
			{
				R = 10,
				G = 50,
				B = 40
			};
			TestPerformanceIteration(stuctSerializer, testStruct);
			watch.Stop();
			Debug.WriteLine($"StuctSerializerResult = {watch.ElapsedMilliseconds}");
			watch.Reset();

			watch.Start();
			var stuctWithReferenceSerializer = new ComplexSerializer<TestStructWithReference>();
			var testStructWithReference = new TestStructWithReference()
			{
				R = 10,
				G = 50,
				B = 40,
				List = new List<int>()
			};
			TestPerformanceIteration(stuctWithReferenceSerializer, testStructWithReference);
			watch.Stop();
			Debug.WriteLine($"StructWithReference (BinarySerializer) = {watch.ElapsedMilliseconds}");
			watch.Reset();
		}

		[Serializable]
		struct TestStructWithReference
		{
			public int R { get; set; }

			public int G { get; set; }

			public int B { get; set; }

			public List<int> List { get; set; }
		}

		[Serializable]
		struct TestStruct
		{
			public int R { get; set; }

			public int G { get; set; }

			public int B { get; set; }
		}
	}
}
