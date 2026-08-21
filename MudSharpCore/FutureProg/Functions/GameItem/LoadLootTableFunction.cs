#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Framework.Revision;
using MudSharp.Work.Loot;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class LoadLootTableFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly ProgVariableTypes _targetType;
	private readonly bool _seeded;

	private LoadLootTableFunction(IList<IFunction> parameters, IFuturemud gameworld, ProgVariableTypes targetType, bool seeded) : base(parameters)
	{
		_gameworld = gameworld;
		_targetType = targetType;
		_seeded = seeded;
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var id = (long)(decimal)ParameterFunctions[0].Result.GetObject;
		var revision = (int)(decimal)ParameterFunctions[1].Result.GetObject;
		var table = _gameworld.LootTables.Get(id, revision);
		if (table is null) return Receipt("ERROR code=TABLE_NOT_FOUND message=The exact LootTable revision does not exist.");
		if (!table.Status.In(RevisionStatus.Current, RevisionStatus.Revised)) return Receipt("ERROR code=TABLE_NOT_APPROVED message=The exact LootTable revision is not approved.");
		var variant = ParameterFunctions[3].Result.GetObject?.ToString() ?? string.Empty;
		var seed = _seeded ? (long)(decimal)ParameterFunctions[4].Result.GetObject : Random.Shared.NextInt64(long.MaxValue);
		if (seed < 0) return Receipt("ERROR code=INVALID_SEED message=The seed must be non-negative.");
		var materialiser = new LootTableMaterialiser(_gameworld);
		var target = ParameterFunctions[2].Result?.GetObject;
		LootMaterialisationResult result;
		if (_targetType == ProgVariableTypes.Location && target is ICell cell) result = materialiser.Materialise(table, variant, seed, cell);
		else if (_targetType == ProgVariableTypes.Item && target is IGameItem item) result = materialiser.Materialise(table, variant, seed, item);
		else if (_targetType == ProgVariableTypes.Character && target is ICharacter character) result = materialiser.Materialise(table, variant, seed, character);
		else return Receipt("ERROR code=NULL_TARGET message=The LootTable target was null or invalid.");
		return Receipt(result.Receipt);
	}

	private StatementResult Receipt(string text)
	{
		Result = new TextVariable(text);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var targetType in new[] { ProgVariableTypes.Location, ProgVariableTypes.Item, ProgVariableTypes.Character })
		{
			Register(targetType, false);
			Register(targetType, true);
		}
	}

	private static void Register(ProgVariableTypes targetType, bool seeded)
	{
		var types = seeded
			? new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, targetType, ProgVariableTypes.Text, ProgVariableTypes.Number }
			: new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, targetType, ProgVariableTypes.Text };
		var names = seeded
			? new List<string> { "tableId", "revision", "target", "variant", "seed" }
			: new List<string> { "tableId", "revision", "target", "variant" };
		var descriptions = seeded
			? new List<string> { "The LootTable ID", "The exact revision", "The destination", "The variant key", "A non-negative deterministic seed" }
			: new List<string> { "The LootTable ID", "The exact revision", "The destination", "The variant key" };
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"loadloottable", types,
			(pars, gameworld) => new LoadLootTableFunction(pars, gameworld, targetType, seeded),
			names, descriptions,
			"Atomically stages and commits an exact LootTable revision into a location, container item, or character inventory. Returns a canonical OK or ERROR receipt.",
			"Items", ProgVariableTypes.Text));
	}
}
