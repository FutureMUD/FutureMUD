#nullable enable

namespace MudSharp.Effects.Concrete;

/// <summary>Creation-time deferred bonuses. The final opening is inserted before this claim is consumed.</summary>
public sealed class RoleSkillBonusClaims : Effect
{
	private readonly Dictionary<long, double> _pending;
	public RoleSkillBonusClaims(ICharacter owner, IReadOnlyDictionary<long, double> pending) : base(owner) => _pending = new(pending);
	private RoleSkillBonusClaims(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		_pending = effect.Element("Effect")!.Elements("Claim").ToDictionary(x => (long)x.Attribute("trait")!, x => (double)x.Attribute("floor")!);
		PruneAcquiredClaims();
	}
	public static void InitialiseEffectType() => RegisterFactory("RoleSkillBonusClaims", (effect, owner) => new RoleSkillBonusClaims(effect, owner));
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "RoleSkillBonusClaims";
	public override string Describe(IPerceiver voyeur) => $"Deferred role skill bonuses ({_pending.Count} pending).";
	protected override XElement SaveDefinition() => new("Effect", _pending.Select(x => new XElement("Claim", new XAttribute("trait", x.Key), new XAttribute("floor", x.Value))));
	public double Opening(long traitId, double otherwise) => _pending.TryGetValue(traitId, out var floor) ? Math.Max(otherwise, floor) : otherwise;
	public void Consume(long traitId)
	{
		if (_pending.Remove(traitId)) Changed = true;
	}
	private void PruneAcquiredClaims()
	{
		// A crash may have committed the final skill insert before saving the effect. Retire stale
		// claims after hydration; never grant or increase an existing skill during restart repair.
		if (Owner is not ICharacter character) return;
		foreach (var id in _pending.Keys.Where(x => character.HasTrait(character.Gameworld.Traits.Get(x))).ToArray()) Consume(id);
	}
}
