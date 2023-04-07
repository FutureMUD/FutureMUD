using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IScoreAddendumEffect : IEffectSubtype
    {
        bool ShowInScore { get; }
        bool ShowInHealth { get; }
        string ScoreAddendum { get; }
    }
}
