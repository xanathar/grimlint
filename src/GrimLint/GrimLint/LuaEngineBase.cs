using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpLua;

namespace GrimLint
{
	public class LuaEngineBase
	{
		protected LuaInterface m_Lua;

		public LuaEngineBase()
		{
			m_Lua = LuaRuntime.GetLua();
		}

		protected void RegisterFn(string functionName, Delegate d)
		{
			m_Lua.RegisterFunction(functionName, d.Target, d.Method);
		}

		protected Dictionary<string, object> LuaTableToDictionary(LuaTable table)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();

			var keys = table.Keys.Cast<object>().ToList();
			var vals = table.Values.Cast<object>().ToList();

			for (int i = 0; i < keys.Count; i++)
			{
				object val = vals[i];
				LuaTable vt = val as LuaTable;
				if (vt != null)
				{
					val = LuaTableToDictionary(vt);
				}

				dic.Add(keys[i].ToString(), val);
			}

			return dic;
		}


		public string Lua_VecDummy(double x = double.NaN, double y = double.NaN, double z = double.NaN)
		{
			string str = string.Format("[{0},{1},{2}]", x, y, z);
			return str;
		}


		public void Lua_DummyTable(LuaTable table)
		{

		}

		public void Lua_DummyString(string mapName)
		{
		}

	}
}
