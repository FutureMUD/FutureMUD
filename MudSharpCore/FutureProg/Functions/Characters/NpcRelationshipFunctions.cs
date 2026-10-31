#nullable enable
using MudSharp.FutureProg.Variables;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class NpcRelationshipFunctions : BuiltInFunction
{
	private readonly string _operation;
	private readonly IFuturemud _gameworld;

	internal NpcRelationshipFunctions(IList<IFunction> parameters, IFuturemud gameworld, string operation) : base(parameters)
	{
		_gameworld = gameworld;
		_operation = operation;
	}

	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public static void RegisterFunctionCompiler()
	{
		foreach (var name in new[] { "setfollowing", "setnpccompanion", "setnpcai" })
		{
			var ai = name == "setnpcai";
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
				ai ? [ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Boolean] : [ProgVariableTypes.Character, ProgVariableTypes.Character],
				(pars, world) => new NpcRelationshipFunctions(pars, world, name),
				ai ? ["npc", "ai", "enabled"] : ["character", "target"],
				ai ? ["The NPC to change.", "An existing AI definition ID.", "Whether the NPC should have that AI."] : ["The character to change (an NPC for companion assignment).", "The character to follow or accompany; null clears the relationship."],
				name switch
				{
					"setfollowing" => "Sets native following between colocated characters, refusing cycles. Null stops following. Does not change ownership or riding permission.",
					"setnpccompanion" => "Assigns a colocated living player as the NPC's companion using the existing bodyguard logout lifecycle. Null releases it. Does not grant combat duties, ownership or riding permission.",
					_ => "Adds or removes an existing AI on an NPC instance, persisting the change without changing its template. Returns false for an invalid NPC or AI."
				}, "Characters", ProgVariableTypes.Boolean));
		}
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter actor) return StatementResult.Normal;
		using var execution = CommandExecutionScope.EnterBodyOperation(actor);
		if (!CommandExecutionScope.TryContinue(actor)) return StatementResult.Normal;
		if (_operation == "setfollowing")
		{
			Result = new BooleanVariable(NpcCompanionLifecycle.SetFollowing(actor, ParameterFunctions[1].Result?.GetObject as ICharacter));
			return StatementResult.Normal;
		}
		if (actor is not INPC npc) return StatementResult.Normal;
		if (_operation == "setnpccompanion")
		{
			Result = new BooleanVariable(NpcCompanionLifecycle.Assign(npc, ParameterFunctions[1].Result?.GetObject as ICharacter));
			return StatementResult.Normal;
		}
		var number = Convert.ToDecimal(ParameterFunctions[1].Result?.GetObject ?? 0M);
		if (number <= 0 || number > long.MaxValue || decimal.Truncate(number) != number) return StatementResult.Normal;
		var id = (long)number;
		var ai = _gameworld.AIs.Get(id);
		if (ai is null) return StatementResult.Normal;
		var enabled = ParameterFunctions[2].Result?.GetObject is true;
		if (!enabled && ai is IMountableAI && npc.Riders.Any()) return StatementResult.Normal;
		CommandExecutionScope.MarkCommitted(actor);
		if (enabled && !npc.AIs.Any(x => x.Id == id)) npc.AddAI(ai);
		if (!enabled && npc.AIs.Any(x => x.Id == id))
		{
			var companion = NpcCompanionLifecycle.FollowsHandler(npc);
			npc.RemoveAI(ai);
			if (companion && !NpcCompanionLifecycle.FollowsHandler(npc))
			{
				npc.BodyguardingCharacterID = null;
				npc.CeaseFollowing();
			}
		}
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
