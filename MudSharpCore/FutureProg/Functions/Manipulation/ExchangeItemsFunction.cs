#nullable enable

using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal sealed class ExchangeItemsFunction(IList<IFunction> parameters, bool exchange) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var giver = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var offered = ParameterFunctions[1].Result?.GetObject as IGameItem;
		var receiver = ParameterFunctions[2].Result?.GetObject as ICharacter;
		var requested = exchange ? ParameterFunctions[3].Result?.GetObject as IGameItem : null;
		Result = new BooleanVariable(giver?.Body is { } source && receiver?.Body is { } target &&
			!ReferenceEquals(source, target) && offered is not null &&
			(exchange ? requested is not null && source.TryExchangeItems(offered, target, requested) :
				source.GiveWithoutMerge(offered, target) is not null));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("exchangeitems",
			[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Item],
			(parameters, _) => new ExchangeItemsFunction(parameters, true), ["giver", "offered", "partner", "requested"],
			["First character.", "Their whole held item.", "Second character.", "Their whole held item."],
			"Exchanges actual whole held native items without merging. Both legs pass ordinary give checks and commit together before callbacks. Returns whether committed; native events still fire. No automatic echo.", "Manipulation", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new("giveitem",
			[ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Character],
			(parameters, _) => new ExchangeItemsFunction(parameters, false), ["giver", "item", "recipient"],
			["The giver.", "Their whole held item.", "The recipient."],
			"Uses ordinary native GiveWithoutMerge checks, output and events to transfer the real whole item. Does not create a replacement.", "Manipulation", ProgVariableTypes.Boolean));
	}
}

internal sealed class TargetHeldItemFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Item; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var owner = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var viewer = ParameterFunctions[1].Result?.GetObject as ICharacter;
		var keyword = ParameterFunctions[2].Result?.GetObject?.ToString() ?? "";
		Result = viewer is null ? null : owner?.Body?.HeldItems.Where(x => viewer.CanSee(x)).GetFromItemListByKeyword(keyword, viewer);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("targethelditem",
		[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Text],
		(parameters, _) => new TargetHeldItemFunction(parameters), ["owner", "viewer", "keyword"],
		["Whose held inventory.", "Who must see the item.", "Native keyword/disambiguation."],
		"Finds a visible whole item in another character's held inventory using ordinary native keyword matching. Does not access worn, wielded or contained goods.", "Manipulation", ProgVariableTypes.Item));
}

internal sealed class TargetItemFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Item; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var viewer = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var items = ParameterFunctions[1].Result?.GetObject as System.Collections.IEnumerable;
		var keyword = ParameterFunctions[2].Result?.GetObject?.ToString() ?? "";
		Result = viewer is null || items is null ? null
			: items.OfType<IGameItem>().Where(x => viewer.CanSee(x)).GetFromItemListByKeyword(keyword, viewer);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("targetitem",
		[ProgVariableTypes.Character, ProgVariableTypes.Item | ProgVariableTypes.Collection, ProgVariableTypes.Text],
		(parameters, _) => new TargetItemFunction(parameters), ["viewer", "items", "keyword"],
		["Who must see the item.", "The candidate items.", "Native keyword/disambiguation."],
		"Finds a visible item in the supplied collection using ordinary native keyword matching. Returns null when none matches. Does not transfer items or inspect nested containers.", "Manipulation", ProgVariableTypes.Item));
}
