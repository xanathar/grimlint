using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Rules.Base
{
	public abstract class SimpleRule : Rule
	{
		public override void Run(Dungeon D)
		{
			foreach(EntityClass C in GetClasses())
			{
				if (!D.EntitiesByClass.ContainsKey(C))
					continue;

				foreach(Entity E in D.EntitiesByClass[C])
				{
					Execute(D, E);
				}
			}
		}

		public abstract IEnumerable<EntityClass> GetClasses();
		public abstract void Execute(Dungeon D, Entity E);
	}
}
