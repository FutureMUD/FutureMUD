#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class ConsumePaperPageFunction(IList<IFunction> parameters, bool query) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var item = ParameterFunctions[1].Result?.GetObject as IGameItem;
		var allowWritten = ParameterFunctions.Count > 2 && ParameterFunctions[2].Result?.GetObject is true;
		var paper = item?.GetItemType<IConsumablePaper>();
		Result = new BooleanVariable(actor is not null && item is not null && paper is not null &&
			actor.Body.ExternalItems.Contains(item) && (query ? paper.CanConsumePage(actor, allowWritten) : paper.ConsumePage(actor, allowWritten)));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var query in new[] { false, true })
		foreach (var explicitOption in new[] { false, true })
			FutureProg.RegisterBuiltInFunctionCompiler(new(query ? "canconsumepage" : "consumepage",
				explicitOption ? [ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Boolean] : [ProgVariableTypes.Character, ProgVariableTypes.Item],
				(parameters, _) => new ConsumePaperPageFunction(parameters, query),
				explicitOption ? ["actor", "item", "allowWritten"] : ["actor", "item"],
				explicitOption ? ["The carrier.", "A single sheet or open book in their external inventory.", "True deliberately selects the current page even if written; false selects blank paper only."] :
				["The carrier.", "A single blank sheet or open book containing a blank page."],
				query ? "Checks page consumption without mutation. Defaults to protecting all writing and drawings." :
				"Consumes one physical page, preferring blank pages by default. Written-page destruction must be explicitly enabled. A book retains its binding and other pages; a loose sheet is deleted.",
				"Items", ProgVariableTypes.Boolean));
	}
}
