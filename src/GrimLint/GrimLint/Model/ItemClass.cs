using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Model
{
	[Flags]
	public enum ItemClass
	{
		Quest = 0x1,
		Shield = 0x2,
		Accessory = 0x4,
		Bomb = 0x8,
		Food = 0x10,
		MissileWeapon = 0x20,
		Tome = 0x40,
		Armor = 0x80,
		Cloth = 0x100,
		Herb = 0x200,
		MachinePart = 0x400,
		Potion = 0x800,
		Staff = 0x1000,
		Treasure = 0x2000,
		Container = 0x4000,
		Key = 0x8000,
		Misc = 0x10000,
		Scroll = 0x20000,
		ThrowingWeapon = 0x40000,
		PlaceHolder = 0x80000,
		Axe = 0x100000,
		Mace = 0x200000,
		Sword = 0x400000,
		Dagger = 0x800000,
		ImprovisedWeapon = 0x1000000,
		MagicWeapon = 0x2000000,
	}


}
