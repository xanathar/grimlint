using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using GrimLint.Model;
using MoonSharp.Interpreter;

namespace GrimLint.Readers.LuaReader
{
	[MoonSharpUserData]
	public class LuaEntity : Entity
	{
		Assets m_Assets;

		public LuaEntity(int level, string name, object x, object y, object facing, object id, Assets assets)
		{
			m_Assets = assets;
			Level = level;

			Name = name;
			if (x != null) X = (int)((double)x);
			if (y != null) Y = (int)((double)y);
			if (facing != null) Facing = (int)((double)facing);
			Id = id as string;

			if (Id != null)
				PostCreate(assets);

		}

		public LuaEntity setSource(string val)
		{
			this.Properties["Source"] = val;
			return this;
		}

		public LuaEntity addPullChain()
		{
			this.HasPullChain = true;
			return this;
		}

		public LuaEntity setWallText(string val)
		{
			this.Properties["WallText"] = val;
			return this;
		}

		public LuaEntity addConnector(string a, string b, string c)
		{
			this.Connectors.Add(new Connector() { Source = this.Id, SourceAction = a, Target = b, TargetAction = c });
			return this;
		}

		public LuaEntity setScrollText(string val)
		{
			this.Properties["ScrollText"] = val;
			return this;
		}

		public LuaEntity setInitialValue(int val)
		{
			this.Properties["InitialValue"] = val.ToString();
			return this;
		}

		public LuaEntity setValue(int val)
		{
			this.Properties["Value"] = val.ToString();
			return this;
		}

		public LuaEntity setTriggeredByParty(bool val)
		{
			this.Properties["TriggeredByParty"] = val.ToString();
			return this;
		}

		public LuaEntity setTriggeredByMonster(bool val)
		{
			this.Properties["TriggeredByMonster"] = val.ToString();
			return this;
		}

		public LuaEntity setTriggeredByItem(bool val)
		{
			this.Properties["TriggeredByItem"] = val.ToString();
			return this;
		}

		public LuaEntity setActivateAlways(bool val)
		{
			this.Properties["ActivateAlways"] = val.ToString();
			return this;
		}

		public LuaEntity setActivateOnce(bool val)
		{
			this.Properties["ActivateOnce"] = val.ToString();
			return this;
		}

		public LuaEntity setSilent(bool val)
		{
			this.Properties["Silent"] = val.ToString();
			return this;
		}

		public LuaEntity setOpenedBy(string val)
		{
			this.Properties["OpenedBy"] = val.ToString();
			return this;
		}


		public LuaEntity setSpawnedEntity(string val)
		{
			this.Properties["SpawnedEntity"] = val.ToString();
			return this;
		}

		public LuaEntity setCoolDown(int val)
		{
			this.Properties["CoolDown"] = val.ToString();
			return this;
		}

		public LuaEntity addTorch()
		{
			this.HasTorch = true;
			return this;
		}

		public LuaEntity addTrapDoor()
		{
			this.HasTrapDoor = true;
			return this;
		}

		public LuaEntity addItem(object o)
		{
			Entity E = o as Entity;
			E.SetContainer(this, this.Items.Count + 1);
			E.PostCreate(this.m_Assets);
			this.Items.Add(E);
			return this;
		}

		public LuaEntity setLevel(int val)
		{
			this.Properties["Level"] = val.ToString();
			return this;
		}

		public LuaEntity setDoorState(string val)
		{
			this.Properties["DoorState"] = val.ToString();
			return this;
		}

		public LuaEntity setPitState(string val)
		{
			this.Properties["PitState"] = val.ToString();
			return this;
		}

		public LuaEntity setTimerInterval(double val)
		{
			this.Properties["TimerInterval"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setAIState(string val)
		{
			this.Properties["AIState"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setLeverState(string val)
		{
			this.Properties["LeverState"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setChangeFacing(bool val)
		{
			this.Properties["ChangeFacing"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setEntityType(string val)
		{
			this.Properties["EntityType"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setHideLight(bool val)
		{
			this.Properties["HideLight"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setScreenFlash(bool val)
		{
			this.Properties["ScreenFlash"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity setInvisible(bool val)
		{
			this.Properties["Invisible"] = val.ToString(CultureInfo.InvariantCulture);
			return this;
		}

		public LuaEntity activate()
		{
			this.IsActive = true;
			return this;
		}

		public LuaEntity deactivate()
		{
			this.IsActive = false;
			return this;
		}

		public LuaEntity setTeleportTarget(int level, int x, int y, int facing)
		{
			this.Properties["TeleportTarget::L"] = level.ToString(CultureInfo.InvariantCulture);
			this.Properties["TeleportTarget::X"] = x.ToString(CultureInfo.InvariantCulture);
			this.Properties["TeleportTarget::Y"] = y.ToString(CultureInfo.InvariantCulture);
			this.Properties["TeleportTarget::F"] = facing.ToString(CultureInfo.InvariantCulture);
			return this;
		}
	}
}
