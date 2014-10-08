using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace GrimLint
{
	public static class Lint
	{
		private static Dictionary<string, Stopwatch> m_Stopwatches = new Dictionary<string, Stopwatch>();

		public static void MsgProfileStart(string profilerName, string format, params object[] args)
		{
			m_Stopwatches[profilerName] = Stopwatch.StartNew();
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(format, args);
		}

		public static void MsgProfileEnd(string profilerName, string opname)
		{
			if (m_Stopwatches.ContainsKey(profilerName))
			{
				Stopwatch sw = m_Stopwatches[profilerName];
				sw.Stop();
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine("{0} {1}\"", opname, sw.Elapsed.TotalSeconds);
			}
		}


		public static void MsgBanner(string format, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(format, args);
		}


		public static void MsgVerbose(string format, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(format, args);
		}

		public static void MsgInfo(string format, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine(format, args);
		}

		public static void MsgErr(string format, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(format, args);
		}

		public static void MsgWarn(string format, params object[] args)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(format, args);
		}


	}
}
