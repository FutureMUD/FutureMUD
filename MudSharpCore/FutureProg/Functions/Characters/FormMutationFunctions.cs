#nullable enable
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Characters;

internal static class CharacterFormMutationFunctionHelper
{
	public static MudSharp.Character.Character? ResolveConcreteCharacter(IProgVariable? characterVariable)
	{
		return characterVariable?.GetObject as MudSharp.Character.Character;
	}

	public static IFutureProg? ResolveProg(IFuturemud gameworld, IProgVariable? progIdentifier,
		ProgVariableTypes returnType, params ProgVariableTypes[][] parameterSets)
	{
		var value = progIdentifier?.GetObject;
		IFutureProg? prog = value switch
		{
			decimal decimalValue => gameworld.FutureProgs.Get(Convert.ToInt64(decimalValue)),
			string text when long.TryParse(text, out var idValue) => gameworld.FutureProgs.Get(idValue),
			_ => gameworld.FutureProgs.GetByName(value?.ToString() ?? string.Empty)
		};

		if (prog is null || prog.ReturnType != returnType)
		{
			return null;
		}

		if (parameterSets.Any() && !parameterSets.Any(x => prog.MatchesParameters(x)))
		{
			return null;
		}

		return prog;
	}

	public static bool TryResolveTraumaMode(IProgVariable? variable, out BodySwitchTraumaMode traumaMode)
	{
		switch (variable?.GetObject)
		{
			case decimal decimalValue:
				var intValue = Convert.ToInt32(decimalValue);
				if (Enum.IsDefined(typeof(BodySwitchTraumaMode), intValue))
				{
					traumaMode = (BodySwitchTraumaMode)intValue;
					return true;
				}

				break;
			default:
				switch ((variable?.GetObject?.ToString() ?? string.Empty).ToLowerInvariant())
				{
					case "auto":
					case "automatic":
						traumaMode = BodySwitchTraumaMode.Automatic;
						return true;
					case "transfer":
						traumaMode = BodySwitchTraumaMode.Transfer;
						return true;
					case "stash":
					case "stasis":
						traumaMode = BodySwitchTraumaMode.Stash;
						return true;
				}

				break;
		}

		traumaMode = BodySwitchTraumaMode.Automatic;
		return false;
	}

	public static IEntityDescriptionPattern? ResolveDescriptionPattern(IFuturemud gameworld, IProgVariable? identifier,
		EntityDescriptionType type)
	{
		var value = identifier?.GetObject;
		IEntityDescriptionPattern? pattern = value switch
		{
			decimal decimalValue => gameworld.EntityDescriptionPatterns.Get(Convert.ToInt64(decimalValue)),
			string text when long.TryParse(text, out var idValue) => gameworld.EntityDescriptionPatterns.Get(idValue),
			_ => gameworld.EntityDescriptionPatterns.GetByIdOrName(value?.ToString() ?? string.Empty)
		};

		return pattern?.Type == type ? pattern : null;
	}
}

internal class CanSeeFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"CanSeeForm".ToLowerInvariant(),
			[ProgVariableTypes.Character, type],
			(pars, _) => new CanSeeFormFunction(pars),
			["character", "form"],
			["The character whose forms you want to inspect", "The form alias or body id"],
			"Returns true if the specified form is currently visible to the owning character.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private CanSeeFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(form?.CanSee(character!) == true);
		return StatementResult.Normal;
	}
}

internal class EnsureFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"EnsureForm".ToLowerInvariant(),
			[ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Race],
			(pars, _) => new EnsureFormFunction(pars),
			["character", "sourceKey", "race"],
			["The character who should own the form", "The stable key used to reuse the form", "The race for the form"],
			"Ensures a keyed form exists for the character and returns its body id.",
			"Characters",
			ProgVariableTypes.Number
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"EnsureForm".ToLowerInvariant(),
			[ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Race, ProgVariableTypes.Ethnicity],
			(pars, _) => new EnsureFormFunction(pars),
			["character", "sourceKey", "race", "ethnicity"],
			["The character who should own the form", "The stable key used to reuse the form", "The race for the form", "The ethnicity for the form"],
			"Ensures a keyed form exists for the character and returns its body id.",
			"Characters",
			ProgVariableTypes.Number
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"EnsureForm".ToLowerInvariant(),
			[ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Race, ProgVariableTypes.Ethnicity, ProgVariableTypes.Gender],
			(pars, _) => new EnsureFormFunction(pars),
			["character", "sourceKey", "race", "ethnicity", "gender"],
			["The character who should own the form", "The stable key used to reuse the form", "The race for the form", "The ethnicity for the form", "The gender for the form"],
			"Ensures a keyed form exists for the character and returns its body id.",
			"Characters",
			ProgVariableTypes.Number
		));
	}

	private EnsureFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Number;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var sourceKey = ParameterFunctions[1].Result?.GetObject?.ToString();
		var race = ParameterFunctions[2].Result?.GetObject as IRace;
		var ethnicity = ParameterFunctions.Count > 3 ? ParameterFunctions[3].Result?.GetObject as IEthnicity : null;
		var gender = ParameterFunctions.Count > 4 ? ParameterFunctions[4].Result?.GetObject as Gender? : null;
		if (character is null || string.IsNullOrWhiteSpace(sourceKey) || race is null)
		{
			Result = new NumberVariable(0);
			return StatementResult.Normal;
		}

		var specification = new CharacterFormSpecification
		{
			Race = race,
			Ethnicity = ethnicity,
			Gender = gender
		};
		Result = new NumberVariable(character.EnsureForm(specification,
			new CharacterFormSource(CharacterFormSourceType.Prog, 0, sourceKey), out var form, out _)
			? form.Body.Id
			: 0);
		return StatementResult.Normal;
	}
}

