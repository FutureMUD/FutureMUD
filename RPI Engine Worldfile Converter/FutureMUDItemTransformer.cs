using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace RPI_Engine_Worldfile_Converter;

internal class FutureMUDItemTransformer
{
	public FutureMUDItemTransformer(FuturemudDatabaseContext context)
	{
		foreach (var material in Enum.GetValues<RPIMaterial>())
		{
			// TODO - create these materials in the seeder
			switch (material)
			{
				case RPIMaterial.None:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "mana");
					break;
				case RPIMaterial.Textile:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "textile");
					break;
				case RPIMaterial.Leather:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "leather");
					break;
				case RPIMaterial.Wood:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "wood");
					break;
				case RPIMaterial.Metal:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "metal");
					break;
				case RPIMaterial.Stone:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "stone");
					break;
				case RPIMaterial.Glass:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "glass");
					break;
				case RPIMaterial.Parchment:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "parchment");
					break;
				case RPIMaterial.Liquid:
					continue;
				case RPIMaterial.Vegetation:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "vegetation");
					break;
				case RPIMaterial.Ceramic:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "ceramic");
					break;
				case RPIMaterial.Other:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "other");
					break;
				case RPIMaterial.Meat:
					MaterialConversion[material] = context.Materials.First(x => x.Name == "meat");
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		foreach (var skill in Enum.GetValues<RPISkill>())
		{
			switch (skill)
			{
				case RPISkill.Brawling:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Brawl");
					break;
				case RPISkill.LightEdge:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Light-Edge");
					break;
				case RPISkill.MediumEdge:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Medium-Edge");
					break;
				case RPISkill.HeavyEdge:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Heavy-Edge");
					break;
				case RPISkill.LightBlunt:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Light-Blunt");
					break;
				case RPISkill.MediumBlunt:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Medium-Blunt");
					break;
				case RPISkill.HeavyBlunt:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Heavy-Blunt");
					break;
				case RPISkill.LightPierce:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Light-Pierce");
					break;
				case RPISkill.MediumPierce:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Medium-Pierce");
					break;
				case RPISkill.HeavyPierce:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Heavy-Pierce");
					break;
				case RPISkill.Staff:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Staff");
					break;
				case RPISkill.Polearm:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Polearm");
					break;
				case RPISkill.Thrown:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Throw");
					break;
				case RPISkill.Blowgun:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Sling:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Shortbow:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Bows");
					break;
				case RPISkill.Longbow:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Bows");
					break;
				case RPISkill.Crossbow:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Crossbows");
					break;
				case RPISkill.Dual:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Dual-Wielding");
					break;
				case RPISkill.Block:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Block");
					break;
				case RPISkill.Parry:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Parry");
					break;
				case RPISkill.Subdue:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Wrestle");
					break;
				case RPISkill.Disarm:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Sneak:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Sneak");
					break;
				case RPISkill.Hide:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Hide");
					break;
				case RPISkill.Steal:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Steal");
					break;
				case RPISkill.Pick:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Pick Locks");
					break;
				case RPISkill.Search:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Search");
					break;
				case RPISkill.Listen:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Listen");
					break;
				case RPISkill.Forage:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Forage");
					break;
				case RPISkill.Ritual:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Scan:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Spot");
					break;
				case RPISkill.Backstab:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Veterancy");
					break;
				case RPISkill.Barter:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Ride:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Ride");
					break;
				case RPISkill.Climb:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Climb");
					break;
				case RPISkill.Swimming:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Swim");
					break;
				case RPISkill.Hunt:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Track");
					break;
				case RPISkill.Skin:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Skin");
					break;
				case RPISkill.Sail:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Poisoning:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Pharmacology");
					break;
				case RPISkill.Alchemy:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Pharmacology");
					break;
				case RPISkill.Herbalism:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Herbalist");
					break;
				case RPISkill.Clairvoyance:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.DangerSense:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.EmpathicHeal:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Hex:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.MentalBolt:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Prescience:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Sensitivity:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Telepathy:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Seafaring:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Dodge:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Dodge");
					break;
				case RPISkill.Tame:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Husbandry");
					break;
				case RPISkill.Break:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Husbandry");
					break;
				case RPISkill.Metalcraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Blacksmith");
					break;
				case RPISkill.Woodcraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Carpenter");
					break;
				case RPISkill.Textilecraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Tailor");
					break;
				case RPISkill.Cookery:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Cook");
					break;
				case RPISkill.Baking:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Baker");
					break;
				case RPISkill.Hideworking:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Tanner");
					break;
				case RPISkill.Stonecraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Mason");
					break;
				case RPISkill.Candlery:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Chandler");
					break;
				case RPISkill.Brewing:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Brewer");
					break;
				case RPISkill.Distilling:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Distiller");
					break;
				case RPISkill.Literacy:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Literacy");
					break;
				case RPISkill.Dyecraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Dyer");
					break;
				case RPISkill.Apothecary:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Pharmacology");
					break;
				case RPISkill.Glasswork:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Glazier");
					break;
				case RPISkill.Gemcraft:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Jeweller");
					break;
				case RPISkill.Milling:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Miller");
					break;
				case RPISkill.Mining:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Miner");
					break;
				case RPISkill.Perfumery:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Perfumer");
					break;
				case RPISkill.Pottery:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Potter");
					break;
				case RPISkill.Tracking:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Track");
					break;
				case RPISkill.Farming:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Farmer");
					break;
				case RPISkill.Healing:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "First Aid");
					break;
				case RPISkill.SpeakAtliduk:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Westron");
					break;
				case RPISkill.SpeakAdunaic:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Adunaic");
					break;
				case RPISkill.SpeakHaradaic:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Haradic");
					break;
				case RPISkill.SpeakWestron:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Westron");
					break;
				case RPISkill.SpeakDunael:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Westron");
					break;
				case RPISkill.SpeakLabba:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.SpeakNorliduk:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Westron");
					break;
				case RPISkill.SpeakRohirric:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Rohirric");
					break;
				case RPISkill.SpeakTalathic:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Varadja");
					break;
				case RPISkill.SpeakUmitic:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.SpeakNahaiduk:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Adunaic");
					break;
				case RPISkill.SpeakPukael:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.SpeakSindarin:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Sindarin");
					break;
				case RPISkill.SpeakQuenya:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Quenya");
					break;
				case RPISkill.SpeakSilvan:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Silvan");
					break;
				case RPISkill.SpeakKhuzdul:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Khuzdul");
					break;
				case RPISkill.SpeakOrkish:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Orkish");
					break;
				case RPISkill.SpeakBlackSpeech:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Black Speech");
					break;
				case RPISkill.ScriptSarati:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptBeleriandTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptCerthasDaeron:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptAngerthasDaeron:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptQuenyanTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptAngerthasMoria:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptGondorianTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptArnorianTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptNumenianTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptNorthernTengwar:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.ScriptAngerthasErebor:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.BlackWise:
					SkillConverstion[skill] =  null;
					break;
				case RPISkill.GreyWise:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.WhiteWise:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Runecasting:
					SkillConverstion[skill] = null;
					break;
				case RPISkill.Gambling:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Palm");
					break;
				case RPISkill.Bonecarving:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Scrimshaw");
					break;
				case RPISkill.Gardening:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Herbalism");
					break;
				case RPISkill.Sleight:
					SkillConverstion[skill] = context.TraitDefinitions.First(x => x.Name == "Palm");
					break;
				case RPISkill.Astronomy:
					SkillConverstion[skill] = null;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		foreach (var component in context.GameItemComponentProtos.Include(x => x.EditableItem))
		{
			if (component.EditableItem.RevisionStatus != 4)
			{
				continue;
			}
			ComponentProtoLookup[component.Name] = component;
		}
	}

	public Dictionary<RPIMaterial, MudSharp.Models.Material> MaterialConversion { get; } =
		new Dictionary<RPIMaterial, Material>();

	public Dictionary<RPISkill, MudSharp.Models.TraitDefinition?> SkillConverstion { get; } = new();

	public Dictionary<string, MudSharp.Models.GameItemComponentProto> ComponentProtoLookup { get; } =
		new(StringComparer.OrdinalIgnoreCase);

	private MudSharp.Models.GameItemComponentProto GetOrCreateLanternProto(double capacity)
	{
		var name = $"Lantern_";
		throw new NotImplementedException();
	}

	//public Dictionary<>

	public MudSharp.Models.GameItemProto ConvertItem(RPIItem item)
	{
		// Common Information
		var dbitem = new MudSharp.Models.GameItemProto();
		dbitem.Name = item.Name;
		dbitem.ShortDescription = item.ShortDescription;
		dbitem.LongDescription = item.LongDescription;
		dbitem.FullDescription = item.FullDescription;
		dbitem.Weight = item.Weight;
		dbitem.CostInBaseCurrency = (decimal)item.Farthings;

		// Handle Wear Flags
		var wearFlags = item.WearBits.GetAllFlags();
		foreach (var flag in wearFlags)
		{
			switch (flag)
			{
				case RPIWearBits.Take:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Holdable"]
					});
					break;
				case RPIWearBits.Finger:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Ring"]
					});
					break;
				case RPIWearBits.Neck:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Gorget"]
					});
					break;
				case RPIWearBits.Body:
					if (item.DescKeys.Contains("dress", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Dress"]
						});
						break;
					}
					if (item.DescKeys.Contains("coat", StringComparison.OrdinalIgnoreCase) || 
					    item.DescKeys.Contains("jacket", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Jacket"]
						});
						break;
					}
					if (item.DescKeys.Contains("vest", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Vest"]
						});
						break;
					}
					if (item.DescKeys.Contains("robe", StringComparison.OrdinalIgnoreCase) || 
					    item.DescKeys.Contains("gown", StringComparison.OrdinalIgnoreCase) || 
					    item.DescKeys.Contains("kaftan", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Gown"]
						});
						break;
					}
					if (item.DescKeys.Contains("tunic", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Tunic"]
						});
						break;
					}
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Shirt"]
					});
					break;
				case RPIWearBits.Head:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Half_Helmet"]
					});
					break;
				case RPIWearBits.Legs:
					
					if (item.DescKeys.Contains("skirt", StringComparison.OrdinalIgnoreCase) ||
					    item.DescKeys.Contains("loincloth", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Skirt"]
						});
						break;
					}
					if (item.DescKeys.Contains("thong", StringComparison.OrdinalIgnoreCase))
					{
						dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
						{
							GameItemProto = dbitem,
							GameItemComponent = ComponentProtoLookup["Wear_Thong"]
						});
						break;
					}
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Trousers"]
					});
					break;
				case RPIWearBits.Feet:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Shoes"]
					});
					break;
				case RPIWearBits.Hands:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Gloves"]
					});
					break;
				case RPIWearBits.Arms:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Bracers"]
					});
					break;
				case RPIWearBits.Wshield:
					break;
				case RPIWearBits.About:
					if (!item.WearBits.HasFlag(RPIWearBits.Body))
					{
						continue;
					}
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Mantle"]
					});
					break;
				case RPIWearBits.Waist:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Waist"]
					});
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Belt_2"]
					});
					break;
				case RPIWearBits.Wrist:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Bracelet"]
					});
					break;
				case RPIWearBits.Wield:
				case RPIWearBits.Unused1:
				case RPIWearBits.Unused2:
				case RPIWearBits.Unused3:
					continue;
				case RPIWearBits.Sheath:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Bracelets"]
					});
					break;
				case RPIWearBits.Belt:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Beltable"]
					});
					break;
				case RPIWearBits.Back:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Cloak (Open)"]
					});
					break;
				case RPIWearBits.Blindfold:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Sunglasses"]
					});
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Blindfold"]
					});
					break;
				case RPIWearBits.Throat:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Necklace"]
					});
					break;
				case RPIWearBits.Ears:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Earrings"]
					});
					break;
				case RPIWearBits.Shoulder:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Shoulder"]
					});
					break;
				case RPIWearBits.Ankle:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Anklet"]
					});
					break;
				case RPIWearBits.Hair:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Wig"]
					});
					break;
				case RPIWearBits.Face:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Wear_Mask"]
					});
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["EyesFreeObscurer"]
					});
					break;
				case RPIWearBits.Armband:
					dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
					{
						GameItemProto = dbitem,
						GameItemComponent = ComponentProtoLookup["Epaulette"]
					});
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		// Handle Item Type Specific Information
		switch (item.ItemType)
		{
			case RPIItemType.Undefined:
				break;
			case RPIItemType.Light:
				if (item.Oval2 == 0)
				{
				}
				break;
			case RPIItemType.Weapon:
				break;
			case RPIItemType.Shield:
				break;
			case RPIItemType.Missile:
				break;
			case RPIItemType.Armor:
				break;
			case RPIItemType.Potion:
				break;
			case RPIItemType.Worn:
				break;
			case RPIItemType.Container:
				break;
			case RPIItemType.Note:
				break;
			case RPIItemType.Liquid_container:
				break;
			case RPIItemType.Key:
				break;
			case RPIItemType.Food:
				break;
			case RPIItemType.Money:
				break;
			case RPIItemType.Board:
				break;
			case RPIItemType.Fountain:
				break;
			case RPIItemType.Poison:
				break;
			case RPIItemType.Lockpick:
				break;
			case RPIItemType.Tool:
				break;
			case RPIItemType.Dye:
				break;
			case RPIItemType.Fluid:
				break;
			case RPIItemType.Liquid_Fuel:
				break;
			case RPIItemType.Parchment:
				break;
			case RPIItemType.Book:
				break;
			case RPIItemType.Writing_inst:
				break;
			case RPIItemType.Ink:
				break;
			case RPIItemType.Quiver:
				break;
			case RPIItemType.Sheath:
				break;
			case RPIItemType.Keyring:
				break;
			case RPIItemType.Bullet:
				break;
			case RPIItemType.Dwelling:
				break;
			case RPIItemType.Repair:
				break;
			case RPIItemType.Tossable:
				break;
			case RPIItemType.Scroll:
			case RPIItemType.Wand:
			case RPIItemType.Staff:
			case RPIItemType.Treasure:
			case RPIItemType.Other:
			case RPIItemType.Trash:
			case RPIItemType.Ore:
			case RPIItemType.Grain:
			case RPIItemType.Perfume:
			case RPIItemType.Pottery:
			case RPIItemType.Salt:
			case RPIItemType.Zone:
			case RPIItemType.Plant:
			case RPIItemType.Component:
			case RPIItemType.Herb:
			case RPIItemType.Salve:
			case RPIItemType.Wind_inst:
			case RPIItemType.Percu_inst:
			case RPIItemType.String_inst:
			case RPIItemType.Fur:
			case RPIItemType.Woodcraft:
			case RPIItemType.Skull:
			case RPIItemType.Cloth:
			case RPIItemType.Ingot:
			case RPIItemType.Timber:
			case RPIItemType.Remedy:
			case RPIItemType.NPC_Object:
			case RPIItemType.Bridle:
			case RPIItemType.Ticket:
			case RPIItemType.Unused:
			case RPIItemType.DO_NOT_USE:
			case RPIItemType.MerchTicket:
			case RPIItemType.RoomRental:
			case RPIItemType.Trap:
			case RPIItemType.Spice:
			case RPIItemType.Usury_note:
				// These all are probably just prop objects in FutureMUD
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		throw new NotImplementedException();
	}
}