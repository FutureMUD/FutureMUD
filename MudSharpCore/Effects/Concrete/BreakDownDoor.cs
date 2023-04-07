using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class BreakDownDoor : Effect, IEffectSubtype
{
	public ICharacter CharacterOwner { get; set; }
	public ICellExit Exit { get; set; }

	public BreakDownDoor(ICharacter owner, ICellExit exit) : base(owner)
	{
		CharacterOwner = owner;
		Exit = exit;
	}

	protected override string SpecificEffectType => "BreakDownDoor";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Breaking down the door to {Exit.OutboundDirectionDescription}.";
	}
}