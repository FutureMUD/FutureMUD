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

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
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
                        new[] { ProgVariableTypes.Item, ProgVariableTypes.Item },
                        (pars, gameworld) => new DetatchFunction(pars),
                        new[]
                        {
                                "attachable",
                                "belt"
                        },
                        new[]
                        {
                                "The item to detach from the belt",
                                "The belt the item is attached to"
                        },
                        "Detaches an item from a belt. Returns true if successful.",
                        "Manipulation",
                        ProgVariableTypes.Boolean
                ));
        }
}