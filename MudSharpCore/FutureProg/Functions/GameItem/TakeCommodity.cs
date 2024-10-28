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

internal class TakeCommodity : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"TakeCommodity".ToLowerInvariant(),
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Number },
				(pars, gameworld) => new TakeCommodity(pars, gameworld),
				new List<string> { "item", "weight" },
				new List<string>
				{
					"The item to take the weight from",
					"The weight of the material to take in base units for this MUD. See MUD owner for configuration info"
				},
				"This function takes a certain weight off of a commodity pile. If the weight is equal to or higher than the weight of the commodity pile, it deletes the pile. Returns true if it succeeded at taking the weight.",
				"Items",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"TakeCommodity".ToLowerInvariant(),
				new[] { ProgVariableTypes.Item, ProgVariableTypes.Text },
				(pars, gameworld) => new TakeCommodity(pars, gameworld),
				new List<string> { "item", "weight" },
				new List<string>
				{
					"The item to take the weight from",
					"The weight of the material to take, e.g. 120kg, 15lb 3oz, etc"
				},
				"This function takes a certain weight off of a commodity pile. If the weight is equal to or higher than the weight of the commodity pile, it deletes the pile. Returns true if it succeeded at taking the weight.",
				"Items",
				ProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected TakeCommodity(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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

		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var commodity = item.GetItemType<ICommodity>();
		if (commodity == null)
		{
			Result = new BooleanVariable(false);
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
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		if (item.DropsWholeByWeight(weight))
		{
			item.Delete();
		}
		else
		{
			commodity.Weight -= weight;
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}