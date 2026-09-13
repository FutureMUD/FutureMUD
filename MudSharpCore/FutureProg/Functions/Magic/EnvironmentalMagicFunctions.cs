#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic.Environment;

namespace MudSharp.FutureProg.Functions.Magic;

internal abstract class EnvironmentalMagicFunctionBase(IList<IFunction> parameterFunctions, IFuturemud gameworld)
	: MagicBuiltInFunctionBase(parameterFunctions, gameworld)
{
	protected bool TryGetEnvironment(out ICell cell, out IEnvironmentalMagicService service)
	{
		cell = (ParameterFunctions[0].Result?.GetObject as ICell)!;
		service = Gameworld.EnvironmentalMagic!;
		if (cell == null)
		{
			ErrorMessage = "The environmental location argument cannot be null.";
			return false;
		}

		if (service == null)
		{
			ErrorMessage = "The environmental magic coordinator is unavailable.";
			return false;
		}

		return true;
	}

	protected StatementResult ReturnNumber(double value)
	{
		try
		{
			Result = new NumberVariable(value);
			return StatementResult.Normal;
		}
		catch (OverflowException)
		{
			ErrorMessage = "The environmental value cannot be represented as a FutureProg number.";
			return StatementResult.Error;
		}
	}
}

internal sealed class EnvironmentalMagicResourceFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld,
	bool rate, bool resourceId) : EnvironmentalMagicFunctionBase(parameterFunctions, gameworld)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetEnvironment(out var cell, out var service))
		{
			return StatementResult.Error;
		}

		if (resourceId && ParameterFunctions[1].Result?.GetObject is decimal id &&
		    (id != decimal.Truncate(id) || id <= 0 || id > long.MaxValue))
		{
			ErrorMessage = "The resource ID must be a positive 64-bit integer.";
			return StatementResult.Error;
		}

		if (!TryGetMagicResource(1, resourceId, out var resource))
		{
			return StatementResult.Error;
		}

		if (!service.TryInspectResource(cell, resource!, out var output))
		{
			ErrorMessage = "That resource has no environmental output bound to the location.";
			return StatementResult.Error;
		}

		if (!output.IsValid)
		{
			ErrorMessage = output.Error ?? "The environmental resource calculation is invalid.";
			return StatementResult.Error;
		}

		return ReturnNumber(rate ? output.Rate : output.Maximum);
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var rate in new[] { false, true })
		{
			foreach (var byId in new[] { false, true })
			{
				FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
					rate ? "environmentrate" : "environmentcap",
					new[] { ProgVariableTypes.Location, byId ? ProgVariableTypes.Number : ProgVariableTypes.Text },
					(pars, world) => new EnvironmentalMagicResourceFunction(pars, world, rate, byId),
					new[] { "location", byId ? "resourceId" : "resource" },
					new[] { "The physical cell to inspect", "The existing magic resource's name or ID" },
					$"Returns the current environmental {(rate ? "regeneration rate per real minute" : "maximum")} for a bound location/resource pair without advancing production, saving or changing scheduling. An unbound output or invalid calculation is a runtime error. Use an uncached policy prog for state-dependent calls.",
					"Magic", ProgVariableTypes.Number));
			}
		}
	}
}

internal sealed class EnvironmentalMagicStateFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld,
	EnvironmentalMagicStateFunction.Query query) : EnvironmentalMagicFunctionBase(parameterFunctions, gameworld)
{
	internal enum Query
	{
		ScarDamage,
		Pressure,
		LastDefile,
		HasDefile
	}

	public override ProgVariableTypes ReturnType
	{
		get => query switch
		{
			Query.LastDefile => ProgVariableTypes.DateTime,
			Query.HasDefile => ProgVariableTypes.Boolean,
			_ => ProgVariableTypes.Number
		};
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetEnvironment(out var cell, out var service))
		{
			return StatementResult.Error;
		}

		var snapshot = service.InspectState(cell);
		switch (query)
		{
			case Query.ScarDamage:
				return ReturnNumber(snapshot.State.ScarDamage);
			case Query.Pressure:
				return ReturnNumber(snapshot.Pressure);
			case Query.LastDefile:
				Result = snapshot.State.LastDefileUtc.HasValue
					? new DateTimeVariable(snapshot.State.LastDefileUtc.Value.UtcDateTime)
					: new NullVariable(ProgVariableTypes.DateTime);
				return StatementResult.Normal;
			case Query.HasDefile:
				Result = new BooleanVariable(snapshot.State.LastDefileUtc.HasValue);
				return StatementResult.Normal;
			default:
				throw new ArgumentOutOfRangeException(nameof(query));
		}
	}

	public static void RegisterFunctionCompiler()
	{
		Register("environmentscardamage", Query.ScarDamage, ProgVariableTypes.Number,
			"Returns persistent remaining scar damage without modifying the cell or evaluating resource input progs. A cell with no damage returns zero.");
		Register("environmentpressure", Query.Pressure, ProgVariableTypes.Number,
			"Returns currently decayed, severity-weighted recent destructive-use pressure without saving its projection or evaluating resource input progs.");
		Register("environmentlastdefile", Query.LastDefile, ProgVariableTypes.DateTime,
			"Returns the last destructive-use time in UTC without evaluating resource input progs. With no event, returns FutureProg's default datetime (year 1); use environmenthasdefile to distinguish absence. Repair does not rewrite this timestamp.");
		Register("environmenthasdefile", Query.HasDefile, ProgVariableTypes.Boolean,
			"Returns whether a destructive-use timestamp exists for the physical cell, without modifying state or evaluating resource input progs.");
	}

	private static void Register(string name, Query query, ProgVariableTypes returnType, string help)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
			new[] { ProgVariableTypes.Location },
			(pars, world) => new EnvironmentalMagicStateFunction(pars, world, query),
			new[] { "location" }, new[] { "The physical cell whose persistent environmental state is inspected" },
			help, "Magic", returnType));
	}
}

internal sealed class InvalidateEnvironmentFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld)
	: EnvironmentalMagicFunctionBase(parameterFunctions, gameworld)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error || !TryGetEnvironment(out var cell, out var service))
		{
			return StatementResult.Error;
		}

		service.MarkDirty(cell, EnvironmentalMagicDirtyReason.Policy);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("invalidateenvironment",
			new[] { ProgVariableTypes.Location },
			(pars, world) => new InvalidateEnvironmentFunction(pars, world),
			new[] { "location" }, new[] { "The physical cell whose external policy inputs changed" },
			"Trusted authoring helper that requests a coalesced environmental policy recheck and returns true when the notification is accepted. It never advances production, grants resources or repairs damage. Use it after changing a dependency that the native source notifications cannot observe; do not call it from a pure environmental input prog.",
			"Magic", ProgVariableTypes.Boolean));
	}
}
