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

namespace MudSharp.FutureProg.Functions.Outfits;

internal class AddItemToOutfit : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"additemtooutfit",
				new[] { FutureProgVariableTypes.Outfit, FutureProgVariableTypes.Item },
				(pars, gameworld) => new AddItemToOutfit(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"additemtooutfit",
				new[] { FutureProgVariableTypes.Outfit, FutureProgVariableTypes.Item, FutureProgVariableTypes.Item },
				(pars, gameworld) => new AddItemToOutfit(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"additemtooutfit",
				new[]
				{
					FutureProgVariableTypes.Outfit, FutureProgVariableTypes.Item, FutureProgVariableTypes.Item,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new AddItemToOutfit(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"additemtooutfit",
				new[] { FutureProgVariableTypes.Outfit, FutureProgVariableTypes.Item, FutureProgVariableTypes.Text },
				(pars, gameworld) => new AddItemToOutfit(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected AddItemToOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
			ErrorMessage = "Null outfit in AddItemToOutfit";
			return StatementResult.Error;
		}

		var item = (IGameItem)ParameterFunctions[1].Result;
		if (item == null)
		{
			ErrorMessage = "Null item in AddItemToOutfit";
			return StatementResult.Error;
		}

		if (outfit.Items.Any(x => x.Id == item.Id))
		{
			Result = null;
			return StatementResult.Normal;
		}

		if (ParameterFunctions.Count == 2)
		{
			Result = outfit.AddItem(item, null, null);
			return StatementResult.Normal;
		}

		if (ParameterFunctions.Count == 4)
		{
			var container = (IGameItem)ParameterFunctions[2].Result;
			var profileText = ParameterFunctions[3].Result?.ToString();
			var profile = string.IsNullOrEmpty(profileText)
				? item.GetItemType<IWearable>()?.Profiles.FirstOrDefault(x => x.Name.EqualTo(profileText))
				: default;
			Result = outfit.AddItem(item, container, profile);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[2].ReturnType == FutureProgVariableTypes.Text)
		{
			var profileText = ParameterFunctions[2].Result?.ToString();
			var profile = string.IsNullOrEmpty(profileText)
				? item.GetItemType<IWearable>()?.Profiles.FirstOrDefault(x => x.Name.EqualTo(profileText))
				: default;
			Result = outfit.AddItem(item, null, profile);
			return StatementResult.Normal;
		}
		else
		{
			var container = (IGameItem)ParameterFunctions[2].Result;
			Result = outfit.AddItem(item, container, null);
			return StatementResult.Normal;
		}
	}
}