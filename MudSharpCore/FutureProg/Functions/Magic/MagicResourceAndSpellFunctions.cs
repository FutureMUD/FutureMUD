#nullable enable

using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Magic;

internal abstract class MagicBuiltInFunctionBase : BuiltInFunction
{
	protected MagicBuiltInFunctionBase(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	protected IFuturemud Gameworld { get; }

	protected bool TryGetCharacter(int index, out ICharacter? character)
	{
		character = ParameterFunctions[index].Result?.GetObject as ICharacter;
		if (character is not null)
		{
			return true;
		}

		ErrorMessage = "The character argument cannot be null.";
		return false;
	}

	protected bool TryGetSpell(int index, out IMagicSpell? spell)
	{
		spell = ParameterFunctions[index].Result?.GetObject as IMagicSpell;
		if (spell is not null)
		{
			return true;
		}

		ErrorMessage = "The magic spell argument cannot be null.";
		return false;
	}

	protected bool TryGetMagicResource(int index, bool useId, out IMagicResource? resource)
	{
		resource = null;
		var reference = ParameterFunctions[index].Result?.GetObject;
		if (reference is null)
		{
			ErrorMessage = "The magic resource argument cannot be null.";
			return false;
		}

		resource = useId
			? Gameworld.MagicResources.Get((long)(decimal)reference)
			: Gameworld.MagicResources.GetByIdOrName(reference.ToString() ?? string.Empty);

		if (resource is not null)
		{
			return true;
		}

		ErrorMessage = $"There was no magic resource found with reference {reference}.";
		return false;
	}

	protected bool TryGetSpellPower(int index, out SpellPower power)
	{
		var reference = ParameterFunctions[index].Result?.GetObject?.ToString() ?? string.Empty;
		if (reference.TryParseEnum<SpellPower>(out power))
		{
			return true;
		}

		foreach (var value in Enum.GetValues<SpellPower>())
		{
			if (value.DescribeEnum().EqualTo(reference))
			{
				power = value;
				return true;
			}
		}

		ErrorMessage =
			$"The text {reference} is not a valid spell power. Valid values are {Enum.GetValues<SpellPower>().Select(x => x.DescribeEnum()).ListToString()}.";
		return false;
	}

	protected static IEnumerable<MagicSpellParent> ActiveSpellParents(ICharacter character, IMagicSpell? spell = null)
	{
		return character.EffectsOfType<MagicSpellParent>(x => x.Applies() && (spell is null || x.Spell == spell));
	}
}

internal class MagicResourceLevelFunction : MagicBuiltInFunctionBase
{
	private readonly bool _useId;

	private MagicResourceLevelFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool useId) :
		base(parameterFunctions, gameworld)
	{
		_useId = useId;
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IHaveMagicResource haver)
		{
			ErrorMessage = "The magic resource holder argument cannot be null.";
			return StatementResult.Error;
		}

		if (!TryGetMagicResource(1, _useId, out var resource))
		{
			return StatementResult.Error;
		}

		Result = new NumberVariable(haver.MagicResourceAmounts.TryGetValue(resource!, out var amount) ? amount : 0.0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register(false, ProgVariableTypes.Text, "resource", "The name or id of the magic resource to inspect");
		Register(true, ProgVariableTypes.Number, "resourceId", "The id of the magic resource to inspect");
	}

	private static void Register(bool useId, ProgVariableTypes resourceType, string resourceName, string resourceHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"magicresourcelevel",
			new[] { ProgVariableTypes.MagicResourceHaver, resourceType },
			(pars, gameworld) => new MagicResourceLevelFunction(pars, gameworld, useId),
			new[] { "thing", resourceName },
			new[] { "The character, item, or room whose magic resource level should be inspected", resourceHelp },
			"Returns the current amount of the specified magic resource on a character, item, or room.",
			"Magic",
			ProgVariableTypes.Number));
	}
}

