#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.RPG.Merits.CharacterMerits;

/// <summary>Builder-assigned descriptive metadata. It grants no skills, resources or magic capability.</summary>
public sealed class ElementalAffinityMerit : CharacterMeritBase, IElementalAffinityMerit
{
	private readonly List<string> _affinities = new();
	public IReadOnlyList<string> ElementalAffinities => _affinities.AsReadOnly();

	public ElementalAffinityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		foreach (var element in XElement.Parse(merit.Definition).Element("ElementalAffinities")?.Elements("Affinity") ?? [])
		{
			var label = element.Value.Trim();
			if (!ValidLabel(label)) throw new FormatException("An elemental affinity must contain 1-64 printable characters.");
			if (!_affinities.Contains(label, StringComparer.OrdinalIgnoreCase)) _affinities.Add(label);
		}
	}

	private ElementalAffinityMerit() { }
	private ElementalAffinityMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, "Elemental Affinity", "@ have|has a declared elemental affinity") => DoDatabaseInsert();

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Elemental Affinity", (merit, world) => new ElementalAffinityMerit(merit, world));
		MeritFactory.RegisterBuilderMeritInitialiser("Elemental Affinity", (world, name) => new ElementalAffinityMerit(world, name));
		MeritFactory.RegisterMeritHelp("Elemental Affinity", "Declares descriptive elemental affinities separately from learned magic",
			new ElementalAffinityMerit().HelpText);
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("ElementalAffinities", _affinities.Select(x => new XElement("Affinity", x))));
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Declared Elemental Affinities: {(_affinities.Count == 0 ? "None" : _affinities.ListToString()).ColourValue()}");
		sb.AppendLine("These labels describe assignment; they do not depend on the applicability prog or grant magic.");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3affinity <label>#0 - toggles a declared affinity, independently of the applicability prog
	#3affinity clear#0 - removes all declared affinities";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() != "affinity") return base.BuildingCommand(actor, command.GetUndo());
		var label = command.SafeRemainingArgument.Trim();
		if (!ValidLabel(label))
		{
			actor.OutputHandler.Send("Specify an affinity of 1-64 printable characters, or clear.");
			return false;
		}
		if (label.EqualTo("clear")) _affinities.Clear();
		else
		{
			var existing = _affinities.FindIndex(x => x.EqualTo(label));
			if (existing >= 0) _affinities.RemoveAt(existing);
			else _affinities.Add(label);
		}
		Changed = true;
		actor.OutputHandler.Send($"Declared affinities: {(_affinities.Count == 0 ? "None" : _affinities.ListToString()).ColourValue()}.");
		return true;
	}

	private static bool ValidLabel(string label) => label.Length is > 0 and <= 64 && !label.Any(char.IsControl);
}
