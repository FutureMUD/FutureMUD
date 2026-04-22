using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Health.Wounds;

public enum BoneFractureStage
{
    Trauma,
    Reaction,
    Relocation,
    Reparation,
    Ossification
}

public class BoneFracture : PerceivedItem, IImmobilisableWound
{
    private static bool DefaultsHaveBeenSetup;
    public static Dictionary<BoneFractureStage, double> StageBaseLengths { get; } = new();

    public static void SetupDefaults(IFuturemud gameworld)
    {
        StageBaseLengths[BoneFractureStage.Trauma] = gameworld.GetStaticDouble("BoneFractureStageLengthTrauma");
        StageBaseLengths[BoneFractureStage.Reaction] = gameworld.GetStaticDouble("BoneFractureStageLengthReaction");
        StageBaseLengths[BoneFractureStage.Relocation] = gameworld.GetStaticDouble("BoneFractureStageLengthRelocation");
        StageBaseLengths[BoneFractureStage.Reparation] = gameworld.GetStaticDouble("BoneFractureStageLengthReparation");
        StageBaseLengths[BoneFractureStage.Ossification] =
            gameworld.GetStaticDouble("BoneFractureStageLengthOssification");
        DefaultsHaveBeenSetup = true;
    }

    private BoneFracture(IFuturemud gameworld)
    {
        Gameworld = gameworld;
        if (!DefaultsHaveBeenSetup)
        {
            SetupDefaults(gameworld);
        }
    }

    public BoneFracture(IHaveWounds parent, Wound wound, IFuturemud gameworld) : this(gameworld)
    {
        _parent = parent as ICharacter;
        LoadFromDb(wound);
    }

    public BoneFracture(IFuturemud gameworld, IHaveWounds owner, double damage, double pain, double stun,
        DamageType damageType, IBodypart bodypart, IGameItem toolOrigin,
        ICharacter actorOrigin) : this(gameworld)
    {
        _parent = (ICharacter)owner ?? throw new ArgumentNullException(nameof(owner));
        CurrentDamage = Math.Min(damage * bodypart.DamageModifier, _parent.Body.HitpointsForBodypart(bodypart));
        OriginalDamage = Math.Min(damage * bodypart.DamageModifier, _parent.Body.HitpointsForBodypart(bodypart));
        CurrentPain = pain * bodypart.PainModifier;
        CurrentStun = stun * bodypart.StunModifier;
        DamageType = damageType;
        Bodypart = bodypart;
        _actorOriginId = actorOrigin?.Id ?? 0;
        _toolOriginId = toolOrigin?.Id ?? 0;
        if (actorOrigin?.Combat?.Friendly == true)
        {
            IsFriendlyWound = true;
        }

        Gameworld.SaveManager.AddInitialisation(this);
    }

    private void LoadFromDb(Wound wound)
    {
        _id = wound.Id;
        IdInitialised = true;
        _name = string.Empty;
        CurrentDamage = wound.CurrentDamage;
        OriginalDamage = wound.OriginalDamage;
        CurrentPain = wound.CurrentPain;
        CurrentStun = wound.CurrentStun;
        DamageType = (DamageType)wound.DamageType;
        Bodypart = Gameworld.BodypartPrototypes.Get(wound.BodypartProtoId ?? 0);
        _actorOriginId = wound.ActorOriginId ?? 0;
        _toolOriginId = wound.ToolOriginId ?? 0;

        XElement root = XElement.Parse(wound.ExtraInformation);
        XElement element = root.Element("Stage");
        if (element != null)
        {
            Stage = (BoneFractureStage)int.Parse(element.Value);
            HasBeenRelocated = Stage > BoneFractureStage.Relocation;
        }

        IsFriendlyWound = bool.Parse(root.Element("IsFriendlyWound")?.Value ?? "false");

        element = root.Element("FractureStageProgress");
        if (element != null)
        {
            FractureStageProgress = double.Parse(element.Value);
        }

        HasBeenSurgicallyReinforced = bool.Parse(root.Element("HasBeenSurgicallyReinforced")?.Value ?? "false");
    }

    public void SufferAdditionalDamage(IDamage damage)
    {
        double progress = StageBaseLengths[Stage] * damage.DamageAmount / OriginalDamage;
        FractureStageProgress -= progress;
        Changed = true;
        if (progress <= 0.0 || Stage < BoneFractureStage.Ossification)
        {
            CurrentDamage += damage.DamageAmount;
            OriginalDamage += damage.DamageAmount;
            SetStage(BoneFractureStage.Trauma);
            HasBeenRelocated = false;
            HasBeenSurgicallyReinforced = false;
            return;
        }
    }

