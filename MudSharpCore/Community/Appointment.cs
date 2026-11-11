using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Community;

public class Appointment : SaveableItem, IAppointment
{
    protected Appointment()
    {
    }

    public Appointment(MudSharp.Models.Appointment appointment, IClan clan)
    {
        Gameworld = clan.Gameworld;
        Clan = clan;
        _id = appointment.Id;
        _name = appointment.Name;
        Paygrade = appointment.PaygradeId.HasValue
            ? clan.Paygrades.FirstOrDefault(x => x.Id == appointment.PaygradeId.Value)
            : null;
        Privileges = (ClanPrivilegeType)appointment.Privileges;
        FameType = (ClanFameType)appointment.FameType;
        if (appointment.InsigniaGameItemId.HasValue && appointment.InsigniaGameItemRevnum.HasValue)
        {
            InsigniaGameItem = Gameworld.ItemProtos.Get(appointment.InsigniaGameItemId.Value,
                appointment.InsigniaGameItemRevnum.Value);
        }

        MaximumSimultaneousHolders = appointment.MaximumSimultaneousHolders;
        MinimumRankToHold = appointment.MinimumRankId.HasValue
            ? clan.Ranks.FirstOrDefault(x => x.Id == appointment.MinimumRankId.Value)
            : null;
        MinimumRankToAppoint = appointment.MinimumRankToAppointId.HasValue
            ? clan.Ranks.FirstOrDefault(x => x.Id == appointment.MinimumRankToAppointId.Value)
            : null;

		FormsOfAddress.AddRange(appointment.FormsOfAddress.OrderBy(x => x.Order).Select(ClanAddressPersistence.Load));
		AddressPriority = appointment.AddressPriority;

        foreach (AppointmentsTitles item in appointment.AppointmentsTitles.OrderBy(x => x.Order))
        {
            TitlesAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0), item.Title));
        }

        foreach (AppointmentsAbbreviations item in appointment.AppointmentsAbbreviations.OrderBy(x => x.Order))
        {
            AbbreviationsAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0),
                item.Abbreviation));
        }

        IsAppointedByElection = appointment.IsAppointedByElection;
        _electionConfirmationAppointmentId = appointment.ElectionConfirmationAppointmentId;
        CanDirectlyAppoint = appointment.CanDirectlyAppoint;
        CanParentDismiss = appointment.CanParentDismiss;
        CanBeRecalled = appointment.CanBeRecalled;
        RecallThreshold = appointment.RecallThreshold;
        CanInitiateRecallProg = Gameworld.FutureProgs.Get(appointment.CanInitiateRecallProgId ?? 0L);
        RecallVotesProg = Gameworld.FutureProgs.Get(appointment.RecallVotesProgId ?? 0L);
        CanResign = appointment.CanResign;
        ElectionTerm = TimeSpan.FromMinutes(appointment.ElectionTermMinutes ?? 0);
        ElectionLeadTime = TimeSpan.FromMinutes(appointment.ElectionLeadTimeMinutes ?? 0);
        NominationPeriod = TimeSpan.FromMinutes(appointment.NominationPeriodMinutes ?? 0);
        VotingPeriod = TimeSpan.FromMinutes(appointment.VotingPeriodMinutes ?? 0);
        MaximumConsecutiveTerms = appointment.MaximumConsecutiveTerms ?? 0;
        MaximumTotalTerms = appointment.MaximumTotalTerms ?? 0;
        IsSecretBallot = appointment.IsSecretBallot ?? false;
        CanNominateProg = Gameworld.FutureProgs.Get(appointment.CanNominateProgId ?? 0L);
        NumberOfVotesProg = Gameworld.FutureProgs.Get(appointment.NumberOfVotesProgId ?? 0L);
        WhyCantNominateProg = Gameworld.FutureProgs.Get(appointment.WhyCantNominateProgId ?? 0L);
    }

    public void LoadElections(IEnumerable<Models.Election> elections)
    {
        if (Clan.IsTemplate)
        {
            return;
        }

        foreach (Models.Election election in elections)
        {
            if (_elections.All(x => x.Id != election.Id))
            {
                _elections.Add(new Election(election, this));
            }
        }

        if (IsAppointedByElection)
        {
            if (ElectionTerm == TimeSpan.Zero)
            {
                CheckForByElections();
                return;
            }

            if (_elections.All(x => x.IsFinalised || x.IsByElection || x.IsRecall))
            {
                IElection byElection = _elections.FirstOrDefault(x => x.IsByElection && !x.IsFinalised);
                if (MaximumSimultaneousHolders >
                    Clan.Memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(this)))
                {
                    _elections.Add(new Election(this, false, MaximumSimultaneousHolders,
                        byElection != null ? byElection.ResultsInEffectDate + ElectionTerm : null));
                }
                else
                {
                    _elections.Add(new Election(this, false, MaximumSimultaneousHolders,
                        byElection != null
                            ? byElection.ResultsInEffectDate + ElectionTerm
                            : Clan.Calendar.CurrentDateTime + ElectionTerm));
                }
            }
        }
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Models.Appointment dbitem = FMDB.Context.Appointments.Find(Id);
            dbitem.Name = Name;
			ClanAddressPersistence.Save(dbitem, FormsOfAddress);
			dbitem.AddressPriority = AddressPriority;
            dbitem.Privileges = (long)Privileges;
            dbitem.MaximumSimultaneousHolders = MaximumSimultaneousHolders;
            dbitem.PaygradeId = Paygrade?.Id;
            dbitem.MinimumRankId = MinimumRankToHold?.Id;
            dbitem.MinimumRankToAppointId = MinimumRankToAppoint?.Id;
            dbitem.ParentAppointmentId = ParentPosition?.Id;
            dbitem.FameType = (int)FameType;
            dbitem.CanResign = CanResign;
            dbitem.CanDirectlyAppoint = CanDirectlyAppoint;
            dbitem.CanParentDismiss = IsAppointedByElection && CanParentDismiss;
            dbitem.CanBeRecalled = IsAppointedByElection && CanBeRecalled;
            dbitem.RecallThreshold = RecallThreshold;
            dbitem.CanInitiateRecallProgId = IsAppointedByElection ? CanInitiateRecallProg?.Id : null;
            dbitem.RecallVotesProgId = IsAppointedByElection ? RecallVotesProg?.Id : null;
            dbitem.ElectionConfirmationAppointmentId = _electionConfirmationAppointmentId;

            if (InsigniaGameItem != null)
            {
                dbitem.InsigniaGameItemId = InsigniaGameItem.Id;
                dbitem.InsigniaGameItemRevnum = InsigniaGameItem.RevisionNumber;
            }
            else
            {
                dbitem.InsigniaGameItemId = null;
                dbitem.InsigniaGameItemRevnum = null;
            }

            if (IsAppointedByElection)
            {
                dbitem.IsAppointedByElection = true;
                dbitem.IsSecretBallot = IsSecretBallot;
                dbitem.ElectionTermMinutes = ElectionTerm.TotalMinutes;
                dbitem.ElectionLeadTimeMinutes = ElectionLeadTime.TotalMinutes;
                dbitem.NominationPeriodMinutes = NominationPeriod.TotalMinutes;
                dbitem.VotingPeriodMinutes = VotingPeriod.TotalMinutes;
                dbitem.MaximumConsecutiveTerms = MaximumConsecutiveTerms;
                dbitem.MaximumTotalTerms = MaximumTotalTerms;
                dbitem.CanNominateProgId = CanNominateProg?.Id;
                dbitem.NumberOfVotesProgId = NumberOfVotesProg?.Id;
                dbitem.WhyCantNominateProgId = WhyCantNominateProg?.Id;
            }
            else
            {
                dbitem.IsAppointedByElection = false;
                dbitem.IsSecretBallot = null;
                dbitem.ElectionTermMinutes = null;
                dbitem.ElectionLeadTimeMinutes = null;
                dbitem.NominationPeriodMinutes = null;
                dbitem.VotingPeriodMinutes = null;
                dbitem.MaximumConsecutiveTerms = null;
                dbitem.MaximumTotalTerms = null;
                dbitem.CanNominateProgId = null;
                dbitem.NumberOfVotesProgId = null;
                dbitem.WhyCantNominateProgId = null;
            }

            FMDB.Context.AppointmentsAbbreviations.RemoveRange(dbitem.AppointmentsAbbreviations);
            int order = 0;
            foreach (Tuple<IFutureProg, string> item in AbbreviationsAndProgs)
            {
                AppointmentsAbbreviations abbreviation = new()
                {
                    FutureProgId = item.Item1?.Id,
                    Abbreviation = item.Item2,
                    Order = order++
                };
                dbitem.AppointmentsAbbreviations.Add(abbreviation);
            }

            FMDB.Context.AppointmentsTitles.RemoveRange(dbitem.AppointmentsTitles);
            order = 0;
            foreach (Tuple<IFutureProg, string> item in TitlesAndProgs)
            {
                AppointmentsTitles title = new()
                {
                    FutureProgId = item.Item1?.Id,
                    Title = item.Item2,
                    Order = order++
                };
                dbitem.AppointmentsTitles.Add(title);
            }

            FMDB.Context.SaveChanges();
        }

        Changed = false;
    }

	public List<ClanAddressRule> FormsOfAddress { get; } = new();
	public int AddressPriority { get; set; }

    public override string FrameworkItemType => "Appointment";

    #region IAppointment Members

    private readonly List<IElection> _elections = new();
    public IEnumerable<IElection> Elections => _elections;

    public void AddElection(IElection election)
    {
        if (Clan.IsTemplate)
        {
            return;
        }

        _elections.Add(election);
    }

    public void RemoveElection(IElection election)
    {
        _elections.Remove(election);
    }

    public void CheckForByElections()
    {
        if (Clan.IsTemplate || !IsAppointedByElection)
        {
            return;
        }

        var uncoveredVacancies = ClanCommandUtilities.GetUncoveredAppointmentVacancies(this, Clan.Memberships,
            Clan.ExternalControls);
        if (uncoveredVacancies <= 0)
        {
            return;
        }

        var primaryElection = ClanCommandUtilities.GetPrimaryOpenElection(this);
        if (primaryElection is not null)
        {
            var earliestByElectionResult = Clan.Calendar.CurrentDateTime + NominationPeriod + VotingPeriod + ElectionLeadTime;
            if (primaryElection.ElectionStage != ElectionStage.Preelection ||
                primaryElection.ResultsInEffectDate <= earliestByElectionResult)
            {
                return;
            }
        }

        Election election = new(this, true, uncoveredVacancies, null);
        _elections.Add(election);
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void Delete()
    {
        Gameworld.SaveManager.Abort(this);
        foreach (IElection election in Elections.ToList())
        {
            election.CancelElection();
        }

        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                Models.Appointment dbitem = FMDB.Context.Appointments.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.ChargenRolesClanMembershipsAppointments.RemoveRange(
                        FMDB.Context.ChargenRolesClanMembershipsAppointments.Where(x => x.AppointmentId == Id));
                    FMDB.Context.Appointments.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    public string Abbreviation(ICharacter character)
    {
        return
            AbbreviationsAndProgs.First(x => x.Item1?.ExecuteBool(character) ?? true)
                                 .Item2;
    }

    public bool IsAppointedByElection { get; set; }
    private long? _electionConfirmationAppointmentId;
    public bool RequiresElectionConfirmation => _electionConfirmationAppointmentId.HasValue;
    public IAppointment ElectionConfirmationAppointment
    {
        get => _electionConfirmationAppointmentId is { } id
            ? Gameworld.Clans.SelectMany(x => x.Appointments).FirstOrDefault(x => x.Id == id)
            : null;
        set => _electionConfirmationAppointmentId = value?.Id;
    }
    public bool CanDirectlyAppoint { get; set; }
    public bool CanParentDismiss { get; set; }
    public bool CanBeRecalled { get; set; }
    public double RecallThreshold { get; set; } = 0.5;
    public IFutureProg CanInitiateRecallProg { get; set; }
    public IFutureProg RecallVotesProg { get; set; }
    public bool CanResign { get; set; } = true;
    public TimeSpan ElectionTerm { get; set; }
    public TimeSpan ElectionLeadTime { get; set; }
    public TimeSpan NominationPeriod { get; set; }
    public TimeSpan VotingPeriod { get; set; }
    public int MaximumConsecutiveTerms { get; set; }
    public int MaximumTotalTerms { get; set; }
    public bool IsSecretBallot { get; set; }

    public IFutureProg CanNominateProg { get; set; }
    public IFutureProg WhyCantNominateProg { get; set; }

    public (bool Truth, string Error) CanNominate(ICharacter character)
    {
        if (CanNominateProg is not null && CanNominateProg.Execute<bool?>(character) != true)
        {
            return (false,
                WhyCantNominateProg?.Execute(character)?.ToString() ??
                "You do not meet the eligibility requirements for that nomination.");
        }

        if (MaximumConsecutiveTerms > 0)
        {
            if (ClanCommandUtilities.HasReachedConsecutiveTermLimit(Elections,
                    CharacterInstanceIdentityComparer.IdentityId(character),
                    MaximumConsecutiveTerms))
            {
                return (false,
                    $"You have reached the limit of {MaximumConsecutiveTerms.ToString("N0", character).ColourValue()} consecutive terms as {Title(character).ColourValue()}.");
            }
        }

        if (MaximumTotalTerms > 0)
        {
            if (ClanCommandUtilities.HasReachedTotalTermLimit(Elections,
                    CharacterInstanceIdentityComparer.IdentityId(character), MaximumTotalTerms))
            {
                return (false,
                    $"You have reached the life-time limit of {MaximumTotalTerms.ToString("N0", character).ColourValue()} total terms as {Title(character).ColourValue()}.");
            }
        }

        return (true, string.Empty);
    }

    public IFutureProg NumberOfVotesProg { get; set; }

    public int NumberOfVotes(ICharacter character)
    {
        return NumberOfVotesProg?.ExecuteInt(character) ?? 0;
    }

    IEnumerable<string> IAppointment.Abbreviations => AbbreviationsAndProgs.Select(x => x.Item2);

    public string Title(ICharacter character)
    {
        return TitlesAndProgs.First(x => x.Item1?.ExecuteBool(character) ?? true).Item2;
    }

    IEnumerable<string> IAppointment.Titles => TitlesAndProgs.Select(x => x.Item2);

    public List<Tuple<IFutureProg, string>> AbbreviationsAndProgs { get; } = new();

    public List<Tuple<IFutureProg, string>> TitlesAndProgs { get; } = new();

    public void FinaliseLoad(MudSharp.Models.Appointment appointment)
    {
        if (appointment.ParentAppointmentId.HasValue)
        {
            ParentPosition = Clan.Appointments.FirstOrDefault(x => x.Id == appointment.ParentAppointmentId);
        }
    }

    public IClan Clan { get; set; }

    public IPaygrade Paygrade { get; set; }

    public IRank MinimumRankToHold { get; set; }

    public IRank MinimumRankToAppoint { get; set; }

    public int MaximumSimultaneousHolders { get; set; }

    public IAppointment ParentPosition { get; set; }

    public ClanPrivilegeType Privileges { get; set; }

    public IGameItemProto InsigniaGameItem { get; set; }

    public ClanFameType FameType { get; set; }

    #endregion

    #region IFutureProgVariable Members

    public IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "id":
                return new NumberVariable(Id);
            case "name":
                return new TextVariable(Name);
            case "paygrade":
                return Paygrade;
            case "minrank":
                return MinimumRankToHold;
            case "minappointer":
                return MinimumRankToAppoint;
            case "parent":
                return ParentPosition;
            case "holders":
                return new NumberVariable(Clan.Memberships.Count(x => x.Appointments.Contains(this)));
            case "maxholders":
                return new NumberVariable(MaximumSimultaneousHolders);
            case "onlinemembers":
                return
                    new CollectionVariable(
                        Gameworld.Characters.Where(
                                     x => x.ClanMemberships.Any(y => y.Clan == Clan && y.Appointments.Contains(this)))
                                 .ToList(), ProgVariableTypes.Character);
            default:
                throw new NotSupportedException("Invalid IFutureProgVariable request in Rank.GetProperty");
        }
    }

    public ProgVariableTypes Type => ProgVariableTypes.ClanAppointment;

    public object GetObject => this;

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "paygrade", ProgVariableTypes.ClanPaygrade },
            { "minrank", ProgVariableTypes.ClanRank },
            { "minappointer", ProgVariableTypes.ClanRank },
            { "parent", ProgVariableTypes.ClanAppointment },
            { "holders", ProgVariableTypes.Number },
            { "maxholders", ProgVariableTypes.Number },
            { "onlinemembers", ProgVariableTypes.Collection | ProgVariableTypes.Character }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", "The Id of the appointment" },
            { "name", "The name of the appointment" },
            { "paygrade", "The paygrade associated with the appointment (can be null)" },
            { "minrank", "The minimum rank required to hold the appointment (can be null)" },
            { "minappointer", "The minimum rank required to appoint to this appointment (can be null)" },
            { "parent", "The parent rank controlling appointments to this appointment (can be null)" },
            { "holders", "A list of all the current holders of this appointment" },
            { "maxholders", "The maximum number of simultaneous holders (0 if unlimited)" },
            { "onlinemembers", "A list of all online characters with this appointment" }
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.ClanAppointment,
            DotReferenceHandler(), DotReferenceHelp());
    }

    #endregion
}
