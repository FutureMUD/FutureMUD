#nullable enable

using System.Collections;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Computers;
using MudSharp.Traps;

namespace MudSharp.FutureProg.Functions.Traps;

/// <summary>FutureProg creation, lookup, and explicit control of runtime trap effects.</summary>
internal sealed class TrapAtFunction : BuiltInFunction
{
	private TrapAtFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Trap;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var anchor = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var trap = anchor?.EffectsOfType<TrapEffect>().FirstOrDefault();
		Result = trap is null ? new NullVariable(ProgVariableTypes.Trap) : new TrapVariable(trap);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"trapat",
			[ProgVariableTypes.Perceivable],
			(pars, _) => new TrapAtFunction(pars),
			["anchor"],
			["The item or location to inspect for a trap."],
			"Returns the first trap anchored to an item or location, or null if there is none.",
			"Traps",
			ProgVariableTypes.Trap));
	}
}

internal sealed class CreateTrapFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private CreateTrapFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Trap;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var anchor = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var creator = ParameterFunctions[2].Result?.GetObject as ICharacter;
		var suppliedItems = ParameterFunctions.Count > 3
			? ExtractSuppliedItems(ParameterFunctions[3].Result?.GetObject)
			: [];
		var template = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? _gameworld.TrapTemplates.Get(Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0L))
			: _gameworld.TrapTemplates.GetByIdOrName(
				ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty);
		if (anchor is null || template is null || template.Status != RevisionStatus.Current || !template.CanSubmit() ||
		    !TrapEffect.IsValidAnchor(template, anchor) ||
		    anchor.EffectsOfType<TrapEffect>().Any(x => x.State is not TrapState.Spent and not TrapState.Expired))
		{
			Result = new NullVariable(ProgVariableTypes.Trap);
			return StatementResult.Normal;
		}
		var componentItems = suppliedItems.Append(anchor as IGameItem).Where(x => x is not null).Cast<IGameItem>()
			.Distinct().ToList();
		var anchorCell = anchor as MudSharp.Construction.ICell ?? anchor.Location;
		var bindings = new List<TrapComponentBinding>();
		if (template.SourceKind == TrapSourceKind.Mechanical &&
		    (componentItems.Any(x => x.Id <= 0 || x.Deleted || x.InInventoryOf is not null ||
		                              !ReferenceEquals(x.Location, anchorCell) ||
		                              x.EffectsOfType<TrapComponentReservationEffect>().Any()) ||
		     !TrapEffect.TryBindComponents(template, componentItems, out bindings) ||
		     template.Triggers.Any(x => x.TriggerType == TrapTriggerType.Signal) &&
		     !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Trigger) &&
		                        x.Item?.GetItemTypes<ISignalSourceComponent>().Any() == true) ||
		     template.Payloads.Any(x => x.PayloadType == TrapPayloadType.DetonateItem) &&
		     !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Payload) && x.Item?.GetItemType<IDetonatable>() is not null) ||
		     template.Payloads.Any(x => x.PayloadType == TrapPayloadType.EmitSignal &&
		                                (!x.Parameters.TryGetValue("targetitem", out var targetText) || targetText == "0")) &&
		     !bindings.Any(x => x.Role.HasFlag(TrapComponentRole.Payload) && x.Item?.Components.OfType<ISignalSink>().Any() == true)))
		{
			Result = new NullVariable(ProgVariableTypes.Trap);
			return StatementResult.Normal;
		}
		var trap = new TrapEffect(anchor, template, creator, components: bindings);
		if (TrapEffect.HasTimedLifetime(template))
		{
			anchor.AddEffect(trap, template.Lifespan!.Value);
		}
		else
		{
			anchor.AddEffect(trap);
		}

		Result = new TrapVariable(trap);
		return StatementResult.Normal;
	}

	internal static IReadOnlyList<IGameItem> ExtractSuppliedItems(object? value)
	{
		if (value is not IEnumerable enumerable)
		{
			return [];
		}

		return enumerable
			.Cast<object>()
			.Select(x => x as IGameItem ?? (x as IProgVariable)?.GetObject as IGameItem)
			.Where(x => x is not null)
			.Cast<IGameItem>()
			.ToList();
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"createtrap",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Number, ProgVariableTypes.Character],
			(pars, gameworld) => new CreateTrapFunction(pars, gameworld),
			["anchor", "template", "creator"],
			["The item or location on which to deploy the trap.", "The current trap template ID.", "The attributed creator, or a natural NPC."],
			"Deploys a current, valid trap template and returns the new trap, or null if deployment was invalid.",
			"Traps",
			ProgVariableTypes.Trap));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"createtrap",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Number, ProgVariableTypes.Character, ProgVariableTypes.Item | ProgVariableTypes.Collection],
			(pars, gameworld) => new CreateTrapFunction(pars, gameworld),
			["anchor", "template", "creator", "components"],
			["The item or location on which to deploy the trap.", "The current trap template ID.", "The attributed creator.", "Physical items satisfying a mechanical template's tagged requirements."],
			"Deploys a trap with physical components. Mechanical components must be colocated, uninstalled, persistent items; magical and natural traps ignore the collection.",
			"Traps",
			ProgVariableTypes.Trap));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"createtrap",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Character],
			(pars, gameworld) => new CreateTrapFunction(pars, gameworld),
			["anchor", "template", "creator"],
			["The item or location on which to deploy the trap.", "The current trap template name or ID.", "The attributed creator, or a natural NPC."],
			"Deploys a current, valid trap template and returns the new trap, or null if deployment was invalid.",
			"Traps",
			ProgVariableTypes.Trap));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"createtrap",
			[ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Character, ProgVariableTypes.Item | ProgVariableTypes.Collection],
			(pars, gameworld) => new CreateTrapFunction(pars, gameworld),
			["anchor", "template", "creator", "components"],
			["The item or location on which to deploy the trap.", "The current trap template name or ID.", "The attributed creator.", "Physical items satisfying a mechanical template's tagged requirements."],
			"Deploys a trap with physical components. Mechanical components must be colocated, uninstalled, persistent items; magical and natural traps ignore the collection.",
			"Traps",
			ProgVariableTypes.Trap));
	}
}

