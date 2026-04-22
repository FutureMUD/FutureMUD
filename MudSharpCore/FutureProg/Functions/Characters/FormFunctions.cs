#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Characters;

internal static class CharacterFormFunctionHelper
{
	public static ICharacterForm? ResolveForm(ICharacter? character, IProgVariable? formIdentifier)
	{
		if (character is null)
		{
			return null;
		}

		var value = formIdentifier?.GetObject;
		if (value is decimal decimalValue)
		{
			var id = Convert.ToInt64(decimalValue);
			return character.Forms.FirstOrDefault(x => x.Body.Id == id);
		}

		var text = value?.ToString();
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		return long.TryParse(text, out var idValue)
			? character.Forms.FirstOrDefault(x => x.Body.Id == idValue)
			: character.Forms.FirstOrDefault(x => x.Alias.EqualTo(text));
	}
}

internal class CanSwitchFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CanSwitchForm".ToLowerInvariant(),
				[ProgVariableTypes.Character, type],
				(pars, _) => new CanSwitchFormFunction(pars),
				["character", "form"],
				["The character whose forms you want to inspect", "The form alias or body id"],
				"Returns true if the character can voluntarily switch to the specified form right now.",
				"Characters",
				ProgVariableTypes.Boolean
			)
		);
	}

	private CanSwitchFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		Result = new BooleanVariable(form is not null &&
		                             character?.CanSwitchBody(form.Body, BodySwitchIntent.Voluntary, out _) == true);
		return StatementResult.Normal;
	}
}

internal class WhyCantSwitchFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"WhyCantSwitchForm".ToLowerInvariant(),
				[ProgVariableTypes.Character, type],
				(pars, _) => new WhyCantSwitchFormFunction(pars),
				["character", "form"],
				["The character whose forms you want to inspect", "The form alias or body id"],
				"Returns the reason why a character cannot voluntarily switch to the specified form.",
				"Characters",
				ProgVariableTypes.Text
			)
		);
	}

	private WhyCantSwitchFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Text;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var form = CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result);
		if (character is null)
		{
			Result = new TextVariable("The target is not a character.");
			return StatementResult.Normal;
		}

		if (form is null)
		{
			Result = new TextVariable("There is no such form.");
			return StatementResult.Normal;
		}

		character.CanSwitchBody(form.Body, BodySwitchIntent.Voluntary, out var whyNot);
		Result = new TextVariable(whyNot ?? string.Empty);
		return StatementResult.Normal;
	}
}

internal class SwitchFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"SwitchForm".ToLowerInvariant(),
				[ProgVariableTypes.Character, type],
				(pars, _) => new SwitchFormFunction(pars),
				["character", "form"],
				["The character who should switch form", "The form alias or body id"],
				"Attempts a voluntary form switch, including ordinary player-facing feedback, and returns true on success.",
				"Characters",
				ProgVariableTypes.Boolean
			)
		);
	}

	private SwitchFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		if (character is null || form is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (!character.CanSwitchBody(form.Body, BodySwitchIntent.Voluntary, out var whyNot))
		{
			if (!string.IsNullOrWhiteSpace(whyNot))
			{
				character.OutputHandler.Send(whyNot);
			}

			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var success = character.SwitchToBody(form.Body, BodySwitchIntent.Voluntary);
		if (success)
		{
			character.OutputHandler.Send($"You switch into your {form.Alias.ColourName()} form.");
		}

		Result = new BooleanVariable(success);
		return StatementResult.Normal;
	}
}

internal class ForceSwitchFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"ForceSwitchForm".ToLowerInvariant(),
				[ProgVariableTypes.Character, type],
				(pars, _) => new ForceSwitchFormFunction(pars),
				["character", "form"],
				["The character who should switch form", "The form alias or body id"],
				"Attempts a scripted form switch that bypasses voluntary gating but still enforces structural validity.",
				"Characters",
				ProgVariableTypes.Boolean
			)
		);
	}

	private ForceSwitchFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		Result = new BooleanVariable(form is not null &&
		                             character?.SwitchToBody(form.Body, BodySwitchIntent.Scripted) == true);
		return StatementResult.Normal;
	}
}

internal class HasFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"HasForm".ToLowerInvariant(),
				[ProgVariableTypes.Character, type],
				(pars, _) => new HasFormFunction(pars),
				["character", "form"],
				["The character whose forms you want to inspect", "The form alias or body id"],
				"Returns true if the character owns a form with the specified alias or body id.",
				"Characters",
				ProgVariableTypes.Boolean
			)
		);
	}

	private HasFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		Result = new BooleanVariable(CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result) is not null);
		return StatementResult.Normal;
	}
}

internal class CurrentFormFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CurrentForm".ToLowerInvariant(),
				[ProgVariableTypes.Character],
				(pars, _) => new CurrentFormFunction(pars),
				["character"],
				["The character whose current form you want to inspect"],
				"Returns the alias of the character's current form.",
				"Characters",
				ProgVariableTypes.Text
			)
		);
	}

	private CurrentFormFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Text;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var form = character?.Forms.FirstOrDefault(x => x.Body == character.CurrentBody);
		Result = new TextVariable(form?.Alias ?? string.Empty);
		return StatementResult.Normal;
	}
}

internal class CurrentFormIdFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"CurrentFormId".ToLowerInvariant(),
				[ProgVariableTypes.Character],
				(pars, _) => new CurrentFormIdFunction(pars),
				["character"],
				["The character whose current form you want to inspect"],
				"Returns the body id of the character's current form.",
				"Characters",
				ProgVariableTypes.Number
			)
		);
	}

	private CurrentFormIdFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType => ProgVariableTypes.Number;

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var form = character?.Forms.FirstOrDefault(x => x.Body == character.CurrentBody);
		Result = new NumberVariable(form?.Body.Id ?? 0);
		return StatementResult.Normal;
	}
}
