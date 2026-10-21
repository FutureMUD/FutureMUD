#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit native adaptations for the two source families without stock weapon consumers.</summary>
internal static class ArmageddonWeaponFamilies
{
	internal static void InstallFresh(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		var families = new[] { (Name: "Razor Weapon", Donor: "Longsword", Key: "razor_weapons"), (Name: "Trident", Donor: "Short Spear", Key: "trident_weapons") };
		foreach (var family in families)
		{
			if (db.WeaponTypes.Any(x => x.Name == family.Name) || db.GameItemComponentProtos.Any(x => x.Name == "Melee_" + family.Name))
				throw new InvalidOperationException($"Unowned Armageddon weapon family collision: {family.Name}.");
		}
		var account = db.Accounts.First();
		foreach (var family in families)
		{
			var donor = db.WeaponTypes.Include(x => x.WeaponAttacks).ThenInclude(x => x.CombatMessagesWeaponAttacks).Single(x => x.Name == family.Donor);
			var weapon = (WeaponType)db.Entry(donor).CurrentValues.Clone().ToObject();
			weapon.Id = 0; weapon.Name = family.Name;
			weapon.AttackTraitId = skills["arm.skill." + family.Key].Id;
			weapon.ParryTraitId = skills["arm.skill.parry"].Id;
			db.WeaponTypes.Add(weapon); db.SaveChanges();
			// Separate rows retain the native donor geometry/tuning. Opening attacks have their own proficiency contract.
			foreach (var source in donor.WeaponAttacks.Where(x => x.MoveType != (int)BuiltInCombatMoveType.BackstabAttack && x.MoveType != (int)BuiltInCombatMoveType.BackstabAttackUnarmed).ToArray())
			{
				var attack = (WeaponAttack)db.Entry(source).CurrentValues.Clone().ToObject();
				attack.Id = 0; attack.Name = family.Name + ": " + source.Name; attack.WeaponTypeId = weapon.Id;
				db.WeaponAttacks.Add(attack); db.SaveChanges();
				foreach (var message in source.CombatMessagesWeaponAttacks)
					db.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks { WeaponAttackId = attack.Id, CombatMessageId = message.CombatMessageId });
				ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.family." + family.Key + ".attack." + source.Id, attack,
					nameof(WeaponAttack.WeaponTypeId), nameof(WeaponAttack.Name), nameof(WeaponAttack.DamageExpressionId), nameof(WeaponAttack.StunExpressionId), nameof(WeaponAttack.PainExpressionId));
			}
			var now = DateTime.UtcNow;
			var component = new GameItemComponentProto
			{
				Id = db.GameItemComponentProtos.Select(x => (long?)x.Id).Max().GetValueOrDefault() + 1, RevisionNumber = 0,
				Type = "MeleeWeapon", Name = "Melee_" + family.Name, Description = "Turns an item into a " + family.Name + " melee weapon",
				Definition = $"<Definition><WeaponType>{weapon.Id}</WeaponType></Definition>",
				EditableItem = new EditableItem { RevisionNumber = 0, RevisionStatus = 4, BuilderAccountId = account.Id,
					BuilderDate = now, ReviewerAccountId = account.Id, ReviewerDate = now, BuilderComment = "Armageddon native family adaptation", ReviewerComment = "Armageddon native family adaptation" }
			};
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			var record = ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.family." + family.Key + ".component", component,
				nameof(GameItemComponentProto.Type), nameof(GameItemComponentProto.Name), nameof(GameItemComponentProto.Definition));
			record.RevisionNumber = component.RevisionNumber;
		}
		db.SaveChanges();
	}
}
