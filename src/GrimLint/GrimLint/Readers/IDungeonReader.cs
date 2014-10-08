using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Readers
{
	public interface IDungeonReader
	{
		void Load(Dungeon D);
	}
}
