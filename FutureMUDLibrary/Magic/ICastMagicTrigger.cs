using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Magic
{
    public interface ICastMagicTrigger : IMagicTrigger
    {
        SpellPower MinimumPower { get; }
        SpellPower MaximumPower { get; }
        void DoTriggerCast(ICharacter actor, StringStack additionalArguments);
    }
}