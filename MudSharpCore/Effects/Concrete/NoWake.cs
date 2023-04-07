using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Effects.Concrete;

public class NoWake : Effect, INoWakeEffect
{
	public NoWake(ICharacter owner, string wakeUpEcho, IFutureProg applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
		WakeUpEcho = wakeUpEcho;
	}

	public NoWake(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		WakeUpEcho = effect.Element("Echo").Value;
	}

	public string WakeUpEcho { get; set; }

	#region Overrides of Effect

	public override bool SavingEffect { get; } = true;

	#endregion

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return new XElement("Echo", new XCData(WakeUpEcho));
	}

	#endregion

	public static void InitialiseEffectType()
	{
		RegisterFactory("NoWake", (effect, owner) => new NoWake(effect, owner));
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} cannot awaken.";
	}

	#region Overrides of Object

	/// <summary>
	///     Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	///     A string that represents the current object.
	/// </returns>
	public override string ToString()
	{
		return "Owner cannot awaken.";
	}

	#endregion

	protected override string SpecificEffectType { get; } = "NoWake";

	/// <summary>
	///     Fires when the scheduled effect "matures"
	/// </summary>
	public override void ExpireEffect()
	{
		Owner.RemoveEffect(this, true);
		Owner.AddEffect(new PendingRestedness(Owner),
			TimeSpan.FromSeconds(Gameworld.GetStaticDouble("WellRestedSleepSeconds")));
	}

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		Owner.Send("You feel as if you could awaken now, if you were so inclined.");
	}

	#endregion
}