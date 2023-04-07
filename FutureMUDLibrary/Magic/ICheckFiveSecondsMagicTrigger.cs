using MudSharp.Character;

namespace MudSharp.Magic
{
    public interface ICheckFiveSecondsMagicTrigger : IMagicTrigger
    {
        void DoTriggerCheck(ICharacter magician);
    }
}