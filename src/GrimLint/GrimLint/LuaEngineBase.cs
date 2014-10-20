using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoonSharp.Interpreter;

namespace GrimLint
{
	public class LuaEngineBase
	{
		protected Script m_Lua;

		public LuaEngineBase()
		{
			m_Lua = new Script();
		}

		protected void RegisterFn(string functionName, Delegate d)
		{
			m_Lua.Globals[functionName] = d;
		}

		protected Dictionary<string, object> TableToDictionary(Table table)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();

			return table.Pairs.ToDictionary(
				kvp => kvp.Key.ToObject<string>(),
				kvp => kvp.Value.ToObject());
		}


		public string Lua_VecDummy(double x = double.NaN, double y = double.NaN, double z = double.NaN)
		{
			string str = string.Format("[{0},{1},{2}]", x, y, z);
			return str;
		}


		public void Lua_DummyTable(Table table)
		{

		}

		public void Lua_DummyString(string mapName)
		{
		}

	}
}
