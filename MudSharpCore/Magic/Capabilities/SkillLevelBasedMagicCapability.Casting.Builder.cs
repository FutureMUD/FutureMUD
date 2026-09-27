using System.Globalization;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private const string CastingHelp = @"Casting options:
	#3casting enable on|off#0
	#3casting resources <source> <reserve> passive|gather#0
	#3casting trait <skill>#0
	#3casting entry add|remove <spell>#0
	#3casting entry trait <spell> <skill|default>#0
	#3casting entry starting <spell> on|off#0
	#3casting entry grades <spell> <min> <max>#0
	#3casting prerequisite add <spell> <prerequisite> <grade> <raw skill>#0
	#3casting prerequisite remove <spell> <prerequisite>#0
	#3casting validate|show#0
Removing admission preserves player knowledge. Starting grants require explicit staff enrolment.";

	private bool BuildingCommandCasting(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		if (action is "show" or "validate")
		{
			var sb = new StringBuilder(); AppendCastingShow(sb, actor);
			actor.OutputHandler.Send(sb.Length > 0 ? sb.ToString() : "No configured casting policy."); return false;
		}
		if (this is IVancianMagicCapability)
		{
			actor.OutputHandler.Send("Configured casting cannot be edited on a Vancian capability."); return false;
		}
		if (_unreadableCasting is not null)
		{
			actor.OutputHandler.Send(_castingLoadError!.ColourError()); return false;
		}
		var p = CastingPolicy ?? new MagicCastingPolicy(1, Guid.NewGuid(), false, ConcentrationTrait.Id,
			0, 0, false, 1, Array.Empty<MagicCastingAdmission>());
		try
		{
			switch (action)
			{
				case "enable": p = p with { Enabled = Toggle(command.PopSpeech()) }; break;
				case "trait":
					var trait = Gameworld.Traits.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such skill.");
					p = p with { DefaultTraitId = trait.Id }; break;
				case "resources":
					var source = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such source resource.");
					var reserve = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such reserve.");
					var mode = command.PopSpeech().ToLowerInvariant();
					if (mode is not ("passive" or "gather")) throw new FormatException("Specify passive or gather entitlement.");
					p = p with { SourceResourceId = source.Id, ReserveResourceId = reserve.Id, PassiveEntitlement = mode == "passive" }; break;
				case "entry":
				case "prerequisite":
					var edit = command.PopSpeech().ToLowerInvariant();
					var spell = Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such spell.");
					var entries = p.Admissions.ToList();
					var admission = entries.FirstOrDefault(x => x.SpellId == spell.Id);
					if (action == "entry" && edit == "add")
					{
						if (admission is not null) throw new FormatException("That spell is already admitted.");
						entries.Add(new(Guid.NewGuid(), spell.Id, null, false, 1, 7, Array.Empty<MagicCastingPrerequisite>()));
					}
					else
					{
						if (admission is null) throw new FormatException("That spell is not admitted.");
						var position = entries.IndexOf(admission);
						if (action == "prerequisite")
						{
							var required = Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such prerequisite spell.");
							var edges = admission.Prerequisites.ToList();
							if (edit == "remove") edges.RemoveAll(x => x.SpellId == required.Id);
							else if (edit == "add")
							{
								if (edges.Any(x => x.SpellId == required.Id)) throw new FormatException("Duplicate prerequisite.");
								edges.Add(new(Guid.NewGuid(), required.Id, int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture),
									double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture)));
							}
							else throw new FormatException("Use prerequisite add or remove.");
							entries[position] = admission with { Prerequisites = edges.AsReadOnly() };
						}
						else switch (edit)
						{
							case "remove": entries.RemoveAt(position); break;
							case "starting": entries[position] = admission with { Starting = Toggle(command.PopSpeech()) }; break;
							case "trait":
								var arg = command.PopSpeech();
								entries[position] = admission with { TraitId = arg.EqualTo("default") ? null :
									(Gameworld.Traits.GetByIdOrName(arg) ?? throw new FormatException("No such skill.")).Id }; break;
							case "grades": entries[position] = admission with { MinimumGrade = int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture),
								MaximumGrade = int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture) }; break;
							default: throw new FormatException("Unknown entry edit.");
						}
					}
					p = p with { Admissions = entries.AsReadOnly() }; break;
				default: actor.OutputHandler.Send(CastingHelp.SubstituteANSIColour()); return false;
			}
			if (!command.IsFinished) throw new FormatException("Unexpected trailing input.");
			CastingPolicy = p; Changed = true;
			(Gameworld.MagicCasting as MagicCastingService)?.DefinitionsChanged();
			actor.OutputHandler.Send("Casting policy updated. Invalid configuration disables its route until repaired.");
			foreach (var error in CastingConfigurationErrors()) actor.OutputHandler.Send(error.ColourError());
			return true;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			actor.OutputHandler.Send(ex.Message.ColourError()); return false;
		}
	}

	private static bool Toggle(string value) => value.ToLowerInvariant() switch
	{
		"on" => true, "off" => false, _ => throw new FormatException("Specify on or off.")
	};
}
