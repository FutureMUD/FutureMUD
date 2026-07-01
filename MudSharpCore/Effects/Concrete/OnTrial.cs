using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public enum TrialPhase
{
    AwaitingLawyers,
    Introduction,
    Charges,
    Plea,
    Case,
    ClosingArguments,
    Verdict,
    Sentencing
}

#nullable enable
public class OnTrial : Effect, IEffect
{
    private DateTime _lastTrialAction;

    public ILegalAuthority LegalAuthority { get; set; } = null!;

    public DateTime LastTrialAction
    {
        get => _lastTrialAction;
        set
        {
            _lastTrialAction = value;
            Changed = true;
        }
    }

    private TrialPhase _phase;

    public TrialPhase Phase
    {
        get => _phase;
        set
        {
            _phase = value;
            Changed = true;
        }
    }

    private bool _manualTrial;

    public bool ManualTrial
    {
        get => _manualTrial;
        set
        {
            _manualTrial = value;
            Changed = true;
        }
    }

    private readonly List<ICrime> _crimes;
    public Queue<ICrime> CrimeQueue { get; private set; } = null!;

    public void ResetCrimeQueue()
    {
        CrimeQueue = new Queue<ICrime>(_crimes);
    }

    public IEnumerable<ICrime> Crimes => _crimes;

    private readonly Dictionary<ICrime, bool> _pleas = new();
    public IDictionary<ICrime, bool> Pleas => _pleas;

    private readonly Dictionary<ICrime, IReadOnlyDictionary<Difficulty, CheckOutcome>> _crimeDefenseCases = new();
    private readonly Dictionary<ICrime, IReadOnlyDictionary<Difficulty, CheckOutcome>> _crimeProsecutionCases = new();

    private readonly Dictionary<ICrime, PunishmentResult> _punishments = new();
    public IDictionary<ICrime, PunishmentResult> Punishments => _punishments;

    private ICharacter? _prosecutor;
    private long? _prosecutorId;

    public ICharacter? Prosecutor
    {
        get => _prosecutor ??= Gameworld.TryGetCharacter(_prosecutorId ?? 0, true);
        set
        {
            _prosecutor = value;
            _prosecutorId = value?.Id;
        }
    }

    private ICharacter? _defender;
    private long? _defenderId;

    public ICharacter? Defender
    {
        get => _defender ??= Gameworld.TryGetCharacter(_defenderId ?? 0, true);
        set
        {
            _defender = value;
            _defenderId = value?.Id;
        }
    }

    public IReadOnlyDictionary<Difficulty, CheckOutcome> ArgueCaseDefender(ICharacter lawyer, ICrime crime)
    {
        _crimeDefenseCases[crime] = Gameworld.GetCheck(CheckType.DefendLegalCase).CheckAgainstAllDifficulties(lawyer, crime.DefenseDifficulty, null);
        return _crimeDefenseCases[crime];
    }

    public IReadOnlyDictionary<Difficulty, CheckOutcome> ArgueCaseProsecution(ICharacter lawyer, ICrime crime)
    {
        _crimeProsecutionCases[crime] = Gameworld.GetCheck(CheckType.ProsecuteLegalCase).CheckAgainstAllDifficulties(lawyer, crime.ProsecutionDifficulty, null);
        return _crimeProsecutionCases[crime];
    }

    public ICrime? NextDefenseCrime()
    {
        return _crimes.FirstOrDefault(x => !_crimeDefenseCases.ContainsKey(x) || _crimeDefenseCases[x][Difficulty.Automatic].Outcome == Outcome.NotTested);
    }

    public ICrime? NextProsecutionCrime()
    {
        return _crimes.FirstOrDefault(x => !_crimeProsecutionCases.ContainsKey(x) || _crimeProsecutionCases[x][Difficulty.Automatic].Outcome == Outcome.NotTested);
    }

