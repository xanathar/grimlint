using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Readers.LineReader
{
	internal static class LineEntityReader
	{
		public static Tuple<Entity, string> CreateEntity(int level, string line, LuaLineReader reader, Assets assets)
		{
			Entity E = new Entity();

			string sline = line.Substring("spawn(".Length, line.Length - ("spawn(".Length + 1));
			string[] parts = sline.Split(',');
			string lastLine = null;

			if (parts.Length != 5)
				Lint.MsgErr("Error parsing line:{0}", line);

			E.Id = parts[4].Replace("\"", "").Trim();

			E.Level = level;
			E.X = int.Parse(parts[1]);
			E.Y = int.Parse(parts[2]);
			E.Facing = int.Parse(parts[3]);
			E.Name = parts[0].Replace("\"", "").Trim();
			Asset A = assets.Get(E.Name);

			if (A != null)
			{
				E.Class = A.Class;
				E.ItemClass = A.ItemClass;
			}
			else
			{
				E.Class = EntityClass.Unknown;
				E.ItemClass = 0;
				Lint.MsgWarn("Can't find asset {0} for entity {1}", E.Name, E);
			}

			bool dontRollback = false;

			try
			{
				while (true)
				{
					string mcall = reader.Get();

					if (mcall == null)
					{
						dontRollback = true;
						break;
					}

					if (mcall.StartsWith(":"))
					{
						lastLine = ParseMethod(E, mcall, reader, assets);
					}
					else
						break;
				}
			}
			finally
			{
				if (!dontRollback)
					reader.RollBack();
			}

			return new Tuple<Entity, string>(E.PostCreate(assets), lastLine);
		}

		private static string ParseMethod(Entity E, string mcall, LuaLineReader reader, Assets assets)
		{
			if (string.IsNullOrEmpty(mcall))
				return null;
			
			if (mcall[0] == ':')
				mcall = mcall.Substring(1);

			if (mcall.StartsWith("setSource"))
			{
				E.HintClass(EntityClass.ScriptEntity);

				while (!mcall.EndsWith(")"))
					mcall = reader.GetOrThrow(true);
			}
			else if (mcall.StartsWith("set"))
			{
				string tcall = mcall.Substring(3, mcall.Length - 4);
				string[] pieces = tcall.Split('(');
				if (pieces.Length != 2)
				{
					Lint.MsgErr("Can't parse: :{0}", mcall);
				}
				else
				{
					E.Properties[pieces[0]] = pieces[1].Replace("\"", "").Replace(")", "").Replace("(", "").Trim();
				}
			}
			else if (mcall.StartsWith("addConnector"))
			{
				// addConnector("activate", "id", "open")
				E.Connectors.Add(new Connector(E.Id, mcall));
			}
			else if (mcall.StartsWith("addItem(spawn("))
			{
				if (mcall.Contains(":addItem"))
				{
					string[] calls = mcall.Split(new string[] { ":", ")spaw" }, StringSplitOptions.RemoveEmptyEntries);

					foreach (string call in calls)
					{
						if (call.StartsWith("n("))
							return "spaw" + call;
						else
						{
							string thiscall = call;
							if (!thiscall.EndsWith(")"))
								thiscall += ")";
							ParseMethod(E, thiscall.Trim(), reader, assets);
						}
					}
				}
				else
				{
					string item = mcall.Substring("addItem(spawn(\"".Length, mcall.Length - "addItem(spawn(\"".Length - 1).Replace("(", "").Replace(")", "").Replace("\"", "").Trim();
					Entity S = new Entity() { Name = item };
					S.SetContainer(E, E.Items.Count + 1);
					S.PostCreate(assets);
					E.Items.Add(S);
				}
			}
			else if (mcall.StartsWith("addTrapDoor"))
			{
				E.HintClass(EntityClass.Pit);
				E.HasTrapDoor = true;
			}
			else if (mcall.StartsWith("addPullChain"))
			{
				E.HintClass(EntityClass.Door);
				E.HasPullChain = true;
			}
			else if (mcall.StartsWith("addTorch"))
			{
				E.HintClass(EntityClass.TorchHolder);
				E.HasTorch = true;
			}
			else if (mcall.StartsWith("activate"))
			{
				E.IsActive = true;
			}
			else if (mcall.StartsWith("deactivate"))
			{
				E.IsActive = false;
			}
			else
			{
				Lint.MsgWarn("Unknown method: {0}", mcall);
			}

			return null;
		}

	}
}
