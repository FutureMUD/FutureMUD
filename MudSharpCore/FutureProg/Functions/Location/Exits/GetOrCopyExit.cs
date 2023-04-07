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

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class GetOrCopyExit : BuiltInFunction
{
	public static ICellExit GetOrCopy(ICellExit exit, ICellOverlayPackage package)
	{
		var overlay = exit.Origin.GetOrCreateOverlay(package);
		var otherOverlay = exit.Destination.GetOrCreateOverlay(package);
		if (!exit.Origin.Overlays.Except(overlay).Any(x => x.ExitIDs.Contains(exit.Exit.Id)))
		{
			return exit;
		}

		var newExit = exit.Exit.Clone();
		overlay.RemoveExit(exit.Exit);
		overlay.AddExit(newExit);
		otherOverlay.RemoveExit(exit.Exit);
		otherOverlay.AddExit(newExit);
		package.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Origin, overlay);
		package.Gameworld.ExitManager.UpdateCellOverlayExits(exit.Destination, otherOverlay);
		return newExit.CellExitFor(exit.Origin);
	}

	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"GetOrCopyExit".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Exit, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new GetOrCopyExit(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected GetOrCopyExit(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Exit;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var exit = (ICellExit)ParameterFunctions[0].Result?.GetObject;
		if (exit == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var package = (ICellOverlayPackage)ParameterFunctions[1].Result?.GetObject;
		if (package == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		Result = GetOrCopy(exit, package);
		return StatementResult.Normal;
	}
}