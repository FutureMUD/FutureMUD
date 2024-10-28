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
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Text },
				(pars, gameworld) => new CreateOverlay(pars, gameworld),
				new List<string>
				{
					"builder",
					"name"
				},
				new List<string>
				{
					"The builder who is creating the package",
					"The name of the package you want to create. Must be unique."
				},
				"Creates a new cell overlay package with the specified name and builder, as if you had done CELL PACKAGE NEW. Can return null if the name is already taken so be sure to check for that.",
				"Rooms",
				ProgVariableTypes.OverlayPackage
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

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.OverlayPackage;
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