internal class SetFormAliasFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormAlias".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Text],
			(pars, _) => new SetFormAliasFunction(pars),
			["character", "form", "alias"],
			["The character who owns the form", "The form alias or body id", "The new alias"],
			"Sets a form alias and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormAliasFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var alias = ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty;
		Result = new BooleanVariable(character?.TrySetFormAlias(form!, alias, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormSortOrderFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormSortOrder".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Number],
			(pars, _) => new SetFormSortOrderFunction(pars),
			["character", "form", "sortOrder"],
			["The character who owns the form", "The form alias or body id", "The new sort order"],
			"Sets a form sort order and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormSortOrderFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var sortOrder = ParameterFunctions[2].Result?.GetObject is decimal decimalValue ? Convert.ToInt32(decimalValue) : 0;
		Result = new BooleanVariable(character?.TrySetFormSortOrder(form!, sortOrder, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormTraumaModeFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Number);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType, ProgVariableTypes traumaType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormTraumaMode".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, traumaType],
			(pars, _) => new SetFormTraumaModeFunction(pars),
			["character", "form", "traumaMode"],
			["The character who owns the form", "The form alias or body id", "The new trauma mode"],
			"Sets a form trauma mode and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormTraumaModeFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             CharacterFormMutationFunctionHelper.TryResolveTraumaMode(ParameterFunctions[2].Result,
			                             out var traumaMode) &&
		                             character.TrySetFormTraumaMode(form, traumaMode, out _));
		return StatementResult.Normal;
	}
}

internal class SetFormTransformationEchoFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormTransformationEcho".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Text],
			(pars, _) => new SetFormTransformationEchoFunction(pars),
			["character", "form", "echo"],
			["The character who owns the form", "The form alias or body id", "The transformation echo text"],
			"Sets a form transformation echo and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormTransformationEchoFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var echo = ParameterFunctions[2].Result?.GetObject?.ToString() ?? string.Empty;
		Result = new BooleanVariable(character?.TrySetFormTransformationEcho(form!, echo, out _) == true);
		return StatementResult.Normal;
	}
}

internal class ClearFormTransformationEchoFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormTransformationEcho".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormTransformationEchoFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form transformation echo so the default echo is used.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormTransformationEchoFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TrySetFormTransformationEcho(form!, null, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormShortDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormShortDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Number],
			(pars, _) => new SetFormShortDescriptionPatternFunction(pars),
			["character", "form", "pattern"],
			["The character who owns the form", "The form alias or body id", "The short description pattern id"],
			"Sets a form short description pattern and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormShortDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var pattern = character is null
			? null
			: CharacterFormMutationFunctionHelper.ResolveDescriptionPattern(character.Gameworld, ParameterFunctions[2].Result,
				EntityDescriptionType.ShortDescription);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             pattern is not null &&
		                             character.TrySetFormDescriptionPattern(form, pattern, out _));
		return StatementResult.Normal;
	}
}

internal class RandomiseFormShortDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"RandomiseFormShortDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new RandomiseFormShortDescriptionPatternFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Randomises a valid short description pattern for a form and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private RandomiseFormShortDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryRandomiseFormDescriptionPattern(form!,
			EntityDescriptionType.ShortDescription, out _) == true);
		return StatementResult.Normal;
	}
}

internal class ClearFormShortDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormShortDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormShortDescriptionPatternFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form short description pattern and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormShortDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryClearFormDescriptionPattern(form!,
			EntityDescriptionType.ShortDescription, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormFullDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormFullDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Number],
			(pars, _) => new SetFormFullDescriptionPatternFunction(pars),
			["character", "form", "pattern"],
			["The character who owns the form", "The form alias or body id", "The full description pattern id"],
			"Sets a form full description pattern and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormFullDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var pattern = character is null
			? null
			: CharacterFormMutationFunctionHelper.ResolveDescriptionPattern(character.Gameworld, ParameterFunctions[2].Result,
				EntityDescriptionType.FullDescription);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             pattern is not null &&
		                             character.TrySetFormDescriptionPattern(form, pattern, out _));
		return StatementResult.Normal;
	}
}

