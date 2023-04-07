using System;
using System.Collections.Generic;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Combat
{
    public interface IAimInformation
    {
        IPerceiver Shooter { get; set; }
        IPerceiver Target { get; set; }
        double AimPercentage { get; set; }
        IEnumerable<ICellExit> Path { get; set; }
        IRangedWeapon Weapon { get; set; }
        event EventHandler AimInvalidated;
        void ReleaseEvents();
    }
}