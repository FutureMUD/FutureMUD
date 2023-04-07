using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Combat {
    public enum AmmunitionLoadType {
        Direct,
        Clip,
        Magazine
    }

    public enum AmmunitionEchoType {
        Subsonic,
        Arcing,
        Supersonic,
        Laser
    }

    public interface IAmmunitionType : IEditableItem, ISaveable {
        string SpecificType { get; }
        IEnumerable<RangedWeaponType> RangedWeaponTypes { get; }
        double BaseAccuracy { get; }
        IDamageProfile DamageProfile { get; }
        AudioVolume Loudness { get; }
        double BreakChanceOnHit { get; }
        double BreakChanceOnMiss { get; }
        AmmunitionEchoType EchoType { get; }
        IAmmunitionType Clone(string newName);
    }
}