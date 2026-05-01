using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.Work.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Economy.Employment;
#nullable enable
public abstract class ActiveJobBase : SaveableItem, IActiveJob, ILazyLoadDuringIdleTime
{
    protected ActiveJobBase(Models.ActiveJob dbitem, IJobListing listing, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        _id = dbitem.Id;
        Listing = listing;
        _characterId = dbitem.CharacterId;
        JobCommenced = MudDateTime.FromStoredStringOrFallback(dbitem.JobCommenced, gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "ActiveJob", dbitem.Id, listing.Name, "JobCommenced");
        JobDueToEnd = string.IsNullOrEmpty(dbitem.JobDueToEnd)
            ? default
            : MudDateTime.FromStoredStringOrFallback(dbitem.JobDueToEnd, gameworld,
                StoredMudDateTimeFallback.Never, "ActiveJob", dbitem.Id, listing.Name, "JobDueToEnd");
        IsJobComplete = dbitem.IsJobComplete;
        AlreadyHadClanPosition = dbitem.AlreadyHadClanPosition;
        JobEnded = string.IsNullOrEmpty(dbitem.JobEnded)
            ? default
            : MudDateTime.FromStoredStringOrFallback(dbitem.JobEnded, gameworld,
                StoredMudDateTimeFallback.Never, "ActiveJob", dbitem.Id, listing.Name, "JobEnded");
        _activeProjectId = dbitem.ActiveProjectId;
        foreach (XElement item in XElement.Parse(dbitem.BackpayOwed).Elements("Pay"))
        {
            ICurrency? currency = Gameworld.Currencies.Get(long.Parse(item.Attribute("currency")!.Value));
            if (currency is null)
            {
                continue;
            }

            BackpayOwed[currency] += decimal.Parse(item.Attribute("amount")!.Value);
        }

        foreach (XElement item in XElement.Parse(dbitem.RevenueEarned).Elements("Pay"))
        {
            ICurrency? currency = Gameworld.Currencies.Get(long.Parse(item.Attribute("currency")!.Value));
            if (currency is null)
            {
                continue;
            }

            RevenueEarned[currency] += decimal.Parse(item.Attribute("amount")!.Value);
        }

        gameworld.SaveManager.AddLazyLoad(this);
    }

