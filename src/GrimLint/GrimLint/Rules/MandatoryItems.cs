using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules.Base;

namespace GrimLint.Rules
{
	public class MandatoryItems : Rule
	{
		string[] MANDATORY_ITEMS = 
		{
			"hand_axe", "battle_axe", "great_axe", "machete", "long_sword", "cutlass", "cudgel", "knoffer", "warhammer", "flail",
				"knife", "dagger", "fist_dagger", "assassin_dagger",
				"short_bow", "sling", "crossbow", "arrow", "quarrel", 
				"hide_vest", "leather_brigandine", "leather_greaves", "leather_cap", "leather_boots", "ring_mail", "ring_greaves", "ring_gauntlets", "ring_boots", 
				"legionary_helmet", "iron_basinet", "plate_cuirass", "plate_greaves", "full_helmet", "plate_gauntlets", "plate_boots",
				"peasant_breeches", "peasant_tunic", "peasant_cap", "loincloth", "leather_pants", "doublet", "silk_hose", "flarefeather_cap", "sandals",
				"grim_cap", "tar_bead", "cave_nettle", "slime_bell", "blooddrop_blossom", "milkreed",
				"whitewood_wand",
				"mortar", "compass", "skull", 
				"scroll_light", "scroll_darkness", "scroll_fireburst", "scroll_shock", "scroll_fireball", "scroll_frostbolt", "scroll_ice_shards", "scroll_poison_bolt", "scroll_poison_cloud", "scroll_lightning_bolt", "scroll_enchant_fire_arrow", "scroll_fire_shield", "scroll_frost_shield", "scroll_poison_shield", "scroll_shock_shield", "scroll_invisibility",
				"rock", "throwing_knife", "shuriken", 
				"huntsman_cloak", "tattered_cloak", "scaled_cloak","leather_gloves","fire_bomb", "shock_bomb", "frost_bomb", "poison_bomb",
		};

		public override void Run(Dungeon D)
		{
			foreach (string mi in MANDATORY_ITEMS)
			{
				if ((!D.EntitiesByName.ContainsKey(mi)) || (D.EntitiesByName[mi].Count == 0))
					Warning(Entity.EmptyEntity, "An item of type {0} is mandatory but not found!", mi);
			}
		}
	}
}
