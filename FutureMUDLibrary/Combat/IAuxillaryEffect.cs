using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Combat
{
	public interface IAuxillaryEffect
	{
		XElement Save();
		void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome);
	}
}
