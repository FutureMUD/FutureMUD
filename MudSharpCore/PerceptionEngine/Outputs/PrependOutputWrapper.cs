using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine.Outputs;

public class PrependOutputWrapper : IOutput
{
	private readonly IOutput _underlyingOutput;
	private readonly string _prependText;

	#region Overrides of Output

	public OutputVisibility Visibility
	{
		get => _underlyingOutput.Visibility;
		set => _underlyingOutput.Visibility = value;
	}

	public OutputStyle Style
	{
		get => _underlyingOutput.Style;
		set => _underlyingOutput.Style = value;
	}

	public OutputFlags Flags
	{
		get => _underlyingOutput.Flags;
		set => _underlyingOutput.Flags = value;
	}

	public string RawString => _underlyingOutput.RawString;

	public string ParseFor(IPerceiver perceiver)
	{
		return $"{_prependText}{_underlyingOutput.ParseFor(perceiver)}";
	}

	public bool ShouldSee(IPerceiver perceiver)
	{
		return _underlyingOutput.ShouldSee(perceiver);
	}

	#endregion

	public PrependOutputWrapper(IOutput underlyingOutput, string prependText)
	{
		_underlyingOutput = underlyingOutput;
		_prependText = prependText;
	}
}