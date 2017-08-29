using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal
{
	public static class SignalFactory 
	{
		public static ISignal<T> GetInstanse<T>()
		{
			return new Signal<T>();
		}

		public static ISignal<T> GetInstanse<T>(string name)
		{
			return new CrossProcessSignal<T>(name);
		}
	}
}
