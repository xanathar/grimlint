using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Model
{
	public class Entity
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Facing { get; set; }
		public EntityClass Class { get; set; }
		public ItemClass ItemClass { get; set; }
		public Dictionary<string, string> Properties = new Dictionary<string, string>();
		public List<Connector> Connectors = new List<Connector>();
		public List<Connector> ReverseConnectors = new List<Connector>();
		public List<Entity> Items = new List<Entity>();
		public bool HasPullChain = false;
		public bool HasTrapDoor = false;
		public bool HasTorch = false;
		public bool IsActive = false;
		public bool KnownName = false;

		public Entity()
		{
		}



		public static Entity EmptyEntity { get { return new Entity(); } }


		public void SetContainer(Entity container, int progressiveId)
		{
			Id = container.Id + "::" + progressiveId.ToString();
			Level = container.Level;
			X = container.X;
			Y = container.Y;
			Facing = container.Facing;
		}

		public Entity PostCreate(Assets assets)
		{
			if (Id.StartsWith(Name + "_"))
			{
				int dummy;
				string id = Id.Substring(Name.Length + 1);
				KnownName = !(int.TryParse(id, out dummy));
			}
			else
			{
				KnownName = true;
			}

			Asset A = assets.Get(Name);

			if (A != null)
			{
				Class = A.Class;
				ItemClass = A.ItemClass;
			}
			else
			{
				Class = EntityClass.Unknown;
				ItemClass = 0;
				Lint.MsgWarn("Can't find asset {0} for entity {1}", Name, this);
			}

			return this;
		}

		public string GetProperty(string propName)
		{
			if (Properties.ContainsKey(propName))
				return Properties[propName];
			return null;
		}

		public void HintClass(EntityClass clss)
		{
			if (this.Class == EntityClass.Unknown)
				this.Class = clss;
		}


		public override string ToString()
		{
			if (IsEmptyEntity()) return "[$]";
			return string.Format("[{0}({1},{2},{3})]", Id, Level, X, Y);
		}

		public bool IsEmptyEntity()
		{
			return Id == null;
		}

	}
}
