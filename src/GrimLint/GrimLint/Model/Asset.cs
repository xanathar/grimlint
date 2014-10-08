using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GrimLint.Model;

namespace GrimLint.Model
{
	public class Asset
	{
		public string Name;
		public EntityClass Class;
		public ItemClass ItemClass;

		public Asset()
		{ }

		public Asset(Asset A)
		{
			this.Name = A.Name;
			this.Class = A.Class;
			this.ItemClass = A.ItemClass;
		}

		public Asset(XmlElement xe, Assets assets)
		{
			Name = xe.GetAttribute("name");

			string clone = xe.GetAttribute("clone");

			if (string.IsNullOrWhiteSpace(clone))
			{
				Class = (EntityClass)Enum.Parse(typeof(EntityClass), xe.GetAttribute("class"), true);

				if (!string.IsNullOrWhiteSpace(xe.GetAttribute("subclasses")))
					ItemClass = (ItemClass)Enum.Parse(typeof(ItemClass), xe.GetAttribute("subclasses"), true);
			}
			else
			{
				Asset A = assets.Get(clone);
				Class = A.Class;
				ItemClass = A.ItemClass;
			}
		}

	}
}
