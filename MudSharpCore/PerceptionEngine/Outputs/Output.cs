using System;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine.Outputs;

public abstract class Output : IOutput
{
	/// <summary>
	///     Base class for Futuremud Outputs.
	/// </summary>
	/// <param name="visibility">What kind of output it is, whether IC or OOC or a specific subset of either.</param>
	/// <param name="style">What visual style should be applied to the output.</param>
	/// <param name="flags">Any additional flags to be raised that modify the output and how it should be shown.</param>
	protected Output(OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
	{
		Visibility = visibility;
		Style = style;
		Flags = flags;
	}

	public OutputVisibility Visibility { get; set; }

	public OutputStyle Style { get; set; }

	public OutputFlags Flags { get; set; }

	public abstract string RawString { get; }

	public abstract string ParseFor(IPerceiver perceiver);

	public virtual bool ShouldSee(IPerceiver perceiver)
	{
		return !Flags.HasFlag(OutputFlags.WizOnly) ||
		       (perceiver as ICharacter)?.IsAdministrator() == true
			;
	}
}