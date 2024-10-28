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
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Location.Exits;

internal class SetExitSize : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetExitSize".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Exit, ProgVariableTypes.OverlayPackage,
					ProgVariableTypes.Number, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetExitSize(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected SetExitSize(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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
		Result = exit;

		var size = (SizeCategory)Convert.ToInt32(ParameterFunctions[2].Result?.GetObject ?? (int)SizeCategory.Titanic);
		if (!Enum.IsDefined(typeof(SizeCategory), size))
		{
			size = SizeCategory.Titanic;
		}

		var upright =
			(SizeCategory)Convert.ToInt32(ParameterFunctions[3].Result?.GetObject ?? (int)SizeCategory.Titanic);
		if (!Enum.IsDefined(typeof(SizeCategory), upright))
		{
			upright = SizeCategory.Titanic;
		}

		exit.Exit.MaximumSizeToEnter = size;
		exit.Exit.MaximumSizeToEnterUpright = upright;
		exit.Exit.Changed = true;

		return StatementResult.Normal;
	}
}