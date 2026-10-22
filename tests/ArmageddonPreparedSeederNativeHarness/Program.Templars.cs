#nullable enable

using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Construction;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using Moq;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.PerceptionEngine.Handlers;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record TemplarConfiguration(Dictionary<long, string> Progs, long Capability, string Definition);
	private sealed record TemplarNativeReceipt(string Database, DateTime Now, long Actor, long Capability,
		long Resource, long Medallion, long[] Selections, int Charges, double Reserve, PokeNativeReceipt[] Pokes);
	private sealed record PokeNativeReceipt(long Target, string Location, double Seconds, long? Limb = null);

	private static TemplarConfiguration SeedSettingTemplarFixtures(TestDatabase database, long territoryRoom)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		Require(TestDatabase.HasOwnedPrefix(database.Name), "Templar qualification requires an owned disposable database.");
		var ids = SettingQualificationIds(database);
		var keys = new[] { "arm.class.jihaen_templar.enabled", "arm.class.lirathan_templar.enabled", "arm.class.southern_templar.enabled",
			"arm.templar.southern.military_authority", "arm.templar.southern.territory" };
		// Resolve the territory through the authored patron policy rather than guessing its stable key.
		var cap = db.MagicCapabilities.Single(x => x.Id == ids["arm.capability.southern_templar"]);
		var definition = XElement.Parse(cap.Definition);
		var patron = JsonSerializer.Deserialize<PatronCastingPolicy>(definition.Element("Casting")!.Element("Patron")!.Value)!;
		var progIds = keys.Take(4).Select(x => ids[x]).Append(patron.TerritoryProgId).ToArray();
		var progs = db.FutureProgs.Where(x => Enumerable.Contains(progIds, x.Id)).ToArray();
		Require(progs.Length == 5 && progs.All(x => x.FunctionText.Trim() == "return false"), "Templar activation/authority/territory must default closed.");
		var original = new TemplarConfiguration(progs.ToDictionary(x => x.Id, x => x.FunctionText), cap.Id, cap.Definition);
		foreach (var prog in progs) prog.FunctionText = prog.Id == patron.TerritoryProgId
			? $"return @ch.Location.Id == {territoryRoom}"
			: prog.Id == ids["arm.templar.southern.military_authority"]
				? $"return Contains(magiccapabilities(@ch), ToMagicCapability(\"{cap.Id}\"))" : "return true";
		Require((bool?)definition.Element("Casting")!.Attribute("enabled") == false, "Southern casting must default disabled.");
		definition.Element("Casting")!.SetAttributeValue("enabled", true);
		cap.Definition = definition.ToString(); db.SaveChanges();
		return original;
	}

	private static void RestoreSettingTemplarFixtures(TestDatabase database, TemplarConfiguration original)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		foreach (var (id, text) in original.Progs) db.FutureProgs.Single(x => x.Id == id).FunctionText = text;
		db.MagicCapabilities.Single(x => x.Id == original.Capability).Definition = original.Definition;
		db.SaveChanges();
	}

	private static TemplarNativeReceipt QualifySettingTemplars(TestDatabase database, IFuturemud world, long accountId,
		long cultureId, IReadOnlyDictionary<string, long> ids, long inside, long outside)
	{
		foreach (var id in ids.Where(x => x.Key.StartsWith("arm.templar.", StringComparison.Ordinal) && world.FutureProgs.Get(x.Value) is not null).Select(x => x.Value))
		{
			var prog = world.FutureProgs.Get(id);
			Require(prog.Compile(), "Templar prog did not compile: " + prog.FunctionName + " " + prog.CompileError);
		}
		ICharacter Create(string? key, string name, Gender gender = Gender.Male)
		{
			var app = NewSettingQualificationApplication(world, accountId, cultureId, name, gender: gender);
			app.StartingLocation = new StartingLocation { Name = "Templar qualification", Blurb = "Disposable fixture",
				Location = world.Rooms.Get(inside), Role = app.StartingLocation.Role };
			if (key is not null) app.SelectedRoles.Add(world.Roles.Get(ids[key]));
			if (key == "arm.class.southern_templar") app.SelectedRoles.Add(world.Roles.Get(ids["arm.subclass.acrobat"]));
			FinaliseAndSaveSettingApplication(app, SettingQualificationFreeProg(world));
			var actor = PersistSettingQualificationCharacter(world, app);
			if (!actor.Location.Characters.Contains(actor)) actor.Location.Enter(actor);
			return actor;
		}
		var southern = Create("arm.class.southern_templar", "QualificationSouthern");
		var kick = world.Traits.Get(ids["arm.skill.kick"]);
		var primaryKick = ArmageddonMundaneProgression.Roles.Single(x => x.Name == "Warrior" && x.Primary).Skills.Single(x => x.Key == "arm.skill.kick");
		var secondaryKick = ArmageddonMundaneProgression.Roles.Single(x => x.Name == "Acrobat" && !x.Primary).Skills.Single(x => x.Key == "arm.skill.kick");
		Require(southern.TraitRawValue(kick) == Math.Min(primaryKick.Cap, primaryKick.Opening + secondaryKick.Bonus) && southern.TraitMaxValue(kick) == primaryKick.Cap,
			"Native Templar/subclass combination lost its primary cap or secondary opening bonus.");
		var lirathan = Create("arm.class.lirathan_templar", "QualificationLirathan", Gender.Female);
		Require(lirathan.Capabilities.Any(x => x.Id == ids["arm.capability.lirathan_templar_psionics"]), "Native Lirathan role did not grant its capability.");
		var cap = (IMagicCastingCapability)world.MagicCapabilities.Get(ids["arm.capability.southern_templar"]);
		Require(southern.Capabilities.Any(x => x.Id == cap.Id) && cap.CastingPolicy is { Enabled: true, Patron: not null }, "Native Southern role did not grant its casting route.");
		var language = world.Languages.Get(cap.CastingPolicy!.Patron!.Incantations!.Values.First().LanguageId);
		if (!southern.HasTrait(language.LinkedTrait)) southern.AddTrait(language.LinkedTrait, 200);
		else southern.SetTraitValue(language.LinkedTrait, 200);
		southern.CurrentLanguage = language; southern.CurrentAccent = language.Accents.First();
		var speech = new StringBuilder(); southern.Register(new PlayerOutputHandler(speech, southern));
		var invocation = new MagicCastingIntent(southern, cap.Id, ids["arm.spell.sense_enchantment"], 7, false, "self");
		var quote = world.MagicCasting.Quote(invocation);
		Require(quote.Invocation?.Delivery?.Method == "Shout", "Seeded Southern route could not resolve native automatic shouting: " + quote.Reason);
		var cast = world.MagicCasting.Cast(invocation);
		Require(cast.Status is MagicCastingStatus.Succeeded or MagicCastingStatus.Failed, "Seeded Southern cast did not complete: " + cast.Message);
		Require(System.Text.RegularExpressions.Regex.Matches(speech.ToString(), "mon granted templar", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Count == 1,
			"Southern invocation did not shout exactly one generated formula: " + speech);
		IGameItem Item(string key, ICharacter holder, bool wear = false)
		{
			var item = world.ItemProtos.Get(ids[key]).CreateNew(holder);
			world.Add(item); item.Login(); item.HandleEvent(EventType.ItemFinishedLoading, item);
			holder.Location.Insert(item, true);
			Require(holder.Body.CanGet(item, 0), "Cannot get seeded Templar item: " + key);
			holder.Body.Get(item, silent: true);
			if (wear) { Require(holder.Body.CanWear(item), "Cannot wear seeded Templar item: " + key); holder.Body.Wear(item, silent: true); }
			return item;
		}
		var shrineItem = Item("arm.templar.southern.shrine", southern);
		var shrine = shrineItem.GetItemType<ISpellSelectionShrine>();
		var policy = ((MudSharp.GameItems.Prototypes.ConditionalMagicItemGameItemComponentProto)shrine.Prototype).Policy;
		var selected = policy.Choices.Take(3).Select(x => x.SpellId).ToArray();
		Require(shrine.SelectSpells(southern, selected, out var reason), "Native shrine selection refused: " + reason);
		Require(PatronMagicService.Selections(southern, cap.Id).Order().SequenceEqual(selected.Order()), "Native shrine did not persist selections.");
		Require(!shrine.SelectSpells(southern, [selected[0], selected[0]], out _), "Shrine accepted duplicates.");
		Require(!shrine.SelectSpells(lirathan, selected, out _), "Shrine granted a foreign capability.");
		selected = policy.Choices.Skip(3).Take(3).Select(x => x.SpellId).ToArray();
		Require(shrine.SelectSpells(southern, selected, out reason), "Native shrine replacement refused: " + reason);
		var resource = world.MagicResources.Get(ids["arm.resource.southern_templar"]);
		Require(southern.MagicResourceAmounts.GetValueOrDefault(resource) == 100, "Territory did not refill native reserve.");
		var bankItem = Item("arm.templar.southern.regen_medallion", southern);
		var bank = bankItem.GetItemType<ResourceRestorerGameItemComponent>();
		Require(bank.Charges == 0 && bank.Use(southern, "recharge", new StringStack(""), out reason) && bank.Charges == 6, "Native medallion recharge failed: " + reason);
		Require(!bank.Use(southern, "use", new StringStack(""), out _) && bank.Charges == 6, "Full reserve consumed a charge.");
		southern.Teleport(world.Rooms.Get(outside), RoomLayer.GroundLevel, false, false);
		southern.UseResource(resource, 60);
		Require(southern.MagicResourceAmounts.GetValueOrDefault(resource) == 40, "Outside reserve unexpectedly regenerated.");
		Require(bank.Use(southern, "use", new StringStack(""), out reason) && bank.Charges == 5 && southern.MagicResourceAmounts.GetValueOrDefault(resource) == 100,
			"Native medallion did not conserve one charge for a full refill: " + reason);
		Require(!bank.Use(southern, "recharge", new StringStack(""), out _), "Medallion recharged outside its territory.");
		// Free the hands used by the shrine/bank, then exercise the actual seeded issuer.
		southern.Body.Drop(shrineItem, silent: true); southern.Body.Drop(bankItem, silent: true);
		var robe = Item("arm.templar.southern.regalia", southern, wear: true);
		var issuer = robe.GetItemType<ItemIssuerGameItemComponent>();
		Require(issuer.Use(southern, "issue", new StringStack(ids["arm.templar.southern.gem"].ToString()), out reason), "Seeded gem issuer failed: " + reason);
		var issued = southern.Body.HeldItems.Single(x => x.Prototype.Id == ids["arm.templar.southern.gem"]);
		Require(world.Items.Contains(issued), "Issued gem was not registered with the world.");
		var elementalRole = new ArmageddonStockProgression(ArmageddonProgressionMode.Simple).Capabilities.Single(x => x.Key == "arm.capability.fire").RoleKey;
		var wearer = Create(elementalRole, "QualificationGemmed");
		southern.Body.Drop(issued, silent: true);
		wearer.Teleport(southern.Location, southern.RoomLayer, false, false);
		Require(wearer.Body.CanGet(issued, 0), "Cannot take issued gem."); wearer.Body.Get(issued, silent: true);
		Require(wearer.Body.CanWear(issued), "Cannot wear issued gem."); wearer.Body.Wear(issued, silent: true);
		var binding = issued.GetItemType<IConditionalBindingItem>();
		Require(binding.IsBinding(wearer), "Actual worn elemental gem was not recognised.");
		Require(!binding.Activate(lirathan, wearer, out _), "Foreign class exercised gem authority.");
		var damage = wearer.Body.Wounds.Sum(x => x.CurrentDamage); var stun = wearer.Body.Wounds.Sum(x => x.CurrentStun); var stamina = wearer.CurrentStamina;
		Require(binding.Activate(southern, wearer, out reason), "Seeded gem control failed: " + reason);
		Require(wearer.Body.Wounds.Sum(x => x.CurrentDamage) > damage && wearer.Body.Wounds.Sum(x => x.CurrentStun) > stun &&
			wearer.CurrentStamina < stamina && wearer.PositionState == PositionSprawled.Instance,
			$"Gem control omitted native bodily consequences: damage {damage}->{wearer.Body.Wounds.Sum(x => x.CurrentDamage)}, stun {stun}->{wearer.Body.Wounds.Sum(x => x.CurrentStun)}, stamina {stamina}->{wearer.CurrentStamina}, position {wearer.PositionState.Name}.");
		damage = wearer.Body.Wounds.Sum(x => x.CurrentDamage);
		Require(!wearer.Body.CanRemoveItem(issued) && wearer.Body.Wounds.Sum(x => x.CurrentDamage) == damage, "Removal query mutated or admitted bound gem.");
		wearer.Body.RemoveItem(issued, playerEmote: null!, silent: true);
		Require(wearer.Body.DirectWornItems.Contains(issued) && wearer.Body.Wounds.Sum(x => x.CurrentDamage) > damage, "Removal attempt detached gem or omitted punishment.");
		Require(!binding.InterceptSpell(wearer, world.MagicSpells.Get(ids["arm.spell.mend_flesh"]), [southern]), "Gem blocked an unlisted beneficial spell.");
		Require(!binding.InterceptSpell(wearer, world.MagicSpells.Get(ids["arm.spell.ember_lance"]), [lirathan]), "Gem blocked a spell against an unprotected target.");
		Require(binding.InterceptSpell(wearer, world.MagicSpells.Get(ids["arm.spell.ember_lance"]), [southern]), "Gem did not intercept listed harm against a protected target.");
		var defilerRole = new ArmageddonStockProgression(ArmageddonProgressionMode.Simple).Capabilities.Single(x => x.Key == "arm.capability.defiler").RoleKey;
		var defiler = Create(defilerRole, "QualificationRejectedGem");
		Require(defiler.Capabilities.Any(x => x.Id == ids["arm.capability.defiler"]), "Defiler fixture did not receive its actual capability.");
		var rejected = Item("arm.templar.southern.gem", defiler, wear: true);
		Require(rejected.Deleted && (defiler.State.IsDead() || defiler.HealthStrategy.MaxHP(defiler) - defiler.Body.Wounds.Sum(x => x.CurrentDamage) <= 0),
			$"Defiler wear did not destroy the gem and apply the native incompatible-wear consequence: deleted={rejected.Deleted}, state={defiler.State}, damage={defiler.Body.Wounds.Sum(x => x.CurrentDamage)}, maxHP={defiler.HealthStrategy.MaxHP(defiler)}, worn={defiler.Body.DirectWornItems.Contains(rejected)}.");
		// Return the bank to native held custody for a cold conservation check.
		Require(southern.Body.CanGet(bankItem, 0), "Cannot retrieve bank after gem issuance."); southern.Body.Get(bankItem, silent: true);
		var pokes = new List<PokeNativeReceipt>();
		var pokeCommand = world.ManualCombatCommands.Single(x => x.Id == ids["arm.templar.jihaen.pressure_point.command"]);
		foreach (var location in new[] { "neck", "spine", "throat", "stomach", "leftarm", "rightarm", "leftleg", "rightleg" })
		{
			var actor = Create("arm.class.jihaen_templar", "QualificationPoke" + location);
			var target = Create(null, "QualificationPokeTarget" + location);
			var skill = world.Traits.Get(ids["arm.skill.pressure_points"]);
			Require(actor.HasTrait(skill), "Jihaen role did not grant Pressure Points."); actor.SetTraitValue(skill, 80);
			Require(pokeCommand.IsUsableBy(actor, target), $"Seeded poke gate refused: enabled={world.FutureProgs.Get(ids["arm.class.jihaen_templar.enabled"]).ExecuteBool()}, roles={string.Join(',', actor.Roles.Select(x => x.Id))}, requiredRole={ids["arm.class.jihaen_templar"]}, trait={actor.TraitValue(skill)}, raw={actor.TraitRawValue(skill)}, prog={pokeCommand.UsabilityProg.FunctionText}.");
			Require(actor.Engage(target, false), "Native poke could not engage."); actor.MeleeRange = target.MeleeRange = true;
			var contextual = pokeCommand.AuxiliaryAction.AuxiliaryEffects.OfType<IContextualAuxiliaryEffect>().Single();
			Require(contextual.TryParseContext(actor, target, new StringStack(location), out var context, out var error), "Poke context refused: " + error);
			if (location is "leftarm" or "stomach") context = context! with { Selection = location.ToUpperInvariant() };
			var limb = target.Body.Limbs.FirstOrDefault(x => x.Id == context!.TargetLimbId);
			IGameItem[] held = [];
			if (limb?.LimbType == LimbType.Arm)
			{
				held = [Item("arm.templar.southern.regen_medallion", target), Item("arm.templar.southern.regen_medallion", target)];
				Require(limb.Parts.SelectMany(target.Body.HeldOrWieldedItemsFor).Any(), "Selected arm has no held disarm fixture.");
			}
			bool Applied() => PokeEffect(target, location) is not null;
			for (var attempt = 0; attempt < 8 && !Applied(); attempt++)
			{
				actor.SetPosition(PositionStanding.Instance, PositionModifier.None, null, null); target.SetPosition(PositionStanding.Instance, PositionModifier.None, null, null);
				((Body)actor.Body).CurrentStamina = actor.MaximumStamina; actor.OffensiveAdvantage = 1000;
				var resolved = ManualCombatCommandResolver.TryResolve(actor, pokeCommand, target, context: context);
				Require(resolved.Success, "Native poke selection failed: " + resolved.Error);
				stamina = actor.CurrentStamina; actor.Combat!.CombatAction(actor, resolved.Move);
				Require(actor.CurrentStamina < stamina, "Native poke omitted stamina cost.");
			}
			Require(Applied(), "Native poke failed to apply " + location + " within eight opposed attempts.");
			var effect = PokeEffect(target, location)!;
			var seconds = location == "stomach" ? 45 : location == "spine" || limb is not null ? 5 : 20;
			Require(world.EffectScheduler.RemainingDuration(effect) == TimeSpan.FromSeconds(seconds), "Native poke duration differs.");
			if (limb is not null)
			{
				Require(target.Body.CanUseLimb(limb).HasFlag(CanUseLimbResult.CantUseNumbness), "Numb limb remained usable.");
				Require(target.Body.BodypartsForLimb(limb).All(x => target.Body.CanUseBodypart(x) != CanUseBodypartResult.CanUse), "Numb limb allowed a bodypart action.");
				if (limb.LimbType == LimbType.Arm)
					Require(held.Any(x => x.InInventoryOf is null && x.Location == target.Location), "Numb arm did not release its item.");
				else
					Require(target.Body.GetLegInformation(false).NonWorkingLegs.Contains(limb), "Numb leg remained a working movement limb.");
			}
			if (location == "stomach")
				Require(target.Body.EffectsOfType<Nausea>().Single().CheckBonus < -6.0, "Stomach poke did not immediately feed native nausea.");
			world.EffectScheduler.Reschedule(effect, TimeSpan.FromSeconds(2));
			contextual.ApplyEffect(actor, target, null!, context!, opposedSuccess: true);
			Require(world.EffectScheduler.RemainingDuration(effect) == TimeSpan.FromSeconds(2), "Repeated poke refreshed the same status.");
			world.EffectScheduler.Reschedule(effect, TimeSpan.FromSeconds(seconds));
			pokes.Add(new(target.Id, location, seconds, limb?.Id)); actor.Combat?.EndCombat(false);
		}
		world.SaveManager.Flush();
		Require(bankItem.Id > 0, "Native medallion was not persisted.");
		Console.WriteLine("TEMPLAR-native=PASS real-role-capabilities shrine-selection replacement territory-refill six-charge-bank outside-debit gem-issue-wear-control-removal eight-native-poke-contexts-grip-movement-nausea-nonrefresh");
		return new(database.Name, RuntimeClock.UtcNow, southern.Id, cap.Id, resource.Id, bankItem.Id, selected, 5, 100, pokes.ToArray());
	}

	private static IEffect? PokeEffect(ICharacter target, string location) => location switch
	{
		"neck" => target.EffectsOfType<MundaneTraitPenaltyEffect>().SingleOrDefault(),
		"spine" => target.EffectsOfType<MundaneParalysisEffect>().SingleOrDefault(),
		"throat" => target.EffectsOfType<MundaneSilenceEffect>().SingleOrDefault(),
		"stomach" => target.Body.EffectsOfType<MundaneNauseaEffect>().SingleOrDefault(),
		_ => target.Body.EffectsOfType<LimbNumbnessEffect>().SingleOrDefault()
	};

	private static int ReadSettingTemplarQualification(string encoded)
	{
		var input = JsonSerializer.Deserialize<TemplarNativeReceipt>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		using var world = LoadProductionRestartWorld(clock); using var session = new FMDB();
		((IFuturemudLoader)world).LoadChargen();
		var actor = world.TryGetCharacter(input.Actor, true); world.Add(actor, false); actor.Register(null); actor.Location.Login(actor);
		Require(actor.Capabilities.Any(x => x.Id == input.Capability), "Cold load lost the Southern capability.");
		Require(PatronMagicService.Selections(actor, input.Capability).Order().SequenceEqual(input.Selections.Order()), "Cold load lost shrine selections.");
		var item = world.TryGetItem(input.Medallion, true);
		Require(item.InInventoryOf == actor.Body && item.GetItemType<ResourceRestorerGameItemComponent>().Charges == input.Charges, "Cold load lost medallion custody or charges.");
		Require(actor.MagicResourceAmounts.GetValueOrDefault(world.MagicResources.Get(input.Resource)) == input.Reserve, "Cold load lost reserve/charge conservation.");
		foreach (var poke in input.Pokes)
		{
			var target = world.TryGetCharacter(poke.Target, true); world.Add(target, false); target.Register(null); target.Location.Login(target);
			var effect = PokeEffect(target, poke.Location)!;
			Require(effect is not null && world.EffectScheduler.RemainingDuration(effect) == TimeSpan.FromSeconds(poke.Seconds), "Cold poke duration changed.");
			if (poke.Limb.HasValue)
			{
				var limb = target.Body.Limbs.Single(x => x.Id == poke.Limb);
				Require(target.Body.CanUseLimb(limb).HasFlag(CanUseLimbResult.CantUseNumbness), "Cold load lost exact numb limb.");
			}
		}
		var stomach = world.TryGetCharacter(input.Pokes.Single(x => x.Location == "stomach").Target, true);
		var needs = new ActiveNeedsModel(stomach);
		needs.FulfilNeeds(new NeedFulfiller { WaterLitres = 2, ThirstPoints = 12, SatiationPoints = 12 }, true);
		SetPrivateMember(stomach, "NeedsModel", needs);
		stomach.Body.RefreshNausea();
		stomach.Body.CurrentStamina = stomach.Body.MaximumStamina;
		var stamina = stomach.CurrentStamina;
		Require(stomach.Body.EffectsOfType<Nausea>().Single().IntensityPerGramMass * stomach.Body.EffectsOfType<Nausea>().Single().GetHungerThirstMultiplier >= 5,
			"Severe nausea fixture is below native vomiting threshold.");
		clock.Advance(TimeSpan.FromSeconds(35)); world.EffectScheduler.CheckSchedules();
		Require(stomach.CurrentStamina <= stamina - 50, "Native severe nausea did not invoke vomiting stamina cost.");
		clock.Advance(TimeSpan.FromSeconds(11)); world.EffectScheduler.CheckSchedules();
		foreach (var poke in input.Pokes)
		{
			var target = world.TryGetCharacter(poke.Target, true);
			Require(PokeEffect(target, poke.Location) is null, "Reloaded poke did not expire.");
			if (poke.Limb.HasValue)
				Require(target.Body.CanUseLimb(target.Body.Limbs.Single(x => x.Id == poke.Limb)) == CanUseLimbResult.CanUse, "Expired numbness did not restore limb use.");
		}
		Require(!stomach.Body.EffectsOfType<Nausea>().Any(), "Nausea symptoms lingered after physical cause expired.");
		// A fixture dose also advertises heartbeat-only effects. A nausea refresh must not run them.
		var drug = new Mock<IDrug>();
		drug.SetupGet(x => x.DrugTypes).Returns(new[] { DrugType.Nausea, DrugType.StaminaRegen, DrugType.BodypartDamage });
		var mass = stomach.Body.Weight * world.UnitManager.BaseWeightToKilograms * 0.001;
		drug.Setup(x => x.IntensityForType(It.IsAny<DrugType>())).Returns((DrugType type) => type == DrugType.Nausea ? mass * 2 : 1000000);
		var dose = new DrugDosage { Drug = drug.Object, Grams = 1, OriginalVector = DrugVector.Injected };
		var doses = (ICollection<DrugDosage>)stomach.Body.ActiveDrugDosages;
		doses.Add(dose);
		var physical = new MundaneNauseaEffect(stomach.Body, 4);
		var previousStamina = stomach.CurrentStamina;
		var previousDamage = stomach.Body.Wounds.Sum(x => x.CurrentDamage);
		stomach.Body.AddEffect(physical, TimeSpan.FromSeconds(45));
		Require(Math.Abs(stomach.Body.EffectsOfType<Nausea>().Single().IntensityPerGramMass - 6) < 0.0001, "Physical and drug nausea did not combine.");
		stomach.Body.RemoveEffect(physical, true);
		Require(Math.Abs(stomach.Body.EffectsOfType<Nausea>().Single().IntensityPerGramMass - 2) < 0.0001, "Physical cause removal erased independent drug nausea.");
		Require(dose.Grams == 1 && stomach.CurrentStamina == previousStamina && stomach.Body.Wounds.Sum(x => x.CurrentDamage) == previousDamage,
			"Nausea-only reconciliation metabolised a dose or invoked unrelated drug consequences.");
		doses.Remove(dose); stomach.Body.RefreshNausea();
		Require(!stomach.Body.EffectsOfType<Nausea>().Any(), "Removing the final independent cause left nausea symptoms.");
		stomach.Body.AddEffect(new MundaneNauseaEffect(stomach.Body, 4), TimeSpan.FromSeconds(45));
		stomach.State = CharacterState.Dead;
		stomach.Body.Die();
		clock.Advance(TimeSpan.FromSeconds(46)); world.EffectScheduler.CheckSchedules();
		stomach.State = CharacterState.Awake;
		stomach.Body.Resurrect(stomach.Location);
		Require(!stomach.Body.EffectsOfType<MundaneNauseaEffect>().Any() && !stomach.Body.EffectsOfType<Nausea>().Any(),
			"Nausea survived source expiry while dead and same-session resurrection.");
		Console.WriteLine("TEMPLAR-cold=PASS canonical-selections medallion-custody charges reserve-conservation eight-poke-duration-and-expiry limb-recovery severe-nausea-native-vomiting");
		return 0;
	}
}
