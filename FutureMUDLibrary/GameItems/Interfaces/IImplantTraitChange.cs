using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantTraitChange : IImplant
    {
        double BonusForTrait(ITrait trait, TraitBonusContext context);
        string ImplantFunctionReport();
    }
}
