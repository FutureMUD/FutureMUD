using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetLockedFunction : BuiltInFunction
{
	private SetLockedFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not IGameItem itemFunction)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var lockComp = itemFunction.GetItemType<ILock>();
		if (lockComp != null)
		{
			Result = new BooleanVariable(lockComp.SetLocked((bool?)ParameterFunctions[1].Result.GetObject ?? true,
				(bool?)ParameterFunctions[2].Result.GetObject ?? true));
			return StatementResult.Normal;
		}

		var lockable = itemFunction.GetItemType<ILockable>();
		if (lockable != null)
		{
			foreach (var theLock in lockable.Locks)
			{
				theLock.SetLocked((bool?)ParameterFunctions[1].Result.GetObject ?? true,
					(bool?)ParameterFunctions[2].Result.GetObject ?? true);
			}

			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setlocked",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Boolean, FutureProgVariableTypes.Boolean },
			(pars, gameworld) => new SetLockedFunction(pars)
		));
	}
}