    public void OnWoundSuffered()
    {
        if (Parent.AffectedBy<SupressWoundMessages>())
        {
            return;
        }

        if (Stage == BoneFractureStage.Trauma)
        {
            ICharacter cParent = (ICharacter)Parent;
            if (Severity >= WoundSeverity.Minor && Severity <= WoundSeverity.Moderate)
            {
                Parent.OutputHandler.Send(new EmoteOutput(
                    new Emote($"You are pretty sure that your {Bone.FullDescription()} has broken.", cParent),
                    style: OutputStyle.NoNewLine));
            }
            else if (Severity > WoundSeverity.Moderate)
            {
                IEnumerable<Tuple<IGameItem, IWearlocProfile>> worn = cParent.Body.WornItemsProfilesFor(
                    cParent.Body.Bodyparts.FirstOrDefault(
                        x => x.BoneInfo.Any(y => y.Key == Bone && y.Value.IsPrimaryInternalLocation)) ??
                    Bone);
                OutputRange range = OutputRange.Local;
                if (worn.Any(x => !x.Item2.HidesSeveredBodyparts && !x.Item2.Transparent))
                {
                    range = OutputRange.Personal;
                }

                string brokenDesc = "";
                switch (Severity)
                {
                    case WoundSeverity.Severe:
                        brokenDesc = "broken";
                        break;
                    case WoundSeverity.VerySevere:
                        brokenDesc = "broken badly, and is poking through the skin";
                        break;
                    case WoundSeverity.Grievous:
                        brokenDesc = "broken extremely badly, and has opened up the skin around it";
                        break;
                    case WoundSeverity.Horrifying:
                        brokenDesc = "been shattered into many pieces, cutting up the skin and flesh around it";
                        break;
                }

                Parent.OutputHandler.Handle(
                    new EmoteOutput(new Emote($"$0's {Bone.FullDescription()} has {brokenDesc}!", cParent),
                        style: OutputStyle.NoNewLine, flags: OutputFlags.Insigificant), range);
            }
        }
    }

    public bool ShouldWoundBeRemoved()
    {
        if (Stage == BoneFractureStage.Ossification &&
            FractureStageProgress >= StageBaseLengths[BoneFractureStage.Ossification])
        {
            return true;
        }

        return CurrentDamage <= 0.0 &&
               Infection == null;
    }

    public bool Repairable => false;
    public bool UseDamagePercentageSeverities => true;

    public string WoundTypeDescription => "Bone Fracture";

    public string TextForAdminWoundsCommand
    {
        get
        {
            StringBuilder sb = new();
            sb.Append("Fracture (");
            sb.Append(Stage.DescribeEnum());
            sb.Append(" ");
            sb.Append(FractureStagePercentage().ToStringP2Colour());
            sb.Append(")");
            if (HasBeenRelocated)
            {
                sb.Append(", ");
                sb.Append(Telnet.BoldCyan);
                sb.Append("Relocated");
                sb.Append(Telnet.RESETALL);
            }

            if (HasBeenSurgicallyReinforced)
            {
                sb.Append(", ");
                sb.Append(Telnet.BoldOrange);
                sb.Append("Surgically Reinforced");
                sb.Append(Telnet.RESETALL);

            }

            if (_parent?.Body.AffectedBy<AntiInflammatoryTreatment>(Bodypart) == true)
            {
                sb.Append(", ");
                sb.Append("Anti-Inflammatory".Colour(Telnet.BoldGreen));
            }

            return sb.ToString();
        }
    }

    public Difficulty ConcentrationDifficulty
    {
        get
        {
            Difficulty difficulty;
            switch (Severity)
            {
                case WoundSeverity.None:
                    difficulty = Difficulty.Automatic;
                    break;
                case WoundSeverity.Superficial:
                    difficulty = Difficulty.Trivial;
                    break;
                case WoundSeverity.Minor:
                    difficulty = Difficulty.ExtremelyEasy;
                    break;
                case WoundSeverity.Small:
                    difficulty = Difficulty.VeryEasy;
                    break;
                case WoundSeverity.Moderate:
                    difficulty = Difficulty.Easy;
                    break;
                case WoundSeverity.Severe:
                    difficulty = Difficulty.Normal;
                    break;
                case WoundSeverity.VerySevere:
                    difficulty = Difficulty.Hard;
                    break;
                case WoundSeverity.Grievous:
                    difficulty = Difficulty.VeryHard;
                    break;
                case WoundSeverity.Horrifying:
                    difficulty = Difficulty.ExtremelyHard;
                    break;
                default:
                    difficulty = Difficulty.Automatic;
                    break;
            }

            if (CurrentPain > CurrentDamage * Gameworld.GetStaticDouble("WoundPainfulRatioThreshold"))
            {
                difficulty = difficulty.StageUp(1);
            }

            return difficulty;
        }
    }

    public BoneFractureStage Stage { get; protected set; }

    public double FractureStageProgress { get; protected set; }

    public double FractureStagePercentage()
    {
        return FractureStageProgress / StageBaseLengths[Stage];
    }

    private void SetStage(BoneFractureStage newStage)
    {
        Stage = newStage;
        FractureStageProgress = 0.0;
        if (Stage > BoneFractureStage.Relocation)
        {
            HasBeenRelocated = true;
        }

        switch (Stage)
        {
            case BoneFractureStage.Trauma:
                CurrentPain = Gameworld.GetStaticDouble("BoneFractureTraumaPainLimit") * CurrentDamage;
                break;
            case BoneFractureStage.Reaction:
                CurrentPain = Gameworld.GetStaticDouble("BoneFractureReactionPainLimit") * CurrentDamage;
                break;
            case BoneFractureStage.Relocation:
                CurrentPain = Gameworld.GetStaticDouble("BoneFractureRelocationPainLimit") * CurrentDamage;
                break;
            case BoneFractureStage.Reparation:
                CurrentPain = Gameworld.GetStaticDouble("BoneFractureReparationPainLimit") * CurrentDamage;
                break;
            case BoneFractureStage.Ossification:
                CurrentPain = 0.0;
                break;
        }

        Changed = true;
    }

    public void AddFractureStageProgress(double percentage)
    {
        FractureStageProgress += StageBaseLengths[Stage] * percentage;
        if (Stage != BoneFractureStage.Ossification && FractureStageProgress >= StageBaseLengths[Stage])
        {
            SetStage(Stage + 1);
        }

        Changed = true;
    }

