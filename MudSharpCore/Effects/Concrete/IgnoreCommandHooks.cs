using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class IgnoreCommandHooks : Effect, IIgnoreCommandHooksEffect
{
	public IgnoreCommandHooks(IPerceivable owner)
		: base(owner)
	{
	}

	protected override string SpecificEffectType => "IgnoreCommandHooks";

	public override string Describe(IPerceiver voyeur)
	{
		return "Ignoring Command Hooks";
	}

	public override string ToString()
	{
		return "IgnoreCommandHooks Effect";
	}
}