internal class MagicResourceMutationFunction : MagicBuiltInFunctionBase
{
	private enum MutationMode
	{
		Set,
		Add,
		Subtract
	}

	private readonly MutationMode _mode;
	private readonly bool _useId;

	private MagicResourceMutationFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld,
		MutationMode mode, bool useId) : base(parameterFunctions, gameworld)
	{
		_mode = mode;
		_useId = useId;
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not IHaveMagicResource haver)
		{
			ErrorMessage = "The magic resource holder argument cannot be null.";
			return StatementResult.Error;
		}

		if (!TryGetMagicResource(1, _useId, out var resource))
		{
			return StatementResult.Error;
		}

		var amount = Convert.ToDouble(ParameterFunctions[2].Result?.GetObject ?? 0.0M);
		var current = haver.MagicResourceAmounts.TryGetValue(resource!, out var value) ? value : 0.0;
		var delta = _mode switch
		{
			MutationMode.Set => amount - current,
			MutationMode.Add => amount,
			MutationMode.Subtract => -amount,
			_ => throw new ApplicationException("Unknown magic resource mutation mode.")
		};

		haver.AddResource(resource!, delta);
		Result = new NumberVariable(haver.MagicResourceAmounts.TryGetValue(resource!, out var result) ? result : 0.0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("setmagicresource", MutationMode.Set);
		Register("setmagicresourcelevel", MutationMode.Set);
		Register("addmagicresource", MutationMode.Add);
		Register("addmagicresourcelevel", MutationMode.Add);
		Register("subtractmagicresource", MutationMode.Subtract);
		Register("subtractmagicresourcelevel", MutationMode.Subtract);
	}

	private static void Register(string name, MutationMode mode)
	{
		Register(name, mode, false, ProgVariableTypes.Text, "resource", "The name or id of the magic resource to alter");
		Register(name, mode, true, ProgVariableTypes.Number, "resourceId", "The id of the magic resource to alter");
	}

	private static void Register(string name, MutationMode mode, bool useId, ProgVariableTypes resourceType,
		string resourceName, string resourceHelp)
	{
		var verb = mode switch
		{
			MutationMode.Set => "Sets",
			MutationMode.Add => "Adds to",
			MutationMode.Subtract => "Subtracts from",
			_ => throw new ApplicationException("Unknown magic resource mutation mode.")
		};

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { ProgVariableTypes.MagicResourceHaver, resourceType, ProgVariableTypes.Number },
			(pars, gameworld) => new MagicResourceMutationFunction(pars, gameworld, mode, useId),
			new[] { "thing", resourceName, "amount" },
			new[]
			{
				"The character, item, or room whose magic resource level should be altered",
				resourceHelp,
				"The amount to set, add, or subtract"
			},
			$"{verb} the specified magic resource on a character, item, or room and returns the resulting clamped amount.",
			"Magic",
			ProgVariableTypes.Number));
	}
}

internal class MagicCharacterCollectionFunction : MagicBuiltInFunctionBase
{
	private enum CollectionMode
	{
		Capabilities,
		KnownSpells,
		CastableSpells,
		CastableSpellsNow
	}

	private readonly CollectionMode _mode;
	private readonly ProgVariableTypes _returnType;

