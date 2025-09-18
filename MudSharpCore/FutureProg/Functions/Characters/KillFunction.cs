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

namespace MudSharp.FutureProg.Functions.Characters;

internal class KillFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Kill".ToLowerInvariant(),
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Boolean },
				(pars, gameworld) => new KillFunction(pars, gameworld),
				new List<string> { "Victim", "MakeCorpse" },
				new List<string>
				{
					"The character who you want to kill",
					"If true, leaves a corpse behind. Otherwise disposes of corpse."
				},
				"Kills the specified character, and optionally leaves a corpse. Returns the corpse as an item.",
				"Characters",
				ProgVariableTypes.Item
			)
		);
	}

	#endregion

	#region Constructors

	protected KillFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Item;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character)
		{
			Result = null;
			return StatementResult.Normal;
		}

		if (character.State.IsDead())
		{
			Result = null;
			return StatementResult.Normal;
		}

		var leaveCorpse = ParameterFunctions[1].Result?.GetObject as bool? ?? true;
		var corpse = character.Die();
		if (!leaveCorpse)
		{
			corpse.Delete();
			corpse = null;
		}

		Result = corpse;
		return StatementResult.Normal;
	}
}
