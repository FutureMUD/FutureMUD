using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Projects.ConcreteTypes;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.Agriculture;

internal static class AgricultureFunctionHelpers
{
	public static IFrameworkItem ResolveTarget(IFuturemud gameworld, IAgricultureOperation operation, string targetText)
	{
		if (operation.TargetType == AgricultureTargetType.None)
		{
			return null;
		}

		if (string.IsNullOrWhiteSpace(targetText))
		{
			return null;
		}

		return operation.TargetType switch
		{
			AgricultureTargetType.Crop => gameworld.AgricultureCropDefinitions.GetByIdOrName(targetText),
			AgricultureTargetType.Herd => gameworld.AgricultureHerdDefinitions.GetByIdOrName(targetText),
			AgricultureTargetType.Woodland => gameworld.AgricultureWoodlandDefinitions.GetByIdOrName(targetText),
			_ => null
		};
	}

	public static bool StartProject(ICharacter actor, IAgricultureField field, IAgricultureOperation operation, IFrameworkItem target)
	{
		if (actor == null || field == null || operation == null || operation.Project == null)
		{
			return false;
		}

		if (!field.CanBeginOperation(actor, operation, target, out _))
		{
			return false;
		}

		if (!operation.Project.CanInitiateProject(actor))
		{
			return false;
		}

		var active = new ActiveLocalProject(operation.Project, actor);
		actor.Gameworld.Add(active);
		actor.Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			FMDB.Context.AgricultureProjectContexts.Add(new MudSharp.Models.AgricultureProjectContext
			{
				ActiveProjectId = active.Id,
				AgricultureFieldId = field.Id,
				OperationId = operation.Id,
				TargetType = (int)operation.TargetType,
				TargetId = target?.Id,
				TargetText = target?.Name ?? string.Empty,
				ActorId = CharacterInstanceIdentityComparer.IdentityId(actor),
				Definition = "<Context />"
			});
			FMDB.Context.SaveChanges();
		}

		return true;
	}
}

internal class FieldAtFunction : BuiltInFunction
{
	private FieldAtFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.AgricultureField;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var cell = (ICell)ParameterFunctions[0].Result?.GetObject;
		Result = cell?.AgricultureField == null
			? new NullVariable(ProgVariableTypes.AgricultureField)
			: cell.AgricultureField;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"FieldAt".ToLowerInvariant(),
			new[] { ProgVariableTypes.Location },
			(pars, _) => new FieldAtFunction(pars),
			new[] { "location" },
			new[] { "The location to query" },
			"Returns the agriculture field in a location, or null if there is none.",
			"Agriculture",
			ProgVariableTypes.AgricultureField));
	}
}

internal class CreateFieldFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private CreateFieldFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.AgricultureField;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var cell = (ICell)ParameterFunctions[0].Result?.GetObject;
		if (cell == null || cell.AgricultureField != null)
		{
			Result = new NullVariable(ProgVariableTypes.AgricultureField);
			return StatementResult.Normal;
		}

		var profile = ParameterFunctions.Count == 1
			? cell.Terrain(null).DefaultAgricultureFieldProfile
			: ResolveProfile();
		if (profile == null)
		{
			Result = new NullVariable(ProgVariableTypes.AgricultureField);
			return StatementResult.Normal;
		}

		var field = new AgricultureField(cell, profile);
		_gameworld.Add(field);
		Result = field;
		return StatementResult.Normal;
	}

	private IAgricultureFieldProfile ResolveProfile()
	{
		return ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? _gameworld.AgricultureFieldProfiles.Get(Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0))
			: _gameworld.AgricultureFieldProfiles.GetByIdOrName((string)ParameterFunctions[1].Result?.GetObject ?? string.Empty);
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"CreateField".ToLowerInvariant(),
			new[] { ProgVariableTypes.Location },
			(pars, gameworld) => new CreateFieldFunction(pars, gameworld),
			new[] { "location" },
			new[] { "The location to create a field in, using the terrain default agriculture profile" },
			"Creates an agriculture field in a location, returning null if one already exists or no profile is available.",
			"Agriculture",
			ProgVariableTypes.AgricultureField));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"CreateField".ToLowerInvariant(),
			new[] { ProgVariableTypes.Location, ProgVariableTypes.Number },
			(pars, gameworld) => new CreateFieldFunction(pars, gameworld),
			new[] { "location", "profile" },
			new[] { "The location to create a field in", "The agriculture field profile ID" },
			"Creates an agriculture field in a location with the specified profile.",
			"Agriculture",
			ProgVariableTypes.AgricultureField));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"CreateField".ToLowerInvariant(),
			new[] { ProgVariableTypes.Location, ProgVariableTypes.Text },
			(pars, gameworld) => new CreateFieldFunction(pars, gameworld),
			new[] { "location", "profile" },
			new[] { "The location to create a field in", "The agriculture field profile name or ID" },
			"Creates an agriculture field in a location with the specified profile.",
			"Agriculture",
			ProgVariableTypes.AgricultureField));
	}
}

