using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetOpenFunction : BuiltInFunction
{
	private readonly bool _forceOpen;

	private SetOpenFunction(IList<IFunction> parameters, bool forceOpen)
		: base(parameters)
	{
		_forceOpen = forceOpen;
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

		var openable = itemFunction.GetItemType<IOpenable>();
		if (openable != null)
		{
			var targetState = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
			var echo = (bool?)ParameterFunctions[2].Result.GetObject ?? true;
			if (openable.IsOpen == targetState)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			if (targetState)
			{
				if (!openable.CanOpen(null) && !_forceOpen)
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}

				openable.Open();
				if (echo)
				{
					itemFunction.OutputHandler.Handle(new EmoteOutput(new Emote("@ open|opens", itemFunction),
						flags: OutputFlags.SuppressObscured));
				}

				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}

			if (!openable.CanClose(null) && !_forceOpen)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			openable.Close();
			if (echo)
			{
				itemFunction.OutputHandler.Handle(new EmoteOutput(new Emote("@ close|closes", itemFunction),
					flags: OutputFlags.SuppressObscured));
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
			"setopen",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Boolean, FutureProgVariableTypes.Boolean },
			(pars, gameworld) => new SetOpenFunction(pars, false)
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"forceopen",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Boolean, FutureProgVariableTypes.Boolean },
			(pars, gameworld) => new SetOpenFunction(pars, true)
		));
	}
}