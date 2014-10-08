using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GrimLint
{
	public class LuaLineReader
	{
		string[] m_Lines;
		int m_Index = 0;

		public LuaLineReader(string filename)
		{
			m_Lines = File.ReadAllLines(filename);
		}

		public string Get(bool dontskipcomments = false)
		{
			if (m_Index < m_Lines.Length)
			{
				string ret = m_Lines[m_Index];
				++m_Index;

				ret = ret.Trim();

				if (string.IsNullOrEmpty(ret) || (!dontskipcomments && ret.StartsWith("--")))
					return Get();

				return ret;
			}
			else
				return null;
		}

		public string GetOrThrow(bool dontskipcomments = false)
		{
			string l = Get(dontskipcomments);

			if (l == null)
				throw new EndOfFileException();

			return l;
		}

		public void RollBack()
		{
			--m_Index;
		}
	}
}
