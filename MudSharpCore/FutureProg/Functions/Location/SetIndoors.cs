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
using MudSharp.Construction;
using MudSharp.Framework.Revision;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetIndoors : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoors".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.Indoors)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsWithWindows".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsWithWindows)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsNoLight".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsNoLight)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsClimateExposed".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsClimateExposed)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetOutdoors".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.Outdoors)
			)
		);
	}

	#endregion

	#region Constructors

	protected SetIndoors(IList<IFunction> parameterFunctions, IFuturemud gameworld, CellOutdoorsType outdoorsType) :
		base(parameterFunctions)
	{
		Gameworld = gameworld;
		OutdoorsType = outdoorsType;
	}

	#endregion

	public CellOutdoorsType OutdoorsType { get; set; }

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
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
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var package = (ICellOverlayPackage)ParameterFunctions[1].Result?.GetObject;
		if (package == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var overlay = cell.GetOrCreateOverlay(package);
		overlay.OutdoorsType = OutdoorsType;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}