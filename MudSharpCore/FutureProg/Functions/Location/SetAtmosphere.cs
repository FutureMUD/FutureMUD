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
using MudSharp.Form.Material;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetAtmosphere : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetAtmosphere".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Number
				},
				(pars, gameworld) => new SetAtmosphere(pars, gameworld, true),
				new List<string>
				{
					"room",
					"package",
					"gas",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The Id number of the gas you want to set as the atmosphere here",
				},
				"Sets the atmosphere of a room as if you had done CELL SET ATMOSPHERE.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetAtmosphere".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new SetAtmosphere(pars, gameworld, true),
				new List<string>
				{
					"room",
					"package",
					"gas",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The name of the gas you want to set as the atmosphere here",
				},
				"Sets the atmosphere of a room as if you had done CELL SET ATMOSPHERE.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetAtmosphereLiquid".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Number
				},
				(pars, gameworld) => new SetAtmosphere(pars, gameworld, false),
				new List<string>
				{
					"room",
					"package",
					"liquid",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The Id number of the liquid you want to set as the atmosphere here",
				},
				"Sets the atmosphere of a room as if you had done CELL SET ATMOSPHERE.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetAtmosphereLiquid".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new SetAtmosphere(pars, gameworld, false),
				new List<string>
				{
					"room",
					"package",
					"liquid",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The name of the liquid you want to set as the atmosphere here",
				},
				"Sets the atmosphere of a room as if you had done CELL SET ATMOSPHERE.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetAtmosphere(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool gas) : base(
		parameterFunctions)
	{
		Gameworld = gameworld;
		Gas = gas;
	}

	#endregion

	public bool Gas { get; set; }

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

		IFluid fluid;
		if (ParameterFunctions[2].ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			fluid = Gas
				? (IFluid)Gameworld.Gases.Get(Convert.ToInt64(ParameterFunctions[2].Result?.GetObject ?? 0))
				: Gameworld.Liquids.Get(Convert.ToInt64(ParameterFunctions[2].Result?.GetObject ?? 0));
		}
		else
		{
			fluid = Gas
				? (IFluid)Gameworld.Gases.GetByName(ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty)
				: Gameworld.Liquids.GetByName(ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty);
		}

		if (fluid == null)
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
		overlay.Atmosphere = fluid;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}