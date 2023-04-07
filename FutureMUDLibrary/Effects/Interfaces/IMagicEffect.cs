using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces
{
    public interface IMagicEffect : IEffectSubtype
    {
        IMagicSchool School { get; }
        IMagicPower PowerOrigin { get; }
        Difficulty DetectMagicDifficulty { get; }
        // TODO IMagicSpell SpellOrigin { get; }
    }
}