    public IReadOnlyDictionary<Difficulty, CheckOutcome> GetResultDefense(ICrime crime)
    {
        return _crimeDefenseCases.TryGetValue(crime, out IReadOnlyDictionary<Difficulty, CheckOutcome>? @case) ? @case : CheckOutcome.NotTestedAllDifficulties(CheckType.DefendLegalCase);
    }

    public IReadOnlyDictionary<Difficulty, CheckOutcome> GetResultProsecution(ICrime crime)
    {
        return _crimeProsecutionCases.TryGetValue(crime, out IReadOnlyDictionary<Difficulty, CheckOutcome>? @case) ? @case : CheckOutcome.NotTestedAllDifficulties(CheckType.ProsecuteLegalCase);
    }

    public OpposedOutcome GetOutcome(ICrime crime)
    {
        return new OpposedOutcome(_crimeProsecutionCases[crime], _crimeDefenseCases[crime], crime.ProsecutionDifficulty, crime.DefenseDifficulty);
    }

    public ICrime? NextCrime()
    {
        Changed = true;
        if (CrimeQueue.Count == 0)
        {
            return null;
        }

        return CrimeQueue.Dequeue();
    }

    public int ChargeNumber(ICrime crime)
    {
        return _crimes.IndexOf(crime) + 1;
    }

    public ICrime? CurrentPleaCrime
    {
        get
        {
            ICharacter? defendant = Owner as ICharacter;
            return defendant?.EffectsOfType<ConsideringPlea>(x => x.LegalAuthority == LegalAuthority)
                             ?.FirstOrDefault()
                             ?.Crime;
        }
    }

    public bool HasPleaBeenEntered(ICrime crime)
    {
        if (ManualTrial || Phase < TrialPhase.Plea)
        {
            return false;
        }

        if (Phase > TrialPhase.Plea)
        {
            return true;
        }

        return !CrimeQueue.Contains(crime) && CurrentPleaCrime != crime;
    }

    public bool HasProsecutionArgumentBeenPresented(ICrime crime)
    {
        return GetResultProsecution(crime)[crime.ProsecutionDifficulty].Outcome != Outcome.NotTested;
    }

    public bool HasDefenseArgumentBeenPresented(ICrime crime)
    {
        return GetResultDefense(crime)[crime.DefenseDifficulty].Outcome != Outcome.NotTested;
    }

    public bool HasVerdictBeenAnnounced(ICrime crime)
    {
        if (ManualTrial)
        {
            return crime.HasBeenFinalised;
        }

        if (Phase < TrialPhase.Verdict)
        {
            return false;
        }

        if (Phase > TrialPhase.Verdict)
        {
            return true;
        }

        return !CrimeQueue.Contains(crime);
    }

    public bool HasSentenceBeenAnnounced(ICrime crime)
    {
        if (ManualTrial)
        {
            return crime.HasBeenFinalised;
        }

        if (!HasVerdictBeenAnnounced(crime) || Phase < TrialPhase.Sentencing)
        {
            return false;
        }

        if (!_punishments.ContainsKey(crime))
        {
            return true;
        }

        return !CrimeQueue.Contains(crime);
    }

    public bool CasesFinishedArguing()
    {
        return NextDefenseCrime() is null && NextProsecutionCrime() is null;
    }

