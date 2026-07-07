using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Projects.Impacts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.Work.Projects.ConcreteTypes;

public abstract class ActiveProject : LateInitialisingItem, IActiveProject, ILazyLoadDuringIdleTime
{
    public sealed override string FrameworkItemType => "ActiveProject";

    private sealed record CachedActiveProjectLabour(
        long CharacterId,
        long? CharacterInstanceId,
        IProjectLabourRequirement Labour
    );

    protected ActiveProject(MudSharp.Models.ActiveProject project, IFuturemud gameworld)
    {
        Gameworld = gameworld;
        Gameworld.SaveManager.AddLazyLoad(this);
        _id = project.Id;
        IdInitialised = true;
        ProjectDefinition = gameworld.Projects.Get(project.ProjectId, project.ProjectRevisionNumber);
        CurrentPhase = ProjectDefinition.Phases.First(x => x.Id == project.CurrentPhaseId);
        _name = ProjectDefinition.Name;
        _paymentCurrencyId = project.PaymentCurrencyId;
        _cachedActiveLabour = LoadCachedActiveLabour(project).ToList();
        foreach (ActiveProjectLabour labour in project.ActiveProjectLabours)
        {
            _labourProgress[CurrentPhase.LabourRequirements.First(x => x.Id == labour.ProjectLabourRequirementsId)] =
                labour.Progress;
            var requirement = CurrentPhase.LabourRequirements.First(x => x.Id == labour.ProjectLabourRequirementsId);
            if (labour.PaymentPerHour > 0.0M)
            {
                _labourPaymentRates[requirement] = labour.PaymentPerHour;
            }
        }

        foreach (ActiveProjectMaterial material in project.ActiveProjectMaterials)
        {
            _materialProgress[
                    CurrentPhase.MaterialRequirements.First(x => x.Id == material.ProjectMaterialRequirementsId)] =
                material.Progress;
            var requirement =
                CurrentPhase.MaterialRequirements.First(x => x.Id == material.ProjectMaterialRequirementsId);
            if (material.PaymentPerUnit > 0.0M)
            {
                _materialPaymentRates[requirement] = material.PaymentPerUnit;
            }
        }
    }

    protected ActiveProject(IProject project)
    {
        Gameworld = project.Gameworld;
        Gameworld.SaveManager.AddInitialisation(this);
        ProjectDefinition = project;
        CurrentPhase = ProjectDefinition.Phases.First();
        _name = ProjectDefinition.Name;
        _cachedActiveLabour = new List<CachedActiveProjectLabour>();
    }

    private IEnumerable<CachedActiveProjectLabour> LoadCachedActiveLabour(MudSharp.Models.ActiveProject project)
    {
        var instanceWorkers = FMDB.Context.CharacterInstances
                                     .AsNoTracking()
                                     .Where(x => x.CurrentProjectId == project.Id && x.CurrentProjectLabourId.HasValue)
                                     .Select(x => new
                                     {
                                         x.CharacterId,
                                         CharacterInstanceId = (long?)x.Id,
                                         LabourId = x.CurrentProjectLabourId.Value
                                     })
                                     .ToList();

        if (instanceWorkers.Any())
        {
            foreach (var worker in instanceWorkers)
            {
                var labour = CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == worker.LabourId);
                if (labour is not null)
                {
                    yield return new CachedActiveProjectLabour(worker.CharacterId, worker.CharacterInstanceId, labour);
                }
            }

            yield break;
        }

