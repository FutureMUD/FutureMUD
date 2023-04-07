using MudSharp.Body;
using MudSharp.Character;
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
        DecayState Decay { get; }
        IEnumerable<IBodypart> Parts { get; }

        IEnumerable<string> ButcheredSubcategories { get; }

        bool Butcher(ICharacter butcher, string subcategory = null);
        void Skin(ICharacter skinner);
    }
}
