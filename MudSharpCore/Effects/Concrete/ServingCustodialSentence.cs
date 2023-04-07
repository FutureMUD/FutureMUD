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

public class ServingCustodialSentence : Effect, IEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("ServingCustodialSentence", (effect, owner) => new ServingCustodialSentence(effect, owner));
	}

	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	public TimeSpan TotalTime { get; set; }
	public MudDateTime ReleaseDate { get; set; }

	#region Constructors

	public ServingCustodialSentence(ICharacter owner, ILegalAuthority legalAuthority, TimeSpan totalTime,
		MudDateTime releaseDate) : base(owner, null)
	{
		LegalAuthority = legalAuthority;
		TotalTime = totalTime;
		ReleaseDate = releaseDate;
	}

	protected ServingCustodialSentence(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		TotalTime = TimeSpan.FromSeconds(double.Parse(root.Element("TotalTime").Value));
		ReleaseDate = new MudDateTime(root.Element("ReleaseDate").Value, Gameworld);
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("TotalTime", TotalTime.TotalSeconds),
			new XElement("ReleaseDate", new XCData(ReleaseDate.GetDateTimeString()))
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "ServingCustodialSentence";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Serving a {TotalTime.Describe(voyeur).ColourValue()} sentence until {ReleaseDate.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
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

	public void ExtendSentence(TimeSpan extension)
	{
		TotalTime += extension;
		ReleaseDate += extension;
		Changed = true;
	}
}