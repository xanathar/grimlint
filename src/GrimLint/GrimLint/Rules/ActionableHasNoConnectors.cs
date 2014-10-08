using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules.Base;

namespace GrimLint.Rules
{
	public class ActionableHasNoConnectors : SimpleRule
	{
		public override IEnumerable<EntityClass> GetClasses()
		{
			yield return EntityClass.Button;
			yield return EntityClass.Counter;
			yield return EntityClass.Lever;
			yield return EntityClass.PressurePlate;
			yield return EntityClass.Timer;
			yield return EntityClass.Lock;
		}

		public override void Execute(Dungeon D, Entity E)
		{
			if (E.Connectors.Count == 0)
				Info(E, "has no connectors!");
		}
	}
}
