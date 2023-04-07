using MudSharp.Character;
using MudSharp.Framework;
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
        Finalised
    }

    public interface IElection : IFrameworkItem
    {
        IAppointment Appointment { get; }
        bool IsByElection { get; }
        bool IsFinalised { get; }
        MudDateTime NominationStartDate { get; }
        IEnumerable<IClanMembership> Nominees { get; }
        int NumberOfAppointments { get; }
        MudDateTime ResultsInEffectDate { get; }
        IEnumerable<(IClanMembership Voter, IClanMembership Nominee, int Votes)> Votes { get; }
        MudDateTime VotingEndDate { get; }
        MudDateTime VotingStartDate { get; }
        ElectionStage ElectionStage { get; }
        Counter<IClanMembership> VotesByNominee { get; }
        IEnumerable<IClanMembership> Victors { get; }

        bool CheckElectionStage();
        void Nominate(IClanMembership nominee);
        void WithdrawNomination(IClanMembership nominee);
        void Vote(IClanMembership voter, IClanMembership nominee, int votes);
        void CancelElection();
        string Show(ICharacter actor);
    }
}