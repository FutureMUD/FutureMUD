using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class BuilderEditingEffect<T> : Effect, IEffectSubtype, IBuilderEditingEffect<T>
{
	public T EditingItem { get; init; }

	public BuilderEditingEffect(ICharacter owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "BuilderEditingEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Editing {EditingItem}";
	}
}