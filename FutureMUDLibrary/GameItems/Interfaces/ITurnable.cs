using MudSharp.Character;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface ITurnable : IGameItemComponent
    {
        bool Turn(ICharacter actor, double turnExtent, IEmote emote);
        bool CanTurn(ICharacter actor, double turnExtent);
        string WhyCannotTurn(ICharacter actor, double turnExtent);
        double CurrentExtent { get; }
        double MinimumExtent { get; }
        double MaximumExtent { get; }
        double DefaultExtentIncrement { get; }

        string ExtentDescriptor { get; }
    }
}