    private void EvaluateStageProgress()
    {
        if (Stage != BoneFractureStage.Ossification && FractureStageProgress >= StageBaseLengths[Stage])
        {
            double leftOverProgress = FractureStageProgress - StageBaseLengths[Stage];
            SetStage(Stage + 1);
            FractureStageProgress = leftOverProgress;
            EvaluateStageProgress();
        }
    }

    public bool HasBeenRelocated { get; protected set; }

    public bool HasBeenSurgicallyReinforced { get; protected set; }

    public IGameItem ImmobilisingItem { get; set; }

    #region Overrides of Item

    public override string FrameworkItemType => "Wound";

    #endregion

    #region Overrides of LateKeywordedInitialisingItem

    public override object DatabaseInsert()
    {
        if (Parent.Id == 0)
        {
            return null;
        }

        Wound dbitem = new();
        FMDB.Context.Wounds.Add(dbitem);
        dbitem.WoundType = "BoneFracture";
        dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
        dbitem.GameItemId = (Parent as IGameItem)?.Id;
        dbitem.OriginalDamage = OriginalDamage;
        dbitem.CurrentDamage = CurrentDamage;
        dbitem.CurrentPain = _currentPain;
        dbitem.DamageType = (int)DamageType;
        dbitem.BodypartProtoId = Bodypart?.Id;
        dbitem.LodgedItemId = Lodged?.Id;
        dbitem.ActorOriginId = _actorOriginId != 0 ? _actorOriginId : default(long?);
        dbitem.ToolOriginId = _toolOriginId != 0 ? _toolOriginId : default(long?);
        dbitem.ExtraInformation = SaveExtras();
        return dbitem;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((Wound)dbitem)?.Id ?? 0;
    }

    public string SaveExtras()
    {
        return new XElement("Definition",
            new XElement("Stage", (int)Stage),
            new XElement("FractureStageProgress", FractureStageProgress),
            new XElement("HasBeenSurgicallyReinforced", HasBeenSurgicallyReinforced),
            new XElement("IsFriendlyWound", IsFriendlyWound)
        ).ToString();
    }

    public override void Save()
    {
        using (new FMDB())
        {
            Wound dbitem = FMDB.Context.Wounds.Find(Id);
            if (dbitem != null)
            {
                dbitem.BodyId = (Parent as ICharacter)?.Body.Id;
                dbitem.GameItemId = (Parent as IGameItem)?.Id;
                dbitem.BodypartProtoId = Bodypart?.Id;
                dbitem.CurrentDamage = CurrentDamage;
                dbitem.OriginalDamage = OriginalDamage;
                dbitem.CurrentPain = _currentPain;
                dbitem.CurrentStun = CurrentStun;
                dbitem.ExtraInformation = SaveExtras();
            }
        }

        base.Save();
    }

    #endregion

    #region Overrides of PerceivedItem

    public override void Register(IOutputHandler handler)
    {
        // Do nothing
    }

    /// <summary>The FutureProgVariableType that represents this IFutureProgVariable</summary>
    public override ProgVariableTypes Type => ProgVariableTypes.Error;

    #endregion

    #region Implementation of IWound

    /// <summary>
    ///     Contains an IGameItem with the game item that is lodged in this wound. It will be null if there is no lodged
    ///     object.
    /// </summary>
    public IGameItem Lodged
    {
        get => null;
        set { }
    }

    public BleedStatus BleedStatus
    {
        get => BleedStatus.NeverBled;
        set { }
    }

    private double _currentPain;

    public double CurrentPain
    {
        get => _parent != null ? this.ApplyPainReduction(_parent, _currentPain) : _currentPain;
        set => _currentPain = value;
    }

    public double CurrentStun
    {
        get => 0.0;
        set { }
    }

    public double CurrentShock
    {
        get => 0.0;
        set { }
    }

    public double OriginalDamage { get; set; }
    public double CurrentDamage { get; set; }

    public WoundSeverity Severity => Parent.GetSeverityFor(this);
    public DamageType DamageType { get; protected set; }

    /// <summary>
    ///     Whether or not this is an internal wound
    /// </summary>
    public bool Internal => false;

    private ICharacter _parent;

    public IHaveWounds Parent
    {
        get => _parent;
        set => _parent = value as ICharacter;
    }

    public void SetNewOwner(IHaveWounds newOwner)
    {
        using (new FMDB())
        {
            Wound dbwound = FMDB.Context.Wounds.Find(Id);
            if (dbwound == null)
            {
                return;
            }

            dbwound.BodyId = (newOwner as ICharacter)?.Body.Id;
            dbwound.GameItemId = (newOwner as IGameItem)?.Id;
            FMDB.Context.SaveChanges();
        }

        _parent = newOwner as ICharacter;
    }

    public void RemapTo(IHaveWounds newOwner, IBodypart newBodypart, IBodypart newSeveredBodypart)
    {
        _parent = newOwner as ICharacter;
        Bodypart = newBodypart;
        SeveredBodypart = newSeveredBodypart;
        Infection?.RemapTo(_parent?.Body, this, newBodypart);
        Changed = true;
    }

    private IBodypart _bodypart;

    /// <summary>
    ///     The body part on which this wound has been inflicted, if any
    /// </summary>
    public IBodypart Bodypart
    {
        get => _bodypart;
        protected set
        {
            _bodypart = value;
            Bone = (IBone)value;
        }
    }

    public IBone Bone { get; protected set; }

    public IBodypart SeveredBodypart { get; set; }

    private long _toolOriginId;

