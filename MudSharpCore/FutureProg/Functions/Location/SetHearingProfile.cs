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
using MudSharp.Form.Audio;
using MudSharp.Framework.Revision;

namespace MudSharp.FutureProg.Functions.Location;

internal class SetHearingProfile : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetHearingProfile".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Number
				},
				(pars, gameworld) => new SetHearingProfile(pars, gameworld),
				new List<string>
				{
					"room",
					"package",
					"profile",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The Id number of the hearing profile to use",
				},
				"Sets the hearing profile of the room as if you had done CELL SET HEARING.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SetHearingProfile".ToLowerInvariant(),
				new[]
				{
					FutureProgVariableTypes.Location, FutureProgVariableTypes.OverlayPackage,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new SetHearingProfile(pars, gameworld),
				new List<string>
				{
					"room",
					"package",
					"profile",
				},
				new List<string>
				{
					"The room you want to edit",
					"The package that the edit belongs to",
					"The name of the hearing profile to use",
				},
				"Sets the hearing profile of the room as if you had done CELL SET HEARING.",
				"Rooms",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected SetHearingProfile(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		IHearingProfile profile;
		if (ParameterFunctions[2].ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			profile = Gameworld.HearingProfiles.Get(Convert.ToInt64(ParameterFunctions[2].Result?.GetObject ?? 0));
		}
		else
		{
			profile = Gameworld.HearingProfiles.GetByName(ParameterFunctions[2].Result?.GetObject?.ToString() ??
			                                              string.Empty);
		}

		if (profile == null)
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
		overlay.HearingProfile = profile;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}