using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.CharacterCreation.Resources
{
    #nullable enable
    public record ChargenResourceCost
    {
	    public required IChargenResource Resource { get; init; }
        public required int Amount { get; init; }
        public required bool RequirementOnly { get; init; }
    }
}
