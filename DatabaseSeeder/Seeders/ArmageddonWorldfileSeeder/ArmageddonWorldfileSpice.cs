#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using Drug = MudSharp.Models.Drug;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal const string SpiceUnitsRegister = "armw_spice_units";
	internal const string SpiceInitialisedRegister = "armw_spice_initialised";
	internal const double SpiceGramsPerUnit = 0.001;
	private static readonly string[] SpiceNames =
		["Methelinoc", "Melem Tuek", "Krelez", "Kemen", "Aggross", "Zharal", "Thodeliv", "Krentakh", "Qel"];

	private void InstallSpiceConsumption()
	{
		var sources = _effectiveItems.Where(x => x.Value.Source.ItemType == 25).ToArray();
		if (sources.Length == 0) return;
		var attributes = _db.TraitDefinitions.Where(x => x.Type == 1).ToArray();
		long Attribute(params string[] names) => names.Select(name => attributes.SingleOrDefault(x =>
			x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault(x => x is not null)?.Id
			?? throw new InvalidOperationException($"Spice requires a native attribute matching {string.Join("/", names)}.");
		var strength = Attribute("Strength", "Upper Body Strength");
		var agility = Attribute("Agility", "Dexterity");
		var wisdom = Attribute("Wisdom", "Willpower", "Intelligence", "Mind");
		var endurance = Attribute("Endurance", "Constitution", "Hardiness", "Resilience", "Stamina");
		var mana = OwnedBehaviourId(ArmageddonProgressionInstaller.ManaResourceKey, nameof(MagicResource))
			?? throw new InvalidOperationException("Spice requires the installed common Mana resource.");
		var drugs = new Dictionary<int, Drug>();
		for (var skill = 141; skill <= 149; skill++)
		{
			var drug = CreateSpiceDrug(skill, strength, agility, wisdom, endurance, mana, false);
			drug.Id = Next<Drug>();
			foreach (var effect in drug.DrugsIntensities) effect.DrugId = drug.Id;
			_db.Drugs.Add(drug);
			drugs.Add(skill, drug);
			Record("spice.drug." + skill, nameof(Drug), drug.Id, null,
				new { SourceSkill = skill, GramsPerUnit = SpiceGramsPerUnit, ReferenceBodyKilograms = 70,
					Disposition = "Native inhaled absorption, active-dose dependence growth and metabolism. Authored physical calibration; not recovered source drug mass." });
		}
		var adverseQel = CreateSpiceDrug(149, strength, agility, wisdom, endurance, mana, true);
		adverseQel.Id = Next<Drug>();
		foreach (var effect in adverseQel.DrugsIntensities) effect.DrugId = adverseQel.Id;
		_db.Drugs.Add(adverseQel);
		Record("spice.drug.qel.defiling", nameof(Drug), adverseQel.Id, null,
			new { Disposition = "Qel taken with negative ecology reduces Mana capacity; the ecology sign is captured at consumption." });
		ItemNumberRegister(SpiceUnitsRegister);
		ItemNumberRegister(SpiceInitialisedRegister);
		var config = _db.StaticConfigurations.Find("DrugDependenceEnabled");
		if (config is null) _db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "DrugDependenceEnabled", Definition = "true" });
		else config.Definition = "true";
		foreach (var source in sources)
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == source.Key);
			var values = source.Value.Values;
			if (values[3] is < 0 or > 10)
				Adapt("spice poison", source.Value.Source.Vnum, $"Effective prototype {proto.Id} retains unsupported legacy poison index {values[3]} as provenance.");
			AddItemOnLoadProg(proto, "SpiceStock", $"setregister @item \"{SpiceUnitsRegister}\" {Math.Max(0, values[0])}\nsetregister @item \"{SpiceInitialisedRegister}\" 1\nreturn true");
			foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
			{
				foreach (var (register, value) in new[] { (SpiceUnitsRegister, Math.Max(0, values[0])), (SpiceInitialisedRegister, 1) })
					_db.VariableValues.Add(new VariableValue
					{
						ReferenceTypeDefinition = ProgVariableTypes.Item.ToStorageString(), ReferenceId = item.Id,
						ReferenceProperty = register, ValueTypeDefinition = ProgVariableTypes.Number.ToStorageString(),
						ValueDefinition = new XElement("var", value).ToString()
					});
			}
			if (!drugs.TryGetValue(values[1], out var drug) || values[0] <= 0 || values[2] is not (0 or 1))
			{
				Adapt("spice consumption", source.Value.Source.Vnum, $"Effective prototype {proto.Id} retains invalid or obsolete spice skill/form/power {values[1]}/{values[2]}/{values[0]}; no inferred drug effect.");
				continue;
			}
			FutureProg? poison = null;
			if (values[3] is >= 1 and <= 10 && !_poisonDoseProgs.TryGetValue(values[3], out poison))
				throw new InvalidOperationException("Spice lacing requires the calibrated legacy poison dose progs to be installed first.");
			InstallSpiceItemConsumption(proto, drug, adverseQel, values[1], values[2] == 1, poison);
			InstallSpiceSalePolicy(proto, values[0]);
			if (values[2] == 0)
			{
				InstallSpiceShaving(proto, values[1]);
				InstallSpiceRolling(proto, values[1]);
			}
		}
		Record("spice.consumption", "NativeDrugProgs", null, null, new
		{
			EffectivePrototypes = sources.Length, ValidSourceSkills = drugs.Keys,
			Disposition = "Saved source potency/puff counts; whole loose portion or one puff uses native inhaled dosing. Valid source lacing delivers a separate calibrated one-to-two duration-unit toxin dose. Oversized loose spice must be shaved; no historical player dependence is imported."
		});
	}

	internal static Drug CreateSpiceDrug(int skill, long strength, long agility, long wisdom, long endurance,
		long mana, bool adverseQel)
	{
		if (skill is < 141 or > 149) throw new ArgumentOutOfRangeException(nameof(skill));
		var drug = new Drug { Name = SpiceNames[skill - 141] + (adverseQel ? " (defiling response)" : ""),
			DrugVectors = (int)DrugVector.Inhaled, IntensityPerGram = 70,
			RelativeMetabolisationRate = skill == 149 ? 0.0006 : 0.0002 };
		void Effect(DrugType type, double intensity, DrugAdditionalInfo? info = null) => drug.DrugsIntensities.Add(new DrugIntensity
		{ DrugType = (int)type, RelativeIntensity = intensity, AdditionalEffects = info?.DatabaseString });
		var trait = skill switch { 141 => wisdom, 142 => strength, 143 => agility, 145 => endurance, _ => 0 };
		var steps = Enumerable.Range(1, 100).Select(level => new DrugModifierStep(level - 0.75,
			TraitId: trait, TraitBonus: trait == 0 ? 0 : level,
			MaximumHitPointsBonus: skill == 144 ? level : 0,
			MagicResourceId: skill == 149 ? mana : 0,
			MagicResourceCapacityBonus: skill == 149 ? (adverseQel ? -2 : 2) * level : 0,
			OffensiveCheckBonus: skill == 146 ? level : 0)).ToList();
		if (skill is not (147 or 148)) Effect(DrugType.Modifier, 1, new DrugModifierAdditionalInfo { Steps = steps });
		if (skill == 147) Effect(DrugType.Analgesic, 1);
		if (skill == 148) Effect(DrugType.StaminaRegen, 0.1);
		if (skill != 149)
		{
			var dependence = new DrugDependenceAdditionalInfo
			{
				ExposureGainPerGram = 10, ExposureDecayPerDay = 16, ToleranceThreshold = 75,
				MinimumToleranceMultiplier = 1, WithdrawalThreshold = 75, WithdrawalDecayPerDay = 16,
				AffectedDrugTypes = [], WithdrawalCheckPenalty = 0, WithdrawalHungerMultiplier = 1,
				WithdrawalThirstMultiplier = 1, WithdrawalStaminaRegenMultiplier = 0.5,
				WithdrawalStaminaCostMultiplier = 1.5, WithdrawalNauseaIntensity = 0,
				WithdrawalRageIntensity = 0, SleepPreventionThreshold = 0,
				RelapseExposureMultiplier = 2, WithdrawalMaximumHitPointsMultiplier = 0.5,
				WithdrawalMagicResourceCapacityMultiplier = 0.5,
				ModifierSteps = Enumerable.Range(1, skill == 148 ? 5 : 10).Select(level => new DrugModifierStep(
					level * (skill == 148 ? 20 : 10), TraitId: skill == 148 ? endurance : trait,
					TraitBonus: trait != 0 || skill == 148 ? -level : 0,
					MaximumHitPointsBonus: skill == 144 ? -5 * level : 0,
					DefensiveCheckBonus: skill == 146 ? -level : 0)).ToList()
			};
			Effect(DrugType.Dependence, 1, dependence);
			Effect(DrugType.Overdose, 1, new OverdoseAdditionalInfo { TraitId = endurance, GramsPerTraitPoint = 0.002, Group = "Armageddon Spice" });
		}
		return drug;
	}

	private void InstallSpiceItemConsumption(GameItemProto proto, Drug drug, Drug adverseQel, int skill, bool rolled, FutureProg? poison)
	{
		var body = new StringBuilder($$"""
			if (@args.Count(word, true) != 1)
				return false
			end if
			if (targetitem(@actor, @actor.helditems, getindex(@args, 0)) != @item)
				return false
			end if
			if (not(@actor.isawake) or @actor.incombat or isincorporeal(@actor))
				send(@actor, "You cannot take spice right now.")
				return true
			end if
			var units = getregister(@item, "{{SpiceUnitsRegister}}")
			if (@units <= 0)
				send(@actor, "There is no spice left.")
				return true
			end if
			""" + "\n");
		if (!rolled) body.AppendLine("if (@units > 5)\nsend(@actor, \"That is too much to inhale. Shave off a smaller pinch first.\")\nreturn true\nend if");
		body.AppendLine($"var drug = todrug({drug.Id})");
		if (skill == 149) body.AppendLine($"if (getregister(@actor, \"{ArmageddonGatheringRegisters.Ecology}\") < 0)\ndrug = todrug({adverseQel.Id})\nend if");
		body.AppendLine($"if (not(dosedrug(@actor, @drug, {(rolled ? "0.001" : "@units * 0.001")}, \"inhaled\")))\nsend(@actor, \"You cannot take that spice right now.\")\nreturn true\nend if");
		if (_spicePoisonDelivery is not null) body.AppendLine($"@{_spicePoisonDelivery.FunctionName}(@actor, @item)");
		else if (poison is not null) body.AppendLine($"@{poison.FunctionName}(@actor, random(1, 2), \"inhaled\", true)");
		if (skill == 147) body.AppendLine("removecombatfrenzyeffect(@actor)");
		body.AppendLine($"setregister @item \"{SpiceUnitsRegister}\" {(rolled ? "(@units - 1)" : "0")}");
		body.AppendLine(rolled
			? "sendloc(@actor.location, \"$0 bring|brings $1 to &0's mouth and inhale|inhales deeply.\", @actor, @item)"
			: "sendloc(@actor.location, \"$0 bring|brings $1 to &0's nose and inhale|inhales deeply.\", @actor, @item)");
		body.AppendLine($"if (getregister(@item, \"{SpiceUnitsRegister}\") <= 0)\ndeleteitem(@item)\nend if\nreturn true");
		var prog = Prog("ArmageddonWorldfileSpiceConsume" + proto.Id, body.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item), ("command", ProgVariableTypes.Text),
			("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = SpiceHook(rolled ? "smoke" : "sniff", prog);
		AttachSpiceHook(proto, "SpiceConsumption", hook);
	}

	private void AttachSpiceHook(GameItemProto proto, string suffix, Hooks hook)
	{
		AddItemOnLoadProg(proto, suffix, $"addhook(@item, {hook.Id})\nreturn true");
		// Opening shop stock is persisted directly and does not execute creation progs on database load.
		foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
			_db.HooksPerceivables.Add(new HooksPerceivable { GameItemId = item.Id, HookId = hook.Id });
	}

	private Hooks SpiceHook(string command, FutureProg prog)
	{
		var hook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile Spice " + command + " " + prog.Id,
			Category = "Worldfile", Type = "HookOnInput", TargetEventType = (int)EventType.CommandInput,
			Definition = new XElement("Definition", new XElement("TargetCommand", command),
				new XElement("UseProgResultForHandling", true), new XElement("FutureProg", prog.Id)).ToString() };
		_db.Hooks.Add(hook);
		return hook;
	}

	private void InstallSpiceSalePolicy(GameItemProto proto, int initialUnits)
	{
		var prog = Prog("ArmageddonWorldfileSpiceSell" + proto.Id, $$"""
			if (@args.Count(word, true) == 0)
				return false
			end if
			if (targetitem(@actor, @actor.helditems, getindex(@args, 0)) != @item)
				return false
			end if
			if (getregister(@item, "{{SpiceUnitsRegister}}") >= {{initialUnits}})
				return false
			end if
			send(@actor, "Shops will not buy partially used spice. Fresh portions and shaved pinches can be sold.")
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = SpiceHook("sell", prog);
		AttachSpiceHook(proto, "SpiceSalePolicy", hook);
	}

	private void InstallSpiceShaving(GameItemProto proto, int skill)
	{
		if (!_items.TryGetValue(20200 + skill, out var pinch)) return;
		var prog = Prog("ArmageddonWorldfileSpiceShave" + proto.Id, $$"""
			if (@args.Count(word, true) != 1)
				return false
			end if
			if (targetitem(@actor, @actor.helditems, getindex(@args, 0)) != @item)
				return false
			end if
			if (not(@actor.isawake) or @actor.incombat or isincorporeal(@actor))
				return true
			end if
			var units = getregister(@item, "{{SpiceUnitsRegister}}")
			if (@units < 5)
				send(@actor, "That is too small to break up any further.")
				return true
			end if
			var pinch = loaditem({{pinch.Id}})
			if (not(silentget(@actor, @pinch)))
				deleteitem(@pinch)
				send(@actor, "You cannot hold the shaved pinch.")
				return true
			end if
			setregister @item "{{SpiceUnitsRegister}}" (@units - 3)
			sendloc(@actor.location, "$0 carefully shave|shaves a pinch from $1.", @actor, @item)
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		var hook = SpiceHook("shave", prog);
		AttachSpiceHook(proto, "SpiceShaving", hook);
	}
}
