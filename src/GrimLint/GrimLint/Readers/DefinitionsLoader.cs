using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;
using MoonSharp.Interpreter;

namespace GrimLint.Readers
{
	public class DefinitionsLoader : LuaEngineBase
	{
		Stack<string> m_LuaFiles = new Stack<string>();
		Assets m_Assets;
		string m_DungeonDirectory;

		public DefinitionsLoader(Assets assets, string dungeonDirectory)
		{
			m_Assets = assets;
			m_DungeonDirectory = dungeonDirectory;
		}

		public void LoadLuaAssets()
		{
			RegisterFn("import", new Action<string>(LoadLuaAssetsFromFile));
			RegisterFn("vec", new Func<double, double, double, string>(Lua_VecDummy));
			RegisterFn("cloneObject", new Action<Table>(Lua_cloneObject));
			RegisterFn("defineObject", new Action<Table>(Lua_defineObject));
			RegisterFn("defineMaterial", new Action<Table>(Lua_defineMaterial));
			RegisterFn("defineParticleSystem", new Action<Table>(Lua_defineParticleSystem));
			RegisterFn("defineAnimationEvent", new Action<Table>(Lua_DummyTable));
			RegisterFn("defineSound", new Action<Table>(Lua_DummyTable));
			RegisterFn("defineSpell", new Action<Table>(Lua_DummyTable));
			RegisterFn("defineRecipe", new Action<Table>(Lua_DummyTable));
			RegisterFn("defineWallSet", new Action<Table>(Lua_defineWallSet));

			LoadLuaAssetsFromFile(Path.Combine(m_DungeonDirectory, "mod_assets\\scripts\\init.lua"));
		}

		void LoadLuaAssetsFromFile(string file)
		{
			Lint.MsgVerbose("Loading {0}...", file);
			m_LuaFiles.Push(file);

			if (file.StartsWith("assets/"))
				return;

			if (file.StartsWith("mod_assets/"))
				file = Path.Combine(m_DungeonDirectory, file.Replace('/', '\\'));

			m_Lua.DoFile(file);
			m_LuaFiles.Pop();
		}


		private void AddTable(DefinitionType defType, Table obj, bool isClone = false)
		{
			var def = TableToDictionary(obj);

			if (isClone)
				m_Assets.CloneDef(defType, new Definition(m_LuaFiles.Peek(), def));
			else
				m_Assets.AddDef(defType, new Definition(m_LuaFiles.Peek(), def));
		}

		private void Lua_defineObject(Table obj)
		{
			AddTable(DefinitionType.Object, obj, false);
		}

		private void Lua_defineWallSet(Table obj)
		{
			AddTable(DefinitionType.Wallset, obj);
		}

		private void Lua_defineParticleSystem(Table obj)
		{
			AddTable(DefinitionType.ParticleSystem, obj);
		}

		private void Lua_defineMaterial(Table obj)
		{
			AddTable(DefinitionType.Material, obj);
		}

		private void Lua_cloneObject(Table obj)
		{
			AddTable(DefinitionType.Object, obj, true);
		}

	}
}
