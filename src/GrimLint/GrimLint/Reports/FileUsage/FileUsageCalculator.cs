using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Reports.FileUsage
{
	class FileUsageCalculator
	{
		Dictionary<string, Model> m_Models = new Dictionary<string, Model>();
		Dictionary<string, Texture> m_Textures = new Dictionary<string, Texture>();
		Dictionary<string, Material> m_Materials = new Dictionary<string, Material>();
		MultiDictionary<string, Model> m_Objects = new MultiDictionary<string, Model>();
		
		Dungeon D;

		public void Process(Dungeon D)
		{
			this.D = D;
			AddAllFiles(D.ModAssetsDirectory);
			AddAllMaterialsFromDeclarations();
			AddAllObjectsFromDeclarations();
			MarkModelsFromWallsets();
			MarkModelsFromObjects();
			PropagateCalculations();
		}

		public IEnumerable<Model> Models { get { return m_Models.Values; } }
		public IEnumerable<Texture> Textures { get { return m_Textures.Values; } }
		public IEnumerable<Material> Materials { get { return m_Materials.Values; } }


		private void PropagateCalculations()
		{
			foreach (Model m in m_Models.Values)
			{
				foreach (Material mat in m.Materials.Select(mm => m_Materials.Find(mm)).Where(mm => mm != null))
				{
					mat.MergeStatsOfParent(m);
				}
			}

			foreach (Material mat in m_Materials.Values)
			{
				foreach (Texture tex in mat.Textures.Select(tt => m_Textures.Find(tt)).Where(tt => tt != null))
				{
					tex.MergeStatsOfParent(mat);
				}
			}
		}

		private void MarkModelsFromObjects()
		{
			foreach(Entity E in D.AllEntities)
			{
				if (E.Class == EntityClass.Spawner)
				{
					string name = E.GetProperty("SpawnedEntity");
					if (name != null)
						MarkEntity(E.Id, name);
				}

				MarkEntity(E.Id, E.Name);
			}
		}

		private void MarkEntity(string id, string name)
		{
			if (m_Objects.ContainsKey(name))
			{
				foreach (Model m in m_Objects[name])
					m.MarkUsedByObject(id, name);
			}
		}

		private void MarkModelsFromWallsets()
		{
			foreach (Definition def in D.Assets.GetAllDefs(DefinitionType.Wallset))
			{
				var models = def.FlattenedValues.OfType<string>()
					.Where(f => !string.IsNullOrWhiteSpace(f))
					.Where(f => f.EndsWith(".fbx", StringComparison.InvariantCultureIgnoreCase) && (!f.StartsWith("assets/", StringComparison.InvariantCultureIgnoreCase)))
					.Select(f => f.Replace(".fbx", ".model"));

				foreach (string fbxc in models)
				{
					string fbx = fbxc.ToLower();
					if (m_Models.ContainsKey(fbx))
					{
						m_Models[fbx].MarkUsedByWallset(def.Name);

						if (fbxc != m_Models[fbx].NameRealCase)
						{
							Lint.MsgWarn("[{0}]: Case-mismatch of filenames in wallset {1} for model {2}.", def.DeclaringFile, def.Name, m_Models[fbx].NameRealCase);
						}
					}
					else
					{
						Lint.MsgWarn("[{0}]: Wallset {1} refers to model {2} which was not found on disk", def.DeclaringFile, def.Name, fbx);
					}
				}
			}
		}

		private void AddAllObjectsFromDeclarations()
		{
			foreach (Definition def in D.Assets.GetAllDefs(DefinitionType.Object))
			{
				var models = GetModelsForDef(def)
					.Where(f => !string.IsNullOrWhiteSpace(f))
					.Where(f => f.EndsWith(".fbx", StringComparison.InvariantCultureIgnoreCase) && (!f.StartsWith("assets/", StringComparison.InvariantCultureIgnoreCase)))
					.Select(f => f.Replace(".fbx", ".model"));

				foreach (string fbxc in models)
				{
					string fbx = fbxc.ToLower();
					if (m_Models.ContainsKey(fbx))
					{
						m_Objects.AddMulti(def.Name, m_Models[fbx]);
						m_Models[fbx].ObjectNamesUsingThisModel.Add(def.Name);

						if (fbxc != m_Models[fbx].NameRealCase)
						{
							Lint.MsgWarn("[{0}]: Case-mismatch of filenames in object {1} for model {2}.", def.DeclaringFile, def.Name, m_Models[fbx].NameRealCase);
						}
					}
					else
					{
						Lint.MsgWarn("[{0}]: Object {1} refers to model {2} which was not found on disk", def.DeclaringFile, def.Name, fbx);
					}
				}
			}
		}

		private void AddAllMaterialsFromDeclarations()
		{
			foreach (Definition def in D.Assets.GetAllDefs(DefinitionType.Material))
			{
				Material m = new Material(def);
				m_Materials.Add(m.Name, m);
			}
		}

		private void AddAllFiles(string directory)
		{
			foreach (string file in Directory.EnumerateFiles(directory))
			{
				if (file.EndsWith(".model", StringComparison.InvariantCultureIgnoreCase))
				{
					Model o = new Model(file);
					m_Models.Add(o.Name, o);
				}
				else if (file.EndsWith(".dds", StringComparison.InvariantCultureIgnoreCase))
				{
					Texture o = new Texture(file);
					m_Textures.Add(o.Name, o);
				}
			}

			foreach (string dir in Directory.EnumerateDirectories(directory))
				AddAllFiles(dir);
		}

		private IEnumerable<string> GetModelsForDef(Definition def)
		{
			string[] keys = { "model", "doorFrameModel", "pullChainModel", "trapDoorModel", "brokenModel" };

			foreach(string key in keys)
			{
				if (def.Properties.ContainsKey(key))
				{
					yield return def.Properties[key] as string;
				}
			}
		}


	}
}
