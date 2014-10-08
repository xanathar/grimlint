using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Readers.LineReader
{
	public class LineDungeonReader : IDungeonReader
	{
		int m_CurLevel = 0;
		Dungeon D;

		public void Load(Dungeon D)
		{
			this.D = D;

			LuaLineReader reader = new LuaLineReader(D.DungeonLuaFile);

			while (true)
			{
				string line = reader.Get();

				if (line == null)
					break;

				try
				{
					LoadLine(line, reader);
				}
				catch (EndOfFileException)
				{
					Lint.MsgErr("FATAL: Unexpected end of file.");
					break;
				}
			}

			Lint.MsgVerbose("{0} entities loaded", D.EntitiesById.Values.Count);

			D.CreateReverseConnectors();
		}

		private void LoadLine(string line, LuaLineReader reader)
		{
			if (line.StartsWith("mapName("))
			{
				++m_CurLevel;
				Lint.MsgVerbose("Reading level {0}", m_CurLevel);
			}
			else if (line.StartsWith("setWallSet") || line.StartsWith("playStream"))
			{
				return;
			}
			else if (line.StartsWith("mapDesc([["))
			{
				while (reader.GetOrThrow() != "]])") ; // skip map
			}
			else if (line.StartsWith("spawn"))
			{
				Tuple<Entity, string> T = LineEntityReader.CreateEntity(m_CurLevel, line, reader, D.Assets);

				D.AddEntity(T.Item1, m_CurLevel);

				if (T.Item2 != null)
				{
					LoadLine(T.Item2, reader);
				}
			}
		}



	}
}