    public IGameItem ToolOrigin
    {
        get => Gameworld.TryGetItem(_toolOriginId);
        set
        {
            _toolOriginId = value?.Id ?? 0;
            Changed = true;
        }
    }

    private long _actorOriginId;

    public ICharacter ActorOrigin
    {
        get => Gameworld.TryGetCharacter(_actorOriginId, true);
        set
        {
            _actorOriginId = value?.Id ?? 0;
            Changed = true;
        }
    }

    public IInfection Infection { get; set; }

    /// <summary>
    ///     Returns a string describing the current state of the wound
    /// </summary>
    /// <param name="type">The type of examination being performed</param>
    /// <param name="outcome">The outcome of the examination test</param>
    /// <returns>A string describing the current state of the wound</returns>
    public string Describe(WoundExaminationType type, Outcome outcome)
    {
        if ((type == WoundExaminationType.Glance && Severity < WoundSeverity.VerySevere) ||
            (type == WoundExaminationType.Look && Severity < WoundSeverity.Severe) ||
            (type == WoundExaminationType.Self && Severity < WoundSeverity.Small) ||
            (type == WoundExaminationType.Examination && Severity < WoundSeverity.Minor)
           )
        {
            return "";
        }

        if (type == WoundExaminationType.Glance && outcome.IsFail())
        {
            return "";
        }

        switch (type)
        {
            case WoundExaminationType.Glance:
            case WoundExaminationType.Look:
                if (HasBeenRelocated)
                {
                    return "";
                }

                goto case WoundExaminationType.Self;
            case WoundExaminationType.Self:
            case WoundExaminationType.Examination:
                if (Stage == BoneFractureStage.Ossification)
                {
                    switch (Severity)
                    {
                        case WoundSeverity.Grievous:
                        case WoundSeverity.Horrifying:
                            return "a former bad bone fracture (ossifying)";
                        case WoundSeverity.Superficial:
                        case WoundSeverity.Minor:
                            return "";
                    }

                    return "a former bone fracture (ossifying)";
                }

                if (Stage == BoneFractureStage.Reparation)
                {
                    switch (Severity)
                    {
                        case WoundSeverity.Minor:
                            return "a minor bone fracture (healing)";
                        case WoundSeverity.Small:
                            return "a partial bone fracture (healing)";
                        case WoundSeverity.Moderate:
                            return "a bone fracture (healing)";
                    }

                    return "a bad bone fracture (healing)";
                }

                switch (Severity)
                {
                    case WoundSeverity.Minor:
                        return "a minor bone fracture";
                    case WoundSeverity.Small:
                        return "a partial bone fracture";
                    case WoundSeverity.Moderate:
                        return "a total bone fracture";
                    case WoundSeverity.Severe:
                        return "a displaced, total bone fracture";
                    case WoundSeverity.VerySevere:
                        if (HasBeenRelocated)
                        {
                            return "a bad bone fracture";
                        }

                        return "a bad bone fracture with displacement";
                    case WoundSeverity.Grievous:
                        if (HasBeenRelocated)
                        {
                            return "a bad bone fracture";
                        }

                        return "an open bone fracture with major displacement";
                    case WoundSeverity.Horrifying:
                        if (HasBeenRelocated)
                        {
                            return "a bad bone fracture";
                        }

                        return "a horrifying complex bone fracture";
                }

                break;
        }

        // Surgery/Omni
        string severityDescription = "";
        switch (Severity)
        {
            case WoundSeverity.Superficial:
                severityDescription = $"hairline fractures";
                break;
            case WoundSeverity.Minor:
                severityDescription = $"a non-displaced minor fracture";
                break;
            case WoundSeverity.Small:
                severityDescription = $"a non-displaced major fracture";
                break;
            case WoundSeverity.Moderate:
                severityDescription = $"a non-displaced total fracture";
                break;
            case WoundSeverity.Severe:
                severityDescription = $"a closed, displaced fracture";
                break;
            case WoundSeverity.VerySevere:
                severityDescription = $"an open compound fracture";
                break;
            case WoundSeverity.Grievous:
                severityDescription = $"a major, open compound fracture";
                break;
            case WoundSeverity.Horrifying:
                severityDescription = $"a severe, open compound fracture with comminutation";
                break;
        }

        switch (Stage)
        {
            case BoneFractureStage.Trauma:
                return $"{severityDescription} in a state of recent trauma";
            case BoneFractureStage.Reaction:
                return $"{severityDescription} in the reactionary stage";
            case BoneFractureStage.Relocation:
                return $"{severityDescription} beginning to relocate itself";
            case BoneFractureStage.Reparation:
                return $"{severityDescription} in the reparation stage";
            case BoneFractureStage.Ossification:
                return $"{severityDescription} in the ossification stage";
        }

        return "";
    }

