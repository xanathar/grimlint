using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules.Base;

namespace GrimLint.Rules
{
	public class DoorHasNoConnectorOrPullChain : SimpleRule
	{
		public override IEnumerable<EntityClass> GetClasses()
		{
			yield return EntityClass.Door;
		}

		public override void Execute(Dungeon D, Entity E)
		{
			if ((!E.HasPullChain) && (E.ReverseConnectors.Count == 0) && E.GetProperty("DoorState") != "open")
				Info(E, "Door has neither a pullchain nor a connector pending to it");
		}
	}
}
