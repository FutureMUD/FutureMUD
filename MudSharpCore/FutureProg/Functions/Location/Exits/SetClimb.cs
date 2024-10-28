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
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class SetClimb : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetClimb".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Exit, ProgVariableTypes.OverlayPackage,
					ProgVariableTypes.Boolean, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetClimb(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected SetClimb(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Exit;
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

		if (package.Status != RevisionStatus.UnderDesign && package.Status != RevisionStatus.PendingRevision)
		{
			Result = null;
			return StatementResult.Normal;
		}

		exit = GetOrCopyExit.GetOrCopy(exit, package);

		var isclimb = (bool)(ParameterFunctions[2].Result?.GetObject ?? false);
		var difficulty = (Difficulty)Convert.ToInt32(ParameterFunctions[3].Result?.GetObject ?? 0);

		exit.Exit.IsClimbExit = isclimb;
		exit.Exit.ClimbDifficulty = difficulty;
		exit.Exit.Changed = true;
		Result = exit;
		return StatementResult.Normal;
	}
}