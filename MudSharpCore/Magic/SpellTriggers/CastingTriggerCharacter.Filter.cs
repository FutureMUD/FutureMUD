#nullable enable
namespace MudSharp.Magic.SpellTriggers;

public partial class CastingTriggerCharacter
{
	public long TargetFilterProgId { get; private set; }
	public string? TargetFilterError => CastingTargetFilter.Error(TargetFilterProgId, TargetFilterProg, ProgVariableTypes.Character);
	public bool AllowsTarget(ICharacter target, ICharacter caster) =>
		CastingTargetFilter.Allows(TargetFilterProgId, TargetFilterProg, ProgVariableTypes.Character, target, caster);
	private string TargetFilterDescription => TargetFilterProgId == 0 ? "" :
		$" Filter: {TargetFilterProg?.MXPClickableFunctionName() ?? $"missing #{TargetFilterProgId}"}{(TargetFilterError is { } error ? $" [invalid: {error}]" : "")}";
}
