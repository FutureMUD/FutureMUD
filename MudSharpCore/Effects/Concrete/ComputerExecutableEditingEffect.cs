#nullable enable

using MudSharp.Computers;

namespace MudSharp.Effects.Concrete;

public class ComputerExecutableEditingEffect : Effect, IEffectSubtype
{
	public ComputerExecutableEditingEffect(ICharacter owner)
		: base(owner)
	{
	}

	public IComputerExecutableDefinition EditingItem { get; init; } = null!;

	protected override string SpecificEffectType => "ComputerExecutableEditingEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Editing computer executable {EditingItem.Name}";
	}
}
