#nullable enable
namespace MudSharp.Effects.Concrete;

/// <summary>The action itself is deliberately never saved or resumed.</summary>
public sealed class HaggleNegotiationAction : BlockingDelayedAction
{
	public HaggleNegotiationAction(ICharacter owner, Action<IPerceivable> complete, Action release)
		: base(owner, complete, "negotiating shop prices", new[] { "general", "movement", "combat" },
			"negotiating prices", _ => release())
	{
	}

	public override void InitialEffect()
	{
		CharacterOwner.OnQuit += OwnerQuit;
	}

	private void OwnerQuit(IPerceivable owner) => owner.RemoveEffect(this, true);
	public override void ExpireEffect()
	{
		if (!Owner.Effects.Contains(this)) return;
		try
		{
			Action(Owner);
		}
		finally
		{
			Owner.RemoveEffect(this, true);
		}
	}
	public override void RemovalEffect()
	{
		CharacterOwner.OnQuit -= OwnerQuit;
		base.RemovalEffect();
	}

	public override void Login() => Owner.RemoveEffect(this, true);
}
