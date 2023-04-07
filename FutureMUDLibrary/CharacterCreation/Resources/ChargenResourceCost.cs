using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.CharacterCreation.Resources
{
    #nullable enable
    public class ChargenResourceCost
    {
	    public ChargenResourceCost()
	    {

	    }

	    public ChargenResourceCost(IChargenResource resource, int amount, bool requirementOnly)
	    {
		    Resource = resource;
		    Amount = amount;
            RequirementOnly = requirementOnly;
	    }

	    public ChargenResourceCost(IChargenResource resource, bool requirementOnly)
	    {
		    Resource = resource;
		    RequirementOnly = requirementOnly;
	    }

	    public IChargenResource Resource { get; init; }
        public int Amount { get; init; }
        public bool RequirementOnly { get; init; }
    }
}