	private MagicCharacterCollectionFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld,
		CollectionMode mode, ProgVariableTypes returnType) : base(parameterFunctions, gameworld)
	{
		_mode = mode;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType { get => _returnType; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (!TryGetCharacter(0, out var character))
		{
			return StatementResult.Error;
		}

		Result = _mode switch
		{
			CollectionMode.Capabilities => new CollectionVariable(character!.Capabilities.ToList(),
				ProgVariableTypes.MagicCapability),
			CollectionMode.KnownSpells => new CollectionVariable(
				character!.Gameworld.MagicSpells.Where(x => x.CharacterKnowsSpell(character)).ToList(),
				ProgVariableTypes.MagicSpell),
			CollectionMode.CastableSpells => new CollectionVariable(
				character!.Gameworld.MagicSpells
				          .Where(x => x.CharacterKnowsSpell(character) && x.ReadyForGame && x.Trigger is ICastMagicTrigger)
				          .ToList(),
				ProgVariableTypes.MagicSpell),
			CollectionMode.CastableSpellsNow => new CollectionVariable(
				character!.Gameworld.MagicSpells.Where(x => x.CharacterCanCast(character, character)).ToList(),
				ProgVariableTypes.MagicSpell),
			_ => throw new ApplicationException("Unknown magic character collection mode.")
		};
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("magiccapabilities", CollectionMode.Capabilities, ProgVariableTypes.MagicCapability | ProgVariableTypes.Collection,
			"Returns the magic capabilities currently applying to the character.");
		Register("knownspells", CollectionMode.KnownSpells, ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection,
			"Returns all spells whose known-spell prog says the character knows them.");
		Register("castablespells", CollectionMode.CastableSpells, ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection,
			"Returns all ready, known spells that can be invoked through a cast trigger.");
		Register("castablespellsnow", CollectionMode.CastableSpellsNow, ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection,
			"Returns all ready, known cast-trigger spells the character can cast right now against themself at any permitted power.");
	}

	private static void Register(string name, CollectionMode mode, ProgVariableTypes returnType, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { ProgVariableTypes.Character },
			(pars, gameworld) => new MagicCharacterCollectionFunction(pars, gameworld, mode, returnType),
			new[] { "character" },
			new[] { "The character whose magic state should be inspected" },
			help,
			"Magic",
			returnType));
	}
}

internal class CanCastSpellFunction : MagicBuiltInFunctionBase
{
	private readonly bool _now;
	private readonly bool _hasTarget;
	private readonly bool _hasPower;

	private CanCastSpellFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool now, bool hasTarget,
		bool hasPower) : base(parameterFunctions, gameworld)
	{
		_now = now;
		_hasTarget = hasTarget;
		_hasPower = hasPower;
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (!TryGetCharacter(0, out var character) || !TryGetSpell(1, out var spell))
		{
			return StatementResult.Error;
		}

		if (!_now)
		{
			Result = new BooleanVariable(spell!.CharacterKnowsSpell(character!) &&
			                             spell.ReadyForGame &&
			                             spell.Trigger is ICastMagicTrigger);
			return StatementResult.Normal;
		}

		IPerceivable target = character!;
		var powerIndex = 2;
		if (_hasTarget)
		{
			if (ParameterFunctions[2].Result?.GetObject is not IPerceivable perceivable)
			{
				ErrorMessage = "The spell target argument cannot be null.";
				return StatementResult.Error;
			}

			target = perceivable;
			powerIndex = 3;
		}

		if (_hasPower)
		{
			if (!TryGetSpellPower(powerIndex, out var power))
			{
				return StatementResult.Error;
			}

			Result = new BooleanVariable(spell!.CharacterCanCast(character!, target, power));
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(spell!.CharacterCanCast(character!, target));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"cancastspell",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell },
			(pars, gameworld) => new CanCastSpellFunction(pars, gameworld, false, false, false),
			new[] { "character", "spell" },
			new[] { "The character to check", "The spell to check" },
			"Returns true if the character knows the spell, the spell is ready, and it uses a cast trigger.",
			"Magic",
			ProgVariableTypes.Boolean));

		RegisterNow(new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell },
			new[] { "character", "spell" },
			new[] { "The character to check", "The spell to check" },
			false,
			false,
			"Returns true if the character can currently cast the spell against themself at any permitted power.");
		RegisterNow(new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Perceivable },
			new[] { "character", "spell", "target" },
			new[] { "The character to check", "The spell to check", "The target to use for target-aware cost and readiness checks" },
			true,
			false,
			"Returns true if the character can currently cast the spell against the target at any permitted power.");
		RegisterNow(new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Text },
			new[] { "character", "spell", "power" },
			new[] { "The character to check", "The spell to check", "The spell power name to test" },
			false,
			true,
			"Returns true if the character can currently cast the spell against themself at the specified power.");
		RegisterNow(new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			new[] { "character", "spell", "target", "power" },
			new[] { "The character to check", "The spell to check", "The target to use for target-aware cost and readiness checks", "The spell power name to test" },
			true,
			true,
			"Returns true if the character can currently cast the spell against the target at the specified power.");
	}

	private static void RegisterNow(IEnumerable<ProgVariableTypes> parameters, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, bool hasTarget, bool hasPower, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"cancastspellnow",
			parameters,
			(pars, gameworld) => new CanCastSpellFunction(pars, gameworld, true, hasTarget, hasPower),
			parameterNames,
			parameterHelp,
			help,
			"Magic",
			ProgVariableTypes.Boolean));
	}
}

