using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class LinkdeadLogout : Effect, IEffectSubtype
{
	public LinkdeadLogout(IPerceivable owner)
		: base(owner)
	{
	}

	protected override string SpecificEffectType => "LinkdeadLogout";

	public override string Describe(IPerceiver voyeur)
	{
		return "Linkdead - will be logged out when effect expires";
	}

	public override void ExpireEffect()
	{
		(Owner as ICharacter)?.Quit();
	}

	public override string ToString()
	{
		return "Linkdead - will be logged out when effect expires";
	}
}