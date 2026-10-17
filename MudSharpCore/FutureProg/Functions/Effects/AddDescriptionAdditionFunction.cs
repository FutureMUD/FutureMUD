#nullable enable
using System;
using SystemDateTime = System.DateTime;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Effects;

/// <summary>One persistent, prog-owned description addendum; a pure applicability prog controls visibility.</summary>
internal sealed class AddDescriptionAdditionFunction(IList<IFunction> parameters, IFuturemud gameworld)
	: BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public static void RegisterFunctionCompiler()
	{
		foreach (var timed in new[] { false, true })
		{
			var types = new List<ProgVariableTypes> { ProgVariableTypes.Perceivable, ProgVariableTypes.Text, ProgVariableTypes.Number };
			var names = new List<string> { "owner", "text", "applicabilityProgId" };
			var help = new List<string> { "The perceivable receiving an addendum", "The additional description text",
				"A non-static boolean prog accepting this owner type and a perceiver" };
			if (timed)
			{
				types.AddRange([ProgVariableTypes.DateTime, ProgVariableTypes.Text]);
				names.AddRange(["expiresUtc", "expirationMessage"]);
				help.AddRange(["The absolute UTC expiry; offline time counts", "The local message sent once on live expiry"]);
			}
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("adddescriptionaddition", types,
				(pars, world) => new AddDescriptionAdditionFunction(pars, world), names, help,
				"Adds a persistent non-player description controlled by an exact applicability prog. Identical text/prog repeats once; conflicting retained text or expiry messages refuse. The optional UTC expiry removes the addendum and announces it locally; overdue cold loads retire silently.",
				"Effects", ProgVariableTypes.Boolean));
		}
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not IPerceivable owner ||
			ParameterFunctions[1].Result?.GetObject is not string text || string.IsNullOrWhiteSpace(text) || text.Length > 4000 ||
			ParameterFunctions[2].Result?.GetObject is not decimal id || id <= 0 || id > long.MaxValue || id != decimal.Truncate(id))
		{
			ErrorMessage = "A perceivable, bounded nonempty description and positive integral applicability prog ID are required.";
			return StatementResult.Error;
		}
		var prog = gameworld.FutureProgs.Get((long)id);
		if (prog is null || prog.AcceptsAnyParameters || prog.StaticType != FutureProgStaticType.NotStatic ||
			prog.ReturnType != ProgVariableTypes.Boolean || !string.IsNullOrEmpty(prog.CompileError) ||
			!prog.MatchesParameters([owner.Type, ProgVariableTypes.Perceiver]))
		{
			ErrorMessage = "The description applicability prog has no compatible exact live signature.";
			return StatementResult.Error;
		}
		SystemDateTime? expires = null;
		string? expirationMessage = null;
		if (ParameterFunctions.Count == 5)
		{
			if (ParameterFunctions[3].Result?.GetObject is not SystemDateTime deadline ||
				ParameterFunctions[4].Result?.GetObject is not string message || string.IsNullOrWhiteSpace(message) || message.Length > 4000)
			{
				ErrorMessage = "A future UTC expiry and bounded nonempty expiration message are required.";
				return StatementResult.Error;
			}
			expires = deadline.Kind == DateTimeKind.Local ? deadline.ToUniversalTime() : SystemDateTime.SpecifyKind(deadline, DateTimeKind.Utc);
			if (expires <= RuntimeClock.UtcNow)
			{
				ErrorMessage = "The description expiry must be in the future.";
				return StatementResult.Error;
			}
			expirationMessage = message;
		}
		var existing = owner.Effects.OfType<DescriptionAddition>()
			.Where(x => !x.PlayerSet && x.ApplicabilityProg?.Id == prog.Id).ToArray();
		if (existing.Length > 1 || existing.Any(x => x.AdditionalText != text || expires is not null &&
			!string.IsNullOrEmpty(x.ExpirationMessage) && x.ExpirationMessage != expirationMessage))
		{
			ErrorMessage = "An existing addendum with this applicability prog differs; resolve the retained authoring explicitly.";
			return StatementResult.Error;
		}
		var effect = existing.SingleOrDefault() ?? new DescriptionAddition(owner, text, false, null!, prog);
		if (expires is { } until) effect.SetExpiration(until, expirationMessage!);
		if (existing.Length == 0) owner.AddEffect(effect);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