internal class DeleteFieldFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private DeleteFieldFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		_gameworld = gameworld;
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

		var field = (IAgricultureField)ParameterFunctions[0].Result?.GetObject;
		if (field == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		_gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.AgricultureFields.Find(field.Id);
			if (dbitem != null)
			{
				FMDB.Context.AgricultureFields.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		_gameworld.SaveManager.Abort(field);
		_gameworld.Destroy(field);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"DeleteField".ToLowerInvariant(),
			new[] { ProgVariableTypes.AgricultureField },
			(pars, gameworld) => new DeleteFieldFunction(pars, gameworld),
			new[] { "field" },
			new[] { "The agriculture field to delete" },
			"Deletes an agriculture field.",
			"Agriculture",
			ProgVariableTypes.Boolean));
	}
}

internal class SetFieldScoreFunction : BuiltInFunction
{
	private readonly bool _adjust;

	private SetFieldScoreFunction(IList<IFunction> parameterFunctions, bool adjust) : base(parameterFunctions)
	{
		_adjust = adjust;
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

		var field = (IAgricultureField)ParameterFunctions[0].Result?.GetObject;
		var scoreText = (string)ParameterFunctions[1].Result?.GetObject;
		var value = Convert.ToInt32(ParameterFunctions[2].Result?.GetObject ?? 0);
		if (field == null || !AgricultureScoreTypeExtensions.TryParseScoreType(scoreText, field.Gameworld, out var score))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (_adjust)
		{
			field.AdjustScore(score, value);
		}
		else
		{
			field.SetScore(score, value);
			field.Changed = true;
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"SetFieldScore".ToLowerInvariant(),
			new[] { ProgVariableTypes.AgricultureField, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, _) => new SetFieldScoreFunction(pars, false),
			new[] { "field", "score", "value" },
			new[] { "The agriculture field", "The score name", "The 0-100 value" },
			"Sets one of a field's agriculture scores.",
			"Agriculture",
			ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"AdjustFieldScore".ToLowerInvariant(),
			new[] { ProgVariableTypes.AgricultureField, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, _) => new SetFieldScoreFunction(pars, true),
			new[] { "field", "score", "delta" },
			new[] { "The agriculture field", "The score name", "The adjustment value" },
			"Adjusts one of a field's agriculture scores.",
			"Agriculture",
			ProgVariableTypes.Boolean));
	}
}

internal class FieldScoreFunction : BuiltInFunction
{
	private FieldScoreFunction(IList<IFunction> parameterFunctions) : base(parameterFunctions)
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

		var field = (IAgricultureField)ParameterFunctions[0].Result?.GetObject;
		var scoreText = (string)ParameterFunctions[1].Result?.GetObject;
		if (field == null || !AgricultureScoreTypeExtensions.TryParseScoreType(scoreText, field.Gameworld, out var score))
		{
			Result = new NumberVariable(0);
			return StatementResult.Normal;
		}