internal class ActiveSpellFunction : MagicBuiltInFunctionBase
{
	private enum ActiveSpellMode
	{
		Spells,
		Effects
	}

	private readonly ActiveSpellMode _mode;
	private readonly bool _hasSpell;
	private readonly ProgVariableTypes _returnType;

	private ActiveSpellFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, ActiveSpellMode mode,
		bool hasSpell, ProgVariableTypes returnType) : base(parameterFunctions, gameworld)
	{
		_mode = mode;
		_hasSpell = hasSpell;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType { get => _returnType; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (!TryGetCharacter(0, out var character))
		{
			return StatementResult.Error;
		}

		IMagicSpell? spell = null;
		if (_hasSpell && !TryGetSpell(1, out spell))
		{
			return StatementResult.Error;
		}

		var parents = ActiveSpellParents(character!, spell).ToList();
		Result = _mode switch
		{
			ActiveSpellMode.Spells => new CollectionVariable(parents.Select(x => x.Spell).Distinct().ToList(),
				ProgVariableTypes.MagicSpell),
			ActiveSpellMode.Effects => new CollectionVariable(parents.Cast<IEffect>().ToList(), ProgVariableTypes.Effect),
			_ => throw new ApplicationException("Unknown active spell mode.")
		};
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"activespells",
			new[] { ProgVariableTypes.Character },
			(pars, gameworld) => new ActiveSpellFunction(pars, gameworld, ActiveSpellMode.Spells, false,
				ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection),
			new[] { "character" },
			new[] { "The character whose active spell effects should be inspected" },
			"Returns the distinct spells currently represented by active spell-parent effects on the character.",
			"Magic",
			ProgVariableTypes.MagicSpell | ProgVariableTypes.Collection));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"activespelleffects",
			new[] { ProgVariableTypes.Character },
			(pars, gameworld) => new ActiveSpellFunction(pars, gameworld, ActiveSpellMode.Effects, false,
				ProgVariableTypes.Effect | ProgVariableTypes.Collection),
			new[] { "character" },
			new[] { "The character whose active spell effects should be inspected" },
			"Returns the active spell-parent effects on the character.",
			"Magic",
			ProgVariableTypes.Effect | ProgVariableTypes.Collection));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"activespelleffects",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell },
			(pars, gameworld) => new ActiveSpellFunction(pars, gameworld, ActiveSpellMode.Effects, true,
				ProgVariableTypes.Effect | ProgVariableTypes.Collection),
			new[] { "character", "spell" },
			new[] { "The character whose active spell effects should be inspected", "The spell to match" },
			"Returns the active spell-parent effects on the character that belong to the specified spell.",
			"Magic",
			ProgVariableTypes.Effect | ProgVariableTypes.Collection));
	}
}

internal class SpellDurationFunction : MagicBuiltInFunctionBase
{
	private enum DurationMode
	{
		Get,
		Set,
		Add,
		Subtract,
		Remove
	}

	private readonly DurationMode _mode;

