using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.RPG.Checks;
using MudSharp.Construction.Boundary;
using MudSharp.PerceptionEngine;
using MudSharp.Framework;
using MudSharp.Body.Position;

namespace MudSharp.Character;
public interface IMountable
{
	bool CanEverBeMounted(ICharacter rider);
	bool CanBeMountedBy(ICharacter rider);
	string WhyCannotBeMountedBy(ICharacter rider);
	IEnumerable<ICharacter> Riders { get; }
	bool Mount(ICharacter rider);
	void Dismount(ICharacter rider);
	void RemoveRider(ICharacter rider);
	Difficulty ControlMountDifficulty(ICharacter rider);
        bool IsPrimaryRider(ICharacter rider);
        bool BuckRider();
        bool PermitControl(ICharacter rider);
        void HandleControlDenied(ICharacter rider);
        bool RiderMove(ICellExit exit, ICharacter rider, IEmote emote = null, bool ignoreSafeMovement = false);
        bool RiderFly(ICharacter rider, IEmote emote = null);
        bool RiderAscend(ICharacter rider, IEmote emote = null);
        bool RiderDive(ICharacter rider, IEmote emote = null);
        bool RiderClimbUp(ICharacter rider, IEmote emote = null);
        bool RiderClimbDown(ICharacter rider, IEmote emote = null);
        bool RiderMovePosition(IPositionState position, PositionModifier modifier, IPerceivable target, ICharacter rider,
            IEmote playerEmote = null, IEmote playerPmote = null,
            bool ignoreMovementRestrictions = false, bool ignoreMovement = false);
}
