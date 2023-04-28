using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace RPI_Engine_Worldfile_Converter
{
	public enum RPIItemType
	{
		Undefined,
		Light,
		Scroll,
		Wand,
		Staff,
		Weapon,
		Shield,
		Missile,
		Treasure,
		Armor,
		Potion,
		Worn,
		Other,
		Trash,
		Trap,
		Container,
		Note,
		Liquid_container,
		Key,
		Food,
		Money,
		Ore,
		Board,
		Fountain,
		Grain,
		Perfume,
		Pottery,
		Salt,
		Zone,
		Plant,
		Component,
		Herb,
		Salve,
		Poison,
		Lockpick,
		Wind_inst,
		Percu_inst,
		String_inst,
		Fur,
		Woodcraft,
		Spice,
		Tool,
		Usury_note,
		Bridle,
		Ticket,
		Skull,
		Dye,
		Cloth,
		Ingot,
		Timber,
		Fluid,
		Liquid_Fuel,
		Remedy,
		Parchment,
		Book,
		Writing_inst,
		Ink,
		Quiver,
		Sheath,
		Keyring,
		Bullet,
		NPC_Object,
		Dwelling,
		Unused,
		Repair,
		Tossable,
		DO_NOT_USE,
		MerchTicket,
		RoomRental,
	}

	[Flags]
	public enum RPIExtraBits : uint
	{
		Destroyed = 1u << 0,
		Pitchable = 1u << 1,
		Invisible = 1u << 2,
		Magic = 1u << 3,
		Nodrop = 1u << 4,
		Benign = 1u << 5,
		Getaffect = 1u << 6,
		Dropaffect = 1u << 7,
		Multiaffect = 1u << 8,
		Wearaffect = 1u << 9,
		Wieldaffect = 1u << 10,
		Hitaffect = 1u << 11,
		Ok = 1u << 12,
		NoPurge = 1u << 13,
		itemleader = 1u << 14,
		itemmember = 1u << 15,
		itemomni = 1u << 16,
		Illegal = 1u << 17,
		Poisoned = 1u << 18,
		Mask = 1u << 19,
		Mount = 1u << 20,
		Table = 1u << 21,
		Stack = 1u << 22,
		vNPCDwelling = 1u << 23,
		Loads = 1u << 24,
		Variable = 1u << 25,
		Timer = 1u << 26,
		PCSold = 1u << 27,
		Thrown = 1u << 28,
		NewSkills = 1u << 29,
		Pitched = 1u << 30,
		IsVNPC = 1u << 31,
	}

	[Flags]
	public enum RPIWearBits : long
	{
		Take = 1 << 0,
		Finger = 1 << 1,
		Neck = 1 << 2,
		Body = 1 << 3,
		Head = 1 << 4,
		Legs = 1 << 5,
		Feet = 1 << 6,
		Hands = 1 << 7,
		Arms = 1 << 8,
		Wshield = 1 << 9,
		About = 1 << 10,
		Waist = 1 << 11,
		Wrist = 1 << 12,
		Wield = 1 << 13,
		Unused1 = 1 << 14,
		Unused2 = 1 << 15,
		Unused3 = 1 << 16,
		Sheath = 1 << 17,
		Belt = 1 << 18,
		Back = 1 << 19,
		Blindfold = 1 << 20,
		Throat = 1 << 21,
		Ears = 1 << 22,
		Shoulder = 1 << 23,
		Ankle = 1 << 24,
		Hair = 1 << 25,
		Face = 1 << 26,
		Armband = 1 << 27,
	}

	//public enum RPIMaterial
	//{
	//	None,
	//	Textile,
	//	Leather,
	//	Wood,
	//	Metal,
	//	Stone,
	//	Glass,
	//	Parchment,
	//	Liquid,
	//	Vegetation,
	//	Ceramic,
	//	Other,
	//	Meat,
	//}

	public enum RPIMaterial
	{
		None,
		Textile,
		Leather,
		Wood,
		Metal,
		Stone,
		Glass,
		Parchment,
		Liquid,
		Vegetation,
		Ceramic,
		Other,
		Meat,
	}

	[Flags]
	public enum RPIEconFlags
	{
		Sinda = 1 << 0,
		Noldo = 1 << 1,
		Orkish = 1 << 2,
		Dwarvish = 1 << 3,
		Beorian = 1 << 4,
		Marachain = 1 << 5,
		Haladin = 1 << 6,
		Generic = 1 << 7,
		Junk = 1 << 8,
		Fine = 1 << 9,
		Poor = 1 << 10,
		Raw = 1 << 11,
		Cooked = 1 << 12,
		Admin1 = 1 << 13,
		BUG = 1 << 14,
		Practice = 1 << 15,
		Used = 1 << 16,
		Admin2 = 1 << 17,
		NoBarter = 1 << 18,
	}

	public enum RPISkill
	{
		None,
		Brawling,
		LightEdge,
		MediumEdge,
		HeavyEdge,
		LightBlunt,
		MediumBlunt,
		HeavyBlunt,
		LightPierce,
		MediumPierce,
		HeavyPierce,
		Staff,
		Polearm,
		Thrown,
		Blowgun,
		Sling,
		Shortbow,
		Longbow,
		Crossbow,
		Dual,
		Block,
		Parry,
		Subdue,
		Disarm, //not used
		Sneak,
		Hide,
		Steal,
		Pick,
		Search,
		Listen,
		Forage,
		Ritual,
		Scan,
		Backstab,
		Barter,
		Ride,
		Climb,
		Swimming,
		Hunt,
		Skin,
		Sail,
		Poisoning,
		Alchemy,
		Herbalism,
		Clairvoyance,
		DangerSense,
		EmpathicHeal,
		Hex,
		MentalBolt,
		Prescience,
		Sensitivity,
		Telepathy,
		Seafaring,
		Dodge,
		Tame,
		Break,
		Metalcraft,
		Woodcraft,
		Textilecraft,
		Cookery,
		Baking,
		Hideworking,
		Stonecraft,
		Candlery,
		Brewing,
		Distilling,
		Literacy,
		Dyecraft,
		Apothecary,
		Glasswork,
		Gemcraft,
		Milling,
		Mining,
		Perfumery,
		Pottery,
		Tracking,
		Farming,
		Healing,
		SpeakAtliduk,
		SpeakAdunaic,
		SpeakHaradaic,
		SpeakWestron,
		SpeakDunael,
		SpeakLabba,
		SpeakNorliduk,
		SpeakRohirric,
		SpeakTalathic,
		SpeakUmitic,
		SpeakNahaiduk,
		SpeakPukael,
		SpeakSindarin,
		SpeakQuenya,
		SpeakSilvan,
		SpeakKhuzdul,
		SpeakOrkish,
		SpeakBlackSpeech,
		ScriptSarati,
		ScriptTengwar,
		ScriptBeleriandTengwar,
		ScriptCerthasDaeron,
		ScriptAngerthasDaeron,
		ScriptQuenyanTengwar,
		ScriptAngerthasMoria,
		ScriptGondorianTengwar,
		ScriptArnorianTengwar,
		ScriptNumenianTengwar,
		ScriptNorthernTengwar,
		ScriptAngerthasErebor,
		BlackWise,
		GreyWise,
		WhiteWise,
		Runecasting,
		Gambling,
		Bonecarving,
		Gardening,
		Sleight,
		Astronomy,
	}

	internal record RPIItem
	{
		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
		{
			return $"Item #{Vnum} - {ShortDescription}";
		}

		#endregion

		public int Vnum { get; init; }
		public string Name { get; init; }
		public string ShortDescription { get; init; }
		public string LongDescription { get; init; }
		public string FullDescription { get; init; }
		public RPIItemType ItemType { get; init; }
		public RPIExtraBits ExtraBits { get; init; }
		public RPIWearBits WearBits { get; init; }
		public RPIMaterial Material { get; init; }
		public int Oval0 { get; init; }
		public int Oval1 { get; init; }
		public int Oval2 { get; init; }
		public int Oval3 { get; init; }
		public int Oval4 { get; init; }
		public int Oval5 { get; init; }
		public int Weight { get; init; }
		public int Activation { get; init; }
		public int Quality { get; init; }
		public RPIEconFlags EconFlags { get; init; }
		public int Size { get; init; }
		public double Farthings { get; init; }
		public int MorphTo { get; init; }
		public string? ClanName { get; init; }
		public string? ClanRank { get; init; }

		public string InkColour { get; init; }
		public string DescKeys { get; init; }
		public IEnumerable<(RPISkill Skill, int Modifier)> SkillModifiers { get; init; }

		public RPIItem(string definition)
		{
			var lines = definition.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
			Vnum = int.Parse(lines[0].Substring(1));
			Name = new StringStack(lines[1]).Pop();
			Material = RPIMaterial.Other;
			foreach (var value in new StringStack(lines[1]).PopAll())
			{
				var keyword = value;
				if (keyword.EndsWith('~'))
				{
					keyword = keyword.RemoveLastCharacter();
				}

				if (keyword.All(x => char.IsUpper(x)) && Enum.TryParse<RPIMaterial>(keyword, true, out var result))
				{
					Material = result;
					break;
				}
			}
			ShortDescription = lines[2].RemoveLastCharacter();
			LongDescription = lines[3].RemoveLastCharacter();
			var i = 4;
			if (lines[3].Last() != '~' && lines[4] == "~")
			{
				i++;
			}
			var sb = new StringBuilder();
			for ( ; i < lines.Length; i++)
			{
				if (lines[i] == "~")
				{
					lines = lines.Skip(i + 1).ToArray();
					break;
				}

				sb.AppendLine(lines[i]);
				if (lines[i].EndsWith('~'))
				{
					lines = lines.Skip(i + 1).ToArray();
					break;
				}
			}
			FullDescription = sb.ToString();
			var line = new StringStack(lines[0]);
			ItemType = (RPIItemType)int.Parse(line.Pop());
			ExtraBits = (RPIExtraBits)int.Parse(line.Pop());
			WearBits = (RPIWearBits)int.Parse(line.Pop());
			line = new StringStack(lines[1]);
			Oval0 = int.Parse(line.Pop());
			Oval1 = int.Parse(line.Pop());
			Oval2 = int.Parse(line.Pop());
			Oval3 = int.Parse(line.Pop());
			Weight = int.Parse(new StringStack(lines[2]).Pop());
			var oval4line = lines[3];
			var oval4SS = new StringStack(oval4line);
			Oval4 = int.Parse(oval4SS.Pop());
			oval4SS.Pop();
			lines = lines.Skip(4).ToArray();
			if (ItemType == RPIItemType.Ink)
			{
				InkColour = lines[0].RemoveLastCharacter();
				lines = lines.Skip(1).ToArray();
				oval4SS = new StringStack(lines[0]);
			}
			else if (ItemType == RPIItemType.Worn || ItemType == RPIItemType.Armor || ItemType == RPIItemType.Container)
			{
				if (ExtraBits.HasFlag(RPIExtraBits.Mask))
				{
					DescKeys = lines[0].RemoveLastCharacter();
					lines = lines.Skip(1).ToArray();
					oval4SS = new StringStack(lines[0]);
				}
			}
			else if (ItemType == RPIItemType.Tossable)
			{
				DescKeys = lines[0].RemoveLastCharacter();
				lines = lines.Skip(1).ToArray();
				oval4SS = new StringStack(lines[0]);
			}

			line = oval4SS;
			Oval5 = int.Parse(line.Pop());
			Activation = int.Parse(line.Pop());
			Quality = int.Parse(line.Pop());
			EconFlags = (RPIEconFlags)int.Parse(line.Pop());
			Size = int.Parse(line.Pop());

			line = new StringStack(lines[0]);
			Farthings = double.Parse(line.Pop());
			line.Pop();
			MorphTo = int.Parse(line.Pop());
			line.Pop();
			//Material = (RPIMaterial)int.Parse(line.Pop());

			lines = lines.Skip(1).ToArray();
			var affected = new List<(RPISkill Skill, int Value)>();
			while (lines.Any())
			{
				if (lines[0] == "C")
				{
					ClanName = lines[1].RemoveLastCharacter();
					ClanRank = lines[2].RemoveLastCharacter();
					lines = lines.Skip(3).ToArray();
					continue;
				}

				if (lines[0] == "A")
				{
					line = new StringStack(lines[1]);
					var skill = (RPISkill)(int.Parse(line.Pop())-10000);
					var impact = int.Parse(line.Pop());
					affected.Add((skill, impact));
				}

				break;
			}

			SkillModifiers = affected;
		}
	}
}
