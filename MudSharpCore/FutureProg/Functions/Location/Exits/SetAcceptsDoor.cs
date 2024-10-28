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

internal class SetAcceptsDoor : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetAcceptsDoor".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Exit, ProgVariableTypes.OverlayPackage,
					ProgVariableTypes.Boolean, ProgVariableTypes.Number
				},
				(pars, gameworld) => new SetAcceptsDoor(pars, gameworld)
			)
		);
	}

	#endregion

	#region Constructors

	protected SetAcceptsDoor(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var truth = (bool)(ParameterFunctions[2].Result?.GetObject ?? false);
		var size = (SizeCategory)Convert.ToInt32(ParameterFunctions[3].Result?.GetObject ?? (int)SizeCategory.Titanic);
		if (!Enum.IsDefined(typeof(SizeCategory), size))
		{
			size = SizeCategory.Titanic;
		}

		exit.Exit.AcceptsDoor = truth;
		exit.Exit.DoorSize = size;
		exit.Exit.Changed = true;

		return StatementResult.Normal;
	}
}