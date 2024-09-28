using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class GoodBehaviourBond : Effect, IEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("GoodBehaviourBond", (effect, owner) => new GoodBehaviourBond(effect, owner));
	}

	#endregion

	#region Constructors

	public GoodBehaviourBond(ICharacter owner, ILegalAuthority authority, MudTimeSpan length) : base(owner, null)
	{
		Authority = authority;
		OriginalLength = length;
		DateUntil = Authority.EnforcementZones.First().DateTime() + length;
		RegisterListener();
	}

	protected GoodBehaviourBond(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Authority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		OriginalLength = MudTimeSpan.Parse(root.Element("OriginalLength").Value);
		DateUntil = new MudDateTime(root.Element("DateUntil").Value, Gameworld);
		RegisterListener();
	}

	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", Authority.Id),
			new XElement("OriginalLength", new XCData(OriginalLength.GetRoundTripParseText)),
			new XElement("DateUntil", new XCData(DateUntil.GetDateTimeString()))
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "GoodBehaviourBond";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Serving a {OriginalLength.Describe(voyeur).ColourValue()} good behaviour bond in {Authority.Name.ColourName()} until {DateUntil.Date.Display(TimeAndDate.Date.CalendarDisplayMode.Short).ColourValue()}.";
	}

	public override bool SavingEffect => true;

	public override void RemovalEffect()
	{
		CancelListener();
	}

	#endregion

	private TimeAndDate.Listeners.ITemporalListener _listener;

	private void RegisterListener()
	{
		_listener = TimeAndDate.Listeners.ListenerFactory.CreateDateListener(DateUntil.Calendar, DateUntil.Date.Day,
			DateUntil.Date.Month.Alias, DateUntil.Date.Year, DateUntil.TimeZone, 0, items => Owner.RemoveEffect(this, true),
			Array.Empty<object>(), $"Good Behaviour Bond for {Owner.HowSeen(null, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}");
	}

	private void CancelListener()
	{
		_listener.CancelListener();
		_listener = null;
	}

	public void AddLengthToBond(MudTimeSpan addition)
	{
		OriginalLength += addition;
		DateUntil += addition;
		CancelListener();
		RegisterListener();
		Changed = true;
	}

	public ILegalAuthority Authority { get; init; }
	public MudTimeSpan OriginalLength { get; protected set; }
	public MudDateTime DateUntil { get; protected set; }
}