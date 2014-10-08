using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules.Base;

namespace GrimLint.Rules
{
	public class LockWithoutKey : SimpleRule
	{

		public override IEnumerable<EntityClass> GetClasses()
		{
			yield return EntityClass.Lock;
		}

		public override void Execute(Dungeon D, Entity E)
		{
			string keyType = E.GetProperty("OpenedBy");

			if (string.IsNullOrWhiteSpace(keyType))
				Error(E, "lock has no openedby property");
			else if ((!D.EntitiesByName.ContainsKey(keyType)) || (D.EntitiesByName[keyType].Count == 0))
				Error(E, "lock is opened by {0} but no key was found", keyType);
		}
	}
}
