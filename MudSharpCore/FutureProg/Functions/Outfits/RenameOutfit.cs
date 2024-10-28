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

internal class RenameOutfit : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"renameoutfit",
				new[] { ProgVariableTypes.Outfit, ProgVariableTypes.Text },
				(pars, gameworld) => new RenameOutfit(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected RenameOutfit(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
			ErrorMessage = "Null outfit in RenameOutfits";
			return StatementResult.Error;
		}

		var newName = ParameterFunctions[1].Result.GetObject?.ToString();
		if (string.IsNullOrEmpty(newName) || outfit.Owner.Outfits.Any(x => x.Name.EqualTo(newName)))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		outfit.Name = newName;
		outfit.Owner.OutfitsChanged = true;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}