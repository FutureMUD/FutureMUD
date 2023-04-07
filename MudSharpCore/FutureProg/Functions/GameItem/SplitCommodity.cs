using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SplitCommodity : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SplitCommodity".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
				(pars, gameworld) => new SplitCommodity(pars, gameworld),
				new List<string> { "item", "weight" },
				new List<string>
				{
					"The item to take the weight from",
					"The weight of the material to split off in base units for this MUD. See MUD owner for configuration info"
				},
				"This function takes weight from one commodity pile and creates a new commodity pile item with the taken weight, which it then returns. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory. If the quantity is the whole weight of the original item or larger, it will just return the original item and no new item will be created.",
				"Items",
				FutureProgVariableTypes.Item
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SplitCommodity".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SplitCommodity(pars, gameworld),
				new List<string> { "item", "weight" },
				new List<string>
				{
					"The item to take the weight from",
					"The weight of the material to split off, e.g. 120kg, 15lb 3oz, etc"
				},
				"This function takes weight from one commodity pile and creates a new commodity pile item with the taken weight, which it then returns. It does not put the item anywhere, so you must then insert it somewhere like in a room or a character's inventory. If the quantity is the whole weight of the original item or larger, it will just return the original item and no new item will be created.",
				"Items",
				FutureProgVariableTypes.Item
			)
		);
	}

	#endregion

	#region Constructors

	protected SplitCommodity(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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
			Result = null;
			return StatementResult.Normal;
		}

		var commodity = item.GetItemType<ICommodity>();
		if (commodity == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		double weight;
		if (ParameterFunctions[1].Result?.GetObject is decimal wvalue)
		{
			weight = (double)wvalue;
		}
		else
		{
			weight = Gameworld.UnitManager.GetBaseUnits(
				ParameterFunctions[1].Result?.GetObject?.ToString() ?? "nothing", Framework.Units.UnitType.Mass,
				out var success);
			if (!success)
			{
				ErrorMessage = "Invalid Weight";
				return StatementResult.Error;
			}
		}

		if (weight >= commodity.Weight)
		{
			Result = item;
			return StatementResult.Normal;
		}

		Result = item.GetByWeight(null, weight);
		return StatementResult.Normal;
	}
}