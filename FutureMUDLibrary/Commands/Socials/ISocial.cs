using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp.Commands.Socials
{
    public interface ISocial
    {
        string Name { get; set; }
        IFutureProg ApplicabilityProg { get; set; }
        string NoTargetEcho { get; set; }
        string OneTargetEcho { get; set; }
        string DirectionTargetEcho { get; set; }
        string MultiTargetEcho { get; set; }
        bool Applies(object actor, string command, bool abbreviations);
        IExecutable<ICharacter> GetCommand();
        void Execute(ICharacter actor, List<IPerceivable> targetList, ICellExit targetExit, IEmote playerEmote);
    }
}