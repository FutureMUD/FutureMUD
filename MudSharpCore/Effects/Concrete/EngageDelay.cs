using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;
public class EngageDelay(IPerceivable owner) : Effect(owner, null), IRemoveOnCombatStart
{
	protected override string SpecificEffectType => "EngageDelay";

	public override string Describe(IPerceiver voyeur)
	{
		return "Unable to voluntarily engage in combat.";
	}

	/// <inheritdoc />
	public override void ExpireEffect()
	{
		base.ExpireEffect();
		Owner.OutputHandler.Send("You can once again engage in combat freely.");
	}

	/// <inheritdoc />
	public override IEnumerable<string> Blocks => ["combat-engage", "aim"];

	/// <inheritdoc />
	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return "recovering from a recent combat";
	}
}