		Result = new NumberVariable(field.Score(score));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"FieldScore".ToLowerInvariant(),
			new[] { ProgVariableTypes.AgricultureField, ProgVariableTypes.Text },
			(pars, _) => new FieldScoreFunction(pars),
			new[] { "field", "score" },
			new[] { "The agriculture field", "The score name" },
			"Returns one of a field's agriculture scores.",
			"Agriculture",
			ProgVariableTypes.Number));
	}
}

internal class StartFieldProjectFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private StartFieldProjectFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		_gameworld = gameworld;
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

		var actor = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		var field = (IAgricultureField)ParameterFunctions[1].Result?.GetObject;
		var operationText = (string)ParameterFunctions[2].Result?.GetObject ?? string.Empty;
		var operation = _gameworld.AgricultureOperations.GetByIdOrName(operationText);
		var targetText = ParameterFunctions.Count > 3 ? (string)ParameterFunctions[3].Result?.GetObject ?? string.Empty : string.Empty;
		var target = operation == null ? null : AgricultureFunctionHelpers.ResolveTarget(_gameworld, operation, targetText);
		Result = new BooleanVariable(AgricultureFunctionHelpers.StartProject(actor, field, operation, target));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"StartFieldProject".ToLowerInvariant(),
			new[] { ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.Text },
			(pars, gameworld) => new StartFieldProjectFunction(pars, gameworld),
			new[] { "actor", "field", "operation" },
			new[] { "The character starting the project", "The field", "The operation name or ID" },
			"Starts an agriculture project with no target.",
			"Agriculture",
			ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"StartFieldProject".ToLowerInvariant(),
			new[] { ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.Text, ProgVariableTypes.Text },
			(pars, gameworld) => new StartFieldProjectFunction(pars, gameworld),
			new[] { "actor", "field", "operation", "target" },
			new[] { "The character starting the project", "The field", "The operation name or ID", "The crop, herd, or woodland target name or ID" },
			"Starts an agriculture project with a dynamic target.",
			"Agriculture",
			ProgVariableTypes.Boolean));
	}
}

internal class FieldHerdFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly bool _absorb;

	private FieldHerdFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool absorb) : base(parameterFunctions)
	{
		_gameworld = gameworld;
		_absorb = absorb;
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

		var actor = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		var field = (IAgricultureField)ParameterFunctions[1].Result?.GetObject;
		var herd = _gameworld.AgricultureHerdDefinitions.GetByIdOrName((string)ParameterFunctions[2].Result?.GetObject ?? string.Empty);
		if (actor == null || field == null || herd == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (_absorb)
		{
			var npc = (ICharacter)ParameterFunctions[3].Result?.GetObject;
			Result = new BooleanVariable(field.AbsorbNpcIntoHerd(npc, herd, actor, out _));
			return StatementResult.Normal;
		}

		var count = Convert.ToInt32(ParameterFunctions[3].Result?.GetObject ?? 1);
		Result = new BooleanVariable(field.DrawDownHerd(herd, count, actor, out _));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"DrawFieldHerd".ToLowerInvariant(),
			new[] { ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.Text, ProgVariableTypes.Number },
			(pars, gameworld) => new FieldHerdFunction(pars, gameworld, false),
			new[] { "actor", "field", "herd", "count" },
			new[] { "The character drawing animals", "The field", "The herd definition name or ID", "The number of animals" },
			"Draws live NPC livestock from an abstract field herd.",
			"Agriculture",
			ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"AbsorbNpcIntoFieldHerd".ToLowerInvariant(),
			new[] { ProgVariableTypes.Character, ProgVariableTypes.AgricultureField, ProgVariableTypes.Text, ProgVariableTypes.Character },
			(pars, gameworld) => new FieldHerdFunction(pars, gameworld, true),
			new[] { "actor", "field", "herd", "npc" },
			new[] { "The character doing the absorption", "The field", "The herd definition name or ID", "The NPC to absorb" },
			"Absorbs a live NPC into an abstract field herd.",
			"Agriculture",
			ProgVariableTypes.Boolean));
	}
}
