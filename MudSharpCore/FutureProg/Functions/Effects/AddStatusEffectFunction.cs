#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;

namespace MudSharp.FutureProg.Functions.Effects;

/// <summary>Creates native, spell-independent saved effects without requiring a synthetic caster or spell.</summary>
internal sealed class AddStatusEffectFunction(IList<IFunction> parameters, string kind) : BuiltInFunction(parameters)
{
	private static readonly IReadOnlyDictionary<string, string> Functions = new Dictionary<string, string>
	{
		["addblindnesseffect"] = "blindness", ["addsilenceeffect"] = "silence", ["addflighteffect"] = "flight",
		["addvisualinviseffect"] = "invisibility", ["adddetectinvisibleeffect"] = "detectinvisible",
		["adddetectetherealeffect"] = "detectethereal", ["adddetectmagickeffect"] = "detectmagick",
		["addinfravisioneffect"] = "infravision", ["addsneakeffect"] = "sneak", ["addhideeffect"] = "hide",
		["adddetectpoisoneffect"] = "detectpoison", ["addclimbingeffect"] = "climbing", ["addpacifismeffect"] = "pacifism"
	};
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Effect; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var target = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var seconds = ParameterFunctions.Count > 1 ? (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? -1m) : 0;
		if (target is null || !double.IsFinite(seconds) || seconds < 0 || seconds > TimeSpan.MaxValue.TotalSeconds / 2)
		{
			Result = new NullVariable(ReturnType); return StatementResult.Normal;
		}
		// Native combat admission checks pacifism on the body; stealth/perception/movement
		// statuses use the character and their existing combined-effect queries.
		IPerceivable owner = kind == "pacifism" ? target.Body : target;
		if (owner is null) { Result = new NullVariable(ReturnType); return StatementResult.Normal; }
		var existing = owner.Effects.FirstOrDefault(effect => kind switch
		{
			"blindness" => effect is BlindnessStatus, "silence" => effect is SilenceStatus, "flight" => effect is FlightStatus,
			"sneak" => effect is MudSharp.Effects.Interfaces.ISneakEffect, "hide" => effect is MudSharp.Effects.Interfaces.IHideEffect,
			"detectpoison" => effect is PoisonSenseStatus,
			"climbing" => effect is ClimbingStatus,
			"pacifism" => effect is PacifismStatus,
			_ => effect is PerceptionStatus perception && perception.Key == kind
		});
		if (existing is not null)
		{
			Result = existing; return StatementResult.Normal;
		}
		if (kind == "hide" && target.Location is null)
		{
			Result = new NullVariable(ReturnType); return StatementResult.Normal;
		}
		Effect effect = kind switch
		{
			"blindness" => new BlindnessStatus(target), "silence" => new SilenceStatus(target), "flight" => new FlightStatus(target),
			"invisibility" => new PerceptionStatus(target, kind, PerceptionTypes.None, PerceptionTypes.DirectVisual),
			"detectinvisible" => new PerceptionStatus(target, kind, PerceptionTypes.VisualMagical),
			"detectethereal" => new PerceptionStatus(target, kind, PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal),
			"detectmagick" => new PerceptionStatus(target, kind, PerceptionTypes.SenseMagical),
			"infravision" => new InfravisionStatus(target), "sneak" => new Sneak(target),
			"detectpoison" => new PoisonSenseStatus(target),
			"climbing" => new ClimbingStatus(target),
			"pacifism" => new PacifismStatus(owner),
			"hide" => new HideInvis(target, target.Gameworld.GetCheck(CheckType.HideCheck).Check(target, target.Location.Terrain(target).HideDifficulty).TargetNumber),
			_ => throw new InvalidOperationException($"Unknown native status {kind}.")
		};
		if (seconds == 0) owner.AddEffect(effect);
		else owner.AddEffect(effect, TimeSpan.FromSeconds(seconds));
		Result = effect;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler()
	{
		foreach (var (name, status) in Functions)
		foreach (var timed in new[] { false, true })
			FutureProg.RegisterBuiltInFunctionCompiler(new(name,
				timed ? [ProgVariableTypes.Character, ProgVariableTypes.Number] : [ProgVariableTypes.Character],
				(parameters, _) => new AddStatusEffectFunction(parameters, status),
				timed ? ["target", "seconds"] : ["target"],
				timed ? ["The character.", "Seconds until expiry; zero means indefinite."] : ["The character."],
				$"Adds a saved, spell-independent {status} effect. Returns that effect, the existing matching effect without stacking/refreshing, or null for invalid inputs. Hiding uses a native terrain hide check; flight grants capability without relocating the character.",
				"Effects", ProgVariableTypes.Effect));
	}
}
