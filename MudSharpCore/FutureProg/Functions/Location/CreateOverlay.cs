using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location;

internal class CreateOverlay : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CreateOverlay".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Text },
				(pars, gameworld) => new CreateOverlay(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected CreateOverlay(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.OverlayPackage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (character == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var text = ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty;
		if (string.IsNullOrWhiteSpace(text) || Gameworld.CellOverlayPackages.Any(x => x.Name.EqualTo(text)))
		{
			Result = null;
			return StatementResult.Normal;
		}

		var overlay = new CellOverlayPackage(Gameworld, character.Account, text);
		Gameworld.Add(overlay);
		Result = overlay;
		return StatementResult.Normal;
	}
}