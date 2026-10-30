using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class WeaponPoisonFunction(IList<IFunction> parameters, IFuturemud world, string operation) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => operation == "actioncheck" ? ProgVariableTypes.Number : ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (operation == "actioncheck")
		{
			var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
			var checkId = (decimal?)ParameterFunctions[1].Result?.GetObject ?? -1m;
			var difficulty = (decimal?)ParameterFunctions[2].Result?.GetObject ?? 5m;
			if (actor?.Body is not { } body || !ReferenceEquals(body.Actor, actor) ||
				checkId < int.MinValue || checkId > int.MaxValue || decimal.Truncate(checkId) != checkId ||
				!Enum.IsDefined(typeof(CheckType), (int)checkId) || !CharacterState.Able.HasFlag(actor.State) ||
				actor.EffectsOfType<ISpellProjectionBoundary>().Any() ||
				!CommandExecutionScope.TryContinue(actor))
			{
				ErrorMessage = "A native action check requires a current able physical actor and a defined check type.";
				return StatementResult.Error;
			}
			var outcome = world.GetCheck((CheckType)(int)checkId).Check(actor, (Difficulty)(int)Math.Clamp(difficulty, 0m, 10m));
			if (!ReferenceEquals(actor.Body, body) || !CommandExecutionScope.TryContinue(actor))
			{
				ErrorMessage = "The actor changed during the native action check.";
				return StatementResult.Error;
			}
			Result = new NumberVariable(outcome.CheckDegrees());
			return StatementResult.Normal;
		}
		if (operation == "canmanipulateitem")
		{
			var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
			var target = ParameterFunctions[1].Result?.GetObject as IGameItem;
			Result = new BooleanVariable(actor is not null && target is not null && CharacterState.Able.HasFlag(actor.State) &&
				!actor.Effects.Any(x => x.Applies() && x.IsBlockingEffect("general")) &&
				ItemManipulationGuard.CanManipulate(actor, out _, target));
			return StatementResult.Normal;
		}
		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		if (operation == "hasweaponpoison")
			Result = new BooleanVariable(item?.EffectsOfType<IWeaponPoisonCoatingEffect>().Any() == true);
		else if (operation == "removeweaponpoison")
		{
			var effects = item?.EffectsOfType<IWeaponPoisonCoatingEffect>().ToArray() ?? [];
			foreach (var effect in effects) item!.RemoveEffect(effect, true);
			Result = new BooleanVariable(effects.Length > 0);
		}
		else
		{
			var id = (decimal?)ParameterFunctions[1].Result?.GetObject ?? 0m;
			var prog = id > 0 && id <= long.MaxValue && decimal.Truncate(id) == id ? world.FutureProgs.Get((long)id) : null;
			Result = new BooleanVariable(item is not null && prog is not null && WeaponPoisonDeliveryHelper.AddScriptedPoisonCoating(item, prog));
		}
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("setweaponpoison", [ProgVariableTypes.Item, ProgVariableTypes.Number], ["item", "deliveryProgId"],
			"Attaches a persistent single-use coating to one uncoated melee weapon or ammunition item. The delivery prog takes attacker, victim and weapon; it owns dosing and saves. No liquid is manufactured.");
		Register("hasweaponpoison", [ProgVariableTypes.Item], ["item"], "Whether the item has any native weapon poison coating.");
		Register("removeweaponpoison", [ProgVariableTypes.Item], ["item"], "Removes native weapon poison coatings, including scripted coatings, without changing unrelated item state.");
		Register("canmanipulateitem", [ProgVariableTypes.Character, ProgVariableTypes.Item], ["actor", "item"],
			"Checks native manual manipulation and access, able character state and general action blockers without changing the item.");
		Register("actioncheck", [ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number], ["actor", "checkType", "difficulty"],
			"Runs an existing native check type, including its configured trait policy and improvements. Returns -3 through 3 check degrees; invalid or changed actors stop the prog.");
	}
	private static void Register(string name, ProgVariableTypes[] types, string[] names, string help) =>
		FutureProg.RegisterBuiltInFunctionCompiler(new(name, types, (p, w) => new WeaponPoisonFunction(p, w, name), names,
			names.Select(x => x).ToList(), help, "Items", name == "actioncheck" ? ProgVariableTypes.Number : ProgVariableTypes.Boolean));
}
