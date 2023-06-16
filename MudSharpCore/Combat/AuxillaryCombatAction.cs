using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat
{
	internal class AuxillaryCombatAction : CombatAction, IAuxillaryCombatAction
	{
		public override void Save()
		{
			throw new NotImplementedException();
		}

		public override string FrameworkItemType => "AuxillaryCombatAction";
		
		public string ShowBuilder(ICharacter actor)
		{
			throw new NotImplementedException();
		}

		public override bool BuildingCommand(ICharacter actor, StringStack command)
		{
			throw new NotImplementedException();
		}

		public IAuxillaryCombatAction Clone()
		{
			throw new NotImplementedException();
		}

		public override string ActionTypeName => "auxillary move";
		public override string HelpText { get; }
	}
}
