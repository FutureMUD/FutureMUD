using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public class SuppliedFunctionOutput : EmoteOutput
{
	public Func<string, IPerceiver, string> SuppliedFunction { get; set; }

	/// <summary>
	///     Handles emote outputs to players and items.
	/// </summary>
	/// <param name="emote">The default emote.</param>
	/// <param name="visibility">What kind of output it is, whether IC or OOC or a specific subset of either.</param>
	/// <param name="style">What visual style should be applied to the output.</param>
	/// <param name="flags">Any additional flags to be raised that modify the output and how it should be shown.</param>
	public SuppliedFunctionOutput(Emote emote, Func<string, IPerceiver, string> suppliedFunction,
		OutputVisibility visibility = OutputVisibility.Normal, OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal) : base(emote, visibility, style, flags)
	{
		SuppliedFunction = suppliedFunction;
	}

	#region Overrides of EmoteOutput

	public override string ParseFor(IPerceiver perceiver)
	{
		return SuppliedFunction(base.ParseFor(perceiver), perceiver);
	}

	#endregion
}