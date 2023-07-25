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

internal class ReviseOverlay : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ReviseOverlay".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.OverlayPackage, FutureProgVariableTypes.Character },
				(pars, gameworld) => new ReviseOverlay(pars, gameworld),
				new List<string>
				{
					"package",
					"builder",
				},
				new List<string>
				{
					"The package that you want to revise",
					"The builder who is doing the revision",
				},
				"Creates a new revision for an overlay package.",
				"Rooms",
				FutureProgVariableTypes.OverlayPackage
			)
		);
	}

	#endregion

	#region Constructors

	protected ReviseOverlay(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.OverlayPackage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var overlay = (ICellOverlayPackage)ParameterFunctions[0].Result?.GetObject;
		if (overlay == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var builder = (ICharacter)ParameterFunctions[1].Result?.GetObject;
		if (builder == null)
		{
			Result = null;
			return StatementResult.Normal;
		}

		if (overlay.Status != RevisionStatus.Current)
		{
			Result = null;
			return StatementResult.Normal;
		}

		var newOverlay = (ICellOverlayPackage)overlay.CreateNewRevision(builder);
		Result = newOverlay;
		return StatementResult.Normal;
	}
}