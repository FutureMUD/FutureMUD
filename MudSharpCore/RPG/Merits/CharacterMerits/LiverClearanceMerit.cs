#nullable enable

using System;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.RPG.Merits.CharacterMerits;

public sealed class LiverClearanceMerit : CharacterMeritBase, ILiverClearanceMerit
{
	public const string MeritTypeName = "Liver Clearance";
	public double ClearanceMultiplier { get; private set; } = 1.0;
	private LiverClearanceMerit() { }
	private LiverClearanceMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var value = (string?)XElement.Parse(merit.Definition).Attribute("multiplier");
		if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var multiplier) ||
			!double.IsFinite(multiplier) || multiplier <= 0)
			throw new InvalidOperationException($"Liver clearance merit #{merit.Id} requires a finite positive multiplier.");
		ClearanceMultiplier = multiplier;
	}
	private LiverClearanceMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, MeritTypeName, "@ have|has an altered liver clearance rate") => DoDatabaseInsert();

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser(MeritTypeName, (merit, world) => new LiverClearanceMerit(merit, world));
		MeritFactory.RegisterBuilderMeritInitialiser(MeritTypeName, (world, name) => new LiverClearanceMerit(world, name));
		MeritFactory.RegisterMeritHelp(MeritTypeName, "Changes effective liver clearance of alcohol and active drugs", new LiverClearanceMerit().HelpText);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("multiplier", ClearanceMultiplier.ToString("R", CultureInfo.InvariantCulture)));
		return root;
	}
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb) =>
		sb.AppendLine($"Effective Liver Clearance: {ClearanceMultiplier.ToString("P2", actor).ColourValue()} (after organ function; alcohol and all active drugs)");
	protected override string SubtypeHelp => $"{base.SubtypeHelp}\n\t#3clearance <percentage>#0 - sets effective liver clearance after injury scaling";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() != "clearance") return base.BuildingCommand(actor, command.GetUndo());
		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var multiplier) ||
			!double.IsFinite(multiplier) || multiplier <= 0)
		{
			actor.OutputHandler.Send("Enter a finite positive percentage for effective liver clearance.");
			return false;
		}
		ClearanceMultiplier = multiplier;
		Changed = true;
		actor.OutputHandler.Send($"Effective liver clearance is now {multiplier.ToString("P2", actor).ColourValue()}. This also speeds clearance of beneficial active drugs.");
		return true;
	}
}
