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

namespace MudSharp.Effects.Concrete;

public class OnTrial : Effect, IEffect
{
	private DateTime _lastTrialAction;
	private bool _introductionGiven;

	public ILegalAuthority LegalAuthority { get; set; }

	public DateTime LastTrialAction
	{
		get => _lastTrialAction;
		init
		{
			_lastTrialAction = value;
			Changed = true;
		}
	}

	private readonly Queue<ICrime> _crimeQueue;
	public IEnumerable<ICrime> CrimeQueue => _crimeQueue;

	public bool IntroductionGiven
	{
		get => _introductionGiven;
		set
		{
			_introductionGiven = value;
			Changed = true;
		}
	}

	public ICrime? NextCrime()
	{
		Changed = true;
		if (_crimeQueue.Count == 0)
		{
			return null;
		}

		return _crimeQueue.Dequeue();
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
		LastTrialAction = lastTrialAction;
		_crimeQueue = new Queue<ICrime>(crimes);
		IntroductionGiven = false;
	}

	protected OnTrial(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		LastTrialAction = DateTime.Parse(root.Element("LastTrialAction").Value, null,
			System.Globalization.DateTimeStyles.RoundtripKind);
		_crimeQueue = new Queue<ICrime>(root.Element("Crimes").Elements()
		                                    .Select(x => Gameworld.Crimes.Get(long.Parse(x.Value))));
		IntroductionGiven = bool.Parse(root.Element("IntroductionGiven").Value);
	}

	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("LastTrialAction", LastTrialAction.ToString("O")),
			new XElement("IntroductionGiven", IntroductionGiven),
			new XElement("Crimes",
				from crime in CrimeQueue
				select new XElement("Crime", crime.Id)
			)
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "OnTrial";

	public override string Describe(IPerceiver voyeur)
	{
		return $"On Trial in the {LegalAuthority.Name.ColourName()} authority.";
	}

	public override bool SavingEffect => true;

	#endregion
}