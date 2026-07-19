#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal static class BodyBackupFunctionHelper
{
	public static int GetPriority(IProgVariable? variable)
	{
		return variable?.GetObject is decimal value ? Convert.ToInt32(value) : 0;
	}

	public static bool TryGetBackupRemainsContext(IProgVariable? variable, out BodyRemainsContext context)
	{
		return BodyBackupEffect.TryParseBackupRemainsContext(variable?.GetObject?.ToString() ?? "sleeve", out context);
	}

	public static string EchoOrDefault(IProgVariable? variable, string defaultEcho)
	{
		return variable is null
			? defaultEcho
			: BodyBackupEffect.NormaliseEcho(variable.GetObject?.ToString() ?? string.Empty, defaultEcho);
	}
}

internal class ReadyBodyBackupFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ReadyBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Location],
			(pars, _) => new ReadyBodyBackupFunction(pars),
			["character", "form", "location"],
			["The character who owns the backup", "The form alias or body id", "The location where the backup awakens"],
			"Readies a form as a death backup with default priority, sleeve remains context, and default transfer echoes.",
			"Characters",
			ProgVariableTypes.Boolean
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ReadyBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Number],
			(pars, _) => new ReadyBodyBackupFunction(pars),
			["character", "form", "location", "priority"],
			["The character who owns the backup", "The form alias or body id", "The location where the backup awakens", "The backup priority"],
			"Readies a form as a death backup with sleeve remains context and default transfer echoes.",
			"Characters",
			ProgVariableTypes.Boolean
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ReadyBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Number, ProgVariableTypes.Text],
			(pars, _) => new ReadyBodyBackupFunction(pars),
			["character", "form", "location", "priority", "remainsContext"],
			["The character who owns the backup", "The form alias or body id", "The location where the backup awakens", "The backup priority", "The old-body remains context"],
			"Readies a form as a death backup with default transfer echoes.",
			"Characters",
			ProgVariableTypes.Boolean
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ReadyBodyBackup".ToLowerInvariant(),
			[
				ProgVariableTypes.Character, formType, ProgVariableTypes.Location, ProgVariableTypes.Number,
				ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text
			],
			(pars, _) => new ReadyBodyBackupFunction(pars),
			["character", "form", "location", "priority", "remainsContext", "oldEcho", "newEcho", "selfEcho"],
			[
				"The character who owns the backup", "The form alias or body id", "The location where the backup awakens",
				"The backup priority", "The old-body remains context", "The echo at the old body", "The echo at the backup body",
				"The private echo to the character"
			],
			"Readies a form as a death backup with custom transfer echoes.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ReadyBodyBackupFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		var location = ParameterFunctions[2].Result?.GetObject as ICell;
		if (character is null || form is null || location is null || form.Body == character.CurrentBody)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var priority = ParameterFunctions.Count > 3 ? BodyBackupFunctionHelper.GetPriority(ParameterFunctions[3].Result) : 0;
		var remainsContext = BodyRemainsContext.SleeveDeath;
		if (ParameterFunctions.Count > 4 &&
		    !BodyBackupFunctionHelper.TryGetBackupRemainsContext(ParameterFunctions[4].Result, out remainsContext))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var oldEcho = ParameterFunctions.Count > 5
			? BodyBackupFunctionHelper.EchoOrDefault(ParameterFunctions[5].Result, BodyBackupEffect.DefaultOldLocationEcho)
			: BodyBackupEffect.DefaultOldLocationEcho;
		var newEcho = ParameterFunctions.Count > 6
			? BodyBackupFunctionHelper.EchoOrDefault(ParameterFunctions[6].Result, BodyBackupEffect.DefaultNewLocationEcho)
			: BodyBackupEffect.DefaultNewLocationEcho;
		var selfEcho = ParameterFunctions.Count > 7
			? BodyBackupFunctionHelper.EchoOrDefault(ParameterFunctions[7].Result, BodyBackupEffect.DefaultSelfEcho)
			: BodyBackupEffect.DefaultSelfEcho;

		character.RemoveAllEffects<BodyBackupEffect>(x => x.BackupBodyId == form.Body.Id, true);
		character.AddEffect(new BodyBackupEffect(character, form.Body.Id, location, RoomLayer.GroundLevel, priority,
			remainsContext, "prog", oldEcho, newEcho, selfEcho, true));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

internal class ClearBodyBackupFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character],
			(pars, _) => new ClearBodyBackupFunction(pars),
			["character"],
			["The character whose prog-created backups should be cleared"],
			"Clears all prog-created death backups from a character.",
			"Characters",
			ProgVariableTypes.Boolean
		));
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"ClearBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new ClearBodyBackupFunction(pars),
			["character", "form"],
			["The character who owns the backup", "The form alias or body id"],
			"Clears a prog-created death backup for a specific form.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private ClearBodyBackupFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		if (character is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var removed = ParameterFunctions.Count == 1
			? character.RemoveAllEffects<BodyBackupEffect>(fireRemovalAction: true)
			: character.RemoveAllEffects<BodyBackupEffect>(
				x => x.BackupBodyId == CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result)?.Body.Id,
				true);
		Result = new BooleanVariable(removed);
		return StatementResult.Normal;
	}
}

internal class HasBodyBackupFunction : BuiltInFunction
{
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"HasBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character],
			(pars, _) => new HasBodyBackupFunction(pars),
			["character"],
			["The character whose backups should be checked"],
			"Returns true if a character has any applicable death backup.",
			"Characters",
			ProgVariableTypes.Boolean
		));
		RegisterCompiler(ProgVariableTypes.Text);
		RegisterCompiler(ProgVariableTypes.Number);
	}

	private static void RegisterCompiler(ProgVariableTypes formType)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"HasBodyBackup".ToLowerInvariant(),
			[ProgVariableTypes.Character, formType],
			(pars, _) => new HasBodyBackupFunction(pars),
			["character", "form"],
			["The character whose backups should be checked", "The form alias or body id"],
			"Returns true if a character has an applicable death backup for a specific form.",
			"Characters",
			ProgVariableTypes.Boolean
		));
	}

	private HasBodyBackupFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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
		if (character is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var form = ParameterFunctions.Count > 1
			? CharacterFormFunctionHelper.ResolveForm(character, ParameterFunctions[1].Result)
			: null;
		Result = new BooleanVariable(character.EffectsOfType<IBodyBackupEffect>()
		                                     .Where(x => x.Applies())
		                                     .Any(x => form is null || x.BackupBodyId == form.Body.Id));
		return StatementResult.Normal;
	}
}
