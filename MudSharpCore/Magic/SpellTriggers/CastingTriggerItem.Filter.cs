#nullable enable
using MudSharp.GameItems;
namespace MudSharp.Magic.SpellTriggers;

public partial class CastingTriggerItem
{
	public long TargetFilterProgId { get; private set; }
	public string? TargetFilterError => CastingTargetFilter.Error(TargetFilterProgId, TargetFilterProg, ProgVariableTypes.Item);
	public bool AllowsTarget(IGameItem target, ICharacter caster) =>
		CastingTargetFilter.Allows(TargetFilterProgId, TargetFilterProg, ProgVariableTypes.Item, target, caster);
	private string TargetFilterDescription => TargetFilterProgId == 0 ? "" :
		$" Filter: {TargetFilterProg?.MXPClickableFunctionName() ?? $"missing #{TargetFilterProgId}"}{(TargetFilterError is { } error ? $" [invalid: {error}]" : "")}";
}