    /// <summary>
    ///     Returns a CheckDifficulty indicating the difficulty and/or possibility of treating this wound in a particular way
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Difficulty CanBeTreated(TreatmentType type)
    {
        if (Severity == WoundSeverity.None)
        {
            return Difficulty.Impossible;
        }

        switch (type)
        {
            case TreatmentType.Close:
            case TreatmentType.Remove:
            case TreatmentType.Repair:
            case TreatmentType.Trauma:
            case TreatmentType.Antiseptic:
            case TreatmentType.Clean:
                return Difficulty.Impossible;
            case TreatmentType.Tend:
                return Difficulty.Impossible;
            case TreatmentType.AntiInflammatory:
                if (CurrentPain <= 0.0)
                {
                    return Difficulty.Impossible;
                }

                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                    case WoundSeverity.Minor:
                        return Difficulty.Trivial;
                    case WoundSeverity.Small:
                    case WoundSeverity.Moderate:
                        return Difficulty.ExtremelyEasy;
                    case WoundSeverity.Severe:
                        return Difficulty.VeryEasy;
                    case WoundSeverity.VerySevere:
                        return Difficulty.Easy;
                    case WoundSeverity.Grievous:
                        return Difficulty.Normal;
                    case WoundSeverity.Horrifying:
                        return Difficulty.Hard;
                }

                break;
            case TreatmentType.Relocation:
                if (HasBeenRelocated)
                {
                    return Difficulty.Impossible;
                }

                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                    case WoundSeverity.Minor:
                    case WoundSeverity.Small:
                    case WoundSeverity.Moderate:
                        return Difficulty.Impossible;
                    case WoundSeverity.Severe:
                        return Difficulty.Easy;
                    case WoundSeverity.VerySevere:
                        return Difficulty.Normal;
                    case WoundSeverity.Grievous:
                        return Difficulty.Hard;
                    case WoundSeverity.Horrifying:
                        return Difficulty.VeryHard;
                }

                break;
            case TreatmentType.Set:
                if (!HasBeenRelocated)
                {
                    return Difficulty.Impossible;
                }

                if (ImmobilisingItem != null)
                {
                    return Difficulty.Impossible;
                }

