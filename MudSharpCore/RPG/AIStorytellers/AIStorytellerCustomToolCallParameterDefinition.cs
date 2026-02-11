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
		switch (type)
		{
			case ProgVariableTypes.Text:
				Type = "string";
				Description = description;
				break;
			case ProgVariableTypes.Number:
				Type = "double";
				Description = description;
				break;
			case ProgVariableTypes.Boolean:
				Type = "boolean";
				Description = description;
				break;
			case ProgVariableTypes.Character:
				Type = "int64";
				Description = $"The ID number of a character. {description}";
				break;
			case ProgVariableTypes.Location:
				Type = "int64";
				Description = $"The ID number of a room location. {description}";
				break;
			case ProgVariableTypes.Item:
				break;
			case ProgVariableTypes.Shard:
				break;
			case ProgVariableTypes.Error:
				break;
			case ProgVariableTypes.Gender:
				break;
			case ProgVariableTypes.Zone:
				break;
			case ProgVariableTypes.Collection:
				break;
			case ProgVariableTypes.Race:
				break;
			case ProgVariableTypes.Culture:
				break;
			case ProgVariableTypes.Chargen:
				break;
			case ProgVariableTypes.Trait:
				break;
			case ProgVariableTypes.Clan:
				break;
			case ProgVariableTypes.ClanRank:
				break;
			case ProgVariableTypes.ClanAppointment:
				break;
			case ProgVariableTypes.ClanPaygrade:
				break;
			case ProgVariableTypes.Currency:
				break;
			case ProgVariableTypes.Exit:
				break;
			case ProgVariableTypes.Literal:
				break;
			case ProgVariableTypes.DateTime:
				break;
			case ProgVariableTypes.TimeSpan:
				break;
			case ProgVariableTypes.Language:
				break;
			case ProgVariableTypes.Accent:
				break;
			case ProgVariableTypes.Merit:
				break;
			case ProgVariableTypes.MudDateTime:
				break;
			case ProgVariableTypes.Calendar:
				break;
			case ProgVariableTypes.Clock:
				break;
			case ProgVariableTypes.Effect:
				break;
			case ProgVariableTypes.Knowledge:
				break;
			case ProgVariableTypes.Role:
				break;
			case ProgVariableTypes.Ethnicity:
				break;
			case ProgVariableTypes.Drug:
				break;
			case ProgVariableTypes.WeatherEvent:
				break;
			case ProgVariableTypes.Shop:
				break;
			case ProgVariableTypes.Merchandise:
				break;
			case ProgVariableTypes.Outfit:
				break;
			case ProgVariableTypes.OutfitItem:
				break;
			case ProgVariableTypes.Project:
				break;
			case ProgVariableTypes.OverlayPackage:
				break;
			case ProgVariableTypes.Terrain:
				break;
			case ProgVariableTypes.Solid:
				break;
			case ProgVariableTypes.Liquid:
				break;
			case ProgVariableTypes.Gas:
				break;
			case ProgVariableTypes.Dictionary:
				break;
			case ProgVariableTypes.CollectionDictionary:
				break;
			case ProgVariableTypes.MagicSpell:
				break;
			case ProgVariableTypes.MagicSchool:
				break;
			case ProgVariableTypes.MagicCapability:
				break;
			case ProgVariableTypes.Bank:
				break;
			case ProgVariableTypes.BankAccount:
				break;
			case ProgVariableTypes.BankAccountType:
				break;
			case ProgVariableTypes.LegalAuthority:
				break;
			case ProgVariableTypes.Law:
				break;
			case ProgVariableTypes.Crime:
				break;
			case ProgVariableTypes.Market:
				break;
			case ProgVariableTypes.MarketCategory:
				break;
			case ProgVariableTypes.LiquidMixture:
				break;
			case ProgVariableTypes.Script:
				break;
			case ProgVariableTypes.Writing:
				break;
			case ProgVariableTypes.Area:
				break;
			case ProgVariableTypes.CollectionItem:
				break;
			case ProgVariableTypes.Perceivable:
				break;
			case ProgVariableTypes.Perceiver:
				break;
			case ProgVariableTypes.MagicResourceHaver:
				break;
			case ProgVariableTypes.ReferenceType:
				break;
			case ProgVariableTypes.ValueType:
				break;
			case ProgVariableTypes.Anything:
				break;
			case ProgVariableTypes.Toon:
				break;
			case ProgVariableTypes.Tagged:
				break;
			case ProgVariableTypes.Material:
				break;
		}
	}
}

