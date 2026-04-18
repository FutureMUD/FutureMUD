using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp.Magic
{
    public interface ICastMagicTrigger : IMagicTrigger
    {
        SpellPower MinimumPower { get; }
        SpellPower MaximumPower { get; }
        void DoTriggerCast(ICharacter actor, StringStack additionalArguments);
    }
}