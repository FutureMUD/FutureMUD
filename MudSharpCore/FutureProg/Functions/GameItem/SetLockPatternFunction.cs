#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class SetLockPatternFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item ||
			ParameterFunctions[1].Result?.GetObject is not decimal pattern ||
			pattern < 0 || pattern > int.MaxValue || decimal.Truncate(pattern) != pattern)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var changed = false;
		if (item.GetItemType<IKey>() is { } key)
		{
			key.Pattern = (int)pattern;
			changed = true;
		}
		if (item.GetItemType<ILock>() is { } itemLock)
		{
			itemLock.Pattern = (int)pattern;
			changed = true;
		}
		Result = new BooleanVariable(changed);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"setlockpattern", [ProgVariableTypes.Item, ProgVariableTypes.Number],
		(parameters, _) => new SetLockPatternFunction(parameters), ["item", "pattern"],
		["The key or built-in lock to configure", "A whole pattern number; zero removes its key pairing"],
		"Sets the matching pattern on a key or an item's built-in lock. Matching lock types and nonzero patterns are required to unlock.",
		"Items", ProgVariableTypes.Boolean));
}
