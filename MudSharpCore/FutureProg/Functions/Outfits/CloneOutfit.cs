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

internal class CloneOutfit : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"cloneoutfit",
				new[]
				{
					ProgVariableTypes.Outfit, ProgVariableTypes.Character, ProgVariableTypes.Text
				},
				(pars, gameworld) => new CloneOutfit(pars, gameworld),
				[
					"outfit",
					"character",
					"name"
				],
				[
					"The outfit that you want to clone",
					"The character who should get the outfit",
					"The name of the new cloned outfit"
				],
				"This function takes an existing outfit and clones it to a new one. This is one way that you can copy an outfit from one player to another for example. Returns the outfit that it creates. Can return null if the name is not valid.",
				"Outfits",
				ProgVariableTypes.Outfit
			)
		);
	}

	#endregion

	#region Constructors

	protected CloneOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Outfit;
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
			ErrorMessage = "Null outfit in CloneOutfit";
			return StatementResult.Error;
		}

		var owner = (ICharacter)ParameterFunctions[1].Result;
		if (owner == null)
		{
			ErrorMessage = "Null owner in CloneOutfit";
			return StatementResult.Error;
		}

		var newName = ParameterFunctions[2].Result.GetObject?.ToString();
		if (string.IsNullOrEmpty(newName) || owner.Outfits.Any(x => x.Name.EqualTo(newName)))
		{
			// This doesn't need to be an error, because it might be a valid case
			Result = null;
			return StatementResult.Normal;
		}

		var newOutfit = outfit.CopyOutfit(owner, newName);
		owner.AddOutfit(newOutfit);
		Result = newOutfit;
		return StatementResult.Normal;
	}
}