    public void HandleArgueCommand(ICharacter actor, bool defense)
    {
        ICrime? crime = defense ? NextDefenseCrime() : NextProsecutionCrime();
        if (crime is null)
        {
            actor.OutputHandler.Send($"The {(defense ? "defense" : "prosecution")} has finished making its case for all crimes.");
            return;
        }

        IReadOnlyDictionary<Difficulty, CheckOutcome> result = defense ? ArgueCaseDefender(actor, crime) : ArgueCaseProsecution(actor, crime);
        CheckOutcome primary = result[defense ? crime.DefenseDifficulty : crime.ProsecutionDifficulty];
        string effectiveness = "";
        if (primary.IsAbjectFailure)
        {
            effectiveness = "The legal arguments are complete nonsense and utterly ineffective.".Colour(Telnet.BoldMagenta);
        }
        else
        {
            switch (primary.Outcome)
            {
                case Outcome.MajorFail:
                    effectiveness = "The case put forward is fairly weak.".Colour(Telnet.Red);
                    break;
                case Outcome.Fail:
                    effectiveness = "The case put forward is weak, but still plausible.".Colour(Telnet.Orange);
                    break;
                case Outcome.MinorFail:
                    effectiveness = "The case put forward is convincing, but has some minor flaws.".Colour(Telnet.Yellow);
                    break;
                case Outcome.MinorPass:
                    effectiveness = "The case put forward is convincing.".Colour(Telnet.Green);
                    break;
                case Outcome.Pass:
                    effectiveness = "The case put forward is solid and well supported.".Colour(Telnet.BoldGreen);
                    break;
                case Outcome.MajorPass:
                    effectiveness = "The case put forward is extremely robust and almost air-tight.".Colour(Telnet.BoldCyan);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ argue|argues the {(defense ? "defense" : "prosecution")} case for the {ChargeNumber(crime).ToOrdinal()} charge of {crime.Name}.\n{effectiveness}", actor, actor, Owner)));
    }

    #region Static Initialisation

    public static void InitialiseEffectType()
    {
        RegisterFactory("OnTrial", (effect, owner) => new OnTrial(effect, owner));
    }

    #endregion

    #region Constructors

    public OnTrial(ICharacter owner, ILegalAuthority legalAuthority, DateTime lastTrialAction,
        IEnumerable<ICrime> crimes, bool manualTrial = false) : base(owner, null)
    {
        LegalAuthority = legalAuthority;
        _lastTrialAction = lastTrialAction;
        _manualTrial = manualTrial;
        _crimes = crimes.ToList();
        foreach (ICrime crime in _crimes)
        {
            _pleas[crime] = true;
            _crimeDefenseCases[crime] = CheckOutcome.NotTestedAllDifficulties(CheckType.DefendLegalCase);
            _crimeProsecutionCases[crime] = CheckOutcome.NotTestedAllDifficulties(CheckType.ProsecuteLegalCase);
        }
        ResetCrimeQueue();
        _phase = TrialPhase.AwaitingLawyers;
    }

    protected OnTrial(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement? root = effect.Element("Effect");
        LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root!.Element("LegalAuthority")!.Value))!;
        _lastTrialAction = DateTime.Parse(root.Element("LastTrialAction")!.Value, null,
            System.Globalization.DateTimeStyles.RoundtripKind);
        _crimes = [];
        foreach (XElement item in root.Element("Crimes")!.Elements())
        {
            ICrime? crime = Gameworld.Crimes.Get(long.Parse(item.Attribute("id")!.Value));
            if (crime is null)
            {
                continue;
            }
            _crimes.Add(crime);
            _pleas[crime] = bool.Parse(item.Attribute("plea")!.Value);
            Dictionary<Difficulty, CheckOutcome> defenseOutcome = new();
            foreach (XElement result in item.Element("Defense")!.Elements())
            {
                defenseOutcome[(Difficulty)int.Parse(result.Attribute("difficulty")!.Value)] = CheckOutcome.SimpleOutcome(CheckType.DefendLegalCase, (Outcome)int.Parse(result.Attribute("outcome")!.Value));
            }
            _crimeDefenseCases[crime] = defenseOutcome;
            Dictionary<Difficulty, CheckOutcome> prosecutionOutcome = new();
            foreach (XElement result in item.Element("Prosecution")!.Elements())
            {
                prosecutionOutcome[(Difficulty)int.Parse(result.Attribute("difficulty")!.Value)] = CheckOutcome.SimpleOutcome(CheckType.ProsecuteLegalCase, (Outcome)int.Parse(result.Attribute("outcome")!.Value));
            }
            _crimeProsecutionCases[crime] = prosecutionOutcome;
            XElement? punishment = item.Element("Punishment");
            if (punishment is not null)
            {
                _punishments[crime] = LoadPunishmentResult(punishment);
            }
        }
        LoadCrimeQueue(root.Element("CrimeQueue"));
        _phase = (TrialPhase)int.Parse(root.Element("Phase")?.Value ?? "0");
        _manualTrial = bool.Parse(root.Element("ManualTrial")?.Value ?? "false");
    }

