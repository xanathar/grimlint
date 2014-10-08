using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules.Base;

namespace GrimLint.Rules
{
	public class AlcoveHasItems : SimpleRule
	{
		public override IEnumerable<EntityClass> GetClasses()
		{
			yield return EntityClass.Alcove;
		}

		public override void Execute(Dungeon D, Entity E)
		{
			if (E.Items.Count == 0)
				Warning(E, "does not contain any items");
		}
	}
}
