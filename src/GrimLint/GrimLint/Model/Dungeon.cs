using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GrimLint.Model
{
	public class Dungeon
	{
		public List<Entity> AllEntities = new List<Entity>();
		public Dictionary<string, Entity> EntitiesById = new Dictionary<string,Entity>();
		public MultiDictionary<int, Entity> EntitiesByLevel = new MultiDictionary<int, Entity>();
		public MultiDictionary<EntityClass, Entity> EntitiesByClass = new MultiDictionary<EntityClass, Entity>();
		public MultiDictionary<string, Entity> EntitiesByName = new MultiDictionary<string, Entity>();
		public Assets Assets { get; set; }
		public string BaseDirectory { get; private set; }
		public string ModAssetsDirectory { get; private set; }
		public string DungeonLuaFile { get; private set; }

		public Dungeon(string directory)
		{
			BaseDirectory = directory;
			ModAssetsDirectory = Path.Combine(BaseDirectory, "mod_assets");
			DungeonLuaFile = Path.Combine(BaseDirectory, "mod_assets", "scripts", "dungeon.lua");
		}

		public void CreateReverseConnectors()
		{
			foreach (Entity E in EntitiesById.Values)
			{
				foreach (Connector C in E.Connectors)
				{
					if (EntitiesById.ContainsKey(C.Target))
					{
						Entity E1 = EntitiesById[C.Target];
						E1.ReverseConnectors.Add(C);
					}
					else
					{
						Lint.MsgWarn("Can't find entity {0} referenced by connector of {1}", C.Target, E);
					}
				}
			}
		}

		public void AddEntity(Entity E, int level)
		{
			if (EntitiesById.ContainsKey(E.Id))
			{
				Lint.MsgErr("Duplicate id: {0}", E.Id);
				return;
			}

			EntitiesById.Add(E.Id, E);

			AllEntities.Add(E);
			EntitiesByLevel.AddMulti(level, E);
			EntitiesByName.AddMulti(E.Name, E);
			EntitiesByClass.AddMulti(E.Class, E);

			foreach (Entity e in E.Items)
			{
				AllEntities.Add(e);
				EntitiesByLevel.AddMulti(level, e);
				EntitiesByName.AddMulti(e.Name, e);
				EntitiesByClass.AddMulti(e.Class, e);
			}
		}


	}
}
