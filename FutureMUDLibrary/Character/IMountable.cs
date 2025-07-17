using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.RPG.Checks;

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
}
