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

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToOverlayPackage : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToOverlayPackage".ToLowerInvariant(),
				new[] { ProgVariableTypes.Number },
				(pars, gameworld) => new ToOverlayPackage(pars, gameworld),
				new List<string> { "id" },
				new List<string> { "The ID to look up" },
				"Converts an ID number into the specified type, if one exists",
				"Lookup",
				ProgVariableTypes.OverlayPackage
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToOverlayPackage".ToLowerInvariant(),
				new[] { ProgVariableTypes.Text },
				(pars, gameworld) => new ToOverlayPackage(pars, gameworld),
				new List<string> { "name" },
				new List<string> { "The name to look up" },
				"Converts a name into the specified type, if one exists",
				"Lookup",
				ProgVariableTypes.OverlayPackage
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToOverlayPackage".ToLowerInvariant(),
				new[] { ProgVariableTypes.Number, ProgVariableTypes.Number },
				(pars, gameworld) => new ToOverlayPackage(pars, gameworld),
				new List<string> { "id", "revision" },
				new List<string> { "The ID to look up", "The revision number to look up" },
				"Converts an ID number into the specified type, if one exists",
				"Lookup",
				ProgVariableTypes.OverlayPackage
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToOverlayPackage".ToLowerInvariant(),
				new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
				(pars, gameworld) => new ToOverlayPackage(pars, gameworld),
				new List<string> { "name", "revision" },
				new List<string> { "The name to look up", "The revision number to look up" },
				"Converts a name into the specified type, if one exists",
				"Lookup",
				ProgVariableTypes.OverlayPackage
			)
		);
	}

	#endregion

	#region Constructors

	protected ToOverlayPackage(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		if (ParameterFunctions.Count == 2)
		{
			Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
				? Gameworld.CellOverlayPackages.GetByName((string)ParameterFunctions[0].Result.GetObject,
					Convert.ToInt32(ParameterFunctions[0].Result.GetObject))
				: Gameworld.CellOverlayPackages.Get(Convert.ToInt64(ParameterFunctions[0].Result.GetObject),
					Convert.ToInt32(ParameterFunctions[0].Result.GetObject));
		}
		else
		{
			Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
				? Gameworld.CellOverlayPackages.GetByName((string)ParameterFunctions[0].Result.GetObject)
				: Gameworld.CellOverlayPackages.Get(Convert.ToInt64(ParameterFunctions[0].Result.GetObject));
		}

		return StatementResult.Normal;
	}
}