using System.Collections.Generic;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Framework.Revision;

namespace MudSharp.RPG.Dreams {
    public class DreamStage {
        public DreamStage(DreamPhase phase) {
            DreamerText = phase.DreamerText;
            DreamerCommand = phase.DreamerCommand;
            WaitSeconds = phase.WaitSeconds;
            PhaseID = phase.PhaseId;
        }

        public DreamStage() {
        }

        public string DreamerText { get; set; }
        public string DreamerCommand { get; set; }
        public int WaitSeconds { get; set; }
        public int PhaseID { get; set; }
    }

    public interface IDream : IEditableItem {
        IEnumerable<DreamStage> DreamStages { get; }
        IFutureProg OnWakeDuringDreamProg { get; }
        IFutureProg OnDreamProg { get; }
        IFutureProg CanDreamProg { get; }
        int Priority { get; }
        bool OnceOnly { get; }
        bool CanDream(ICharacter character);
        void FinishDream(ICharacter character);
        void GiveDream(ICharacter character);
        void RemoveDream(ICharacter character);
        IDream Clone(string newName);
    }
}