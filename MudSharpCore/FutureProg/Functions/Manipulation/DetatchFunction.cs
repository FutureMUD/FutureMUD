using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class DetatchFunction : BuiltInFunction
{
	public DetatchFunction(IList<IFunction> parameters)
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

		var attachableItem = (IGameItem)ParameterFunctions[1].Result;
		if (attachableItem == null)
		{
			ErrorMessage = "Attachable GameItem was null in Detatch function.";
			return StatementResult.Error;
		}

		var attachedtoItem = (IGameItem)ParameterFunctions[2].Result;
		if (attachedtoItem == null)
		{
			ErrorMessage = "Attachedto GameItem was null in Detatch function.";
			return StatementResult.Error;
		}

		var attachable = attachableItem.GetItemType<IBeltable>();
		if (attachable == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var attachedto = attachedtoItem.GetItemType<IBelt>();
		if (attachedto == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (attachable.ConnectedTo == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		attachedto.RemoveConnectedItem(attachable);

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"detatch",
			new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Item },
			(pars, gameworld) => new DetatchFunction(pars)
		));
	}
}