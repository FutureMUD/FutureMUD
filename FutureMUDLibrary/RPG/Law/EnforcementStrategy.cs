using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.Law
{
    public enum EnforcementStrategy
    {
        NoActiveEnforcement,
        ArrestAndDetainedUnarmedOnly,
        ArrestAndDetainIfArmed,
        ArrestAndDetain,
        ArrestAndDetainNoWarning,
        LethalForceArrestAndDetain,
        LethalForceArrestAndDetainNoWarning,
        KillOnSight,
    }
}
