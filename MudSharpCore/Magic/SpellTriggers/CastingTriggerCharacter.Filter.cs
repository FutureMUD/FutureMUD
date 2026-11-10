#nullable enable
namespace MudSharp.Magic.SpellTriggers;

public partial class CastingTriggerCharacter
{
	public long TargetFilterProgId { get; private set; }
	private ProgVariableTypes TargetVariableType => CanTargetItems ? ProgVariableTypes.Perceivable : ProgVariableTypes.Character;
	public string? TargetFilterError => CastingTargetFilter.Error(TargetFilterProgId, TargetFilterProg, TargetVariableType);
	public bool AllowsTarget(IPerceivable target, ICharacter caster) =>
		(target is ICharacter || CanTargetItems && target is MudSharp.GameItems.IGameItem) &&
		CastingTargetFilter.Allows(TargetFilterProgId, TargetFilterProg, TargetVariableType, target, caster);
	private string TargetFilterDescription => TargetFilterProgId == 0 ? "" :
		$" Filter: {TargetFilterProg?.MXPClickableFunctionName() ?? $"missing #{TargetFilterProgId}"}{(TargetFilterError is { } error ? $" [invalid: {error}]" : "")}";
}
