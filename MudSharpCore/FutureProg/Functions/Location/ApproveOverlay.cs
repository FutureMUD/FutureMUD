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

internal class ApproveOverlay : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ApproveOverlay".ToLowerInvariant(),
				new[] { FutureProgVariableTypes.OverlayPackage, FutureProgVariableTypes.Character },
				(pars, gameworld) => new ApproveOverlay(pars, gameworld),
				new List<string>
				{
					"package",
					"builder",
				},
				new List<string>
				{ 
					"The package that you want to approve",
					"The builder who is approving the package, or null for 'system'",
				},
				"Approves the specified overlay package as ready for use. Returns true if successful",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ApproveOverlay".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.OverlayPackage, FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new ApproveOverlay(pars, gameworld),
				new List<string>
				{
					"package",
					"builder",
					"comment"
				},
				new List<string>
				{
					"The package that you want to approve",
					"The builder who is approving the package, or null for 'system'",
					"A comment for the builder approval log"
				},
				"Approves the specified overlay package as ready for use. Returns true if successful",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected ApproveOverlay(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

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

		var package = (ICellOverlayPackage)ParameterFunctions[0].Result?.GetObject;
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

		var character = (ICharacter)ParameterFunctions[1].Result?.GetObject;
		if (character == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var text = ParameterFunctions.Count == 3
			? ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty
			: string.Empty;
		package.ChangeStatus(RevisionStatus.Current, text, character.Account);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}