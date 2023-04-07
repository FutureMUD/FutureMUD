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

public class AwaitingSentencing : Effect, IEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("AwaitingSentencing", (effect, owner) => new AwaitingSentencing(effect, owner));
	}

	#endregion

	public ILegalAuthority LegalAuthority { get; set; }
	public MudDateTime ArrestTime { get; set; }

	#region Constructors

	public AwaitingSentencing(ICharacter owner, ILegalAuthority legalAuthority, MudDateTime arrestTime) : base(owner,
		null)
	{
		LegalAuthority = legalAuthority;
		ArrestTime = arrestTime;
	}

	protected AwaitingSentencing(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		ArrestTime = new MudDateTime(root.Element("ArrestTime").Value, Gameworld);
	}

	#endregion

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

	protected override string SpecificEffectType => "AwaitingSentencing";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Awaiting sentencing in the {LegalAuthority.Name.ColourName()} jurisdiction since {ArrestTime.ToString(TimeAndDate.Date.CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}.";
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