        foreach (var worker in project.Characters.Where(x => x.CurrentProjectLabourId.HasValue))
        {
            var labour = CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Id == worker.CurrentProjectLabourId);
            if (labour is not null)
            {
                yield return new CachedActiveProjectLabour(worker.Id, null, labour);
            }
        }
    }

    protected void Delete()
    {
        RefundRemainingCashToOwner();
        _labourProgress.Clear();
        _materialProgress.Clear();
        _labourPaymentRates.Clear();
        _materialPaymentRates.Clear();
        _activeLabour.Clear();
        _cachedActiveLabour.Clear();
        CurrentPhase = null;
        Gameworld.Destroy(this);
        Changed = false;
        _noSave = true;
        Gameworld.SaveManager.Abort(this);
        Gameworld.EffectScheduler.Destroy(this);
        Gameworld.Scheduler.Destroy(this);
        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                Models.ActiveProject dbitem = FMDB.Context.ActiveProjects.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.ActiveProjects.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    #region Overrides of FrameworkItem

    private static readonly Regex _projectNameRegex = new("@job", RegexOptions.IgnoreCase);

    /// <inheritdoc />
    public override string Name => _projectNameRegex.Replace(_name, m =>
                                            {
                                                IActiveJob job = Gameworld.ActiveJobs.FirstOrDefault(x => x.ActiveProject == this);
                                                if (job is not null)
                                                {
                                                    return job.Name;
                                                }

                                                return "a job";
                                            });

    #endregion

    public IProject ProjectDefinition { get; protected set; }
    public IProjectPhase CurrentPhase { get; protected set; }

    protected readonly DoubleCounter<IProjectLabourRequirement> _labourProgress = new();
    public IReadOnlyDictionary<IProjectLabourRequirement, double> LabourProgress => _labourProgress;

    protected readonly DoubleCounter<IProjectMaterialRequirement> _materialProgress = new();
    public IReadOnlyDictionary<IProjectMaterialRequirement, double> MaterialProgress => _materialProgress;

    protected readonly Dictionary<IProjectLabourRequirement, decimal> _labourPaymentRates = new();
    public IReadOnlyDictionary<IProjectLabourRequirement, decimal> LabourPaymentRates => _labourPaymentRates;

    protected readonly Dictionary<IProjectMaterialRequirement, decimal> _materialPaymentRates = new();
    public IReadOnlyDictionary<IProjectMaterialRequirement, decimal> MaterialPaymentRates => _materialPaymentRates;

    private long? _paymentCurrencyId;
    private ICurrency _paymentCurrency;

    public ICurrency PaymentCurrency
    {
        get
        {
            if (_paymentCurrency != null)
            {
                return _paymentCurrency;
            }

            _paymentCurrency = Gameworld.Currencies.Get(_paymentCurrencyId ?? 0L) ??
                               CharacterOwner?.Currency ??
                               Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID")) ??
                               Gameworld.Currencies.FirstOrDefault();
            return _paymentCurrency;
        }
    }

    private void RefundRemainingCashToOwner()
    {
        var currency = PaymentCurrency;
        if (currency == null)
        {
            return;
        }

        var balance = CashBalance;
        if (balance <= 0.0M || CharacterOwner == null)
        {
            return;
        }

        if (!VirtualCashLedger.Debit(this, currency, balance, CharacterOwner, CharacterOwner, "ProjectRefund",
                $"Unspent funds returned from project {Name}", null, null, out _, this, $"Project #{Id:N0}"))
        {
            return;
        }

        ProjectPaymentService.CreateProjectRefundPayable(this, CharacterOwner, currency, balance);
    }

    public decimal CashBalance => PaymentCurrency == null ? 0.0M : VirtualCashLedger.Balance(this, PaymentCurrency);

    public decimal LabourPaymentRateFor(IProjectLabourRequirement labour)
    {
        return _labourPaymentRates.GetValueOrDefault(labour);
    }

    public decimal MaterialPaymentRateFor(IProjectMaterialRequirement material)
    {
        return _materialPaymentRates.GetValueOrDefault(material);
    }

    public void SetPaymentCurrency(ICurrency currency)
    {
        if (currency == null)
        {
            return;
        }

        if (_paymentCurrencyId == currency.Id)
        {
            return;
        }

        _paymentCurrency = currency;
        _paymentCurrencyId = currency.Id;
        Changed = true;
    }

    public void SetLabourPaymentRate(IProjectLabourRequirement labour, decimal amount)
    {
        if (amount <= 0.0M)
        {
            _labourPaymentRates.Remove(labour);
        }
        else
        {
            _labourPaymentRates[labour] = amount;
            EnsurePaymentCurrency();
        }

        Changed = true;
    }

    public void SetMaterialPaymentRate(IProjectMaterialRequirement material, decimal amount)
    {
        if (amount <= 0.0M)
        {
            _materialPaymentRates.Remove(material);
        }
        else
        {
            _materialPaymentRates[material] = amount;
            EnsurePaymentCurrency();
        }

        Changed = true;
    }

    private void EnsurePaymentCurrency()
    {
        var currency = PaymentCurrency;
        if (currency == null)
        {
            return;
        }

        if (_paymentCurrencyId == currency.Id)
        {
            _paymentCurrency = currency;
            return;
        }

        _paymentCurrency = currency;
        _paymentCurrencyId = currency.Id;
        Changed = true;
    }

    public bool DepositFunds(ICharacter actor, decimal amount, out string error)
    {
        error = string.Empty;
        if (amount <= 0.0M)
        {
            error = "The amount must be positive.";
            return false;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            error = "This project does not have a payment currency.";
            return false;
        }

        EnsurePaymentCurrency();
        _ = Id;
        OtherCashPayment payment = new(currency, actor);
        var available = payment.AccessibleMoneyForPayment();
        if (available < amount)
        {
            error = $"You only have {currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal)} available.";
            return false;
        }

        payment.TakePayment(amount);
        VirtualCashLedger.Credit(this, currency, amount, actor, actor, "Cash",
            $"Cash deposit to project {Name}", null, this, $"Project #{Id:N0}");
        return true;
    }

    public bool WithdrawFunds(ICharacter actor, decimal amount, out string error)
    {
        error = string.Empty;
        if (amount <= 0.0M)
        {
            error = "The amount must be positive.";
            return false;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            error = "This project does not have a payment currency.";
            return false;
        }

        if (CashBalance < amount)
        {
            error = $"This project only has {currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal)} available.";
            return false;
        }

        if (!ProjectPaymentService.CanCreateCash(currency, amount, out error))
        {
            return false;
        }

        if (!VirtualCashLedger.Debit(this, currency, amount, actor, actor, "Cash",
                $"Cash withdrawal from project {Name}", null, null, out error, this, $"Project #{Id:N0}"))
        {
            return false;
        }

        return ProjectPaymentService.TryGiveCash(actor, currency, amount, out error);
    }

    private decimal LabourPaymentAmountFor(IProjectLabourRequirement labour, double hours)
    {
        var rate = LabourPaymentRateFor(labour);
        return rate <= 0.0M || hours <= 0.0 ? 0.0M : rate * (decimal)hours;
    }

    private decimal MaterialPaymentAmountFor(IProjectMaterialRequirement material, double progress)
    {
        var rate = MaterialPaymentRateFor(material);
        return rate <= 0.0M || progress <= 0.0 ? 0.0M : rate * (decimal)progress;
    }

    public bool CanPayLabourContribution(IProjectLabourRequirement labour, double hours, out string error)
    {
        error = string.Empty;
        var amount = LabourPaymentAmountFor(labour, hours);
        if (amount <= 0.0M)
        {
            return true;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            error = "the project does not have a payment currency";
            return false;
        }

        if (CashBalance < amount)
        {
            error = $"the project only has {currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal)} available to pay the configured {currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)} labour tick";
            return false;
        }

        return true;
    }

    public bool CanPayMaterialContribution(IProjectMaterialRequirement material, double progress, out string error)
    {
        error = string.Empty;
        var amount = MaterialPaymentAmountFor(material, progress);
        if (amount <= 0.0M)
        {
            return true;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            error = "The project does not have a payment currency.";
            return false;
        }

        if (CashBalance < amount)
        {
            error = $"The project only has {currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal)} available, but this contribution would require {currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal)}.";
            return false;
        }

        return ProjectPaymentService.CanCreateCash(currency, amount, out error);
    }

    public decimal AwardLabourPayment(ICharacter actor, IProjectLabourRequirement labour, double hours)
    {
        var amount = LabourPaymentAmountFor(labour, hours);
        if (amount <= 0.0M)
        {
            return 0.0M;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            return 0.0M;
        }

        if (CashBalance < amount)
        {
            return 0.0M;
        }

        if (!VirtualCashLedger.Debit(this, currency, amount, actor, actor, "ProjectPayable",
                $"Labour payment for {actor.PersonalName.GetName(MudSharp.Character.Name.NameStyle.FullName)} on {labour.Name}",
                null, null, out _, this, $"Project #{Id:N0}"))
        {
            return 0.0M;
        }

        ProjectPaymentService.CreateLabourPayable(this, actor, labour, currency, amount);
        return amount;
    }

    public decimal PayMaterialContribution(ICharacter actor, IProjectMaterialRequirement material, double progress)
    {
        var amount = MaterialPaymentAmountFor(material, progress);
        if (amount <= 0.0M)
        {
            return 0.0M;
        }

        var currency = PaymentCurrency;
        if (currency == null)
        {
            return 0.0M;
        }

        if (!CanPayMaterialContribution(material, progress, out var error))
        {
            actor.OutputHandler.Send(error.ColourError());
            return 0.0M;
        }

        if (!VirtualCashLedger.Debit(this, currency, amount, actor, actor, "MaterialContributor",
                $"Material payment for {material.Name} on project {Name}", null, null, out error, this,
                $"Project #{Id:N0}"))
        {
            actor.OutputHandler.Send(error.ColourError());
            return 0.0M;
        }

        ProjectPaymentService.TryGiveCash(actor, currency, amount, out var message);
        actor.OutputHandler.Send(
            $"The project pays you {currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} for supplying {material.Name.ColourName()}.");
        if (!string.IsNullOrWhiteSpace(message))
        {
            actor.OutputHandler.Send(message.ColourCommand());
        }

        return amount;
    }

    private readonly List<CachedActiveProjectLabour> _cachedActiveLabour;

    protected void CheckCachedLabour()
    {
        if (_cachedActiveLabour.Count > 0)
        {
            foreach (var labour in _cachedActiveLabour)
            {
                var actor = ResolveCachedLabourActor(labour);
                if (actor is null)
                {
                    continue;
                }

                _activeLabour.Add((actor, labour.Labour));
            }

            _cachedActiveLabour.Clear();
        }
    }

    private ICharacter ResolveCachedLabourActor(CachedActiveProjectLabour labour)
    {
        var identity = Gameworld.TryGetCharacter(labour.CharacterId, true);
        if (identity is null)
        {
            return null;
        }

        if (labour.CharacterInstanceId is not long instanceId)
        {
            return identity;
        }

        return identity.Identity.Instances.FirstOrDefault(x => x.InstanceId == instanceId) as ICharacter;
    }


    protected readonly List<(ICharacter Character, IProjectLabourRequirement Labour)> _activeLabour = new();

    public IEnumerable<(ICharacter Character, IProjectLabourRequirement Labour)> ActiveLabour
    {
        get
        {
            CheckCachedLabour();
            return _activeLabour;
        }
    }

    public bool HasFreeLabourSlot(IProjectLabourRequirement labour)
    {
        return CurrentPhase?.LabourRequirements.Contains(labour) == true &&
               ActiveLabour.Count(x => x.Labour == labour) < labour.MaximumSimultaneousWorkers;
    }

    public bool HasDisplaceableNpcWorker(ICharacter actor, IProjectLabourRequirement labour)
    {
        return CurrentPhase?.LabourRequirements.Contains(labour) == true &&
               actor.IsPlayerCharacter &&
               !CharacterAlreadyWorkingOnLabour(actor, labour) &&
               labour.CharacterIsQualified(actor) &&
               !HasFreeLabourSlot(labour) &&
               DisplaceableNpcWorkerFor(labour) is not null;
    }

    public bool CanJoinLabour(ICharacter actor, IProjectLabourRequirement labour)
    {
        return CurrentPhase?.LabourRequirements.Contains(labour) == true &&
               !CharacterAlreadyWorkingOnLabour(actor, labour) &&
               labour.CharacterIsQualified(actor) &&
               (HasFreeLabourSlot(labour) || HasDisplaceableNpcWorker(actor, labour));
    }

    public bool TryJoinLabour(ICharacter actor, IProjectLabourRequirement labour, bool allowNpcDisplacement,
        out ICharacter displacedWorker)
    {
        displacedWorker = null;
        if (CurrentPhase?.LabourRequirements.Contains(labour) != true ||
            CharacterAlreadyWorkingOnLabour(actor, labour) ||
            !labour.CharacterIsQualified(actor))
        {
            return false;
        }

        if (HasFreeLabourSlot(labour))
        {
            Join(actor, labour);
            return true;
        }

        if (!allowNpcDisplacement || !HasDisplaceableNpcWorker(actor, labour))
        {
            return false;
        }

        displacedWorker = DisplaceableNpcWorkerFor(labour);
        var workerToDisplace = displacedWorker;
        _activeLabour.RemoveAll(x => x.Labour == labour &&
                                     CharacterInstanceIdentityComparer.SamePhysicalInstance(x.Character,
                                         workerToDisplace));
        if (displacedWorker.CurrentProject.Project == this)
        {
            displacedWorker.CurrentProject = (null, null);
        }

        SendNpcDisplacementMessages(actor, displacedWorker, labour);
        Join(actor, labour);
        displacedWorker.TryJoinQueuedProjectLabour();
        return true;
    }

    public IReadOnlyCollection<ICharacter> RemoveWorkersFromLabour(IProjectLabourRequirement labour)
    {
        var workers = ActiveLabour
                      .Where(x => x.Labour == labour)
                      .Select(x => x.Character)
                      .DistinctPhysicalInstances()
                      .ToList();
        if (!workers.Any())
        {
            return workers;
        }

        foreach (var worker in workers)
        {
            if (worker.CurrentProject.Project == this && worker.CurrentProject.Labour == labour)
            {
                worker.CurrentProject = (null, null);
            }
        }

        _activeLabour.RemoveAll(x => x.Labour == labour);
        Changed = true;
        return workers;
    }

    private bool CharacterAlreadyWorkingOnLabour(ICharacter actor, IProjectLabourRequirement labour)
    {
        return ActiveLabour.Any(x => x.Labour == labour &&
                                     CharacterInstanceIdentityComparer.SamePhysicalInstance(x.Character, actor));
    }

    private ICharacter DisplaceableNpcWorkerFor(IProjectLabourRequirement labour)
    {
        return ActiveLabour
               .Where(x => x.Labour == labour)
               .Select(x => x.Character)
               .DistinctPhysicalInstances()
               .FirstOrDefault(x => !x.IsPlayerCharacter);
    }

    protected bool HasSatisfiedButJoinableLabour(ICharacter actor)
    {
        return CurrentPhase.LabourRequirements.Any(x => !HasFreeLabourSlot(x) && HasDisplaceableNpcWorker(actor, x));
    }

    protected virtual void SendNpcDisplacementMessages(ICharacter actor, ICharacter displacedWorker,
        IProjectLabourRequirement labour)
    {
        actor.OutputHandler.Send(
            $"You take over the {labour.Name.ColourName()} task of {Name.ColourName()}, displacing {displacedWorker.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreCorpse | PerceiveIgnoreFlags.IgnoreLoadThings)}.");
        displacedWorker.OutputHandler.Send(
            $"{actor.HowSeen(displacedWorker, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreCorpse | PerceiveIgnoreFlags.IgnoreLoadThings)} takes over the {labour.Name.ColourName()} task of {Name.ColourName()}, so you stop working on it.");
    }

    public abstract void Cancel(ICharacter actor);

    public abstract bool FulfilLabour(IProjectLabourRequirement labour, double progress);

    public abstract void FulfilMaterial(IProjectMaterialRequirement material, double progress);

    public abstract void Join(ICharacter actor, IProjectLabourRequirement labour);

    public abstract void Leave(ICharacter actor);

    protected bool AreMandatoryLabourRequirementsComplete()
    {
        return CurrentPhase.LabourRequirements
            .Where(x => x.IsMandatoryForProjectCompletion)
            .All(x => LabourProgress[x] >= x.TotalProgressRequired);
    }

    protected bool AreMandatoryMaterialRequirementsComplete()
    {
        return CurrentPhase.MaterialRequirements
            .Where(x => x.IsMandatoryForProjectCompletion)
            .All(x => MaterialProgress[x] >= x.QuantityRequired);
    }

    protected bool AreCurrentPhaseCompletionRequirementsMet()
    {
        return AreMandatoryLabourRequirementsComplete() &&
               AreMandatoryMaterialRequirementsComplete();
    }

    protected IEnumerable<IProjectAction> OrderedCompletionActions()
    {
        return CurrentPhase.CompletionActions
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id);
    }

    protected void TryJoinQueuedProjectLabourFor(IEnumerable<ICharacter> characters)
    {
        foreach (var character in characters
                     .Where(x => x != null)
                     .DistinctBy(x => x.InstanceId)
                     .Where(x => x.CurrentProject.Project == null))
        {
            character.TryJoinQueuedProjectLabour();
        }
    }

    protected double? MandatoryMaterialCompletionRatio()
    {
        var mandatoryMaterials = CurrentPhase.MaterialRequirements
            .Where(x => x.IsMandatoryForProjectCompletion)
            .ToList();
        if (!mandatoryMaterials.Any())
        {
            return null;
        }

        var totalRequired = mandatoryMaterials.Sum(x => x.QuantityRequired);
        if (totalRequired <= 0.0)
        {
            return null;
        }

        return mandatoryMaterials.Sum(x => MaterialProgress[x]) / totalRequired;
    }

    public sealed override void Save()
    {
        Models.ActiveProject dbitem = FMDB.Context.ActiveProjects.Find(Id);
        dbitem.CurrentPhaseId = CurrentPhase.Id;
        dbitem.PaymentCurrencyId = _paymentCurrencyId ?? _paymentCurrency?.Id;
        FMDB.Context.ActiveProjectLabours.RemoveRange(dbitem.ActiveProjectLabours);
        foreach (IProjectLabourRequirement requirement in _labourProgress.Keys.Union(_labourPaymentRates.Keys))
        {
            ActiveProjectLabour dbprogress = new();
            dbitem.ActiveProjectLabours.Add(dbprogress);
            dbprogress.ProjectLabourRequirementsId = requirement.Id;
            dbprogress.Progress = _labourProgress[requirement];
            dbprogress.PaymentPerHour = LabourPaymentRateFor(requirement);
        }

        FMDB.Context.ActiveProjectMaterials.RemoveRange(dbitem.ActiveProjectMaterials);
        foreach (IProjectMaterialRequirement requirement in _materialProgress.Keys.Union(_materialPaymentRates.Keys))
        {
            ActiveProjectMaterial dbprogress = new();
            dbitem.ActiveProjectMaterials.Add(dbprogress);
            dbprogress.ProjectMaterialRequirementsId = requirement.Id;
            dbprogress.Progress = _materialProgress[requirement];
            dbprogress.PaymentPerUnit = MaterialPaymentRateFor(requirement);
        }

        Changed = false;
    }

    public sealed override object DatabaseInsert()
    {
        Models.ActiveProject dbitem = new();
        FMDB.Context.ActiveProjects.Add(dbitem);
        dbitem.CurrentPhaseId = CurrentPhase.Id;
        dbitem.ProjectId = ProjectDefinition.Id;
        dbitem.ProjectRevisionNumber = ProjectDefinition.RevisionNumber;
        dbitem.PaymentCurrencyId = _paymentCurrencyId ?? _paymentCurrency?.Id;
        foreach (IProjectLabourRequirement requirement in _labourProgress.Keys.Union(_labourPaymentRates.Keys))
        {
            ActiveProjectLabour dbprogress = new();
            dbitem.ActiveProjectLabours.Add(dbprogress);
            dbprogress.ProjectLabourRequirementsId = requirement.Id;
            dbprogress.Progress = _labourProgress[requirement];
            dbprogress.PaymentPerHour = LabourPaymentRateFor(requirement);
        }

        foreach (IProjectMaterialRequirement requirement in _materialProgress.Keys.Union(_materialPaymentRates.Keys))
        {
            ActiveProjectMaterial dbprogress = new();
            dbitem.ActiveProjectMaterials.Add(dbprogress);
            dbprogress.ProjectMaterialRequirementsId = requirement.Id;
            dbprogress.Progress = _materialProgress[requirement];
            dbprogress.PaymentPerUnit = MaterialPaymentRateFor(requirement);
        }

        DatabaseInsert(dbitem);
        return dbitem;
    }

    protected abstract void DatabaseInsert(MudSharp.Models.ActiveProject project);

    public sealed override void SetIDFromDatabase(object item)
    {
        Models.ActiveProject dbitem = (MudSharp.Models.ActiveProject)item;
        _id = dbitem.Id;
    }

    protected long _characterOwnerId;
    protected ICharacter _characterOwner;

    public ICharacter CharacterOwner
    {
        get
        {
            if (_characterOwner == null)
            {
                InitialiseCharacterOwner(false);
            }

            return _characterOwner;
        }
    }

    public virtual ICell Location { get; protected init; }

    public void DoLoad()
    {
        InitialiseCharacterOwner(true);
    }

    private void InitialiseCharacterOwner(bool viaSaveManager)
    {
        _characterOwner = Gameworld.TryGetCharacter(_characterOwnerId, true);
        CheckCachedLabour();
        if (!viaSaveManager)
        {
            Gameworld.SaveManager.AbortLazyLoad(this);
        }
    }

    public abstract string ProjectsCommandOutput(ICharacter actor);

    public virtual void DoProjectsTick()
    {
        double multiplier = Gameworld.GetStaticDouble("ProjectProgressMultiplier");
        var activeLabour = ActiveLabour.ToList();
        foreach ((ICharacter Character, IProjectLabourRequirement Labour) labour in activeLabour)
        {
            if (!ActiveLabour.Any(x => x.Character == labour.Character && x.Labour == labour.Labour))
            {
                continue;
            }

            var labourHours = 1.0 * multiplier;
            if (!CanPayLabourContribution(labour.Labour, labourHours, out var paymentError))
            {
                labour.Character.OutputHandler.Send(
                    $"You stop working on the {labour.Labour.Name.ColourName()} task of the {Name.ColourName()} project because {paymentError}.");
                Leave(labour.Character);
                continue;
            }

            var requiresPayment = LabourPaymentRateFor(labour.Labour) > 0.0M;
            var payment = AwardLabourPayment(labour.Character, labour.Labour, labourHours);
            if (requiresPayment && payment <= 0.0M)
            {
                labour.Character.OutputHandler.Send(
                    $"You stop working on the {labour.Labour.Name.ColourName()} task of the {Name.ColourName()} project because the project could not reserve your labour payment.");
                Leave(labour.Character);
                continue;
            }

            var currentActiveLabour = ActiveLabour.ToList();
            double supervisorMultiplier = currentActiveLabour.Aggregate(1.0,
                (sum, y) => sum * y.Labour.ProgressMultiplierForOtherLabourPerPercentageComplete(labour.Labour, this));
            var hourlyProgress = labour.Labour.HourlyProgress(labour.Character);
            var progress = hourlyProgress * multiplier * supervisorMultiplier;
            AgricultureProjectSkillTracker.RecordLabourTick(this, labour.Character, labour.Labour, labourHours,
                progress);
            if (FulfilLabour(labour.Labour, progress))
            {
                break;
            }
        }

        foreach ((ICharacter Character, IProjectLabourRequirement Labour) labour in ActiveLabour)
        {
            labour.Character.CurrentProjectHours += 1.0 * multiplier;
            labour.Character.CurrentProjectProjectHours += 1.0 * multiplier;
            foreach (ILabourImpactActionAtTick impact in labour.Labour.LabourImpacts.OfType<ILabourImpactActionAtTick>())
            {
                impact.DoAction(labour.Character, this, labour.Labour);
            }
        }
    }

    public virtual string ShowToPlayer(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Project #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"About: {ProjectDefinition.Tagline.ColourCommand()}");
        sb.AppendLine($"Current Phase: {CurrentPhase.Description.ColourCommand()} ({CurrentPhase.PhaseNumber.ToStringN0(actor)}/{ProjectDefinition.Phases.Count().ToStringN0(actor)})");
        if (PaymentCurrency != null)
        {
            sb.AppendLine($"Payment Reserve: {PaymentCurrency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
        }
        sb.AppendLine();
        sb.AppendLine("Phase Labour".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        CheckCachedLabour();
        if (CurrentPhase.LabourRequirements.Any())
        {
            foreach (IProjectLabourRequirement item in CurrentPhase.LabourRequirements)
            {
                sb.AppendLine();
                sb.AppendLine($"{item.Name.ColourName()} - {item.Description}");
                sb.AppendLine();
                sb.AppendLine($"Mandatory: {item.IsMandatoryForProjectCompletion.ToColouredString()}");
                sb.AppendLine($"Required Skill: {item.RequiredTrait?.Name.ColourValue() ?? "None".ColourError()}");
                sb.AppendLine($"Person-Hours: {item.TotalProgressRequiredForDisplay.ToStringN2Colour(actor)}");
                sb.AppendLine($"Payment: {DescribeLabourPayment(item, actor)}");
                var currentWorkers = _activeLabour.Count(x => x.Labour == item);
                var joinableBecauseNpc = currentWorkers >= item.MaximumSimultaneousWorkers &&
                                         HasDisplaceableNpcWorker(actor, item);
                sb.AppendLine($"Current Workers: {currentWorkers.ToStringN0Colour(actor)}/{item.MaximumSimultaneousWorkers.ToStringN0Colour(actor)}{(joinableBecauseNpc ? " (satisfied but joinable; an NPC can be displaced)".ColourCommand() : "")}");
                sb.AppendLine($"You Qualify: {item.CharacterIsQualified(actor).ToColouredString()}");
                sb.AppendLine($"Impacts: {item.LabourImpacts.Select(x => x.DescriptionForProjectsCommand.SubstituteANSIColour()).ListToString()}");
                if (_activeLabour.Any(x => x.Labour == item))
                {
                    sb.AppendLine();
                    sb.AppendLine("Active Workers:");
                    sb.AppendLine();
                    foreach ((ICharacter Character, IProjectLabourRequirement Labour) labour in _activeLabour.Where(x => x.Labour == item))
                    {
                        sb.AppendLine($"\t{labour.Character.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreCorpse | PerceiveIgnoreFlags.IgnoreLoadThings)}");
                    }
                }
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("\tNone");
        }

        sb.AppendLine();
        sb.AppendLine("Phase Material Requirements".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        if (CurrentPhase.MaterialRequirements.Any())
        {
            foreach (IProjectMaterialRequirement material in CurrentPhase.MaterialRequirements)
            {
                sb.AppendLine($"\t{material.ShowToPlayer(actor)} ({(_materialProgress[material] / material.QuantityRequired).ToStringP2Colour(actor)} complete, pay {DescribeMaterialPayment(material, actor)})");
            }
        }
        else
        {
            sb.AppendLine("\tNone");
        }
        return sb.ToString();
    }

    private string DescribeLabourPayment(IProjectLabourRequirement labour, ICharacter actor)
    {
        var rate = LabourPaymentRateFor(labour);
        return rate <= 0.0M || PaymentCurrency == null
            ? "None".ColourError()
            : $"{PaymentCurrency.Describe(rate, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per hour";
    }

    private string DescribeMaterialPayment(IProjectMaterialRequirement material, ICharacter actor)
    {
        var rate = MaterialPaymentRateFor(material);
        return rate <= 0.0M || PaymentCurrency == null
            ? "none".ColourError()
            : $"{PaymentCurrency.Describe(rate, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per unit";
    }

    #region Futureprogs

    public ProgVariableTypes Type => ProgVariableTypes.Project;
    public object GetObject => this;

    public virtual IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "id":
                return new NumberVariable(Id);
            case "name":
                return new TextVariable(Name);
            case "location":
                return Location;
            case "owner":
                return CharacterOwner;
            case "workers":
                return new CollectionVariable(ActiveLabour.Select(x => x.Character).ToList(),
                    ProgVariableTypes.Character);
        }

        throw new ApplicationException("There was an invalid property requested in ActiveProject.GetProperty: " +
                                       property);
    }

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "location", ProgVariableTypes.Location },
            { "owner", ProgVariableTypes.Character },
            { "workers", ProgVariableTypes.Character | ProgVariableTypes.Collection }
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", "" },
            { "name", "" },
            { "location", "" },
            { "owner", "" },
            { "workers", "" }
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Project, DotReferenceHandler(),
            DotReferenceHelp());
    }

    #endregion
}
