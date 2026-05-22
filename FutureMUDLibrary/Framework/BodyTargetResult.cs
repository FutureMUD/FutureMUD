using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.Framework;

public sealed class BodyTargetResult
{
	public BodyTargetResult(IPerceivable perceivable, IHaveABody bodyOwner, ICharacter? character, ICorpse? corpse)
	{
		Perceivable = perceivable;
		BodyOwner = bodyOwner;
		Character = character;
		Corpse = corpse;
	}

	public IPerceivable Perceivable { get; }
	public IHaveABody BodyOwner { get; }
	public ICharacter? Character { get; }
	public ICorpse? Corpse { get; }
	public IBody Body => BodyOwner.Body;
	public bool IsCorpse => Corpse is not null;
}
