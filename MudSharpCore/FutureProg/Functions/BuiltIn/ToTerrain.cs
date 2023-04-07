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

internal class ToTerrain : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToTerrain".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Number },
				(pars, gameworld) => new ToTerrain(pars, gameworld),
				new List<string> { "id" },
				new List<string> { "The ID to look up" },
				"Converts an ID number into the specified type, if one exists",
				"Lookup",
				FutureProgVariableTypes.Terrain
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ToTerrain".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Text },
				(pars, gameworld) => new ToTerrain(pars, gameworld),
				new List<string> { "name" },
				new List<string> { "The name to look up" },
				"Converts a name into the specified type, if one exists",
				"Lookup",
				FutureProgVariableTypes.Terrain
			)
		);
	}

	#endregion

	#region Constructors

	protected ToTerrain(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Terrain;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? Gameworld.Terrains.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: Gameworld.Terrains.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}
}