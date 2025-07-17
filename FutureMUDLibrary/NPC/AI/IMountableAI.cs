using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.NPC.AI;
public interface IMountableAI : IArtificialIntelligence
{
	bool PermitRider(ICharacter mount, ICharacter rider);
	string WhyCannotPermitRider(ICharacter mount, ICharacter rider);
	Difficulty NonConsensualMountDifficulty(ICharacter mount, ICharacter rider);
	int MaximumNumberOfRiders { get; }
	int RiderSlots { get; }
	int RiderSlotsOccupiedBy(ICharacter rider);
	Difficulty ControlDifficulty(ICharacter mount, ICharacter rider);
	Difficulty ResistBuckDifficulty(ICharacter mount, ICharacter rider);
	string MountEmote(ICharacter mount, ICharacter rider);
	string DismountEmote(ICharacter mount, ICharacter rider);
	string BuckEmote(ICharacter mount, ICharacter rider);
}
