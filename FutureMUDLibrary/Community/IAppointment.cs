using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Community {
    public interface IAppointment : IFrameworkItem, ISaveable, IFutureProgVariable {
        void LoadElections(IEnumerable<Models.Election> elections);
        IEnumerable<string> Abbreviations { get; }

        IEnumerable<string> Titles { get; }

        List<Tuple<IFutureProg, string>> AbbreviationsAndProgs { get; }
        List<Tuple<IFutureProg, string>> TitlesAndProgs { get; }

        IClan Clan { get; set; }

        IPaygrade Paygrade { get; set; }
        IRank MinimumRankToHold { get; set; }
        IRank MinimumRankToAppoint { get; set; }
        int MaximumSimultaneousHolders { get; set; }
        IAppointment ParentPosition { get; set; }
        ClanFameType FameType { get; set; }

        ClanPrivilegeType Privileges { get; set; }

        IGameItemProto InsigniaGameItem { get; set; }
        
        bool IsAppointedByElection { get; set; }
        TimeSpan ElectionTerm { get; set; }
        TimeSpan ElectionLeadTime { get; set; }
        TimeSpan NominationPeriod { get; set; }
        TimeSpan VotingPeriod { get; set; } 
        int MaximumConsecutiveTerms { get; set; }
        int MaximumTotalTerms { get; set; }
        bool IsSecretBallot { get; set; }
        IFutureProg CanNominateProg { get; set; }
        IFutureProg WhyCantNominateProg { get; set; }
        (bool Truth, string Error) CanNominate(ICharacter character);
        IFutureProg NumberOfVotesProg { get; set; }
        int NumberOfVotes(ICharacter character);
        
        string Abbreviation(ICharacter character);

        /// <summary>
        ///     Formal name of appointment when used as a form of address, e.g. Master of the Stables
        /// </summary>
        string Title(ICharacter character);

        void FinaliseLoad(MudSharp.Models.Appointment appointment);

        void SetName(string name);

        IEnumerable<IElection> Elections { get; }
        void AddElection(IElection election);
        void RemoveElection(IElection election);
        void CheckForByElections();
    }
}