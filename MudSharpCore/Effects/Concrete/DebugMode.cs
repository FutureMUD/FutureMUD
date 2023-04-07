using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class DebugMode : Effect
{
	public DebugMode(IPerceivable owner) : base(owner, null)
	{
	}

	protected override string SpecificEffectType => "DebugMode";

	public override string Describe(IPerceiver voyeur)
	{
		return "Tuning in to debug messages.";
	}
}