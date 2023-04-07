using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Interfaces
{
    public interface ILaserPowerPack : IGameItemComponent
    {
        string ClipType { get; }
        double PowerLevel { get; }
        bool CanDraw(double watts);
        void Draw(double watts);
        void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome, OpposedOutcome defenseOutcome, IBodypart bodypart, IRangedWeaponType weaponType, double painMultiplier, double stunMultiplier, IEmoteOutput defenseEmote);
    }
}
