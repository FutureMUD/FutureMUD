using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using System;
using System.Collections.Generic;
using System.Linq;

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

internal sealed class SpawnBodyInstanceFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private SpawnBodyInstanceFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter owner)
		{
			ErrorMessage = "Owner Character was null in SpawnBodyInstance function.";
			return StatementResult.Error;
		}

		var form = CharacterFormFunctionHelper.ResolveForm(owner, ParameterFunctions[1].Result);
		if (form is null)
		{
			ErrorMessage = "SpawnBodyInstance could not find a matching dormant form on the owner character.";
			return StatementResult.Error;
		}

		if (ParameterFunctions[2].Result?.GetObject is not ICell location)
		{
			ErrorMessage = "Location was null in SpawnBodyInstance function.";
			return StatementResult.Error;
		}

		var modeText = ParameterFunctions[3].Result?.GetObject?.ToString() ?? string.Empty;
		if (!TryParseMode(owner, modeText, out var mode, out var modeError))
		{
			ErrorMessage = modeError;
			return StatementResult.Error;
		}

		var cloneInventory = false;
		if (ParameterFunctions.Count > 4)
		{
			if (ParameterFunctions[4].Result?.GetObject is not bool clone)
			{
				ErrorMessage = "CloneInventory was not a boolean in SpawnBodyInstance function.";
				return StatementResult.Error;
			}

			cloneInventory = clone;
		}

		var artificialIntelligences = Enumerable.Empty<IArtificialIntelligence>();
		if (ParameterFunctions.Count > 5)
		{
			if (!TryParseArtificialIntelligences(
				    ParameterFunctions[5].Result?.GetObject?.ToString() ?? string.Empty,
				    out var ais,
				    out var aiError))
			{
				ErrorMessage = aiError;
				return StatementResult.Error;
			}

			artificialIntelligences = ais;
		}

		var result = CharacterInstanceService.SpawnBodyInstance(
			owner,
			form,
			location,
			owner.RoomLayer,
			mode,
			CharacterInstancePersistencePolicy.DespawnOnReboot,
			artificialIntelligences,
			cloneInventory);
		if (!result.Success)
		{
			ErrorMessage = result.Message;
			return StatementResult.Error;
		}

		Result = result.Instance;
		return StatementResult.Normal;
	}

	private static bool TryParseMode(
		ICharacter owner,
		string modeText,
		out SecondaryCharacterInstanceSpawnMode mode,
		out string error)
	{
		switch (modeText.ToLowerInvariant().Trim())
		{
			case "":
			case "passive":
			case "temporary":
			case "temp":
				mode = SecondaryCharacterInstanceSpawnMode.Passive;
				error = string.Empty;
				return true;
			case "focus":
			case "focusable":
			case "player":
			case "playerfocusable":
				mode = SecondaryCharacterInstanceSpawnMode.PlayerFocusable;
				error = string.Empty;
				return true;
			case "npcai":
			case "npc":
				mode = SecondaryCharacterInstanceSpawnMode.NpcAiControlled;
				error = string.Empty;
				return true;
			case "script":
			case "scriptai":
			case "scriptedai":
			case "scripted":
				mode = SecondaryCharacterInstanceSpawnMode.ScriptAiControlled;
				error = string.Empty;
				return true;
			case "ai":
				mode = owner is INPC || owner.Identity is INPC
					? SecondaryCharacterInstanceSpawnMode.NpcAiControlled
					: SecondaryCharacterInstanceSpawnMode.ScriptAiControlled;
				error = string.Empty;
				return true;
			default:
				mode = SecondaryCharacterInstanceSpawnMode.Passive;
				error =
					"SpawnBodyInstance mode must be one of passive, focusable, ai, npcai, or scriptai.";
				return false;
		}
	}

	private bool TryParseArtificialIntelligences(
		string input,
		out IReadOnlyList<IArtificialIntelligence> artificialIntelligences,
		out string error)
	{
		var ais = new List<IArtificialIntelligence>();
		foreach (var token in input.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
		                           .Select(x => x.Trim())
		                           .Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			var ai = _gameworld.AIs.GetByIdOrName(token);
			if (ai is null)
			{
				artificialIntelligences = Array.Empty<IArtificialIntelligence>();
				error = $"SpawnBodyInstance could not find an AI called '{token}'.";
				return false;
			}

			if (!ai.IsReadyToBeUsed)
			{
				artificialIntelligences = Array.Empty<IArtificialIntelligence>();
				error = $"The AI '{ai.Name}' has building issues and is not ready to be used.";
				return false;
			}

			if (!ais.Contains(ai))
			{
				ais.Add(ai);
			}
		}

		artificialIntelligences = ais;
		error = string.Empty;
		return true;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text, false);
		RegisterCompiler(ProgVariableTypes.Number, false);
		RegisterCompiler(ProgVariableTypes.Text, true);
		RegisterCompiler(ProgVariableTypes.Number, true);
		RegisterCompilerWithAIs(ProgVariableTypes.Text);
		RegisterCompilerWithAIs(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType, bool includeCloneInventory)
	{
		var parameterTypes = includeCloneInventory
			? new[]
			{
				ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Text,
				ProgVariableTypes.Boolean
			}
			: new[] { ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Text };
		var parameterNames = includeCloneInventory
			? new List<string> { "owner", "form", "location", "mode", "cloneInventory" }
			: new List<string> { "owner", "form", "location", "mode" };
		var parameterHelp = includeCloneInventory
			? new List<string>
			{
				"The character identity whose dormant form should become a secondary body instance.",
				"The form alias or body id to spawn.",
				"The location where the body instance should appear.",
				"The spawn mode: passive, focusable, ai, npcai, or scriptai.",
				"Whether to deep-copy the owner's current direct inventory onto the spawned body."
			}
			: new List<string>
			{
				"The character identity whose dormant form should become a secondary body instance.",
				"The form alias or body id to spawn.",
				"The location where the body instance should appear.",
				"The spawn mode: passive, focusable, ai, npcai, or scriptai."
			};

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"spawnbodyinstance",
			parameterTypes,
			(pars, gameworld) => new SpawnBodyInstanceFunction(pars, gameworld),
			parameterNames,
			parameterHelp,
			"Spawns a secondary body instance from one of a loaded character's owned dormant forms. The returned character is the new physical actor, or the function errors with a builder-facing reason.",
			"Character",
			ProgVariableTypes.Character
		));
	}

	private static void RegisterCompilerWithAIs(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"spawnbodyinstance",
			new[]
			{
				ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Text,
				ProgVariableTypes.Boolean, ProgVariableTypes.Text
			},
			(pars, gameworld) => new SpawnBodyInstanceFunction(pars, gameworld),
			new List<string> { "owner", "form", "location", "mode", "cloneInventory", "ais" },
			new List<string>
			{
				"The character identity whose dormant form should become a secondary body instance.",
				"The form alias or body id to spawn.",
				"The location where the body instance should appear.",
				"The spawn mode: passive, focusable, ai, npcai, or scriptai.",
				"Whether to deep-copy the owner's current direct inventory onto the spawned body.",
				"A comma, semicolon, or pipe separated list of AI ids or names to attach to a script-AI body instance."
			},
			"Spawns a secondary body instance from one of a loaded character's owned dormant forms. For ai/scriptai modes, the final text argument attaches ready AI routines by id or name.",
			"Character",
			ProgVariableTypes.Character
		));
	}
}
