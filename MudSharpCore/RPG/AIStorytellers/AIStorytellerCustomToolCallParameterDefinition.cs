using MudSharp.Framework;
using MudSharp.FutureProg;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public record AIStorytellerCustomToolCallParameterDefinition
{
	public string Name { get; }
	public string Type { get; }
	public string Description { get; }

	public string ToJSONElement()
	{
		return $$"""
	"{{Name.EscapeForJson()}}": {
		"type": "{{Type}}",
		"description": "{{Description.EscapeForJson()}}"
	}
""";
	}

	public AIStorytellerCustomToolCallParameterDefinition(string name, string description, ProgVariableTypes type)
	{
		Name = name;
		switch (type.LegacyCode)
		{
			case ProgVariableTypeCode.Text:
				Type = "string";
				Description = description;
				break;
			case ProgVariableTypeCode.Number:
				Type = "double";
				Description = description;
				break;
			case ProgVariableTypeCode.Boolean:
				Type = "boolean";
				Description = description;
				break;
			case ProgVariableTypeCode.Character:
				Type = "int64";
				Description = $"The ID number of a character. {description}";
				break;
			case ProgVariableTypeCode.Location:
				Type = "int64";
				Description = $"The ID number of a room location. {description}";
				break;
			case ProgVariableTypeCode.Item:
				break;
			case ProgVariableTypeCode.Shard:
				break;
			case ProgVariableTypeCode.Error:
				break;
			case ProgVariableTypeCode.Gender:
				break;
			case ProgVariableTypeCode.Zone:
				break;
			case ProgVariableTypeCode.Collection:
				break;
			case ProgVariableTypeCode.Race:
				break;
			case ProgVariableTypeCode.Culture:
				break;
			case ProgVariableTypeCode.Chargen:
				break;
			case ProgVariableTypeCode.Trait:
				break;
			case ProgVariableTypeCode.Clan:
				break;
			case ProgVariableTypeCode.ClanRank:
				break;
			case ProgVariableTypeCode.ClanAppointment:
				break;
			case ProgVariableTypeCode.ClanPaygrade:
				break;
			case ProgVariableTypeCode.Currency:
				break;
			case ProgVariableTypeCode.Exit:
				break;
			case ProgVariableTypeCode.Literal:
				break;
			case ProgVariableTypeCode.DateTime:
				break;
			case ProgVariableTypeCode.TimeSpan:
				break;
			case ProgVariableTypeCode.Language:
				break;
			case ProgVariableTypeCode.Accent:
				break;
			case ProgVariableTypeCode.Merit:
				break;
			case ProgVariableTypeCode.MudDateTime:
				break;
			case ProgVariableTypeCode.Calendar:
				break;
			case ProgVariableTypeCode.Clock:
				break;
			case ProgVariableTypeCode.Effect:
				break;
			case ProgVariableTypeCode.Knowledge:
				break;
			case ProgVariableTypeCode.Role:
				break;
			case ProgVariableTypeCode.Ethnicity:
				break;
			case ProgVariableTypeCode.Drug:
				break;
			case ProgVariableTypeCode.WeatherEvent:
				break;
			case ProgVariableTypeCode.Shop:
				break;
			case ProgVariableTypeCode.Merchandise:
				break;
			case ProgVariableTypeCode.Outfit:
				break;
			case ProgVariableTypeCode.OutfitItem:
				break;
			case ProgVariableTypeCode.Project:
				break;
			case ProgVariableTypeCode.OverlayPackage:
				break;
			case ProgVariableTypeCode.Terrain:
				break;
			case ProgVariableTypeCode.Solid:
				break;
			case ProgVariableTypeCode.Liquid:
				break;
			case ProgVariableTypeCode.Gas:
				break;
			case ProgVariableTypeCode.Dictionary:
				break;
			case ProgVariableTypeCode.CollectionDictionary:
				break;
			case ProgVariableTypeCode.MagicSpell:
				break;
			case ProgVariableTypeCode.MagicSchool:
				break;
			case ProgVariableTypeCode.MagicCapability:
				break;
			case ProgVariableTypeCode.Bank:
				break;
			case ProgVariableTypeCode.BankAccount:
				break;
			case ProgVariableTypeCode.BankAccountType:
				break;
			case ProgVariableTypeCode.LegalAuthority:
				break;
			case ProgVariableTypeCode.Law:
				break;
			case ProgVariableTypeCode.Crime:
				break;
			case ProgVariableTypeCode.Market:
				break;
			case ProgVariableTypeCode.MarketCategory:
				break;
			case ProgVariableTypeCode.LiquidMixture:
				break;
			case ProgVariableTypeCode.Script:
				break;
			case ProgVariableTypeCode.Writing:
				break;
			case ProgVariableTypeCode.Area:
				break;
			case ProgVariableTypeCode.CollectionItem:
				break;
			case ProgVariableTypeCode.Perceivable:
				break;
			case ProgVariableTypeCode.Perceiver:
				break;
			case ProgVariableTypeCode.MagicResourceHaver:
				break;
			case ProgVariableTypeCode.ReferenceType:
				break;
			case ProgVariableTypeCode.ValueType:
				break;
			case ProgVariableTypeCode.Anything:
				break;
			case ProgVariableTypeCode.Toon:
				break;
			case ProgVariableTypeCode.Tagged:
				break;
			case ProgVariableTypeCode.Material:
				break;
		}
	}
}


