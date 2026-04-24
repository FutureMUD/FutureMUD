using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Planes;

internal abstract class PlanarFunctionBase : BuiltInFunction
{
	protected PlanarFunctionBase(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	protected IFuturemud Gameworld { get; }
}

internal class PlaneOfFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"planeof",
			new[] { ProgVariableTypes.Perceivable },
			(pars, gameworld) => new PlaneOfFunction(pars, gameworld),
			new[] { "perceivable" },
			new[] { "The perceivable whose primary plane should be returned" },
			"Returns the name of the first plane on which a perceivable is present.",
			"Planes",
			ProgVariableTypes.Text));
	}

	private PlaneOfFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceivable = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var planeId = perceivable?.GetPlanarPresence().PresencePlaneIds.FirstOrDefault() ?? 0;
		Result = new TextVariable(Gameworld.Planes.Get(planeId)?.Name ?? string.Empty);
		return StatementResult.Normal;
	}
}

internal class PlanesOfFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"planesof",
			new[] { ProgVariableTypes.Perceivable },
			(pars, gameworld) => new PlanesOfFunction(pars, gameworld),
			new[] { "perceivable" },
			new[] { "The perceivable whose planes should be returned" },
			"Returns a text collection of all planes on which a perceivable is present.",
			"Planes",
			ProgVariableTypes.Collection | ProgVariableTypes.Text));
	}

	private PlanesOfFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Collection | ProgVariableTypes.Text; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceivable = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var planes = perceivable?.GetPlanarPresence()
			.PresencePlaneIds
			.Select(x => Gameworld.Planes.Get(x)?.Name ?? $"Plane #{x:N0}")
			.ToList() ?? new List<string>();
		Result = new CollectionVariable(planes, ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal class CanPerceivePlanarFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"canperceiveplanar",
			new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.Perceivable },
			(pars, gameworld) => new CanPerceivePlanarFunction(pars, gameworld),
			new[] { "perceiver", "target" },
			new[] { "The perceiver doing the looking", "The target being perceived" },
			"Returns true if the perceiver can perceive the target under planar rules.",
			"Planes",
			ProgVariableTypes.Boolean));
	}

	private CanPerceivePlanarFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceiver = ParameterFunctions[0].Result?.GetObject as IPerceiver;
		var target = ParameterFunctions[1].Result?.GetObject as IPerceivable;
		Result = new BooleanVariable(perceiver?.CanPerceivePlanar(target) == true);
		return StatementResult.Normal;
	}
}

internal class CanInteractPlanarFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"caninteractplanar",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, gameworld) => new CanInteractPlanarFunction(pars, gameworld),
			new[] { "actor", "target", "kind" },
			new[] { "The actor", "The target", "The interaction kind" },
			"Returns true if the actor can affect the target under planar rules.",
			"Planes",
			ProgVariableTypes.Boolean));
	}

	private CanInteractPlanarFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var actor = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var target = ParameterFunctions[1].Result?.GetObject as IPerceivable;
		var kindText = ParameterFunctions[2].Result?.GetObject as string;
		Result = new BooleanVariable(
			actor is not null &&
			target is not null &&
			Enum.TryParse(kindText, true, out PlanarInteractionKind kind) &&
			actor.CanInteractPlanar(target, kind));
		return StatementResult.Normal;
	}
}

internal class ApplyPlanarStateFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"applyplanarstate",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, gameworld) => new ApplyPlanarStateFunction(pars, gameworld),
			new[] { "target", "state" },
			new[] { "The target", "corporeal or noncorporeal" },
			"Applies a saved planar state to a perceivable on the default plane.",
			"Planes",
			ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"applyplanarstate",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new ApplyPlanarStateFunction(pars, gameworld),
			new[] { "target", "state", "plane" },
			new[] { "The target", "corporeal or noncorporeal", "The plane name or id" },
			"Applies a saved planar state to a perceivable on a named plane.",
			"Planes",
			ProgVariableTypes.Boolean));
	}

	private ApplyPlanarStateFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var state = ParameterFunctions[1].Result?.GetObject as string;
		var planeText = ParameterFunctions.Count > 2 ? ParameterFunctions[2].Result?.GetObject as string : string.Empty;
		var plane = string.IsNullOrWhiteSpace(planeText) ? Gameworld.DefaultPlane : Gameworld.Planes.GetByIdOrName(planeText);
		if (target is null || plane is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var definition = state?.ToLowerInvariant() switch
		{
			"corporeal" or "manifest" or "manifested" => PlanarPresenceDefinition.Manifested(plane),
			"noncorporeal" or "incorporeal" or "dissipated" => PlanarPresenceDefinition.NonCorporeal(plane),
			_ => null
		};

		if (definition is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.RemoveAllEffects<PlanarStateEffect>(x => true, true);
		target.AddEffect(new PlanarStateEffect(target, definition, 100, true));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class ClearPlanarStateFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"clearplanarstate",
			new[] { ProgVariableTypes.Perceivable },
			(pars, gameworld) => new ClearPlanarStateFunction(pars, gameworld),
			new[] { "target" },
			new[] { "The target whose planar overlays should be cleared" },
			"Clears saved planar state effects from a perceivable.",
			"Planes",
			ProgVariableTypes.Boolean));
	}

	private ClearPlanarStateFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		target?.RemoveAllEffects<PlanarStateEffect>(x => true, true);
		Result = new BooleanVariable(target is not null);
		return StatementResult.Normal;
	}
}

internal class SetPlaneFunction : PlanarFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setplane",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, gameworld) => new SetPlaneFunction(pars, gameworld),
			new[] { "target", "plane" },
			new[] { "The target", "The plane name or id" },
			"Sets a perceivable to ordinary corporeality on a named plane.",
			"Planes",
			ProgVariableTypes.Boolean));
	}

	private SetPlaneFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions, gameworld)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var planeText = ParameterFunctions[1].Result?.GetObject as string;
		var plane = Gameworld.Planes.GetByIdOrName(planeText);
		if (target is null || plane is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.RemoveAllEffects<PlanarStateEffect>(x => true, true);
		target.AddEffect(new PlanarStateEffect(target, PlanarPresenceDefinition.DefaultMaterial(plane.Id), 100, true));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