internal class RandomiseFormFullDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"RandomiseFormFullDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new RandomiseFormFullDescriptionPatternFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Randomises a valid full description pattern for a form and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private RandomiseFormFullDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryRandomiseFormDescriptionPattern(form!,
			EntityDescriptionType.FullDescription, out _) == true);
		return StatementResult.Normal;
	}
}

internal class ClearFormFullDescriptionPatternFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormFullDescriptionPattern".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormFullDescriptionPatternFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form full description pattern and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormFullDescriptionPatternFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryClearFormDescriptionPattern(form!,
			EntityDescriptionType.FullDescription, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormAllowVoluntaryFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormAllowVoluntary".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Boolean],
			(pars, _) => new SetFormAllowVoluntaryFunction(pars),
			["character", "form", "allowVoluntary"],
			["The character who owns the form", "The form alias or body id", "Whether voluntary switching should be allowed"],
			"Sets whether a form permits voluntary switching and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormAllowVoluntaryFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var allowVoluntary = ParameterFunctions[2].Result?.GetObject as bool? == true;
		Result = new BooleanVariable(character?.TrySetFormAllowVoluntary(form!, allowVoluntary, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormVisibilityProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Number);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType, ProgVariableTypes progType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormVisibilityProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, progType],
			(pars, _) => new SetFormVisibilityProgFunction(pars),
			["character", "form", "prog"],
			["The character who owns the form", "The form alias or body id", "The visibility prog name or id"],
			"Sets a form visibility prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormVisibilityProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var prog = character is null
			? null
			: CharacterFormMutationFunctionHelper.ResolveProg(character.Gameworld, ParameterFunctions[2].Result,
				ProgVariableTypes.Boolean, [ProgVariableTypes.Character]);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             prog is not null &&
		                             character.TrySetFormVisibilityProg(form, prog, out _));
		return StatementResult.Normal;
	}
}

internal class ClearFormVisibilityProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormVisibilityProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormVisibilityProgFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form visibility prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormVisibilityProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryClearFormVisibilityProg(form!, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormCanSwitchProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Number);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType, ProgVariableTypes progType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormCanSwitchProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, progType],
			(pars, _) => new SetFormCanSwitchProgFunction(pars),
			["character", "form", "prog"],
			["The character who owns the form", "The form alias or body id", "The voluntary eligibility prog name or id"],
			"Sets a form voluntary-switch eligibility prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormCanSwitchProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var prog = character is null
			? null
			: CharacterFormMutationFunctionHelper.ResolveProg(character.Gameworld, ParameterFunctions[2].Result,
				ProgVariableTypes.Boolean, [ProgVariableTypes.Character]);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             prog is not null &&
		                             character.TrySetFormCanSwitchProg(form, prog, out _));
		return StatementResult.Normal;
	}
}

internal class ClearFormCanSwitchProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormCanSwitchProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormCanSwitchProgFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form voluntary-switch eligibility prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormCanSwitchProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryClearFormCanSwitchProg(form!, out _) == true);
		return StatementResult.Normal;
	}
}

internal class SetFormWhyCantProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Text, ProgVariableTypes.Number);
		RegisterCompiler(ProgVariableTypes.Number, ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType, ProgVariableTypes progType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFormWhyCantProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, progType],
			(pars, _) => new SetFormWhyCantProgFunction(pars),
			["character", "form", "prog"],
			["The character who owns the form", "The form alias or body id", "The denial-message prog name or id"],
			"Sets a form voluntary-switch denial-message prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private SetFormWhyCantProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		var prog = character is null
			? null
			: CharacterFormMutationFunctionHelper.ResolveProg(character.Gameworld, ParameterFunctions[2].Result,
				ProgVariableTypes.Text, [ProgVariableTypes.Character]);
		Result = new BooleanVariable(character is not null &&
		                             form is not null &&
		                             prog is not null &&
		                             character.TrySetFormWhyCantProg(form, prog, out _));
		return StatementResult.Normal;
	}
}

internal class ClearFormWhyCantProgFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearFormWhyCantProg".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearFormWhyCantProgFunction(pars),
			["character", "form"],
			["The character who owns the form", "The form alias or body id"],
			"Clears a form denial-message prog and returns true on success.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearFormWhyCantProgFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Boolean;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = CharacterFormMutationFunctionHelper.ResolveConcreteCharacter(ParameterFunctions[0].Result);
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		Result = new BooleanVariable(character?.TryClearFormWhyCantProg(form!, out _) == true);
		return StatementResult.Normal;
	}
}
