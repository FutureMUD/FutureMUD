#nullable enable

using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;
using GameItemProto = MudSharp.Models.GameItemProto;
using CultureInfo = System.Globalization.CultureInfo;
using WeaponType = MudSharp.Models.WeaponType;
using WeaponAttack = MudSharp.Models.WeaponAttack;
using CombatMessage = MudSharp.Models.CombatMessage;

internal static class FlameKnifeFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var token = Path.GetFileName(Path.GetDirectoryName(receipt))!.Split('-').Last();
		var suffix = string.Concat(token.Select(c => (char)('a' + int.Parse(c.ToString(), NumberStyles.HexNumber))));
		var seed = db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.UniqueName == "QA N18 Effigy");
		var hold = db.GameItemComponentProtos.Include(x => x.EditableItem)
			.First(x => x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var mass = 1.0 / double.Parse(db.StaticConfigurations.Single(x => x.SettingName == "BaseWeightUOMToKilograms").Definition, CultureInfo.InvariantCulture);
		Tag Tag(string label, long? parent = null)
		{
			var tag = new Tag { Name = "QA N21 " + token + " " + label, ParentId = parent };
			db.Tags.Add(tag); db.SaveChanges(); return tag;
		}
		var six = Tag("Conjuration six"); var seven = Tag("Conjuration seven", six.Id);
		var low = Tag("Conjuration five"); var wrong = Tag("Creation six");
		var weaponType = new WeaponType
		{
			Name = "QA N21 " + token + " burning weapon", Classification = (int)WeaponClassification.Lethal,
			AttackTraitId = 297, ParryTraitId = 297, Reach = 1, StaminaPerParry = 1
		};
		db.WeaponTypes.Add(weaponType); db.SaveChanges();
		var expressions = new[] { "6", "4", "2" }.Select((formula, index) => new TraitExpression
			{ Name = $"QA N21 {token} burning channel {index}", Expression = formula }).ToArray();
		db.TraitExpressions.AddRange(expressions); db.SaveChanges();
		var attack = new WeaponAttack
		{
			Name = "QA N21 " + token + " burning thrust", WeaponTypeId = weaponType.Id,
			MoveType = (int)BuiltInCombatMoveType.UseWeaponAttack, DamageType = (int)DamageType.Burning,
			DamageExpressionId = expressions[0].Id, PainExpressionId = expressions[1].Id, StunExpressionId = expressions[2].Id,
			BaseAttackerDifficulty = (int)Difficulty.Automatic, BaseBlockDifficulty = (int)Difficulty.Impossible,
			BaseParryDifficulty = (int)Difficulty.Impossible, BaseDodgeDifficulty = (int)Difficulty.Impossible,
			BaseAngleOfIncidence = Math.PI / 2, RecoveryDifficultySuccess = (int)Difficulty.Normal,
			RecoveryDifficultyFailure = (int)Difficulty.Normal, BaseDelay = 1, Weighting = 1, MaximumTargets = 1,
			StaminaCost = 1, ExertionLevel = (int)ExertionLevel.Heavy, Verb = (int)MeleeWeaponVerb.Stab,
			Orientation = (int)Orientation.Centre, Alignment = (int)Alignment.FrontRight,
			HandednessOptions = (int)AttackHandednessOptions.OneHandedOnly,
			Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound),
			RequiredPositionStateIds = PositionStanding.Instance.Id.ToString(), AdditionalInfo = ""
		};
		db.WeaponAttacks.Add(attack); db.SaveChanges();
		var message = new CombatMessage { Type = attack.MoveType, Chance = 1, Priority = 100,
			Message = "$0 thrust|thrusts $2 at $1", FailureMessage = "$0 thrust|thrusts $2 at $1" };
		message.CombatMessagesWeaponAttacks.Add(new() { WeaponAttackId = attack.Id });
		db.CombatMessages.Add(message); db.SaveChanges();
		GameItemComponentProto Component(string type, XElement definition)
		{
			var component = new GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1,
				RevisionNumber = 0, Name = "QA N21 " + token + " " + type, Description = "Disposable N21 qualification fixture.",
				Type = type, Definition = definition.ToString(), EditableItem = Copy(seed.EditableItem) };
			component.EditableItem.Id = 0; db.GameItemComponentProtos.Add(component); db.SaveChanges(); return component;
		}
		var weapon = Component("MeleeWeapon", new XElement("Definition", new XElement("WeaponType", weaponType.Id),
			new XElement("CanWieldProg", 0), new XElement("WhyCannotWieldProg", 0)));
		var container = Component("Container", new XElement("Definition", new XAttribute("Weight", 100 * mass),
			new XAttribute("MaxSize", (int)SizeCategory.Normal), new XAttribute("Closable", true),
			new XAttribute("Transparent", true), new XAttribute("OnceOnly", false), new XAttribute("Preposition", "in")));
		var prototypes = new Dictionary<string, long>();
		GameItemProto Item(string label, IReadOnlyList<GameItemComponentProto> components, long? tag = null)
		{
			var item = Copy(seed); item.Id = db.GameItemProtos.Max(x => x.Id) + 1; item.RevisionNumber = 0;
			item.Name = "QA N21 " + token + " " + label; item.UniqueName = item.Name; item.Keywords = "qanflame" + label + suffix;
			item.ShortDescription = "a " + item.Keywords + " fixture"; item.FullDescription = "An explicitly authored disposable N21 native profile.";
			item.Weight = .1 * mass; item.Size = (int)SizeCategory.Small;
			item.EditableItemId = 0; item.EditableItem = Copy(seed.EditableItem); item.EditableItem.Id = 0;
			foreach (var c in components) item.GameItemProtosGameItemComponentProtos.Add(new()
				{ GameItemComponentProtoId = c.Id, GameItemComponentRevision = c.RevisionNumber, GameItemProto = item });
			if (tag is { } tagId) item.GameItemProtosTags.Add(new() { TagId = tagId });
			db.GameItemProtos.Add(item); db.SaveChanges(); prototypes.Add(label, item.Id); return item;
		}
		var scrap = Item("scrap", [hold]);
		var salvage = Component("Salvageable", new XElement("Definition", new XElement("Trait", 297),
			new XElement("Difficulty", (int)Difficulty.Automatic), new XElement("ToolTag", 0),
			new XElement("Stages", new XElement("Stage", new XAttribute("delay", 1), "$0 take|takes $1 apart.")),
			new XElement("ItemProducts", new XElement("Product", new XAttribute("id", scrap.Id), new XAttribute("revision", 0),
				new XAttribute("successQuantity", 1), new XAttribute("failureQuantity", 1),
				new XAttribute("successChance", 1), new XAttribute("failureChance", 1)))));
		var knifeLabels = new[] { "weka", "yuqa", "kral", "een", "pav", "sul" };
		var staffLabels = new[] { "staffa", "staffb", "staffc", "staffd", "staffe", "stafff", "staffg", "staffh" };
		// Tagging temporary knives as otherwise valid casting material proves the origin guard,
		// rather than simply relying on an unsuitable component tag.
		foreach (var label in knifeLabels) Item(label, [hold, weapon, salvage], six.Id);
		foreach (var label in staffLabels) Item(label, [hold, weapon, salvage]);
		Item("six", [hold], six.Id); Item("seven", [hold], seven.Id); Item("low", [hold], low.Id);
		Item("wrong", [hold], wrong.Id); Item("foreign", [hold]); Item("bag", [hold, container]);
		var actor = db.Characters.Single(x => x.Id == 1);
		var setting = Copy(db.CharacterCombatSettings.Single(x => x.Id == actor.CurrentCombatSettingId));
		setting.Id = 0; setting.Name = "QA N21 " + token + " armed"; setting.CharacterOwnerId = 1;
		setting.GlobalTemplate = false; setting.WeaponUsePercentage = 1; setting.NaturalWeaponPercentage = 0;
		setting.MagicUsePercentage = 0; setting.PsychicUsePercentage = 0; setting.AuxiliaryPercentage = 0;
		setting.PreferToFightArmed = true; setting.FallbackToUnarmedIfNoWeapon = false;
		setting.AttackHelpless = true; setting.AttackUnarmed = true; setting.AttackCriticallyInjured = true;
		setting.PreferredMeleeMode = (int)CombatStrategyMode.StandardMelee;
		db.CharacterCombatSettings.Add(setting); db.SaveChanges();
		transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Prototypes = prototypes,
			Knives = knifeLabels.Select(x => prototypes[x]).ToArray(), Staffs = staffLabels.Select(x => prototypes[x]).ToArray(),
			Keywords = prototypes.Keys.ToDictionary(x => x, x => "qanflame" + x + suffix),
			ComponentTag = six.Id, CombatSetting = setting.Id, OriginalCombatSetting = actor.CurrentCombatSettingId,
			Modes = new { Permanent = (int)SpellLifecycleMode.Permanent, Temporary = (int)SpellLifecycleMode.TemporaryCleanup },
			States = new { Active = (int)SpellLifecycleState.Active, Completed = (int)SpellLifecycleState.Completed },
			ItemKind = (int)SpellOwnedEntityKind.GameItem, BurningDamageType = (int)DamageType.Burning,
			ExpiryReason = (int)SpellRetirementReason.Expiry,
			WeaponType = weaponType.Id, Attack = attack.Id, Profile = new { Damage = 6, Pain = 4, Stun = 2 } },
			new JsonSerializerOptions { WriteIndented = true }));
	}
	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string)))) p.SetValue(row, p.GetValue(source));
		return row;
	}
}
