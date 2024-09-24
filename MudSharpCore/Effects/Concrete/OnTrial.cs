using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.FutureProg.Statements;
using MudSharp.RPG.Checks;

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

	public ILegalAuthority LegalAuthority { get; set; }

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

	private readonly List<ICrime> _crimes;
	public Queue<ICrime> CrimeQueue { get; private set; }

	public void ResetCrimeQueue()
	{
		CrimeQueue = new Queue<ICrime>(_crimes);
	}

	public IEnumerable<ICrime> Crimes => _crimes;

	private readonly Dictionary<ICrime, bool> _pleas = new();
	public IDictionary<ICrime, bool> Pleas => _pleas;

	private readonly Dictionary<ICrime, IReadOnlyDictionary<Difficulty,CheckOutcome>> _crimeDefenseCases = new();
	private readonly Dictionary<ICrime, IReadOnlyDictionary<Difficulty, CheckOutcome>> _crimeProsecutionCases = new();

	private readonly Dictionary<ICrime, PunishmentResult> _punishments = new();
	public IDictionary<ICrime, PunishmentResult> Punishments => _punishments;

	private ICharacter? _prosecutor;
	private long? _prosecutorId;

	public ICharacter? Prosecutor
	{
		get
		{
			return _prosecutor ??= Gameworld.TryGetCharacter(_prosecutorId ?? 0, true);
		}
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
		get
		{
			return _defender ??= Gameworld.TryGetCharacter(_defenderId ?? 0, true);
		}
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
		return _crimeDefenseCases.TryGetValue(crime, out var @case) ? @case : CheckOutcome.NotTestedAllDifficulties(CheckType.DefendLegalCase);
	}

	public IReadOnlyDictionary<Difficulty, CheckOutcome> GetResultProsecution(ICrime crime)
	{
		return _crimeProsecutionCases.TryGetValue(crime, out var @case) ? @case : CheckOutcome.NotTestedAllDifficulties(CheckType.ProsecuteLegalCase);
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
	
	public void HandleArgueCommand(ICharacter actor, bool defense)
	{
		var crime = defense ? NextDefenseCrime() : NextProsecutionCrime();
		if (crime is null)
		{
			actor.OutputHandler.Send($"The {(defense ? "defense" : "prosecution")} has finished making its case for all crimes.");
			return;
		}

		var result = defense ? ArgueCaseDefender(actor, crime) : ArgueCaseProsecution(actor, crime);
		var primary = result[defense ? crime.DefenseDifficulty : crime.ProsecutionDifficulty];
		string adverb = "";
		if (primary.IsAbjectFailure)
		{
			adverb = "The legal arguments are complete nonsense and utterly ineffective.";
		}
		else
		{
			switch (primary.Outcome)
			{
				case Outcome.MajorFail:
					adverb = "The case put forward is fairly weak.";
					break;
				case Outcome.Fail:
					adverb = "The case put forward is weak, but still plausible.";
					break;
				case Outcome.MinorFail:
					adverb = "The case put forward is convincing, but has some minor flaws.";
					break;
				case Outcome.MinorPass:
					adverb = "The case put forward is convincing.";
					break;
				case Outcome.Pass:
					adverb = "The case put forward is solid and well supported.";
					break;
				case Outcome.MajorPass:
					adverb = "The case put forward is extremely robust and almost air-tight.";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ argue|argues the {(defense ? "defense" : "prosecution")} case for the {ChargeNumber(crime).ToOrdinal()} charge of {crime.Name}. {adverb}", actor, actor, Owner)));
	}

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("OnTrial", (effect, owner) => new OnTrial(effect, owner));
	}

	#endregion

	#region Constructors

	public OnTrial(ICharacter owner, ILegalAuthority legalAuthority, DateTime lastTrialAction,
		IEnumerable<ICrime> crimes) : base(owner, null)
	{
		LegalAuthority = legalAuthority;
		_lastTrialAction = lastTrialAction;
		_crimes = crimes.ToList();
		foreach (var crime in _crimes)
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
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		_lastTrialAction = DateTime.Parse(root.Element("LastTrialAction").Value, null,
			System.Globalization.DateTimeStyles.RoundtripKind);
		_crimes = [];
		foreach (var item in root.Element("Crimes").Elements())
		{
			var crime = Gameworld.Crimes.Get(long.Parse(item.Attribute("id").Value));
			if (crime is null)
			{
				continue;
			}
			_crimes.Add(crime);
			_pleas[crime] = bool.Parse(item.Attribute("plea").Value);
			var defenseOutcome = new Dictionary<Difficulty, CheckOutcome>();
			foreach (var result in item.Element("Defense").Elements())
			{
				defenseOutcome[(Difficulty)int.Parse(result.Attribute("difficulty").Value)] = CheckOutcome.SimpleOutcome(CheckType.DefendLegalCase, (Outcome)int.Parse(result.Attribute("outcome").Value));
			}
			_crimeDefenseCases[crime] = defenseOutcome;
			var prosecutionOutcome = new Dictionary<Difficulty, CheckOutcome>();
			foreach (var result in item.Element("Prosecution").Elements())
			{
				prosecutionOutcome[(Difficulty)int.Parse(result.Attribute("difficulty").Value)] = CheckOutcome.SimpleOutcome(CheckType.ProsecuteLegalCase, (Outcome)int.Parse(result.Attribute("outcome").Value));
			}
			_crimeProsecutionCases[crime] = prosecutionOutcome;
		}
		ResetCrimeQueue();
		_phase = (TrialPhase)int.Parse(effect.Element("Phase")?.Value ?? "0");
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("LastTrialAction", LastTrialAction.ToString("O")),
			new XElement("Phase", (int)Phase),
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
					)
				)
			)
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "OnTrial";

	public override string Describe(IPerceiver voyeur)
	{
		return $"On Trial in the {LegalAuthority.Name.ColourName()} authority - {Phase.DescribeEnum().ColourValue()}.";
	}

	public override bool SavingEffect => true;

	#endregion
}