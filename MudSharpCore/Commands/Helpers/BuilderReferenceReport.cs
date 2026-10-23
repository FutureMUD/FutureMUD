#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Combat;
using MudSharp.Economy;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Commands.Helpers;

internal sealed record BuilderReference(string Source, string Field);

/// <summary>Explicit typed edges. Absence of an edge never authorises deletion.</summary>
internal static class BuilderReferenceReport
{
	public static IReadOnlyList<BuilderReference> Find(IFuturemud world, IFrameworkItem target)
	{
		var result = new List<BuilderReference>();
		void Add(IFrameworkItem source, string field) => result.Add(new(BuilderRevisionDiff.Reference(source), field));
		switch (target)
		{
			case IWeaponAttack attack:
				foreach (var type in world.WeaponTypes.Where(x => x.Attacks.Any(a => a.Id == attack.Id))) Add(type, "Attacks");
				foreach (var race in world.Races.Where(x => x.NaturalWeaponAttacks.Any(a => a.Attack.Id == attack.Id))) Add(race, "Natural attacks (including inherited)");
				foreach (var message in world.CombatMessageManager.CombatMessages.Where(x => x.WeaponAttackIds.Contains(attack.Id))) Add(message, "Weapon attack IDs");
				foreach (var command in world.ManualCombatCommands.Where(x => x.WeaponAttack?.Id == attack.Id)) Add(command, "Weapon attack");
				break;
			case ITraitExpression expression:
				foreach (var weapon in world.WeaponAttacks)
				{
					if (weapon.Profile?.DamageExpression?.Id == expression.Id) Add(weapon, "Damage expression");
					if (weapon.Profile?.StunExpression?.Id == expression.Id) Add(weapon, "Stun expression");
					if (weapon.Profile?.PainExpression?.Id == expression.Id) Add(weapon, "Pain expression");
				}
				foreach (var trait in world.Traits.OfType<DerivedTraitDefinition>().Where(x => x.Expression?.Id == expression.Id)) Add(trait, "Derived expression");
				foreach (var trait in world.Traits.OfType<SkillDefinition>().Where(x => x.Cap?.Id == expression.Id)) Add(trait, "Skill cap");
				break;
			case IFutureProg prog:
				foreach (var hook in world.Hooks.OfType<IHookWithProgs>().Where(x => x.FutureProgs.Any(p => p.Id == prog.Id))) Add(hook, "FutureProgs");
				foreach (var attack in world.WeaponAttacks)
				{
					if (attack.UsabilityProg?.Id == prog.Id) Add(attack, "Usability prog");
					if (attack.OnUseAttackProg?.Id == prog.Id) Add(attack, "On-use prog");
				}
				foreach (var type in world.BankAccountTypes.OfType<MudSharp.Economy.Banking.BankAccountType>().Where(x => x.CanCloseAccountProg?.Id == prog.Id)) Add(type, "Close-account prog");
				foreach (var help in world.Helpfiles)
				{
					if (help.Rule?.Id == prog.Id) Add(help, "Visibility prog");
					var index = 0;
					foreach (var extra in help.AdditionalTexts)
					{
						index++;
						if (extra.Item1?.Id == prog.Id) Add(help, $"Extra text {index} prog");
					}
				}
				break;
			case IBankAccountType type:
				foreach (var account in world.BankAccounts.Where(x => x.BankAccountType.Id == type.Id)) Add(account, "Account type");
				break;
			case IGameItemProto prototype:
				foreach (var skin in world.ItemSkins.Where(x => x.ItemProto.Id == prototype.Id)) Add(skin, "Item prototype");
				foreach (var item in world.Items.Where(x => x.Prototype.Id == prototype.Id)) Add(item, "Loaded item prototype");
				break;
			case IGameItemSkin skin:
				foreach (var item in world.Items.Where(x => x.Skin?.Id == skin.Id)) Add(item, "Loaded item skin");
				break;
		}
		return result.OrderBy(x => x.Source).ThenBy(x => x.Field).ToList();
	}

	public static string Describe(IFuturemud world, IFrameworkItem target)
	{
		var coverage = target switch
		{
			IWeaponAttack => "weapon types, racial natural attacks (including inherited), combat messages and manual combat commands",
			ITraitExpression => "weapon damage/stun/pain profiles, derived traits and skill caps",
			IFutureProg => "hook prog lists, weapon attack progs, bank account closing progs and help visibility/extra-text progs",
			IBankAccountType => "loaded bank accounts",
			IGameItemProto => "skin definitions and loaded items (all prototype revisions by logical ID)",
			IGameItemSkin => "loaded items (all skin revisions by logical ID)",
			_ => "no provider registered for this type"
		};
		var references = Find(world, target);
		return $"Where used: {BuilderRevisionDiff.Reference(target).ColourName()}\nCoverage: {coverage}.\n" +
		       $"{references.Count} known references:\n" + string.Join("\n", references.Select(x => $"  {x.Source}: {x.Field}")) +
		       "\nPartial report: other persisted, unloaded or serialized consumers may exist. Zero known references does not establish safe deletion; unsupported deletion remains refused.";
	}
}
