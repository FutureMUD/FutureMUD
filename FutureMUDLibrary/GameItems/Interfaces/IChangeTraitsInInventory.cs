using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.GameItems.Interfaces
{
    public interface IChangeTraitsInInventory : IGameItemComponent {
        double BonusForTrait(ITraitDefinition definition, TraitBonusContext context);
    }
}