                if (!Bone.CanBeImmobilised)
                {
                    return Difficulty.Impossible;
                }

                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                    case WoundSeverity.Minor:
                        return Difficulty.Impossible;
                    case WoundSeverity.Small:
                        return Difficulty.ExtremelyEasy;
                    case WoundSeverity.Moderate:
                        return Difficulty.VeryEasy;
                    case WoundSeverity.Severe:
                        return Difficulty.Easy;
                    case WoundSeverity.VerySevere:
                        return Difficulty.Normal;
                    case WoundSeverity.Grievous:
                        return Difficulty.Hard;
                    case WoundSeverity.Horrifying:
                        return Difficulty.VeryHard;
                }

                break;
            case TreatmentType.SurgicalSet:
                if (Stage == BoneFractureStage.Ossification)
                {
                    return Difficulty.Impossible;
                }

                if (HasBeenSurgicallyReinforced)
                {
                    return Difficulty.Impossible;
                }

                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                        return Difficulty.Impossible;
                    case WoundSeverity.Minor:
                        return Difficulty.Easy;
                    case WoundSeverity.Small:
                        return Difficulty.ExtremelyEasy;
                    case WoundSeverity.Moderate:
                        return Difficulty.VeryEasy;
                    case WoundSeverity.Severe:
                        return Difficulty.Easy;
                    case WoundSeverity.VerySevere:
                        return Difficulty.Normal;
                    case WoundSeverity.Grievous:
                        return Difficulty.Hard;
                    case WoundSeverity.Horrifying:
                        return Difficulty.VeryHard;
                }

                break;
        }

        Difficulty difficulty = Difficulty.Impossible;
        switch (Severity)
        {
            case WoundSeverity.Superficial:
                difficulty = Difficulty.Automatic;
                break;
            case WoundSeverity.Minor:
                difficulty = Difficulty.Trivial;
                break;
            case WoundSeverity.Small:
                difficulty = Difficulty.ExtremelyEasy;
                break;
            case WoundSeverity.Moderate:
                difficulty = Difficulty.VeryEasy;
                break;
            case WoundSeverity.Severe:
                difficulty = Difficulty.Easy;
                break;
            case WoundSeverity.VerySevere:
                difficulty = Difficulty.Normal;
                break;
            case WoundSeverity.Grievous:
                difficulty = Difficulty.Hard;
                break;
            case WoundSeverity.Horrifying:
                difficulty = Difficulty.VeryHard;
                break;
        }

        return difficulty;
    }

    /// <summary>
    ///     Returns a string designed to be returned as an error message as to why a particular kind of treatment cannot be
    ///     performed on this wound
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string WhyCannotBeTreated(TreatmentType type)
    {
        if (Severity == WoundSeverity.None)
        {
            return "That wound is simply not serious enough to require any kind of treatment.";
        }

        switch (type)
        {
            case TreatmentType.Close:
            case TreatmentType.Remove:
            case TreatmentType.Repair:
            case TreatmentType.Trauma:
            case TreatmentType.Antiseptic:
            case TreatmentType.Clean:
                return "That kind of treatment is not effective for that wound.";
            case TreatmentType.AntiInflammatory:
                if (CurrentPain <= 0.0)
                {
                    return "That fracture is not currently causing any pain that anti-inflammatory treatment would improve.";
                }

                break;
            case TreatmentType.Relocation:
                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                    case WoundSeverity.Minor:
                    case WoundSeverity.Small:
                    case WoundSeverity.Moderate:
                        return
                            "That fracture does not have any substantial displacement and does not require relocation.";
                }

                if (HasBeenRelocated)
                {
                    return "That break has already been relocated.";
                }

                break;
            case TreatmentType.Set:
                if (!HasBeenRelocated)
                {
                    return "You must first relocate the bone before you immobilise the fracture.";
                }

                if (ImmobilisingItem != null)
                {
                    return "That fracture has already been immobilised.";
                }

                if (!Bone.CanBeImmobilised)
                {
                    return "That bone is not one that can be immobilised.";
                }

                switch (Severity)
                {
                    case WoundSeverity.Superficial:
                    case WoundSeverity.Minor:
                        return
                            "That wound is not so severe as to require immobilisation and will heal just as well without it.";
                }

                break;
            case TreatmentType.SurgicalSet:
                if (Severity == WoundSeverity.Superficial)
                {
                    return
                        "The fracturing of that bone is simply too minor to benefit from any intervention; the situation would be made worse by it.";
                }

                if (Stage == BoneFractureStage.Ossification)
                {
                    return
                        "The fracture has progressed too far with natural healing for surgical intervention to be of any benefit.";
                }

                if (HasBeenSurgicallyReinforced)
                {
                    return "The fracture has already been surgically reinforced and set.";
                }

                break;
            default:
                return "That wound cannot be treated in that way.";
        }

        throw new ApplicationException("Unknown WhyCannotTreat reason in BoneFracture.");
    }

    /// <summary>
    ///     Performs a treatment on the item, with the boolean representing whether or not it was successful
    /// </summary>
    /// <param name="treater"></param>
    /// <param name="type"></param>
    /// <param name="treatmentItem"></param>
    /// <param name="testOutcome"></param>
    /// <param name="silent"></param>
    public void Treat(IPerceiver treater, TreatmentType type, ITreatment treatmentItem, Outcome testOutcome,
        bool silent)
    {
        switch (type)
        {
            case TreatmentType.Mend:
                if (testOutcome.IsFail())
                {
                    if (treater != null && !silent)
                    {
                        treater.OutputHandler.Handle(new EmoteOutput(new Emote(
                            $"$0's efforts to treat {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} have been unsuccessful.",
                            treater, treater)));
                    }
                }
                else
                {
                    if (treater != null && !silent)
                    {
                        treater.OutputHandler.Handle(new EmoteOutput(new Emote(
                            $"$0's efforts to mend {Describe(WoundExaminationType.Glance, Outcome.MajorPass).Colour(Telnet.Cyan)} have been {(testOutcome == Outcome.MajorPass ? "majorly" : testOutcome == Outcome.Pass ? "" : "marginally")} successful.",
                            treater, treater)));
                    }

                    CurrentDamage = Parent.GetSeverityFloor(Severity.StageDown(testOutcome.SuccessDegrees()), true) *
                                    _parent.Body.HitpointsForBodypart(Bone);
                    _currentPain = Math.Min(_currentPain, CurrentDamage);
                    CurrentStun = Math.Min(CurrentStun, CurrentDamage);

                    switch (Stage)
                    {
                        case BoneFractureStage.Trauma:
                        case BoneFractureStage.Reaction:
                        case BoneFractureStage.Relocation:
                            SetStage(BoneFractureStage.Reparation);
                            break;
                        case BoneFractureStage.Reparation:
                        case BoneFractureStage.Ossification:
                            AddFractureStageProgress(
                                Gameworld.GetStaticDouble("BoneFractureMendProgressPerSuccessDegree") *
                                testOutcome.SuccessDegrees());
                            break;
                    }
                }

                return;
            case TreatmentType.AntiInflammatory:
                {
                    int strength = testOutcome switch
                    {
                        Outcome.MajorPass => 4,
                        Outcome.Pass => 3,
                        Outcome.MinorPass => 2,
                        Outcome.MinorFail => 1,
                        _ => 0
                    };

                    treatmentItem?.UseTreatment();
                    if (strength <= 0)
                    {
                        if (treater != null && !silent)
                        {
                            treater.OutputHandler.Handle(new EmoteOutput(new Emote(
                                $"$0's efforts to reduce the inflammation around {Describe(WoundExaminationType.Look, Outcome.MajorPass).Colour(Telnet.Cyan)} have been unsuccessful.",
                                treater, treater)));
                        }

                        return;
                    }

                    AntiInflammatoryTreatment.ApplyOrUpdate(_parent.Body, Bodypart,
                        Math.Max(Gameworld.GetStaticDouble("AntiInflammatoryMinimumPainMultiplier"),
                            1.0 - strength * Gameworld.GetStaticDouble("AntiInflammatoryPainMultiplierReductionPerStrength")),
                        Math.Max(Gameworld.GetStaticDouble("AntiInflammatoryMinimumFlatReduction"),
                            strength * (int)Severity * Gameworld.GetStaticDouble("AntiInflammatoryBoneFlatReductionPerSeverity")),
                        TimeSpan.FromMinutes(
                            Gameworld.GetStaticDouble("AntiInflammatoryBaseDurationMinutes") +
                            strength * Gameworld.GetStaticDouble("AntiInflammatoryDurationMinutesPerStrength")));
                    if (treater != null && !silent)
                    {
                        treater.OutputHandler.Handle(new EmoteOutput(new Emote(
                            treatmentItem == null
                                ? $"$0 finish|finishes applying anti-inflammatory treatment to {Describe(WoundExaminationType.Look, Outcome.MajorPass).Colour(Telnet.Cyan)}."
                                : $"$0 finish|finishes applying anti-inflammatory treatment to {Describe(WoundExaminationType.Look, Outcome.MajorPass).Colour(Telnet.Cyan)} with $1.",
                            treater, treater, treatmentItem?.Parent)));
                    }

                    return;
                }
            case TreatmentType.Relocation:
                if (testOutcome.IsFail())
                {
                    if (!silent)
                    {
                        treater?.OutputHandler.Handle(new EmoteOutput(new Emote(
                                $"$0's efforts to relocate {Describe(WoundExaminationType.Look, Outcome.MajorPass).Colour(Telnet.Cyan)} has been {(testOutcome == Outcome.MajorPass ? "majorly" : testOutcome == Outcome.Pass ? "" : "marginally")} unsuccessful.",
                                treater, treater)));
                    }

                    if (testOutcome != Outcome.MinorFail)
                    {
                        Parent.OutputHandler.Send(
                            $"The pain levels in your {Bodypart.FullDescription()} shoot up {(testOutcome == Outcome.MajorFail ? "enormously" : "substantially")} due to the unsuccessful treatment."
                                .Colour(Telnet.Red));
                    }
                    // TODO - temporary pain?
                }
                else
                {
                    if (!silent)
                    {
                        if (treater is not null)
                        {
                            treater.OutputHandler.Handle(new EmoteOutput(new Emote(
                                $"$0's efforts to relocate {Describe(WoundExaminationType.Look, Outcome.MajorPass).Colour(Telnet.Cyan)} has been {(testOutcome == Outcome.MajorPass ? "majorly" : testOutcome == Outcome.Pass ? "" : "marginally")} successful.",
                                treater, treater)));
                        }
                        else
                        {
                            Parent.OutputHandler.Handle(new EmoteOutput(new Emote($"{Describe(WoundExaminationType.Look, Outcome.MajorPass).Strip_A_An().The(true).Colour(Telnet.Cyan)} on $0's {Bodypart.FullDescription()} straightens up and re-sets itself.", _parent, _parent)));
                        }

                    }

                    SetStage(BoneFractureStage.Reparation);
                    switch (testOutcome)
                    {
                        case Outcome.Pass:
                            AddFractureStageProgress(Gameworld.GetStaticDouble("BoneFractureRelocationProgressPass"));
                            break;
                        case Outcome.MajorPass:
                            AddFractureStageProgress(
                                Gameworld.GetStaticDouble("BoneFractureRelocationProgressMajorPass"));
                            break;
                    }

                    Changed = true;
                }

                return;
            case TreatmentType.Set:
                throw new ApplicationException(
                    "TreatmentType.Set should not be called directly in IWound.Treat; instead, the immobilisation item should be set.");
            case TreatmentType.SurgicalSet:
                HasBeenSurgicallyReinforced = true;
                Changed = true;
                if (Stage < BoneFractureStage.Reparation)
                {
                    SetStage(BoneFractureStage.Reparation);
                }

                // TODO - consequences for failure
                return;
        }
    }

    /// <summary>
    ///     Performs a bleed tick and returns the amount of blood lost in ml
    /// </summary>
    /// <returns></returns>
    public BleedResult Bleed(double currentBloodLitres, ExertionLevel activityExertionLevel,
        double totalBloodLitres)
    {
        return BleedResult.NoBleed;
    }

    /// <summary>
    ///     Returns the amount of blood that wound be lost if this wound bled in ml
    /// </summary>
    /// <param name="bloodTotal"></param>
    /// <param name="activityExertionLevel"></param>
    /// <returns></returns>
    public double PeekBleed(double bloodTotal, ExertionLevel activityExertionLevel)
    {
        return 0.0;
    }

    /// <summary>
    ///     Performs a check to see if there is a chance the wound opens up, gets worse etc. with exertion, and returns the
    ///     pain
    /// </summary>
    /// <param name="exertion">The ExertionType representing the level of exersion this wound is exposed to</param>
    /// <returns>An number representing the pain of exertion</returns>
    public double Exert(ExertionType exertion)
    {
        return 0.0;
    }

    private double GetSeverityHealingMultiplier()
    {
        switch (Severity)
        {
            case WoundSeverity.None:
            case WoundSeverity.Superficial:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierSuperficial");
            case WoundSeverity.Minor:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierMinor");
            case WoundSeverity.Small:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierSmall");
            case WoundSeverity.Moderate:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierModerate");
            case WoundSeverity.Severe:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierSevere");
            case WoundSeverity.VerySevere:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierVerySevere");
            case WoundSeverity.Grievous:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierGrievous");
            case WoundSeverity.Horrifying:
                return Gameworld.GetStaticDouble("BoneFractureHealingMultiplierHorrifying");
            default:
                return 1.0;
        }
    }

    private double GetHealingProgressAmount(double externalRateMultiplier)
    {
        if (Infection != null)
        {
            return 0.0;
        }

        if (Stage == BoneFractureStage.Relocation &&
            Severity == WoundSeverity.Horrifying &&
            !HasBeenSurgicallyReinforced)
        {
            return 0.0;
        }

        return
            Gameworld.GetStaticDouble("BaseBoneHealingPerMinute") *
            Bone.BoneHealingModifier *
            GetSeverityHealingMultiplier() *
            (ImmobilisingItem != null ? Gameworld.GetStaticDouble("BoneFractureHealingImmobilisedMultiplier") : 1.0) *
            (HasBeenSurgicallyReinforced ? Gameworld.GetStaticDouble("BoneFractureHealingSurgicalMultiplier") : 1.0) *
            externalRateMultiplier;
        ;
    }

    /// <summary>
    ///     Performs a healing tick and returns the new pain of the wound
    /// </summary>
    /// <returns></returns>
    public bool HealingTick(double externalRateMultiplier, double externalCheckBonus)
    {
        double amount = GetHealingProgressAmount(externalRateMultiplier);
        FractureStageProgress += amount;
        EvaluateStageProgress();
        Changed = true;
        CheckInfection();
        return amount > 0.0;
    }

    public bool EligableForInfection()
    {
        if (_parent == null)
        {
            return false;
        }

        if (Infection != null)
        {
            return false;
        }

        if (Stage > BoneFractureStage.Relocation)
        {
            return false;
        }

        if (Severity <= WoundSeverity.VerySevere)
        {
            return false;
        }

        return _parent.Body.EffectsOfType<IAntisepticTreatmentEffect>().All(x => x.Bodypart != Bodypart);
    }

    private double GetInfectionChanceDamageMultiplier()
    {
        return DamageType switch
        {
            DamageType.Ballistic => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierBallistic"),
            DamageType.Bite => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierBite"),
            DamageType.Burning => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierBurning"),
            DamageType.Chemical => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierChemical"),
            DamageType.Chopping => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierChopping"),
            DamageType.Claw => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierClaw"),
            DamageType.Freezing => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierFreezing"),
            DamageType.Piercing => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierPiercing"),
            DamageType.Slashing => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierSlashing"),
            DamageType.Hypoxia => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierHypoxia"),
            DamageType.Cellular => Gameworld.GetStaticDouble("InfectionChanceDamageMultiplierCellular"),
            _ => 1.0
        };
    }

    private double GetInfectionChanceSeverityMultiplier()
    {
        return Severity switch
        {
            WoundSeverity.VerySevere => Gameworld.GetStaticDouble("BoneFractureInfectionSeverityMultiplierVerySevere"),
            WoundSeverity.Grievous => Gameworld.GetStaticDouble("BoneFractureInfectionSeverityMultiplierGrievous"),
            WoundSeverity.Horrifying => Gameworld.GetStaticDouble("BoneFractureInfectionSeverityMultiplierHorrifying"),
            _ => 1.0
        };
    }

    /// <summary>
    ///     Called in the health heartbeat to determine whether an uninfected wound becomes infected, and also to trigger
    ///     subsequent processing if there is an existing infection
    /// </summary>
    private void CheckInfection()
    {
        if (_parent == null)
        {
            return;
        }

        Infection?.InfectionTick();
        if (Infection?.InfectionHealed() == true)
        {
            Infection.Delete();
            Infection = null;
            return;
        }

        if (!EligableForInfection())
        {
            return;
        }

        double chance = Gameworld.GetStaticDouble("BaseInfectionChanceBoneFractures");
        chance *= GetInfectionChanceDamageMultiplier();

        Construction.ITerrain terrain = _parent.Location.Terrain(_parent);
        chance *= terrain.InfectionMultiplier;

        if (Severity <= WoundSeverity.Severe)
        {
            return;
        }

        chance *= GetInfectionChanceSeverityMultiplier();

        if (RandomUtilities.DoubleRandom(0.0, 1.0) > chance)
        {
            return;
        }

        ICharacter charParent = (ICharacter)Parent;
        Difficulty virulence =
            charParent.Merits.OfType<IInfectionResistanceMerit>()
                      .Where(x => x.Applies(charParent))
                      .Select(x => x.GetNewInfectionDifficulty(terrain.InfectionVirulence, terrain.PrimaryInfection))
                      .DefaultIfEmpty(terrain.InfectionVirulence)
                      .Min();
#if DEBUG
        if (virulence != terrain.InfectionVirulence)
        {
            Console.WriteLine(
                $"Infection Virulance Changed by Merits - Original {terrain.InfectionVirulence.Describe()} New {virulence.Describe()}.");
        }
#endif
        Infection = Infections.Infection.LoadNewInfection(terrain.PrimaryInfection, virulence,
            Gameworld.GetStaticDouble("BaseInfectionInitialIntensity"), _parent.Body, this, Bodypart,
            terrain.InfectionMultiplier);
        Changed = true;
#if DEBUG
        Console.WriteLine(
            $"{_parent.HowSeen(_parent, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} on {Describe(WoundExaminationType.Look, Outcome.MajorPass)} has become infected with {terrain.PrimaryInfection} {terrain.InfectionVirulence}.");
#endif
    }

    public void Delete(bool ignoreDatabaseDeletion = false)
    {
        _noSave = true;
        Changed = false;
        Gameworld.SaveManager.Abort(this);
        Infection?.Delete();
        if (!IdInitialised || ignoreDatabaseDeletion)
        {
            return;
        }

        using (new FMDB())
        {
            Gameworld.SaveManager.Flush();
            Wound dbitem = FMDB.Context.Wounds.Find(Id);
            if (dbitem != null)
            {
                FMDB.Context.Wounds.Remove(dbitem);
                FMDB.Context.SaveChanges();
            }
            //FMDB.Context.Entry(new MudSharp.Models.Wound{Id = _ID}).State = EntityState.Deleted;
            //FMDB.Context.SaveChanges();
        }
    }

    public void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus)
    {
        if (Infection != null || ShouldWoundBeRemoved())
        {
            return;
        }

        EvaluateStageProgress(); // Just in case we're already sitting at or above a threshold

        while (!ShouldWoundBeRemoved())
        {
            double amount = timePassed.TotalMinutes * GetHealingProgressAmount(externalRateMultiplier);
            if (amount < StageBaseLengths[Stage] - FractureStageProgress)
            {
                FractureStageProgress += amount;
                break;
            }

            timePassed =
                TimeSpan.FromTicks(
                    timePassed.Ticks * (long)((StageBaseLengths[Stage] - FractureStageProgress) / amount));
            FractureStageProgress = StageBaseLengths[Stage];
            EvaluateStageProgress();
        }
    }

    public bool IsFriendlyWound { get; protected set; }

    #endregion
}
