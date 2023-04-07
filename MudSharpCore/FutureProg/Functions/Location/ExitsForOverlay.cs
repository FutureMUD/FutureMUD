using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location;

internal class ExitsForOverlay : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ExitsForOverlay".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new ExitsForOverlay(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected ExitsForOverlay(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Exit | FutureProgVariableTypes.Collection;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var cell = (ICell)ParameterFunctions[0].Result?.GetObject;
		if (cell == null)
		{
			Result = new CollectionVariable(new List<ICellExit>(), FutureProgVariableTypes.Exit);
			return StatementResult.Normal;
		}

		var package = (ICellOverlayPackage)ParameterFunctions[1].Result?.GetObject;
		if (package == null)
		{
			Result = new CollectionVariable(new List<ICellExit>(), FutureProgVariableTypes.Exit);
			return StatementResult.Normal;
		}

		Result = new CollectionVariable(Gameworld.ExitManager.GetExitsFor(cell, package).ToList(),
			FutureProgVariableTypes.Exit);
		return StatementResult.Normal;
	}
}