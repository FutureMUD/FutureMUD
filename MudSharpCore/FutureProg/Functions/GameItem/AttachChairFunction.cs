#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class AttachChairFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var chair = (ParameterFunctions[0].Result?.GetObject as IGameItem)?.GetItemType<IChair>();
		var table = (ParameterFunctions[1].Result?.GetObject as IGameItem)?.GetItemType<ITable>();
		var cycle = false;
		var visited = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		for (var ancestor = table?.Parent; ancestor is not null; ancestor = ancestor.ContainedIn)
		{
			if (!visited.Add(ancestor) || ReferenceEquals(ancestor, chair?.Parent))
			{
				cycle = true;
				break;
			}
		}
		var allowed = !cycle && chair is not null && table is not null && (ReferenceEquals(chair.Table, table) || chair.Table is null && table.CanAddChair(null, chair));
		if (allowed && !ReferenceEquals(chair!.Table, table)) chair.SetTable(table);
		Result = new BooleanVariable(allowed); return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("attachchair",
		[ProgVariableTypes.Item, ProgVariableTypes.Item], (p, _) => new AttachChairFunction(p), ["chair", "table"],
		["An unattached native chair item.", "A native table item."],
		"Attaches an unassigned chair through native table capacity checks and persistence. Does not take a chair from another table.", "Items", ProgVariableTypes.Boolean));
}
