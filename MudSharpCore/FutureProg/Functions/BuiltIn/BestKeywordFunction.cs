using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class BestKeywordFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public BestKeywordFunction(IList<IFunction> parameters, IFuturemud gameworld, bool considerInventory, bool considerRoomContents)
		: base(parameters)
	{
		_gameworld = gameworld;
		ConsiderInventory = considerInventory;
		ConsiderRoomContents = considerRoomContents;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
		protected set { }
	}

	public bool ConsiderInventory { get; set; }
	public bool ConsiderRoomContents { get; set; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var source = (ICharacter)ParameterFunctions[0].Result;
		if (source == null)
		{
			ErrorMessage = "Source Character was null in BestKeyword function.";
			return StatementResult.Error;
		}

		if (ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Character))
		{
			var target = (ICharacter)ParameterFunctions[1].Result;
			if (target == null)
			{
				ErrorMessage = "Target Character was null in BestKeyword function.";
				return StatementResult.Error;
			}

			var keywords = target.GetKeywordsFor(source);
			var targets = source.Location.LayerCharacters(source.RoomLayer).Except(source)
				.Where(x => x.HasKeywords(keywords, source))
				.Where(x => source.CanSee(x))
				.ToList();
			var index = targets.IndexOf(target);
			if (index == -1)
			{
				ErrorMessage = "Unable to find suitable target in BestKeyword function.";
				// DEBUG
				Console.WriteLine("Unable to find suitable target in BestKeyword function.");
				// END DEBUG
				return StatementResult.Error;
			}

			Result = new TextVariable(
				$"{index + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}");
			return StatementResult.Normal;
		}

		if (ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Item))
		{
			var target = (IGameItem)ParameterFunctions[1].Result;
			if (target == null)
			{
				ErrorMessage = "Target Item was null in BestKeyword function.";
				return StatementResult.Error;
			}

			var keywords = target.GetKeywordsFor(source);
			List<IGameItem> targets;
			if (ConsiderInventory && ConsiderRoomContents)
			{
				targets =
					source.Body.ExternalItems
					.Concat(source.Location.LayerGameItems(source.RoomLayer))
					.Where(x => x.HasKeywords(keywords, source))
					.Where(x => source.CanSee(x))
					.ToList();
			}
			else if (ConsiderInventory)
			{
				targets = source.Body.ExternalItems
					.Where(x => x.HasKeywords(keywords, source))
					.Where(x => source.CanSee(x))
					.ToList();
			}
			else
			{
				targets = source.Location.LayerGameItems(source.RoomLayer)
					.Where(x => x.HasKeywords(keywords, source))
					.Where(x => source.CanSee(x))
					.ToList();
			}
			
			var index = targets.IndexOf(target);
			if (index == -1)
			{
				ErrorMessage = "Unable to find suitable target in BestKeyword function.";
				// DEBUG
				Console.WriteLine("Unable to find suitable target in BestKeyword function.");
				// END DEBUG
				return StatementResult.Error;
			}

			Result = new TextVariable(
				$"{index + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}");
			return StatementResult.Normal;
		}

		Result = new TextVariable("");
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bestkeyword",
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Character
			},
			(pars, gameworld) =>
				new BestKeywordFunction(pars, gameworld, false, false),
			new List<string> { "source", "target" },
			new List<string>
			{
				"The source for whom you want to identify the keyword",
				"The target whose keyword you want to identify"
			},
			"This function allows you to determine what the 'best' keyword is for a target from a source. In this case, best implies a unique keyword that will allow the source to target the target through all regular commands.",
			"Utilities",
			FutureProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bestkeyword",
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			},
			(pars, gameworld) =>
				new BestKeywordFunction(pars, gameworld, true, true),
			new List<string> { "source", "target" },
			new List<string>
			{
				"The source for whom you want to identify the keyword",
				"The target whose keyword you want to identify"
			},
			"This function allows you to determine what the 'best' keyword is for a target from a source. In this case, best implies a unique keyword that will allow the source to target the target through all regular commands. This version of the command considers both the player's inventory and the room contents.",
			"Utilities",
			FutureProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bestkeywordinventory",
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			},
			(pars, gameworld) =>
				new BestKeywordFunction(pars, gameworld, true, false),
			new List<string> { "source", "target" },
			new List<string>
			{
				"The source for whom you want to identify the keyword",
				"The target whose keyword you want to identify"
			},
			"This function allows you to determine what the 'best' keyword is for a target from a source. In this case, best implies a unique keyword that will allow the source to target the target through all regular commands. This version of the command considers only the player's inventory and not the room contents.",
			"Utilities",
			FutureProgVariableTypes.Text
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"bestkeywordroom",
			new[]
			{
				FutureProgVariableTypes.Character,
				FutureProgVariableTypes.Item
			},
			(pars, gameworld) =>
				new BestKeywordFunction(pars, gameworld, false, true),
			new List<string> { "source", "target" },
			new List<string>
			{
				"The source for whom you want to identify the keyword",
				"The target whose keyword you want to identify"
			},
			"This function allows you to determine what the 'best' keyword is for a target from a source. In this case, best implies a unique keyword that will allow the source to target the target through all regular commands. This version of the command considers only the room contents and not the player's inventory.",
			"Utilities",
			FutureProgVariableTypes.Text
		));
	}
}