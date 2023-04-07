using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;

namespace MudSharp.Work.Projects.Impacts
{
	public interface ILabourImpactActionAtTick : ILabourImpact
	{
		void DoAction(ICharacter character, IActiveProject project, IProjectLabourRequirement requirement);
	}
}
