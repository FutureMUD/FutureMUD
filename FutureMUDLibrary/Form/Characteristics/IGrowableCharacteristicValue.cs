using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Characteristics
{
    public interface IGrowableCharacteristicValue : ICharacteristicValue
    {
        int GrowthStage { get; }
        Difficulty StyleDifficulty { get; }
        ITag StyleToolTag { get; }
    }
}
