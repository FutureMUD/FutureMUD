using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Law
{
    public interface IWitnessProfile : IFrameworkItem, IEditableItem
    {
        void WitnessCrime(ICrime crime);
        string StreetwiseText(ICharacter enquirer);
    }
}