	private SpellDurationFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, DurationMode mode) :
		base(parameterFunctions, gameworld)
	{
		_mode = mode;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _mode == DurationMode.Get ? ProgVariableTypes.TimeSpan : ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (!TryGetCharacter(0, out var character) || !TryGetSpell(1, out var spell))
		{
			return StatementResult.Error;
		}

		var parents = ActiveSpellParents(character!, spell).ToList();
		if (_mode == DurationMode.Get)
		{
			Result = new TimeSpanVariable(parents.Select(character!.ScheduledDuration).DefaultIfEmpty(TimeSpan.Zero).Max());
			return StatementResult.Normal;
		}

		if (_mode == DurationMode.Remove)
		{
			foreach (var parent in parents)
			{
				character!.RemoveEffect(parent, true);
			}

			Result = new NumberVariable(parents.Count);
			return StatementResult.Normal;
		}

		var duration = (TimeSpan)(ParameterFunctions[2].Result?.GetObject ?? TimeSpan.Zero);
		var count = 0;
		foreach (var parent in parents)
		{
			switch (_mode)
			{
				case DurationMode.Set:
					if (duration <= TimeSpan.Zero)
					{
						character!.RemoveEffect(parent, true);
					}
					else
					{
						character!.Reschedule(parent, duration);
					}
					count++;
					break;
				case DurationMode.Add:
					if (duration < TimeSpan.Zero)
					{
						character!.RemoveDuration(parent, duration.Duration(), true);
						count++;
						break;
					}

					if (character!.ScheduledDuration(parent) > TimeSpan.Zero)
					{
						character.AddDuration(parent, duration);
						count++;
					}
					break;
				case DurationMode.Subtract:
					if (duration < TimeSpan.Zero)
					{
						character!.AddDuration(parent, duration.Duration());
						count++;
						break;
					}

					if (character!.ScheduledDuration(parent) > TimeSpan.Zero)
					{
						character.RemoveDuration(parent, duration, true);
						count++;
					}
					break;
			}
		}

		Result = new NumberVariable(count);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterGet("spellremainingduration");
		RegisterGet("spellduration");
		RegisterMutation("setspellduration", DurationMode.Set, "Sets the remaining duration of active effects from the specified spell and returns the number of spell-parent effects altered.");
		RegisterMutation("addspellduration", DurationMode.Add, "Adds to the remaining duration of scheduled active effects from the specified spell and returns the number altered.");
		RegisterMutation("subtractspellduration", DurationMode.Subtract, "Subtracts from the remaining duration of scheduled active effects from the specified spell and returns the number altered. Effects reduced below zero are removed.");
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"removespell",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell },
			(pars, gameworld) => new SpellDurationFunction(pars, gameworld, DurationMode.Remove),
			new[] { "character", "spell" },
			new[] { "The character whose active spell effects should be removed", "The spell to remove" },
			"Removes active spell-parent effects from the character for the specified spell and returns the number removed.",
			"Magic",
			ProgVariableTypes.Number));
	}

	private static void RegisterGet(string name)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell },
			(pars, gameworld) => new SpellDurationFunction(pars, gameworld, DurationMode.Get),
			new[] { "character", "spell" },
			new[] { "The character whose active spell effects should be inspected", "The spell to inspect" },
			"Returns the longest remaining scheduled duration of active spell-parent effects from the specified spell, or zero if none are scheduled.",
			"Magic",
			ProgVariableTypes.TimeSpan));
	}

	private static void RegisterMutation(string name, DurationMode mode, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.MagicSpell, ProgVariableTypes.TimeSpan },
			(pars, gameworld) => new SpellDurationFunction(pars, gameworld, mode),
			new[] { "character", "spell", "duration" },
			new[]
			{
				"The character whose active spell effects should be altered",
				"The spell to alter",
				"The duration to set, add, or subtract"
			},
			help,
			"Magic",
			ProgVariableTypes.Number));
	}
}
