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
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetCellLightMultiplier : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetCellLightMultiplier".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Location, ProgVariableTypes.OverlayPackage,
					ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetCellLightMultiplier(pars, gameworld),
				new List<string>
				{
					"room",
					"package",
					"multiplier",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"A multiplier for natural light levels - 1.0 = full natural light, 0.0 = no natural light",
				},
				"Sets the light level multiplier of a room as if you had done CELL SET LIGHTMULTIPLIER.",
				"Rooms",
				ProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetCellLightMultiplier(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(
		parameterFunctions)
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

		var multiplier = Convert.ToDouble(ParameterFunctions[2].Result?.GetObject ?? 0.0);

		if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var overlay = cell.GetOrCreateOverlay(package);
		overlay.AmbientLightFactor = multiplier;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}