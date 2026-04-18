using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Collections.Generic;

namespace MudSharp.RPG.Dreams
{
    public class DreamStage
    {
        public DreamStage(DreamPhase phase)
        {
            DreamerText = phase.DreamerText;
            DreamerCommand = phase.DreamerCommand;
            WaitSeconds = phase.WaitSeconds;
            PhaseID = phase.PhaseId;
        }

        public DreamStage()
        {
        }

        public string DreamerText { get; set; }
        public string DreamerCommand { get; set; }
        public int WaitSeconds { get; set; }
        public int PhaseID { get; init; }
    }

    public interface IDream : IEditableItem
    {
        IEnumerable<DreamStage> DreamStages { get; }
        IFutureProg OnWakeDuringDreamProg { get; }
        IFutureProg OnDreamProg { get; }
        IFutureProg CanDreamProg { get; }
        int Priority { get; }
        bool OnceOnly { get; }
        bool CanDream(ICharacter character);
        void FinishDream(ICharacter character);
        bool GiveDream(ICharacter character);
        bool RemoveDream(ICharacter character);
        IDream Clone(string newName);
    }
}