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

internal class CreateCell : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CreateCell".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.OverlayPackage, FutureProgVariableTypes.Zone },
				(pars, gameworld) => new CreateCell(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CreateCell".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.OverlayPackage, FutureProgVariableTypes.Zone,
					FutureProgVariableTypes.Location
				},
				(pars, gameworld) => new CreateCell(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected CreateCell(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Location;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var package = (ICellOverlayPackage)ParameterFunctions[0].Result?.GetObject;
		if (package == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var zone = (IZone)ParameterFunctions[1].Result?.GetObject;
		if (zone == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var cell = ParameterFunctions.Count == 3 ? (ICell)ParameterFunctions[2].Result?.GetObject : default;
		var newRoom = new Room(zone, package, cell, false);
		Result = newRoom.Cells.First();
		return StatementResult.Normal;
	}
}