using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Manipulation;

internal class TakeFunction : BuiltInFunction
{
	public bool Quantity { get; set; }
	public bool Delete { get; set; }

	protected TakeFunction(IList<IFunction> parameterFunctions, bool quantity, bool delete) : base(parameterFunctions)
	{
		Quantity = quantity;
		Delete = delete;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new NullVariable(FutureProgVariableTypes.Item);
			return StatementResult.Normal;
		}

		var quantity = 0;
		if (Quantity)
		{
			quantity = Convert.ToInt32(ParameterFunctions[1].Result?.GetObject ?? 0M);
		}

		if (item.DropsWhole(quantity))
		{
			if (Delete)
			{
				item.Delete();
				Result = new NullVariable(FutureProgVariableTypes.Item);
				return StatementResult.Normal;
			}

			item.ContainedIn?.Take(item);
			item.InInventoryOf?.Take(item);
			Result = item;
			return StatementResult.Normal;
		}

		if (Delete)
		{
			var stack = item.GetItemType<IStackable>();
			stack.Quantity -= quantity;
			Result = new NullVariable(FutureProgVariableTypes.Item);
			return StatementResult.Normal;
		}

		var newItem = item.Drop(null, quantity);
		Result = newItem;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"take",
			new[]
			{
				FutureProgVariableTypes.Item
			},
			(pars, gameworld) => new TakeFunction(pars, false, false),
			new List<string>
			{
				"item"
			},
			new List<string>
			{
				"The item to take"
			},
			"Takes an item from its inventory or container. Returns the item.",
			"Items"
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"takedelete",
			new[]
			{
				FutureProgVariableTypes.Item
			},
			(pars, gameworld) => new TakeFunction(pars, false, true),
			new List<string>
			{
				"item"
			},
			new List<string>
			{
				"The item to take"
			},
			"Takes an item from its inventory or container and deletes it. Returns null.",
			"Items"
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"take",
			new[]
			{
				FutureProgVariableTypes.Item, FutureProgVariableTypes.Number
			},
			(pars, gameworld) => new TakeFunction(pars, true, false),
			new List<string>
			{
				"item",
				"quantity"
			},
			new List<string>
			{
				"The item to take",
				"The quantity to take. Use 0 for all"
			},
			"Takes an item from its inventory or container. Returns the new item, which may be the same as the original item if the quantity is equal to the existing quantity.",
			"Items"
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"takedelete",
			new[]
			{
				FutureProgVariableTypes.Item, FutureProgVariableTypes.Number
			},
			(pars, gameworld) => new TakeFunction(pars, true, false),
			new List<string>
			{
				"item",
				"quantity"
			},
			new List<string>
			{
				"The item to take",
				"The quantity to take. Use 0 for all"
			},
			"Takes an item from its inventory or container. Deletes the item and returns null.",
			"Items"
		));
	}
}