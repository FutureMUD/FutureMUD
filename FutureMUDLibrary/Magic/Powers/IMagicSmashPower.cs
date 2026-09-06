#nullable enable
using MudSharp.Character;
using MudSharp.GameItems;
namespace MudSharp.Magic.Powers;
public interface IMagicSmashPower : IMagicAttackPower
{
	bool CanInvokePower(ICharacter actor, IGameItem target);
}
