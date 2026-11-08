#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC.AI;

namespace MudSharp.FutureProg.Functions.Effects;

internal sealed class NpcMemoryFunction(IList<IFunction> parameters, string operation) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => operation == "rememberedcharacters" ? ProgVariableTypes.Character | ProgVariableTypes.Collection : ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var owner = ParameterFunctions[0].Result?.GetObject as ICharacter;
		if (operation == "rememberedcharacters")
		{
			var name = ParameterFunctions[1].Result?.GetObject?.ToString();
			var subjects = owner is not null && Enum.TryParse<NpcDisposition>(name, true, out var reaction) && Enum.IsDefined(reaction)
				? owner.Effects.OfType<NpcDispositionMemory>().Where(x => x.Disposition == reaction)
					.Select(x => x.Subject).Distinct(ReferenceEqualityComparer.Instance).OfType<ICharacter>().ToList()
				: new List<ICharacter>();
			Result = new CollectionVariable(subjects, ProgVariableTypes.Character);
			return StatementResult.Normal;
		}
		var subject = ParameterFunctions[1].Result?.GetObject as ICharacter;
		var mode = ParameterFunctions[2].Result?.GetObject?.ToString();
		var valid = owner is not null && subject is not null && !ReferenceEquals(owner, subject) &&
			Enum.TryParse<NpcDisposition>(mode, true, out var parsed) && Enum.IsDefined(parsed);
		Result = new BooleanVariable(false);
		if (!valid) return StatementResult.Normal;
		var disposition = Enum.Parse<NpcDisposition>(mode!, true);
		var matches = owner!.Effects.OfType<NpcDispositionMemory>()
			.Where(x => ReferenceEquals(x.Subject, subject) && x.Disposition == disposition).ToArray();
		switch (operation)
		{
			case "remembers":
				Result = new BooleanVariable(NpcDispositionMemory.Has(owner, subject!, disposition));
				break;
			case "forget":
				foreach (var memory in matches) owner.RemoveEffect(memory, true);
				Result = new BooleanVariable(matches.Length > 0);
				break;
			case "remember":
				if (owner.State.IsDead() || subject!.State.IsDead()) break;
				if (matches.Length == 0) owner.AddEffect(new NpcDispositionMemory(owner, subject, disposition));
				Result = new BooleanVariable(true);
				break;
		}
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("rememberedcharacters",
			[ProgVariableTypes.Character, ProgVariableTypes.Text],
			(parameters, _) => new NpcMemoryFunction(parameters, "rememberedcharacters"),
			["actor", "reaction"], ["The remembering character.", "Hate, Fear or Like."],
			"Returns the distinct live subjects of the actor's session memories of that kind; invalid arguments return an empty collection. This is knowledge only and grants no perception or movement permission.",
			"NPC", ProgVariableTypes.Character | ProgVariableTypes.Collection));
		foreach (var operation in new[] { "remember", "forget", "remembers" })
			FutureProg.RegisterBuiltInFunctionCompiler(new(operation,
				[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Text],
				(parameters, _) => new NpcMemoryFunction(parameters, operation),
				["actor", "subject", "reaction"],
				["The remembering character.", "The live character remembered.", "Hate, Fear or Like (independent reactions)."],
				$"{operation} a session-only target reaction. Memories do not stack or expire by time; logout, death or deletion of either participant clears them, and they are never saved. Invalid inputs return false. Remember returns success; forget returns whether anything was removed; remembers queries membership.",
				"NPC", ProgVariableTypes.Boolean));
	}
}
