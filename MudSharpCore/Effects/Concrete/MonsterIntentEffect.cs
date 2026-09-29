#nullable enable

using MudSharp.NPC.AI;

namespace MudSharp.Effects.Concrete;

/// <summary>Saved pursuit purpose plus shared observations; constructors perform no gameplay actions.</summary>
public sealed class MonsterIntentEffect : CreaturePursuitEffect
{
	public MonsterMotive Motive { get; }
	public MonsterIntentEffect(ICharacter owner, MonsterAI ai, ICharacter target, MonsterMotive motive)
		: base(owner, ai, target) => Motive = motive;
	private MonsterIntentEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		Motive = Enum.TryParse<MonsterMotive>(root.Element("Effect")?.Element("Motive")?.Value, out var value) &&
		         Enum.IsDefined(value) ? value : MonsterMotive.None;
	}
	public static void InitialiseEffectType() => RegisterFactory("MonsterIntent", (xml, owner) => new MonsterIntentEffect(xml, owner));
	protected override string SpecificEffectType => "MonsterIntent";
	// A disabled controller must still perform ordinary disengagement for an active fight.
	protected override bool AllowsTactics => Ai is MonsterAI;
	protected override XElement SaveDefinition()
	{
		var xml = base.SaveDefinition();
		xml.Add(new XElement("Motive", Motive));
		return xml;
	}
	public override void ExpireEffect()
	{
		if (Owner is ICharacter { Combat: null } actor && Ai is MonsterAI ai)
		{
			ai.FinishIntent(actor, this, "pursuit deadline elapsed");
			return;
		}
		base.ExpireEffect();
	}
}
