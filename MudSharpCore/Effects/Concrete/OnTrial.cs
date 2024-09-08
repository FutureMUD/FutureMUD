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
	Introduction,
	Charges,
	Plea,
	Case,
	Verdict,
	Sentencing
}

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

	public ICrime? NextCrime()
	{
		Changed = true;
		if (CrimeQueue.Count == 0)
		{
			return null;
		}

		return CrimeQueue.Dequeue();
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
		ResetCrimeQueue();
		_phase = TrialPhase.Introduction;
	}

	protected OnTrial(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		_lastTrialAction = DateTime.Parse(root.Element("LastTrialAction").Value, null,
			System.Globalization.DateTimeStyles.RoundtripKind);
		_crimes = new List<ICrime>(root.Element("Crimes").Elements()
		                                    .Select(x => Gameworld.Crimes.Get(long.Parse(x.Value))));
		ResetCrimeQueue();
		_phase = (TrialPhase)int.Parse(effect.Element("Phase")?.Value ?? "0");
	}

	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("LastTrialAction", LastTrialAction.ToString("O")),
			new XElement("Phase", (int)Phase),
			new XElement("Crimes",
				from crime in Crimes
				select new XElement("Crime", crime.Id)
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