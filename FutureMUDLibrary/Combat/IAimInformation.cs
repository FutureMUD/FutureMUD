using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;

namespace MudSharp.Combat
{
    public interface IAimInformation
    {
        IPerceiver Shooter { get; set; }
        IPerceiver Target { get; set; }
        double AimPercentage { get; set; }
        IEnumerable<IRoomExit> Path { get; set; }
		IRangedWeaponPlatform Weapon { get; set; }
        event EventHandler AimInvalidated;
        void ReleaseEvents();
    }
}
