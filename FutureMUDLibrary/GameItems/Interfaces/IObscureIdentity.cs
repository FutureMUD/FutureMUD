#nullable enable annotations

﻿using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IObscureIdentity : IObscureCharacteristics
    {
        bool CurrentlyApplies { get; }
        string OverriddenShortDescription { get; }
        /// <summary>Null preserves the wearer's ordinary full description.</summary>
        string? OverriddenFullDescription { get; }
        Difficulty SeeThroughDisguiseDifficulty { get; }
    }
}
