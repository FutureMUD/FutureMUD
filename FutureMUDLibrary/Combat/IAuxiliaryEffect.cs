using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Xml.Linq;

namespace MudSharp.Combat
{
	public interface IAuxiliaryEffect
	{
		XElement Save();
		void ApplyEffect(ICharacter attacker, IPerceiver target, CheckOutcome outcome);
		string DescribeForShow(ICharacter actor);

		static virtual void RegisterTypeHelp()
		{
			throw new NotImplementedException("There was an Auxiliary Effect Register Type Help that didn't have an implementation!");
		}

		bool BuildingCommand(ICharacter actor, StringStack command);
		string Show(ICharacter actor);
	}
}
