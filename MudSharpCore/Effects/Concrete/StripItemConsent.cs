#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

internal sealed class StripItemConsent(ICharacter owner, ICharacter stripper, IGameItem item) : Effect(owner), IEffectSubtype
{
	public ICharacter Stripper { get; } = stripper;
	public IGameItem Item { get; } = item;
	protected override string SpecificEffectType => "StripItemConsent";

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Stripper.HowSeen(voyeur)} is removing {Item.HowSeen(voyeur)} with consent.";
	}
}
