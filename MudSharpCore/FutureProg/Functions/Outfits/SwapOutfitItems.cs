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
					ProgVariableTypes.Outfit, ProgVariableTypes.OutfitItem,
					ProgVariableTypes.OutfitItem
				},
				(pars, gameworld) => new SwapOutfitItems(pars, gameworld),
				[
					"outfit",
					"item1",
					"item2"
				],
				[
					"The outfit whose items you want to swap the order of",
					"The first item",
					"The second item"
				],
				"Swaps the order of two outfit items in an outfit, so they are worn or removed in a different order. Returns true if the items were swapped.",
				"Outfits",
				ProgVariableTypes.Boolean
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

		if (!outfit.Items.Contains(item1) || !outfit.Items.Contains(item2))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		outfit.SwapItems(item1, item2);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}