internal sealed class TrapControlFunction : BuiltInFunction
{
	private readonly TrapControl _control;

	private TrapControlFunction(IList<IFunction> parameterFunctions, TrapControl control) : base(parameterFunctions)
	{
		_control = control;
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

		var trap = ParameterFunctions[0].Result?.GetObject as TrapEffect;
		var actor = ParameterFunctions.Count > 1
			? ParameterFunctions[1].Result?.GetObject as ICharacter
			: null;
		var result = trap is not null && _control switch
		{
			TrapControl.Arm => trap.Arm(),
			TrapControl.Disarm => trap.Disarm(),
			TrapControl.Trigger => trap.ForceTrigger(actor),
			_ => false
		};
		Result = new BooleanVariable(result);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("armtrap", TrapControl.Arm, [ProgVariableTypes.Trap]);
		Register("disarmtrap", TrapControl.Disarm, [ProgVariableTypes.Trap]);
		Register("triggertrap", TrapControl.Trigger, [ProgVariableTypes.Trap]);
		Register("triggertrap", TrapControl.Trigger, [ProgVariableTypes.Trap, ProgVariableTypes.Character]);
	}

	private static void Register(string name, TrapControl control, ProgVariableTypes[] parameterTypes)
	{
		var hasTriggerer = control == TrapControl.Trigger && parameterTypes.Length > 1;
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameterTypes,
			(pars, _) => new TrapControlFunction(pars, control),
			hasTriggerer ? ["trap", "triggerer"] : ["trap"],
			hasTriggerer
				? ["The trap to activate.", "The character to use as the target of the forced activation."]
				: ["The trap to control."],
			$"{control.DescribeEnum()}s a trap and returns whether the state change or activation succeeded.",
			"Traps",
			ProgVariableTypes.Boolean));
	}

	private enum TrapControl
	{
		Arm,
		Disarm,
		Trigger
	}
}
