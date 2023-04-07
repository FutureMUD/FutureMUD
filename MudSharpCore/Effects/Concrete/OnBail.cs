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
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;

public class OnBail : Effect, IEffect, IScoreAddendumEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("OnBail", (effect, owner) => new OnBail(effect, owner));
	}

	#endregion

	#region Constructors

	public OnBail(ICharacter owner, ILegalAuthority legalAuthority, MudDateTime arrestTime) : base(owner, null)
	{
		LegalAuthority = legalAuthority;
		ArrestTime = arrestTime;
	}

	protected OnBail(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		ArrestTime = new MudDateTime(root.Element("ArrestTime").Value, Gameworld);
	}

	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	public MudDateTime ArrestTime { get; set; }

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("ArrestTime", new XCData(ArrestTime.GetDateTimeString()))
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "OnBail";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"On bail until {ArrestTime.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
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

	public bool ShowInScore => true;
	public bool ShowInHealth => false;

	public string ScoreAddendum =>
		$"You are on bail until {ArrestTime.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
}