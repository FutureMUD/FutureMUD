using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Commands.Trees;
using MudSharp.Framework.Units;

namespace MudSharp.Combat;

public class EnvenomingAttack : WeaponAttack, IEnvenomingAttack
{
	public EnvenomingAttack(Models.WeaponAttack attack, IFuturemud gameworld) : base(attack, gameworld)
	{
	}

	public EnvenomingAttack(IFuturemud gameworld, BuiltInCombatMoveType type) : base(gameworld, type)
	{
		Liquid = Gameworld.Liquids.First();
		MinimumWoundSeverity = WoundSeverity.Moderate;
		MaximumQuantity = 0.0;
	}

	public ILiquid Liquid { get; set; }
	public double MaximumQuantity { get; set; }
	public WoundSeverity MinimumWoundSeverity { get; set; }

	protected override void LoadFromDatabase(Models.WeaponAttack attack)
	{
		base.LoadFromDatabase(attack);
		var root = XElement.Parse(attack.AdditionalInfo);
		Liquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid").Value));
		MaximumQuantity = double.Parse(root.Element("MaximumQuantity").Value);
		MinimumWoundSeverity = (WoundSeverity)int.Parse(root.Element("MinimumWoundSeverity").Value);
	}

	public string GetAdditionalInfo =>
		new XElement("Data",
				new XElement("Liquid", Liquid.Id),
				new XElement("MaximumQuantity", MaximumQuantity),
				new XElement("MinimumWoundSeverity", (int)MinimumWoundSeverity)
			)
			.ToString();

	protected override void SeedInitialData(Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = new XElement("Data",
				new XElement("Liquid", Gameworld.Liquids.First().Id),
				new XElement("MaximumQuantity", 0.0),
				new XElement("MinimumWoundSeverity", (int)WoundSeverity.Moderate)
			)
			.ToString();
	}

	protected override void AddAttackSpecificCloneData(Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = GetAdditionalInfo;
	}

	protected override void SaveAttackSpecificData(Models.WeaponAttack attack)
	{
		attack.AdditionalInfo = GetAdditionalInfo;
	}

	protected override string ShowBuilderInternal(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Liquid: {Liquid.Name.Colour(Liquid.DisplayColour)}");
		sb.AppendLine(
			$"Maximum Amount: {Gameworld.UnitManager.DescribeMostSignificantExact(MaximumQuantity, UnitType.FluidVolume, actor).ColourValue()}");
		sb.AppendLine($"Minimum Severity: {MinimumWoundSeverity.Describe().ColourValue()}");
		return sb.ToString();
	}

	#region Overrides of WeaponAttack

	protected override void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
	{
		sb.Append(" Venom: >=");
		sb.Append(MinimumWoundSeverity.Describe().ColourValue());
		sb.Append(" = ");
		sb.AppendLine(Gameworld.UnitManager
		                       .DescribeMostSignificantExact(MaximumQuantity, UnitType.FluidVolume, actor)
		                       .ColourValue());
		sb.Append(' ');
		sb.Append(Liquid.Name.Colour(Liquid.DisplayColour));
	}

	#endregion

	public override string HelpText => $@"{base.HelpText}
	#3liquid <which>#0 - sets the liquid to inject with this attack
	#3amount <amount>#0 - sets the maximum amount injected (on a major pass)
	#3wound <severity>#0 - sets the minimum wound severity required to inject any of the liquid";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "liquid":
				return BuildingCommandLiquid(actor, command);
			case "amount":
				return BuildingCommandAmount(actor, command);
			case "wound":
				return BuildingCommandWound(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandWound(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the minimum wound severity caused to inject venom?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<WoundSeverity>(out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid wound severity. The valid values are {Enum.GetValues<WoundSeverity>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		MinimumWoundSeverity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon attack will now require a wound of severity {value.DescribeEnum().ColourValue()} or more to inject venom.");
		return true;
	}

	private bool BuildingCommandAmount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the maximum amount of venom delivered with a bite (inflicted on a major pass)?");
			return false;
		}

		var amount =
			Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid liquid volume.");
			return false;
		}

		MaximumQuantity = amount;
		Changed = true;
		actor.OutputHandler.Send(
			$"This weapon attack will now inject {Gameworld.UnitManager.DescribeMostSignificantExact(MaximumQuantity, UnitType.FluidVolume, actor).ColourValue()} of venom at a maximum.");
		return true;
	}

	private bool BuildingCommandLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which liquid would you like this attack to inject? See {"show liquids".ColourCommand()} for a list of liquids.");
			return false;
		}

		var liquid = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByName(command.SafeRemainingArgument);
		if (liquid == null)
		{
			actor.OutputHandler.Send(
				$"There is no such liquid. See {"show liquids".ColourCommand()} for a list of liquids.");
			return false;
		}

		Liquid = liquid;
		Changed = true;
		actor.OutputHandler.Send($"This attack will now apply the {Liquid.Name.Colour(Liquid.DisplayColour)} liquid.");
		return true;
	}
}