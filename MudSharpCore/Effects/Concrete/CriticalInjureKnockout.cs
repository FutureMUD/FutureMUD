using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class CriticalInjureKnockout : Effect
{
	public DateTime WakeupTime { get; set; }

	public CriticalInjureKnockout(IPerceivable owner, DateTime wakeupTime) : base(owner, null)
	{
		WakeupTime = wakeupTime;
	}

	protected CriticalInjureKnockout(XElement root, IPerceivable owner) : base(root, owner)
	{
		WakeupTime = DateTime.Parse(root.Element("WakeupTime").Value, null,
			System.Globalization.DateTimeStyles.RoundtripKind);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Knocked out from taking critical damage.";
	}

	protected override string SpecificEffectType => "CriticalInjureKnockout";

	public static void InitialiseEffectType()
	{
		RegisterFactory("CriticalInjureKnockout", (effect, owner) => new CriticalInjureKnockout(effect, owner));
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("WakeupTime", WakeupTime.ToString("o"))
		);
	}
}