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
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetQuality : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setquality",
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Number },
				(pars, gameworld) => new SetQuality(pars, gameworld),
				new List<string> { "item", "quality" },
				new List<string>
				{
					"The item whose quality you wish to set", "The quality of the item. 0 = Terribly, 11 = Legendary"
				},
				"This function sets the quality of an item to whatever quality you specify.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setquality",
				new[] { FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
				(pars, gameworld) => new SetQuality(pars, gameworld),
				new List<string> { "item", "quality" },
				new List<string>
				{
					"The item whose quality you wish to set",
					"The quality of the item. Valid qualities are Terrible, Extremely Bad, Bad, Poor, Substandard, Standard, Good, Very Good, Great, Excellent, Heroic, Legendary."
				},
				"This function sets the quality of an item to whatever quality you specify.",
				"Items",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetQuality(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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

		var item = (IGameItem)ParameterFunctions[0].Result?.GetObject;
		if (item == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		ItemQuality quality;
		if (ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			quality = (ItemQuality)Convert.ToInt32(ParameterFunctions[1].Result?.GetObject ?? 0);
			if (!Enum.IsDefined(typeof(ItemQuality), quality))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}
		else
		{
			if (!(ParameterFunctions[1].Result?.GetObject?.ToString() ?? "").TryParseEnum(out quality))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		item.Quality = quality;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}