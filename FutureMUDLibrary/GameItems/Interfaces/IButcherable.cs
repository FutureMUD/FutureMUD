using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IButcherable : IGameItemComponent
    {
        ICharacter OriginalCharacter { get; }
        IRace OriginalRace { get; }
        long OriginalBodyId { get; }
        IBody OriginalBody { get; }
        DecayState Decay { get; }
        IEnumerable<IBodypart> Parts { get; }

        IEnumerable<string> ButcheredSubcategories { get; }

        bool Butcher(ICharacter butcher, string subcategory = null);
        void Skin(ICharacter skinner);
    }
}
