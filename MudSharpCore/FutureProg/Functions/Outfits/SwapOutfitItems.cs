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

namespace MudSharp.FutureProg.Functions.Outfits;

internal class SwapOutfitItems : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"swapoutfititems".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Outfit, FutureProgVariableTypes.OutfitItem,
					FutureProgVariableTypes.OutfitItem
				},
				(pars, gameworld) => new SwapOutfitItems(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected SwapOutfitItems(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var outfit = (IOutfit)ParameterFunctions[0].Result;
		if (outfit == null)
		{
			ErrorMessage = "Null outfit in SwapOutfitItems";
			return StatementResult.Error;
		}

		var item1 = (IOutfitItem)ParameterFunctions[1].Result;
		if (item1 == null)
		{
			ErrorMessage = "Null outfititem1 in SwapOutfitItems";
			return StatementResult.Error;
		}

		var item2 = (IOutfitItem)ParameterFunctions[2].Result;
		if (item2 == null)
		{
			ErrorMessage = "Null outfititem2 in SwapOutfitItems";
			return StatementResult.Error;
		}

		outfit.SwapItems(item1, item2);

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}