using System.Security.Cryptography;
using System.Text.Json;
using MudSharp.Body.Traits;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp.Magic;

public partial class MagicSpell
{
	internal StoredSpellSnapshot? StoredSnapshot { get; set; }
	internal Models.MagicSpell SnapshotModel() => new()
	{
		Id = Id, Name = Name, Blurb = Blurb, Description = Description, SpellLevel = SpellLevel,
		ScrollInscriptionAllowed = ScrollInscriptionAllowed, MagicSchoolId = School.Id, SpellKnownProgId = SpellKnownProg?.Id ?? 0,
		ExclusiveDelay = ExclusiveDelay.TotalSeconds, NonExclusiveDelay = NonExclusiveDelay.TotalSeconds,
		AppliedEffectsAreExclusive = AppliedEffectsAreExclusive, CastingTraitDefinitionId = CastingTrait?.Id,
		ResistingTraitDefinitionId = OpposedTrait?.Id, CastingDifficulty = (int)CastingDifficulty, ResistingDifficulty = (int?)OpposedDifficulty,
		MinimumSuccessThreshold = (int)MinimumSuccessThreshold, CastingEmote = CastingEmote, FailCastingEmote = FailCastingEmote,
		TargetEmote = TargetEmote, TargetResistedEmote = TargetResistedEmote, TargetNullEmote = TargetNullEmote,
		CastingEmoteFlags = (int)CastingEmoteFlags, TargetEmoteFlags = (int)TargetEmoteFlags, Definition = SaveDefinition().ToString(SaveOptions.DisableFormatting)
	};
	internal MagicSpell InvocationCopy(SpellNumericalContext context)
	{
		var model = SnapshotModel();
		var copy = new MagicSpell(model, Gameworld) { EffectDurationExpression = EffectDurationExpression };
		copy.SetNoSave(true);
		foreach (var (resource, expression) in copy._castingCosts.ToArray())
			copy._castingCosts[resource] = new ContextualSpellExpression(expression, context, $"cost/{resource.Id}", Gameworld);
		ScrollSpellCompatibility.Bind(copy, context, null);
		return copy;
	}
}
