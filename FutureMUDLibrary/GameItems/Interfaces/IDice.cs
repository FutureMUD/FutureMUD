using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IDice : IGameItemComponent
    {
        string Roll();
        string Roll(ICharacter cheater, string desiredFace);
    }
}