    #endregion

    #region Saving and Loading

    private static XElement SavePunishmentResult(PunishmentResult result)
    {
        return new XElement("Punishment",
            new XElement("Execution", result.Execution),
            new XElement("Fine", result.Fine.ToString(CultureInfo.InvariantCulture)),
            new XElement("CustodialSentence", new XCData(result.CustodialSentence.GetRoundTripParseText)),
            new XElement("GoodBehaviourBond", new XCData(result.GoodBehaviourBondLength.GetRoundTripParseText))
        );
    }

    private static PunishmentResult LoadPunishmentResult(XElement punishment)
    {
        return new PunishmentResult
        {
            Execution = bool.Parse(punishment.Element("Execution")?.Value ?? "false"),
            Fine = decimal.Parse(punishment.Element("Fine")?.Value ?? "0.0", CultureInfo.InvariantCulture),
            CustodialSentence = LoadMudTimeSpan(punishment.Element("CustodialSentence")?.Value),
            GoodBehaviourBondLength = LoadMudTimeSpan(punishment.Element("GoodBehaviourBond")?.Value)
        };
    }

    private static MudTimeSpan LoadMudTimeSpan(string? text)
    {
        return MudTimeSpan.TryParse(text ?? string.Empty, out MudTimeSpan result) ? result : MudTimeSpan.Zero;
    }

    private void LoadCrimeQueue(XElement? queueElement)
    {
        if (queueElement is null)
        {
            ResetCrimeQueue();
            return;
        }

        var crimesById = _crimes.ToDictionary(x => x.Id);
        CrimeQueue = new Queue<ICrime>(
            queueElement
                .Elements("Crime")
                .Select(x => long.Parse(x.Attribute("id")?.Value ?? "0"))
                .SelectNotNull(x => crimesById.GetValueOrDefault(x))
        );
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("LegalAuthority", LegalAuthority.Id),
            new XElement("LastTrialAction", LastTrialAction.ToString("O")),
            new XElement("Phase", (int)Phase),
            new XElement("ManualTrial", ManualTrial),
            new XElement("CrimeQueue",
                from crime in CrimeQueue
                select new XElement("Crime",
                    new XAttribute("id", crime.Id)
                )
            ),
            new XElement("Crimes",
                from crime in Crimes
                select new XElement("Crime",
                    new XAttribute("id", crime.Id),
                    new XAttribute("plea", _pleas[crime]),
                    new XElement("Defense",
                        from item in _crimeDefenseCases[crime]
                        select new XElement("Result",
                            new XAttribute("difficulty", (int)item.Key),
                            new XAttribute("outcome", (int)item.Value.Outcome)
                        )
                    ),
                    new XElement("Prosecution",
                        from item in _crimeProsecutionCases[crime]
                        select new XElement("Result",
                            new XAttribute("difficulty", (int)item.Key),
                            new XAttribute("outcome", (int)item.Value.Outcome)
                        )
                    ),
                    _punishments.TryGetValue(crime, out PunishmentResult? punishment) ? SavePunishmentResult(punishment) : null
                )
            )
        );
    }

    #endregion

    #region Overrides of Effect

    protected override string SpecificEffectType => "OnTrial";

    public override string Describe(IPerceiver voyeur)
    {
        return $"On Trial in the {LegalAuthority.Name.ColourName()} authority - {Phase.DescribeEnum().ColourValue()}{(ManualTrial ? " (PC judge)" : "")}.";
    }

    public override bool SavingEffect => true;

    public override bool Applies(object target)
    {
        if (target is ILegalAuthority authority)
        {
            return base.Applies(target) && authority == LegalAuthority;
        }

        return base.Applies(target);
    }

    #endregion
}
