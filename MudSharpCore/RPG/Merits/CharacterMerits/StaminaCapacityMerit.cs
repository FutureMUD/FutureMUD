#nullable enable

using System;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public sealed class StaminaCapacityMerit : CharacterMeritBase, IStaminaCapacityMerit
{
	public const string MeritTypeName = "Stamina Capacity";
	public double StaminaBonus { get; private set; }
	public double StaminaMultiplier { get; private set; } = 1.0;

	private StaminaCapacityMerit() { }

	private StaminaCapacityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		StaminaBonus = ReadValue(definition, "Bonus", 0.0, merit.Id);
		StaminaMultiplier = ReadValue(definition, "Multiplier", 1.0, merit.Id);
		if (StaminaMultiplier < 0.0)
			throw new InvalidOperationException($"Stamina capacity merit #{merit.Id} requires a nonnegative multiplier.");
	}

	private StaminaCapacityMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, MeritTypeName, "@ have|has an altered stamina capacity")
	{
		MeritScope = MeritScope.Body;
		DoDatabaseInsert();
	}

	private static double ReadValue(XElement definition, string field, double fallback, long meritId)
	{
		var text = (string?)definition.Element(field);
		if (text is null) return fallback;
		if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
			throw new InvalidOperationException($"Stamina capacity merit #{meritId} requires a finite {field.ToLowerInvariant()}.");
		return value;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser(MeritTypeName, (merit, world) => new StaminaCapacityMerit(merit, world));
		MeritFactory.RegisterBuilderMeritInitialiser(MeritTypeName, (world, name) => new StaminaCapacityMerit(world, name));
		MeritFactory.RegisterMeritHelp(MeritTypeName, "Changes maximum stamina independently of recovery", new StaminaCapacityMerit().HelpText);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Bonus", StaminaBonus.ToString("R", CultureInfo.InvariantCulture)),
			new XElement("Multiplier", StaminaMultiplier.ToString("R", CultureInfo.InvariantCulture)));
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Maximum Stamina Addition: {StaminaBonus.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Maximum Stamina Multiplier: {StaminaMultiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine("Additions are applied before multipliers. This merit does not change recovery or refill stamina.");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <amount>#0 - sets the finite addition to maximum stamina
	#3multiplier <percentage>#0 - sets the nonnegative maximum stamina multiplier";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				if (!double.TryParse(command.SafeRemainingArgument, NumberStyles.Float, actor.Account.Culture, out var bonus) ||
					!double.IsFinite(bonus))
				{
					actor.OutputHandler.Send("Enter a finite stamina addition; negative additions are permitted.");
					return false;
				}
				StaminaBonus = bonus;
				Changed = true;
				actor.OutputHandler.Send($"The addition to maximum stamina is now {bonus.ToString("N2", actor).ColourValue()}.");
				return true;
			case "multiplier":
				if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var multiplier) ||
					!double.IsFinite(multiplier) || multiplier < 0.0)
				{
					actor.OutputHandler.Send("Enter a finite nonnegative percentage for maximum stamina.");
					return false;
				}
				StaminaMultiplier = multiplier;
				Changed = true;
				actor.OutputHandler.Send($"The maximum stamina multiplier is now {multiplier.ToString("P2", actor).ColourValue()}.");
				return true;
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}
}
