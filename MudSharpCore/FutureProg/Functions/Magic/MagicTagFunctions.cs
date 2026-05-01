#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Magic;

internal abstract class MagicTagFunctionBase : BuiltInFunction
{
	protected MagicTagFunctionBase(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	protected static IEnumerable<IMagicTagEffect> TagsFor(IPerceivable? perceivable)
	{
		return perceivable?.EffectsOfType<IMagicTagEffect>(x => x.Applies()) ?? [];
	}
}

internal class HasMagicTagFunction : MagicTagFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"hasmagictag",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, _) => new HasMagicTagFunction(pars),
			new[] { "perceivable", "tag" },
			new[] { "The perceivable to inspect", "The magic tag key" },
			"Returns true if the perceivable has an active magic tag with the specified key.",
			"Magic",
			ProgVariableTypes.Boolean));
	}

	private HasMagicTagFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceivable = ParameterFunctions[0].Result?.GetObject as IPerceivable;
		var tag = ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty;
		Result = new BooleanVariable(TagsFor(perceivable).Any(x => x.Tag.EqualTo(tag)));
		return StatementResult.Normal;
	}
}

internal class MagicTagValueFunction : MagicTagFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"magictagvalue",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, _) => new MagicTagValueFunction(pars),
			new[] { "perceivable", "tag" },
			new[] { "The perceivable to inspect", "The magic tag key" },
			"Returns the first active magic tag value for the specified key, or empty text.",
			"Magic",
			ProgVariableTypes.Text));
	}

	private MagicTagValueFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		var tag = ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty;
		Result = new TextVariable(TagsFor(perceivable).FirstOrDefault(x => x.Tag.EqualTo(tag))?.Value ?? string.Empty);
		return StatementResult.Normal;
	}
}

internal class MagicTagValuesFunction : MagicTagFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"magictagvalues",
			new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
			(pars, _) => new MagicTagValuesFunction(pars),
			new[] { "perceivable", "tag" },
			new[] { "The perceivable to inspect", "The magic tag key" },
			"Returns all active magic tag values for the specified key.",
			"Magic",
			ProgVariableTypes.Collection | ProgVariableTypes.Text));
	}

	private MagicTagValuesFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		var tag = ParameterFunctions[1].Result?.GetObject?.ToString() ?? string.Empty;
		Result = new CollectionVariable(TagsFor(perceivable).Where(x => x.Tag.EqualTo(tag)).Select(x => x.Value).ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}

internal class MagicTagsFunction : MagicTagFunctionBase
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"magictags",
			new[] { ProgVariableTypes.Perceivable },
			(pars, _) => new MagicTagsFunction(pars),
			new[] { "perceivable" },
			new[] { "The perceivable to inspect" },
			"Returns all active magic tag keys on the perceivable.",
			"Magic",
			ProgVariableTypes.Collection | ProgVariableTypes.Text));
	}

	private MagicTagsFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		Result = new CollectionVariable(TagsFor(perceivable).Select(x => x.Tag).Distinct().ToList(),
			ProgVariableTypes.Text);
		return StatementResult.Normal;
	}
}
