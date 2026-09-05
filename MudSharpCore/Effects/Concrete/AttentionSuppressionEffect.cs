#nullable enable

using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public sealed class AttentionSuppressionEffect : TimedPsychicEffect
{
	private readonly Dictionary<long, (DateTime Until, bool Noticed)> _observers = [];
	public Difficulty Difficulty { get; }
	public AttentionSuppressionEffect(ICharacter owner, Difficulty difficulty) : base(owner) => Difficulty = difficulty;
	private AttentionSuppressionEffect(XElement xml, IPerceivable owner) : base(xml, owner) => Difficulty = (Difficulty)(int)xml.Element("Effect")!.Attribute("difficulty")!;
	public static void InitialiseEffectType() => RegisterFactory("AttentionSuppression", (xml, owner) => new AttentionSuppressionEffect(xml, owner));
	protected override string SpecificEffectType => "AttentionSuppression";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => "Psychically difficult to notice.";
	protected override XElement SaveDefinition() => WithOrigin(new XElement("Effect", new XAttribute("difficulty", (int)Difficulty)));
	public bool Notices(ICharacter observer, bool deliberate = false)
	{
		if (observer == Owner || observer.IsAdministrator()) return true;
		var id = CharacterInstanceIdentityComparer.IdentityId(observer);
		if (_observers.TryGetValue(id, out var cached) && (cached.Noticed || !deliberate && cached.Until > RuntimeClock.UtcNow)) return cached.Noticed;
		var result = Gameworld.GetCheck(CheckType.MagicTelepathyCheck).Check(observer, Difficulty, Owner);
		var noticed = result.Outcome >= Outcome.MinorPass;
		if (_observers.Count >= 256) _observers.Remove(_observers.Keys.First());
		_observers[id] = (RuntimeClock.UtcNow.AddSeconds(30), noticed);
		return noticed;
	}
}
