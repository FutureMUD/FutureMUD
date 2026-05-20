using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.Work.Agriculture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Work.Crafts.Inputs;

public class AgricultureFieldInput : BaseInput
{
	public override string InputType => "AgricultureField";
	public AgricultureFieldUse RequiredUse { get; private set; } = AgricultureFieldUse.Woodland;
	public long CropDefinitionId { get; private set; }
	public long WoodlandDefinitionId { get; private set; }
	public int MinimumFieldCondition { get; private set; }
	public int MinimumHealth { get; private set; }
	public int MinimumYield { get; private set; }
	public int YieldConsumed { get; private set; }

	protected AgricultureFieldInput(CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
	{
		var root = XElement.Parse(input.Definition);
		RequiredUse = Enum.TryParse<AgricultureFieldUse>(root.Element("RequiredUse")?.Value, true, out var use)
			? use
			: AgricultureFieldUse.Woodland;
		CropDefinitionId = long.Parse(root.Element("CropDefinitionId")?.Value ?? "0");
		WoodlandDefinitionId = long.Parse(root.Element("WoodlandDefinitionId")?.Value ?? "0");
		MinimumFieldCondition = (int.Parse(root.Element("MinimumFieldCondition")?.Value ?? "0")).ClampScore();
		MinimumHealth = (int.Parse(root.Element("MinimumHealth")?.Value ?? "0")).ClampScore();
		MinimumYield = (int.Parse(root.Element("MinimumYield")?.Value ?? "0")).ClampScore();
		YieldConsumed = (int.Parse(root.Element("YieldConsumed")?.Value ?? "0")).ClampScore();
	}

	protected AgricultureFieldInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("AgricultureField",
			(input, craft, game) => new AgricultureFieldInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("field",
			(craft, game) => new AgricultureFieldInput(craft, game));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("RequiredUse", RequiredUse.ToString()),
			new XElement("CropDefinitionId", CropDefinitionId),
			new XElement("WoodlandDefinitionId", WoodlandDefinitionId),
			new XElement("MinimumFieldCondition", MinimumFieldCondition),
			new XElement("MinimumHealth", MinimumHealth),
			new XElement("MinimumYield", MinimumYield),
			new XElement("YieldConsumed", YieldConsumed)
		).ToString();
	}

	public override bool IsValid()
	{
		return RequiredUse switch
		{
			AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard => CropDefinitionId == 0 || Gameworld.AgricultureCropDefinitions.Get(CropDefinitionId) != null,
			AgricultureFieldUse.Woodland => WoodlandDefinitionId == 0 || Gameworld.AgricultureWoodlandDefinitions.Get(WoodlandDefinitionId) != null,
			_ => true
		};
	}

	public override string WhyNotValid()
	{
		return RequiredUse switch
		{
			AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard => "The selected crop definition no longer exists.",
			AgricultureFieldUse.Woodland => "The selected woodland definition no longer exists.",
			_ => "Unknown agriculture field input validation failure."
		};
	}

	public override string Name => $"a local {RequiredUse.DescribeEnum().ToLowerInvariant()} agriculture field";

