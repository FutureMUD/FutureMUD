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
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.Indoors),
				new List<string>
				{
					"room",
					"package",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to"
				},
				"Sets the indoors-type of the room to 'Indoors', as if you had done CELL SET TYPE INDOORS.",
				"Rooms",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsWithWindows".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsWithWindows),
				new List<string>
				{
					"room",
					"package",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to"
				},
				"Sets the indoors-type of the room to 'Indoors With Windows', as if you had done CELL SET TYPE WINDOWS.",
				"Rooms",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsNoLight".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsNoLight),
				new List<string>
				{
					"room",
					"package",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to"
				},
				"Sets the indoors-type of the room to 'Indoors With No Light', as if you had done CELL SET TYPE CAVE.",
				"Rooms",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetIndoorsClimateExposed".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.IndoorsClimateExposed),
				new List<string>
				{
					"room",
					"package",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to"
				},
				"Sets the indoors-type of the room to 'Indoors But Climate Exposed', as if you had done CELL SET TYPE EXPOSED.",
				"Rooms",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetOutdoors".ToLowerInvariant(),
				new[] { ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage },
				(pars, gameworld) => new SetIndoors(pars, gameworld, CellOutdoorsType.Outdoors),
				new List<string>
				{
					"room",
					"package",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to"
				},
				"Sets the indoors-type of the room to 'Outdoors', as if you had done CELL SET TYPE OUTDOORS.",
				"Rooms",
				ProgVariableTypes.Boolean
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