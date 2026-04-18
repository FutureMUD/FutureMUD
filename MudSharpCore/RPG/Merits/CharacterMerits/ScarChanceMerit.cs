#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ScarChanceMerit : CharacterMeritBase, IScarChanceMerit
{
	private readonly HashSet<DamageType> _damageTypes = [];

	protected ScarChanceMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		AppliesToWounds = bool.Parse(root.Attribute("wounds")?.Value ?? "false");
		AppliesToSurgery = bool.Parse(root.Attribute("surgery")?.Value ?? "false");
		FlatModifier = double.Parse(root.Attribute("flat")?.Value ?? "0.0", System.Globalization.CultureInfo.InvariantCulture);
		Multiplier = double.Parse(root.Attribute("multiplier")?.Value ?? "1.0", System.Globalization.CultureInfo.InvariantCulture);
		foreach (var item in root.Element("DamageTypes")?.Elements("DamageType") ?? Enumerable.Empty<XElement>())
		{
			_damageTypes.Add((DamageType)int.Parse(item.Value, System.Globalization.CultureInfo.InvariantCulture));
		}
	}

	protected ScarChanceMerit() { }

	protected ScarChanceMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, "Scar Chance", "@ have|has an altered chance to receive scars")
	{
		AppliesToWounds = false;
		AppliesToSurgery = false;
		FlatModifier = 0.0;
		Multiplier = 1.0;
		DoDatabaseInsert();
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Scar Chance", (merit, gameworld) => new ScarChanceMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Scar Chance", (gameworld, name) => new ScarChanceMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Scar Chance", "Modifies the chance of a character receiving scars", new ScarChanceMerit().HelpText);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("wounds", AppliesToWounds));
		root.Add(new XAttribute("surgery", AppliesToSurgery));
		root.Add(new XAttribute("flat", FlatModifier));
		root.Add(new XAttribute("multiplier", Multiplier));
		root.Add(new XElement("DamageTypes",
			from item in _damageTypes
			select new XElement("DamageType", (int)item)
		));
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Applies To Wounds: {AppliesToWounds.ToColouredString()}");
		sb.AppendLine($"Applies To Surgery: {AppliesToSurgery.ToColouredString()}");
		sb.AppendLine($"Wound Damage Types: {(_damageTypes.Count == 0 ? "None".ColourError() : _damageTypes.Select(x => x.DescribeEnum().ColourValue()).ListToString())}");
		sb.AppendLine($"Flat Modifier: {FlatModifier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3wounds#0 - toggles this merit applying to ordinary wounds
	#3surgery#0 - toggles this merit applying to surgery scars
	#3damage <type>#0 - toggles a wound damage type this merit applies to
	#3damage all#0 - makes this merit apply to all wound damage types
	#3damage none#0 - clears all wound damage types
	#3flat <%>#0 - sets a flat modifier to scar chance
	#3multiplier <%>#0 - sets a multiplier to scar chance";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "wounds":
			case "wound":
				return BuildingCommandWounds(actor);
			case "surgery":
			case "surgeries":
				return BuildingCommandSurgery(actor);
			case "damage":
			case "damagetype":
				return BuildingCommandDamage(actor, command);
			case "flat":
			case "add":
			case "modifier":
				return BuildingCommandFlat(actor, command);
			case "multiplier":
			case "mult":
				return BuildingCommandMultiplier(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	public bool AppliesToWounds { get; private set; }
	public bool AppliesToSurgery { get; private set; }
	public IEnumerable<DamageType> DamageTypes => _damageTypes;
	public double FlatModifier { get; private set; }
	public double Multiplier { get; private set; }

	public bool AppliesTo(IWound wound)
	{
		var context = ScarGenerationSupport.GetContext(wound);
		if (context.IsSurgery)
		{
			return AppliesToSurgery;
		}

		return AppliesToWounds && _damageTypes.Contains(context.DamageType);
	}

	private bool BuildingCommandWounds(ICharacter actor)
	{
		AppliesToWounds = !AppliesToWounds;
		Changed = true;
		actor.OutputHandler.Send($"This merit will {(AppliesToWounds ? "now" : "no longer")} apply to ordinary wound scars.");
		return true;
	}

	private bool BuildingCommandSurgery(ICharacter actor)
	{
		AppliesToSurgery = !AppliesToSurgery;
		Changed = true;
		actor.OutputHandler.Send($"This merit will {(AppliesToSurgery ? "now" : "no longer")} apply to surgery scars.");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which damage type should this merit apply to?\nValid options are {Enum.GetValues<DamageType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}, plus {"all".ColourCommand()} and {"none".ColourCommand()}.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("all"))
		{
			_damageTypes.Clear();
			foreach (var item in Enum.GetValues<DamageType>())
			{
				_damageTypes.Add(item);
			}

			Changed = true;
			actor.OutputHandler.Send("This merit now applies to all wound damage types.");
			return true;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_damageTypes.Clear();
			Changed = true;
			actor.OutputHandler.Send("This merit no longer applies to any wound damage types until you opt specific ones in.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out DamageType damageType))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid damage type.\nValid options are {Enum.GetValues<DamageType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		Changed = true;
		if (_damageTypes.Remove(damageType))
		{
			actor.OutputHandler.Send($"This merit will no longer apply to {damageType.DescribeEnum().ColourValue()} wound scars.");
			return true;
		}

		_damageTypes.Add(damageType);
		actor.OutputHandler.Send($"This merit will now apply to {damageType.DescribeEnum().ColourValue()} wound scars.");
		return true;
	}

	private bool BuildingCommandFlat(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What flat percentage modifier should be added to scar chance?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		FlatModifier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now add {FlatModifier.ToString("P2", actor).ColourValue()} to scar chance when it applies.");
		return true;
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier should be applied to scar chance?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (value < 0.0)
		{
			actor.OutputHandler.Send("The multiplier cannot be negative.");
			return false;
		}

		Multiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply scar chance by {Multiplier.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}
}