	public override string HowSeen(IPerceiver voyeur)
	{
		var use = RequiredUse.DescribeEnum().ToLowerInvariant();
		var target = RequiredUse switch
		{
			AgricultureFieldUse.Crop when CropDefinitionId != 0 => $" growing {Gameworld.AgricultureCropDefinitions.Get(CropDefinitionId)?.Name.ColourName() ?? "a missing crop".ColourError()}",
			AgricultureFieldUse.Orchard when CropDefinitionId != 0 => $" planted as {Gameworld.AgricultureCropDefinitions.Get(CropDefinitionId)?.Name.ColourName() ?? "a missing crop".ColourError()}",
			AgricultureFieldUse.Woodland when WoodlandDefinitionId != 0 => $" planted with {Gameworld.AgricultureWoodlandDefinitions.Get(WoodlandDefinitionId)?.Name.ColourName() ?? "a missing woodland".ColourError()}",
			_ => string.Empty
		};
		var requirements = new List<string>();
		if (MinimumFieldCondition > 0)
		{
			requirements.Add($"field condition >= {MinimumFieldCondition.ToString("N0", voyeur).ColourValue()}");
		}

		if (MinimumHealth > 0)
		{
			requirements.Add($"health >= {MinimumHealth.ToString("N0", voyeur).ColourValue()}");
		}

		if (MinimumYield > 0)
		{
			requirements.Add($"yield >= {MinimumYield.ToString("N0", voyeur).ColourValue()}");
		}

		if (YieldConsumed > 0)
		{
			requirements.Add($"consumes {YieldConsumed.ToString("N0", voyeur).ColourValue()} yield");
		}

		return $"a local {use} agriculture field{target}{(requirements.Count == 0 ? string.Empty : $" ({requirements.ListToString()})")}";
	}

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		var location = character.Location;
		var field = location?.AgricultureField;
		return location != null && field != null && FieldMatches(field)
			? [location]
			: Enumerable.Empty<IPerceivable>();
	}

	public override bool IsInput(IPerceivable item)
	{
		return item is ICell cell && cell.AgricultureField != null && FieldMatches(cell.AgricultureField);
	}

	public override void UseInput(IPerceivable item, ICraftInputData data)
	{
	}

	public override double ScoreInputDesirability(IPerceivable item)
	{
		return item is ICell cell && cell.AgricultureField is { } field
			? FieldYield(field) + FieldHealth(field) + field.Condition
			: 0.0;
	}

	public override ICraftInputData ReserveInput(IPerceivable input)
	{
		var field = ((ICell)input).AgricultureField;
		var quality = QualityForField(field);
		if (YieldConsumed > 0)
		{
			string reason;
			var success = RequiredUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard
				? field.ConsumeCropYield(YieldConsumed, out reason)
				: field.ConsumeWoodlandYield(YieldConsumed, out reason);
			if (!success)
			{
				throw new ApplicationException($"Agriculture field craft input could not consume yield: {reason}");
			}
		}

		return new AgricultureFieldInputData(field, quality, YieldConsumed);
	}

	public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
	{
		return new AgricultureFieldInputData(root, gameworld);
	}

	private bool FieldMatches(IAgricultureField field)
	{
		if (field.CurrentUse != RequiredUse || field.Condition < MinimumFieldCondition)
		{
			return false;
		}

		if (RequiredUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard)
		{
			return field.CurrentCrop != null &&
			       (CropDefinitionId == 0 || field.CurrentCrop.Id == CropDefinitionId) &&
			       field.CropHealth >= MinimumHealth &&
			       field.CropYieldPotential >= Math.Max(MinimumYield, YieldConsumed);
		}

		if (RequiredUse == AgricultureFieldUse.Woodland)
		{
			return field.CurrentWoodland != null &&
			       (WoodlandDefinitionId == 0 || field.CurrentWoodland.Id == WoodlandDefinitionId) &&
			       field.WoodlandHealth >= MinimumHealth &&
			       field.WoodlandYieldPotential >= Math.Max(MinimumYield, YieldConsumed);
		}

		return MinimumHealth == 0 && MinimumYield == 0 && YieldConsumed == 0;
	}

	private static int FieldHealth(IAgricultureField field)
	{
		return field.CurrentUse switch
		{
			AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard => field.CropHealth,
			AgricultureFieldUse.Woodland => field.WoodlandHealth,
			_ => field.Condition
		};
	}

	private static int FieldYield(IAgricultureField field)
	{
		return field.CurrentUse switch
		{
			AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard => field.CropYieldPotential,
			AgricultureFieldUse.Woodland => field.WoodlandYieldPotential,
			_ => 0
		};
	}

	private static ItemQuality QualityForField(IAgricultureField field)
	{
		var score = (FieldHealth(field) + FieldYield(field) + field.Condition) / 3;
		return (ItemQuality)Math.Clamp(score / 10, (int)ItemQuality.Terrible, (int)ItemQuality.Legendary);
	}

	protected override string BuildingHelpString =>
		@"You can use the following options with this input type:

	#3quality <weighting>#0 - sets the weighting of this input in determining overall quality
	#3use <fallow|crop|pasture|woodland|orchard>#0 - sets the required local field use
	#3crop <which>|none#0 - requires a specific crop definition
	#3woodland <which>|none#0 - requires a specific woodland definition
	#3condition <0-100>#0 - sets the minimum field condition
	#3health <0-100>#0 - sets the minimum crop or woodland health
	#3yield <0-100>#0 - sets the minimum crop or woodland yield
	#3consume <0-100>#0 - consumes crop or woodland yield when the input is reserved";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "use":
				return BuildingCommandUse(actor, command);
			case "crop":
				return BuildingCommandCrop(actor, command);
			case "woodland":
				return BuildingCommandWoodland(actor, command);
			case "condition":
				return BuildingCommandScore(actor, command, value => MinimumFieldCondition = value, "minimum field condition");
			case "health":
				return BuildingCommandScore(actor, command, value => MinimumHealth = value, "minimum crop or woodland health");
			case "yield":
				return BuildingCommandScore(actor, command, value => MinimumYield = value, "minimum crop or woodland yield");
			case "consume":
			case "consumeyield":
			case "yieldcost":
				return BuildingCommandScore(actor, command, value => YieldConsumed = value, "yield consumed");
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandUse(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Enum.TryParse<AgricultureFieldUse>(command.PopSpeech(), true, out var use))
		{
			actor.OutputHandler.Send($"Valid field uses are {Enum.GetValues(typeof(AgricultureFieldUse)).OfType<AgricultureFieldUse>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		RequiredUse = use;
		InputChanged = true;
		actor.OutputHandler.Send($"This input now requires a local field with {RequiredUse.DescribeEnum().ColourName()} use.");
		return true;
	}

	private bool BuildingCommandCrop(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which crop definition should this input require, or none?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CropDefinitionId = 0;
			InputChanged = true;
			actor.OutputHandler.Send("This input will no longer require a specific crop definition.");
			return true;
		}

		var crop = Gameworld.AgricultureCropDefinitions.GetByIdOrName(command.SafeRemainingArgument);
		if (crop == null)
		{
			actor.OutputHandler.Send("There is no such crop definition.");
			return false;
		}

		CropDefinitionId = crop.Id;
		RequiredUse = crop.IsPerennial ? AgricultureFieldUse.Orchard : AgricultureFieldUse.Crop;
		InputChanged = true;
		actor.OutputHandler.Send($"This input now requires a local field growing {crop.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandWoodland(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which woodland definition should this input require, or none?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			WoodlandDefinitionId = 0;
			InputChanged = true;
			actor.OutputHandler.Send("This input will no longer require a specific woodland definition.");
			return true;
		}

		var woodland = Gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(command.SafeRemainingArgument);
		if (woodland == null)
		{
			actor.OutputHandler.Send("There is no such woodland definition.");
			return false;
		}

		WoodlandDefinitionId = woodland.Id;
		RequiredUse = AgricultureFieldUse.Woodland;
		InputChanged = true;
		actor.OutputHandler.Send($"This input now requires a local field planted with {woodland.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandScore(ICharacter actor, StringStack command, Action<int> setter, string description)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send($"What 0-100 value do you want to set for {description}?");
			return false;
		}

		value = value.ClampScore();
		setter(value);
		InputChanged = true;
		actor.OutputHandler.Send($"This input now has {description} set to {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	internal class AgricultureFieldInputData : ICraftInputData
	{
		public AgricultureFieldInputData(XElement root, IFuturemud gameworld)
		{
			FieldId = long.Parse(root.Element("FieldId")?.Value ?? "0");
			ConsumedYield = int.Parse(root.Element("ConsumedYield")?.Value ?? "0");
			InputQuality = (ItemQuality)int.Parse(root.Element("Quality")?.Value ?? ((int)ItemQuality.Standard).ToString());
			Perceivable = gameworld.AgricultureFields.Get(FieldId)?.Cell is IPerceivable perceivable
				? perceivable
				: new DummyPerceivable("a missing agriculture field", "a missing agriculture field");
		}

		public AgricultureFieldInputData(IAgricultureField field, ItemQuality quality, int consumedYield)
		{
			FieldId = field.Id;
			Perceivable = field.Cell;
			InputQuality = quality;
			ConsumedYield = consumedYield;
		}

		public long FieldId { get; }
		public int ConsumedYield { get; }
		public ItemQuality InputQuality { get; }
		public IPerceivable Perceivable { get; }

		public XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("FieldId", FieldId),
				new XElement("Quality", (int)InputQuality),
				new XElement("ConsumedYield", ConsumedYield)
			);
		}

		public void FinaliseLoadTimeTasks()
		{
		}

		public void Delete()
		{
		}

		public void Quit()
		{
		}
	}
}
