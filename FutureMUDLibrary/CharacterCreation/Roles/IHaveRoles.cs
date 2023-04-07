using System.Collections.Generic;

namespace MudSharp.CharacterCreation.Roles {
    public interface IHaveRoles {
        IEnumerable<IChargenRole> Roles { get; }
    }
}