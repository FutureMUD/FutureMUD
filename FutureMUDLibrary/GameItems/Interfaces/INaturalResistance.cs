using System.Collections.Generic;
using MudSharp.Health;

namespace MudSharp.GameItems.Interfaces {
    /// <summary>
    ///     An item with an INaturalResistance component takes less damage due to some intrinsic property of itself
    /// </summary>
    public interface INaturalResistance : IGameItemComponent {
        IDamage SufferDamage(IDamage damage, List<IWound> wounds);
    }
}