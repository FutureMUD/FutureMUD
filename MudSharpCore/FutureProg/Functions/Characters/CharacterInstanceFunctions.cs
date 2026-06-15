using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class SameIdentityFunction : BuiltInFunction
{
	private SameIdentityFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

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

		Result = new BooleanVariable(
			ParameterFunctions[0].Result?.GetObject is ICharacter lhs &&
			ParameterFunctions[1].Result?.GetObject is ICharacter rhs &&
			CharacterInstanceIdentityComparer.SameIdentity(lhs, rhs));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"sameidentity",
			[ProgVariableTypes.Character, ProgVariableTypes.Character],
			(pars, _) => new SameIdentityFunction(pars),
			["lhs", "rhs"],
			["The first character to compare.", "The second character to compare."],
			"Returns true if both character variables represent the same durable character identity, even if they are different active bodies.",
			"Character",
			ProgVariableTypes.Boolean
		));
	}
}

internal sealed class SamePhysicalInstanceFunction : BuiltInFunction
{
	private SamePhysicalInstanceFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

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

		Result = new BooleanVariable(
			ParameterFunctions[0].Result?.GetObject is ICharacter lhs &&
			ParameterFunctions[1].Result?.GetObject is IPerceivable rhs &&
			CharacterInstanceIdentityComparer.SamePhysicalInstance(lhs, rhs));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"samephysicalinstance",
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable],
			(pars, _) => new SamePhysicalInstanceFunction(pars),
			["character", "target"],
			["The character instance to compare.", "The perceivable target, normally another character or body."],
			"Returns true if both arguments are the same active physical actor/body, and false for another simultaneous body belonging to the same identity.",
			"Character",
			ProgVariableTypes.Boolean
		));
	}
}

internal sealed class CharacterIdentityIdFunction : BuiltInFunction
{
	private CharacterIdentityIdFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new NumberVariable(ParameterFunctions[0].Result?.GetObject is ICharacter character
			? CharacterInstanceIdentityComparer.IdentityId(character)
			: 0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"characteridentityid",
			[ProgVariableTypes.Character],
			(pars, _) => new CharacterIdentityIdFunction(pars),
			["character"],
			["The character to inspect."],
			"Returns the durable identity id for a character, shared by all simultaneous bodies.",
			"Character",
			ProgVariableTypes.Number
		));
	}
}

internal sealed class CharacterInstanceIdFunction : BuiltInFunction
{
	private CharacterInstanceIdFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new NumberVariable(ParameterFunctions[0].Result?.GetObject is ICharacter character
			? CharacterInstanceIdentityComparer.InstanceId(character) ?? 0
			: 0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"characterinstanceid",
			[ProgVariableTypes.Character],
			(pars, _) => new CharacterInstanceIdFunction(pars),
			["character"],
			["The character instance to inspect."],
			"Returns the CharacterInstances row id for this active body, or 0 for legacy actors without an instance row.",
			"Character",
			ProgVariableTypes.Number
		));
	}
}

internal sealed class CharacterBodyIdFunction : BuiltInFunction
{
	private CharacterBodyIdFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new NumberVariable(ParameterFunctions[0].Result?.GetObject is ICharacter character
			? character.Body?.Id ?? 0
			: 0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"characterbodyid",
			[ProgVariableTypes.Character],
			(pars, _) => new CharacterBodyIdFunction(pars),
			["character"],
			["The character instance to inspect."],
			"Returns the current physical body id for this actor instance, or 0 if no body is available.",
			"Character",
			ProgVariableTypes.Number
		));
	}
}

internal sealed class ToCharacterInstanceFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToCharacterInstanceFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Character;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var identityId = (long?) (decimal?) ParameterFunctions[0].Result?.GetObject ?? 0;
		var instanceId = (long?) (decimal?) ParameterFunctions[1].Result?.GetObject ?? 0;
		Result = CharacterInstanceIdentityComparer.ResolvePhysicalInstance(_gameworld, identityId, instanceId,
			fallbackToPrimary: false);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tocharacterinstance",
			[ProgVariableTypes.Number, ProgVariableTypes.Number],
			(pars, gameworld) => new ToCharacterInstanceFunction(pars, gameworld),
			["identityId", "instanceId"],
			["The durable character identity id.", "The active CharacterInstances row id."],
			"Retrieves the currently loaded physical actor for an identity and instance id. Returns null if that specific instance is not loaded.",
			"Lookup",
			ProgVariableTypes.Character
		));
	}
}
