#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>An observer-facing death illusion; it never changes the owner's living physiology.</summary>
public sealed class SpellFeignedDeathEffect : MagicSpellEffectBase, ISDescAdditionEffect, IDescriptionAdditionEffect
{
	public static void InitialiseEffectType() => RegisterFactory("SpellFeignedDeath",
		(root, owner) => new SpellFeignedDeathEffect(root, owner));

	public SpellFeignedDeathEffect(ICharacter owner, IMagicSpellEffectParent parent) : base(owner, parent, null)
	{
	}

	private SpellFeignedDeathEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		if (owner is not ICharacter) throw new FormatException("Feigned death requires a character owner.");
	}

	protected override string SpecificEffectType => "SpellFeignedDeath";
	protected override XElement SaveDefinition() => new("Effect", new XElement("ApplicabilityProg", 0));
	public override string Describe(IPerceiver voyeur) => "Appearing dead to ordinary observers";

	public string AddendumText => "(apparently dead)";
	public string GetAddendumText(bool colour) => AddendumText;
	public bool DescriptionAdditionApplies(IPerceiver voyeur) => VisibleTo(voyeur) && Applies(voyeur);
	public string GetAdditionalText(IPerceiver voyeur, bool colour) =>
		"They lie motionless, with no visible breath or sign of life.";
	public bool PlayerSet => false;

	private bool VisibleTo(IPerceiver voyeur) => Owner is ICharacter target &&
		voyeur is ICharacter viewer && !viewer.IsAdministrator() &&
		!CharacterInstanceIdentityComparer.SamePhysicalInstance(viewer, target);

	public static bool AppearsDeadTo(ICharacter observer, ICharacter target) =>
		target.EffectsOfType<SpellFeignedDeathEffect>()
			.Any(effect => effect.VisibleTo(observer) && effect.Applies(observer));

	public static (string Pulse, string Breathing) ObservedVitalSigns(ICharacter observer, ICharacter target,
		string naturalPulse, string naturalBreathing) => AppearsDeadTo(observer, target)
		? ("no pulse", "not breathing")
		: (naturalPulse, naturalBreathing);

	public static string? ApparentPositionDescription(ICharacter observer, ICharacter target,
		string shortDescription, bool plural) => AppearsDeadTo(observer, target)
		? $"{shortDescription} {(plural ? "are" : "is")} lying motionless, apparently dead"
		: null;

	private static readonly HashSet<string> PassiveCommands = new(StringComparer.OrdinalIgnoreCase)
	{
		"look", "l", "lo", "loo", "help", "score", "sc", "sco", "scor",
		"inventory", "inv", "i", "equipment", "effects"
	};

	/// <summary>Any voluntary command beyond observation ends the appearance before it executes.</summary>
	public static void BreakOnVoluntaryAction(ICharacter actor, string commandName)
	{
		if (string.IsNullOrWhiteSpace(commandName) || PassiveCommands.Contains(commandName)) return;
		foreach (var effect in actor.EffectsOfType<SpellFeignedDeathEffect>().ToArray())
		{
			if (effect.ParentEffect is { } parent && actor.Effects.Contains(parent))
				actor.RemoveEffect(parent, true);
			else
				actor.RemoveEffect(effect, true);
		}
	}
}
