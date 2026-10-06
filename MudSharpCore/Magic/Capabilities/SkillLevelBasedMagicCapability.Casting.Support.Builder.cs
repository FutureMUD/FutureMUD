using System.Globalization;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Magic.Capabilities;

public partial class SkillLevelBasedMagicCapability
{
	private bool BuildingCommandCastingSupport(ICharacter actor, StringStack command, string action, MagicCastingPolicy p)
	{
		try
		{
			var edit = command.PopSpeech().ToLowerInvariant();
			long Skill() => (Gameworld.Traits.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such native skill.")).Id;
			long Spell() => (Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech()) ?? throw new FormatException("No such spell.")).Id;
			double Number() => double.Parse(command.PopSpeech(), CultureInfo.InvariantCulture);
			MagicCastingPrerequisite TraitEdge(long trait) => new(Guid.NewGuid(), 0, 0, Number(), MagicCastingPrerequisiteKind.SupportTrait, trait);
			if (action == "prerequisite")
			{
				var spellId = Spell(); var traitId = Skill();
				var admissions = p.Admissions.ToList(); var index = admissions.FindIndex(x => x.SpellId == spellId);
				if (index < 0) throw new FormatException("Spell is not admitted.");
				var edges = admissions[index].Prerequisites.Where(x => x.Kind != MagicCastingPrerequisiteKind.SupportTrait || x.TraitId != traitId).ToList();
				if (edit == "trait") edges.Add(TraitEdge(traitId));
				admissions[index] = admissions[index] with { Prerequisites = edges.AsReadOnly() };
				p = p with { Admissions = admissions.AsReadOnly() };
			}
			else
			{
				var supports = p.Supports.ToList();
				if (edit == "prerequisite")
				{
					var kind = command.PopSpeech().ToLowerInvariant();
					var removing = kind == "remove";
					if (removing) kind = command.PopSpeech().ToLowerInvariant();
					if (kind is not ("spell" or "trait")) throw new FormatException("Use spell or trait.");
					var traitId = Skill(); var index = supports.FindIndex(x => x.TraitId == traitId);
					if (index < 0) throw new FormatException("Support skill is not configured.");
					var sourceId = kind == "spell" ? Spell() : Skill();
					var edgeKind = kind == "spell" ? MagicCastingPrerequisiteKind.Spell : MagicCastingPrerequisiteKind.SupportTrait;
					var edges = supports[index].Prerequisites.Where(x => x.Kind != edgeKind || x.SourceId != sourceId).ToList();
					if (!removing) edges.Add(kind == "spell" ? new(Guid.NewGuid(), sourceId, int.Parse(command.PopSpeech(), CultureInfo.InvariantCulture), Number()) : TraitEdge(sourceId));
					supports[index] = supports[index] with { Prerequisites = edges.AsReadOnly() };
				}
				else
				{
					var traitId = Skill();
					if (edit == "remove") supports.RemoveAll(x => x.TraitId == traitId);
					else if (edit == "add")
					{
						if (supports.Any(x => x.TraitId == traitId)) throw new FormatException("Support skill already configured.");
						var opening = Number(); var capArgument = command.PopSpeech();
						double? cap = capArgument.EqualTo("native") ? null : double.Parse(capArgument, CultureInfo.InvariantCulture);
						supports.Add(new(Guid.NewGuid(), traitId, opening, cap, Toggle(command.PopSpeech()), Array.Empty<MagicCastingPrerequisite>()));
					}
					else throw new FormatException("Use support add, remove or prerequisite.");
				}
				p = p with { SupportGrants = supports.AsReadOnly() };
			}
			if (!command.IsFinished) throw new FormatException("Unexpected trailing input.");
			CastingPolicy = p; Changed = true;
			(Gameworld.MagicCasting as MagicCastingService)?.DefinitionsChanged();
			actor.OutputHandler.Send("Typed support policy updated. Validate before use.");
			foreach (var error in CastingConfigurationErrors()) actor.OutputHandler.Send(error.ColourError());
			return true;
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			actor.OutputHandler.Send(ex.Message.ColourError()); return false;
		}
	}
}
