using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class BeDressedEffect : Effect, IEffectSubtype
{
	protected override string SpecificEffectType => "BeDressedEffect";

	public ICharacter Dresser { get; set; }

	public ICharacter Dressee { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur)} is allowing themselves to be dressed by {Dresser.HowSeen(voyeur)}.";
	}

	public BeDressedEffect(ICharacter owner, ICharacter dresser) : base(owner)
	{
		Dresser = dresser;
		Dressee = owner;
	}

	public override void ExpireEffect()
	{
		base.ExpireEffect();
		Dressee.OutputHandler.Send($"Your consent for {Dresser.HowSeen(Dressee)} has expired.");
		Dresser.OutputHandler.Send(
			$"{Dressee.HowSeen(Dresser, true, DescriptionType.Possessive)} consent for you to dress {Dressee.ApparentGender(Dresser).Objective()} has expired.");
	}
}