    protected ActiveJobBase(IJobListing listing, ICharacter character, MudDateTime commenced, MudDateTime? ending,
        bool alreadyHadClanPosition, IActiveProject? activeProject)
    {
        Gameworld = listing.Gameworld;
        Listing = listing;
        _characterId = character.Id;
        _character = character;
        JobCommenced = new MudDateTime(commenced);
        JobDueToEnd = ending;
        AlreadyHadClanPosition = alreadyHadClanPosition;
        _activeProject = activeProject;
        _activeProjectId = activeProject?.Id;
        using (new FMDB())
        {
            ActiveJob dbitem = new()
            {
                JobListingId = listing.Id,
                CharacterId = character.Id,
                JobCommenced = JobCommenced.GetDateTimeString(),
                JobDueToEnd = JobDueToEnd?.GetDateTimeString(),
                JobEnded = null,
                IsJobComplete = false,
                AlreadyHadClanPosition = AlreadyHadClanPosition,
                ActiveProjectId = activeProject?.Id,
                BackpayOwed = new XElement("Pays").ToString(),
                RevenueEarned = new XElement("Pays").ToString(),
                CurrentPerformance = 0.0
            };
            FMDB.Context.ActiveJobs.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }

    public void Delete()
    {
        Gameworld.Destroy(this);
        Gameworld.SaveManager.Abort(this);
        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                ActiveJob? dbitem = FMDB.Context.ActiveJobs.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.ActiveJobs.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    protected virtual void EndEmployment()
    {
        IsJobComplete = true;
        JobEnded = Listing.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        Changed = true;
        if (Listing.ClanMembership is not null)
        {
            IClanMembership membership = Character.ClanMemberships.First(x => x.Clan == Listing.ClanMembership);
            if (!AlreadyHadClanPosition)
            {
                Listing.ClanMembership.RemoveMembership(membership);
            }
            else if (Listing.ClanAppointment is not null)
            {
                membership.Clan.DismissAppointment(membership, Listing.ClanAppointment);
            }
        }

        if (BackpayOwed.All(x => x.Value <= 0.0M) && RevenueEarned.All(x => x.Value <= 0.0M))
        {
            Listing.RemoveJob(this);
            Character.RemoveJob(this);
            Delete();
        }
    }

    #region Implementation of IActiveJob

    public IJobListing Listing { get; }

    private long _characterId;
    private ICharacter? _character;

    public ICharacter Character
    {
        get
        {
            if (_character is null)
            {
                _character = Gameworld.TryGetCharacter(_characterId, true);
                Gameworld.SaveManager.AbortLazyLoad(this);
            }

            return _character;
        }
    }

    public MudDateTime JobCommenced { get; }
    public MudDateTime? JobDueToEnd { get; }
    public MudDateTime? JobEnded { get; set; }
    public bool IsJobComplete { get; protected set; }
    public bool AlreadyHadClanPosition { get; }
    public double FullTimeEquivalentRatio => Listing.FullTimeEquivalentRatio;
    private double _currentPerformance;

    public double CurrentPerformance
    {
        get => _currentPerformance;
        set
        {
            _currentPerformance = value;
            Changed = true;
        }
    }

    private long? _activeProjectId;
    private IActiveProject? _activeProject;

    public IActiveProject? ActiveProject
    {
        get
        {
            if (_activeProject is null && _activeProjectId.HasValue)
            {
                _activeProject = Gameworld.ActiveProjects.Get(_activeProjectId.Value);
            }

            return _activeProject;
        }
    }

    public DecimalCounter<ICurrency> BackpayOwed { get; } = new();
    public DecimalCounter<ICurrency> RevenueEarned { get; } = new();
    public abstract void QuitJob();
    public abstract void FireFromJob();
    public abstract void FinishFixedTerm();
    public abstract void BeginJob();
    public abstract DecimalCounter<ICurrency> DailyPay();

    #endregion

    #region Implementation of ILazyLoadDuringIdleTime

    public void DoLoad()
    {
        _character = Gameworld.TryGetCharacter(_characterId, true);
        Gameworld.SaveManager.AbortLazyLoad(this);
    }

    #endregion

    #region Overrides of SaveableItem

    public override void Save()
    {
        ActiveJob? dbitem = FMDB.Context.ActiveJobs.Find(Id);
        if (dbitem is null)
        {
            throw new InvalidOperationException($"Active job {Id:N0} no longer exists in the database.");
        }

        dbitem.IsJobComplete = IsJobComplete;
        dbitem.AlreadyHadClanPosition = AlreadyHadClanPosition;
        dbitem.CurrentPerformance = CurrentPerformance;
        dbitem.JobEnded = JobEnded?.GetDateTimeString();
        dbitem.BackpayOwed = new XElement("Pays",
            from item in BackpayOwed
            select new XElement("Pay", new XAttribute("currency", item.Key.Id), new XAttribute("amount", item.Value))
        ).ToString();
        dbitem.RevenueEarned = new XElement("Pays",
            from item in RevenueEarned
            select new XElement("Pay", new XAttribute("currency", item.Key.Id), new XAttribute("amount", item.Value))
        ).ToString();
        Save(dbitem);
        Changed = false;
    }

    protected virtual void Save(Models.ActiveJob dbitem)
    {
        // Do nothing
    }

    #endregion

    #region Overrides of FrameworkItem

    public sealed override string FrameworkItemType => "ActiveJob";

    public sealed override string Name => Listing.Name;

    #endregion
}
