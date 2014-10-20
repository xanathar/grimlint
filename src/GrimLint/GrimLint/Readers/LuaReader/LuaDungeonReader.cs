using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using System.IO;

namespace GrimLint.Readers.LuaReader
{
	public class LuaDungeonReader : LuaEngineBase, IDungeonReader
	{
		Dungeon D;
		int m_Level = 0;
		List<LuaEntity> allEntities = new List<LuaEntity>();

		public void Load(Dungeon D)
		{
			this.D = D;

			RegisterFn("mapName", new Action<string>(Lua_MapName));
			RegisterFn("setWallSet", new Action<string>(Lua_DummyString));
			RegisterFn("playStream", new Action<string>(Lua_DummyString));
			RegisterFn("mapDesc", new Action<string>(Lua_DummyString));
			RegisterFn("spawn", new Func<string, object, object, object, object, object>(Lua_Spawn));

			m_Lua.DoFile(D.DungeonLuaFile);

			foreach (LuaEntity E in allEntities)
				D.AddEntity(E, E.Level);

			D.CreateReverseConnectors();			
		}


		public object Lua_Spawn(string name, object x = null, object y = null, object f = null, object id = null)
		{
			LuaEntity E = new LuaEntity(m_Level, name, x, y, f, id, D.Assets);
			if (id != null)
			{
				allEntities.Add(E);
			}
			return E;
		}

		public void Lua_MapName(string mapName)
		{
			++m_Level;
			Lint.MsgVerbose("Reading level {0}", m_Level);
		}



	}
}
