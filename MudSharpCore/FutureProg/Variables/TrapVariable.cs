#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Variables;

/// <summary>
/// A short-lived FutureProg value that exposes a persisted trap effect. Trap variables are intentionally resolved
/// from an anchor at runtime; scripts should retain the anchor/template rather than persist a trap effect reference.
/// </summary>
public sealed class TrapVariable(TrapEffect trap) : ProgVariable
{
	public override ProgVariableTypes Type => ProgVariableTypes.Trap;
	public override object GetObject => trap;

	public override IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new TextVariable(trap.InstanceId.ToString()),
			"templateid" => new NumberVariable(trap.TemplateId),
			"templaterevision" => new NumberVariable(trap.TemplateRevisionNumber),
			"state" => new TextVariable(trap.State.DescribeEnum()),
			"charges" => new NumberVariable(trap.RemainingCharges),
			"source" => new TextVariable(trap.SourceKind.DescribeEnum()),
			"owner" => trap.Owner,
			_ => throw new NotSupportedException($"Unsupported property type {property} in TrapVariable.GetProperty")
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(
			ProgVariableTypes.Trap,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Text,
				["templateid"] = ProgVariableTypes.Number,
				["templaterevision"] = ProgVariableTypes.Number,
				["state"] = ProgVariableTypes.Text,
				["charges"] = ProgVariableTypes.Number,
				["source"] = ProgVariableTypes.Text,
				["owner"] = ProgVariableTypes.Perceivable
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The stable runtime GUID for the trap instance.",
				["templateid"] = "The trap template ID.",
				["templaterevision"] = "The immutable trap template revision used by the instance.",
				["state"] = "The current trap state.",
				["charges"] = "The number of remaining activations.",
				["source"] = "The trap source domain.",
				["owner"] = "The item or cell to which the trap is anchored."
			});
	}
}
