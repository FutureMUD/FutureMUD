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

internal class RemoveOutfit : BuiltInFunction
{
	public bool Force { get; set; }
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removeoutfit",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Outfit },
				(pars, gameworld) => new RemoveOutfit(pars, gameworld, false)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removeoutfit",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Outfit, ProgVariableTypes.Item
				},
				(pars, gameworld) => new RemoveOutfit(pars, gameworld, false)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removeoutfitforce",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Outfit },
				(pars, gameworld) => new RemoveOutfit(pars, gameworld, true)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removeoutfitforce",
				new[]
				{
					ProgVariableTypes.Character, ProgVariableTypes.Outfit, ProgVariableTypes.Item
				},
				(pars, gameworld) => new RemoveOutfit(pars, gameworld, true)
			)
		);
	}

	#endregion

	#region Constructors

	protected RemoveOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool force) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		Force = force;
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

		var character = (ICharacter)ParameterFunctions[0].Result;
		if (character == null)
		{
			ErrorMessage = "Null character in RemoveOutfit";
			return StatementResult.Error;
		}

		var outfit = (IOutfit)ParameterFunctions[1].Result;
		if (outfit == null)
		{
			ErrorMessage = "Null outfit in RemoveOutfit";
			return StatementResult.Error;
		}

		IGameItem targetContainer = null;
		if (ParameterFunctions.Count == 3)
		{
			targetContainer = (IGameItem)ParameterFunctions[2].Result;
			if (targetContainer?.IsItemType<IContainer>() == false)
			{
				targetContainer = null;
			}
		}

		var items = character.Body.DirectWornItems.Where(x => outfit.Items.Any(y => y.Id == x.Id)).Reverse().ToList();
		if (!items.Any())
		{
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		foreach (var outfitItem in items)
		{
			if (Force)
			{
				character.Body.Take(outfitItem);
				if (targetContainer?.GetItemType<IContainer>()?.CanPut(outfitItem) == true)
				{
					targetContainer.GetItemType<IContainer>().Put(character, outfitItem);
				}
				else
				{
					outfitItem.RoomLayer = character.RoomLayer;
					character.Location.Insert(outfitItem);
				}

				continue;
			}

			var coverItemInfo = character.Body.CoverInformation(outfitItem).Where(x => x.Item2 != null).ToList();
			var coveringItems = new List<IGameItem>();
			if (coverItemInfo.Any())
			{
				coveringItems.AddRange(coverItemInfo.SelectNotNull(x => x.Item2).Reverse());
			}

			coveringItems.Add(outfitItem);
			foreach (var item in coveringItems)
			{
				var containerId = targetContainer?.Id ??
				                  outfit.Items.FirstOrDefault(x => x.Id == item.Id)?.PreferredContainerId;
				var container = targetContainer.GetItemType<IContainer>() ?? character.ContextualItems.FirstOrDefault(
						x => x.Id == containerId && x.GetItemType<IContainer>()?.CanPut(item) == true)
					?.GetItemType<IContainer>();
				if (!Commands.Modules.InventoryModule.RecursiveRemoveItem(character, item, container, out _))
				{
					Result = new BooleanVariable(false);
					return StatementResult.Normal;
				}
			}
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}