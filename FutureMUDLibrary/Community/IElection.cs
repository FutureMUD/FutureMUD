using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System.Collections.Generic;

namespace MudSharp.Community
{
    public enum ElectionStage
    {
        Preelection,
        Nomination,
        Voting,
        Preinstallation,
        Finalised,
        AwaitingConfirmation,
        Vetoed,
        Cancelled
    }

    public interface IElection : IFrameworkItem, ISaveable
    {
        IAppointment Appointment { get; }
        bool IsByElection { get; }
        bool IsRecall { get; }
        IClanMembership RecallTarget { get; }
        bool RecallPassed { get; }
        /// <summary>Liege authorities for this recall; completed ballots retain their decision-time authorities.</summary>
        IEnumerable<(long ClanId, long? AppointmentId)> RecallConfirmationAuthorities { get; }
        bool IsFinalised { get; }
        MudDateTime NominationStartDate { get; set; }
        IEnumerable<IClanMembership> Nominees { get; }
        int NumberOfAppointments { get; }
        MudDateTime ResultsInEffectDate { get; set; }
        IEnumerable<(IClanMembership Voter, IClanMembership Nominee, int Votes)> Votes { get; }
        MudDateTime VotingEndDate { get; set; }
        MudDateTime VotingStartDate { get; set; }
        ElectionStage ElectionStage { get; }
        Counter<IClanMembership> VotesByNominee { get; }
        IEnumerable<IClanMembership> Victors { get; }

        bool CheckElectionStage();
        /// <summary>Refreshes the scheduled transition, cancelling it when the clan is a template.</summary>
        void SetupListeners(ElectionStage stage);
        bool Confirm(ICharacter actor);
        bool Veto(ICharacter actor);
        /// <summary>Revokes this control's pending recall approval when its authority changes or is released.</summary>
        void InvalidateRecallConfirmation(IExternalClanControl control);
        void Nominate(IClanMembership nominee);
        void WithdrawNomination(IClanMembership nominee);
        void Vote(IClanMembership voter, IClanMembership nominee, int votes);
        void CancelElection();
        string Show(ICharacter actor);
    }
}
