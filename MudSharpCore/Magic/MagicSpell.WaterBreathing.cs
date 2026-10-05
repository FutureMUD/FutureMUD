#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Construction;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.WaterBreathing;

namespace MudSharp.Magic;

public partial class MagicSpell
{
	private sealed record WaterRecipient(ICharacter Character, object Body, object Location, RoomLayer Layer,
		LifetimeMember[] Members);
	private sealed record WaterSelection(long SpellId, int ProfileVersion, ICharacter Caster,
		XElement Configuration, WaterBreathingDurationSelection Duration, WaterRecipient[] Recipients)
		: IMagicSpellEffectPreparedSelectionToken;

	internal IMagicSpellEffectPreparedSelectionToken CaptureWaterSelection(SourceWaterBreathingEffect effect,
		ICharacter caster, IPerceivable recipient, IMagicSpellEffectPreparedSelectionToken? previous)
	{
		ConfirmWaterConfiguration(effect);
		if (recipient is not ICharacter character || InvocationGrade is not { } grade || grade is < 1 or > 7 || GradeProfile is null)
			throw new InvalidOperationException("Source water breathing requires a character and explicit source grade.");
		var power = GradeProfile.Grades.Single(x => x.Grade == grade).Power;
		if (previous is not null)
		{
			if (previous is not WaterSelection token || token.SpellId != Id || token.ProfileVersion != GradeProfile.Version ||
				!ReferenceEquals(token.Caster, caster) || !token.Duration.MatchesBinding(grade, power, effect.LifetimePolicy!) ||
				!XNode.DeepEquals(token.Configuration, effect.SaveToXml()) ||
				!token.Recipients.Any(x => ReferenceEquals(x.Character, character)))
				throw new InvalidOperationException("Water breathing selection binding changed after preparation.");
			foreach (var bound in token.Recipients)
			{
				ConfirmWaterRecipient(bound);
				ConfirmLifetimeMembers(new(bound.Character, effect.LifetimePolicy!, token.Duration.Increment, grade, power,
					bound.Members, effect), bound.Members, new HashSet<MagicSpellParent>(ReferenceEqualityComparer.Instance));
			}
			return token;
		}
		var recipients = new[] { character, caster }.Distinct(ReferenceEqualityComparer.Instance)
			.Cast<ICharacter>().Select(x => new WaterRecipient(x, x.Body, x.Location, x.RoomLayer,
				CaptureLifetimeMembers(x, effect.LifetimePolicy!, new HashSet<MagicSpellParent>(ReferenceEqualityComparer.Instance), effect))).ToArray();
		// Draw only after all target, mapping and retained-cohort validation succeeds.
		var duration = WaterBreathingDurationSelection.Select(grade, power, effect.LifetimePolicy!, RandomUtilities.Random);
		return new WaterSelection(Id, GradeProfile.Version, caster, effect.SaveToXml(), duration, recipients);
	}

	private static void ConfirmWaterRecipient(WaterRecipient bound)
	{
		if (!ReferenceEquals(bound.Body, bound.Character.Body) || !ReferenceEquals(bound.Location, bound.Character.Location) ||
			bound.Layer != bound.Character.RoomLayer)
			throw new InvalidOperationException("Water breathing recipient body, location or layer changed after preparation.");
	}

	private void ConfirmWaterConfiguration(SourceWaterBreathingEffect effect)
	{
		if (effect.ConfigurationError is { } error) throw new InvalidOperationException(error);
		if (!WaterBreathingScopeXml.IsCurrent(effect.Scope!, Gameworld))
			throw new InvalidOperationException("Water breathing liquid mappings changed after loading/preparation.");
	}

	internal void ConfirmWaterBinding(SourceWaterBreathingEffect effect, ICharacter caster, ICharacter recipient)
	{
		if (effect.Selection is null) throw new InvalidOperationException("Source water breathing requires a prepared selection.");
		CaptureWaterSelection(effect, caster, recipient, effect.Selection);
	}

	private LifetimeAdmission CaptureWaterLifetimeAdmission(SourceWaterBreathingEffect effect,
		ICharacter caster, IPerceivable recipient, int grade, SpellPower power)
	{
		var token = (WaterSelection)effect.CapturePreparedSelection(caster, recipient);
		if (!token.Duration.MatchesBinding(grade, power, effect.LifetimePolicy!))
			throw new InvalidOperationException("Water breathing invocation power disagrees with its selected source grade.");
		var bound = token.Recipients.Single(x => ReferenceEquals(x.Character, recipient));
		return new(recipient, effect.LifetimePolicy!, token.Duration.Increment, grade, power, bound.Members, effect);
	}
}
