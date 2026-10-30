using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

#nullable enable
namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal const string PoisonKindRegister = "armw_poison_kind";
	internal const string PoisonTargetRegister = "armw_poison_target";
	internal const string PoisonFoodDoseRegister = "armw_poison_food_dose";
	private readonly Dictionary<int, FutureProg> _proportionalPoisonProgs = new();
	private readonly Dictionary<int, FutureProg> _weaponPoisonProgs = new();
	private readonly Dictionary<(long BaseLiquid, int Poison), Liquid> _poisonLiquids = new();
	private readonly Dictionary<long, long> _poisonLiquidBases = new();
	private FutureProg? _spicePoisonDelivery;

	private void InstallPoisonDelivery()
	{
		ItemNumberRegister(PoisonKindRegister);
		ItemNumberRegister(PoisonTargetRegister);
		ItemNumberRegister(PoisonFoodDoseRegister);
		var halfGiant = _races.Values.FirstOrDefault(x => x.Name.Equals("Half-Giant", StringComparison.OrdinalIgnoreCase) ||
			x.Name.Equals("Half Giant", StringComparison.OrdinalIgnoreCase));
		foreach (var (kind, drugId) in _poisonDrugIds)
		{
			var text = new StringBuilder($"if (@grams <= 0)\nreturn false\nend if\nvar drug = todrug({drugId})\n");
			if (kind is 3 or 9 or 10 && halfGiant is not null)
				text.AppendLine($"if (@actor.race == torace({halfGiant.Id}))\nreturn false\nend if");
			if (kind == 6) text.AppendLine("grams = @grams / 2");
			text.AppendLine("if (@save)\nif (resistpoison(@actor, 5))\nreturn false\nend if\nend if\nreturn dosedrug(@actor, @drug, @grams, @vector)");
			_proportionalPoisonProgs.Add(kind, Prog("ArmageddonWorldfileProportionalPoison" + kind, text.ToString(),
				ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("grams", ProgVariableTypes.Number),
				("vector", ProgVariableTypes.Text), ("save", ProgVariableTypes.Boolean)));
			_weaponPoisonProgs.Add(kind, Prog("ArmageddonWorldfileWeaponPoison" + kind,
				$"return @{_poisonDoseProgs[kind].FunctionName}(@victim, random(1, 4), \"injected\", true)", ProgVariableTypes.Boolean,
				("attacker", ProgVariableTypes.Character), ("victim", ProgVariableTypes.Character), ("weapon", ProgVariableTypes.Item)));
		}
		var foodBody = new StringBuilder($"var kind = getregister(@food, \"{PoisonKindRegister}\")\nvar grams = @bites * getregister(@food, \"{PoisonFoodDoseRegister}\")\n");
		var spiceBody = new StringBuilder($"var kind = getregister(@item, \"{PoisonKindRegister}\")\n");
		foreach (var kind in _poisonDrugIds.Keys)
		{
			foodBody.AppendLine($"if (@kind == {kind})\n@{_proportionalPoisonProgs[kind].FunctionName}(@actor, @grams, \"ingested\", true)\nend if");
			spiceBody.AppendLine($"if (@kind == {kind})\n@{_poisonDoseProgs[kind].FunctionName}(@actor, random(1, 2), \"inhaled\", true)\nend if");
		}
		foodBody.AppendLine("return true"); spiceBody.AppendLine("return true");
		var foodProg = Prog("ArmageddonWorldfileFoodPoison", foodBody.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("food", ProgVariableTypes.Item), ("bites", ProgVariableTypes.Number), ("remaining", ProgVariableTypes.Number));
		_spicePoisonDelivery = Prog("ArmageddonWorldfileSpicePoison", spiceBody.ToString(), ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item));
		var foodHook = new Hooks { Id = Next<Hooks>(), Name = "Worldfile food poison", Category = "Worldfile", Type = "FutureProgHook",
			TargetEventType = (int)EventType.ItemEaten, Definition = new XElement("Definition", new XElement("FutureProg", foodProg.Id)).ToString() };
		_db.Hooks.Add(foodHook);
		var baseLiquids = _db.Liquids.Where(x => NativeDrinkNames.Contains(x.Name)).ToArray();
		foreach (var kind in _poisonDrugIds.Keys)
		foreach (var liquid in baseLiquids) PoisonedLiquid(liquid, kind);

		foreach (var (protoId, effective) in _effectiveItems.ToArray())
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == protoId);
			var source = effective.Source; var values = effective.Values;
			var type = source.ItemType;
			if (type is 5 or 17 or 19 or 24 or 25 or 33)
			{
				var kind = type switch { 5 when (source.Flags[1] & (1 << 20)) != 0 => values[5],
					17 => values[5], 19 or 24 or 25 => values[3], 33 when values[1] == 2 => 1, _ => 0 };
				if (kind is < 0 or > 10) { Adapt("poison payload", source.Vnum, $"Unsupported poison index {kind} is retained as provenance, not dosed."); kind = 0; }
				InitialisePoisonRegisters(proto, type, kind, type == 19 ? Math.Max(0, values[0]) * 0.002 : type == 33 ? 0.001 : 0.0);
				if (type is 5 or 24 && kind > 0) InstallInitialWeaponPoison(proto, _weaponPoisonProgs[kind]);
				if (type == 19 || type == 33 && values[1] is 1 or 2)
				{
					if (type == 33) AttachDeliveryComponent(proto, CreateFoodComponent(source, [0, 0, 0, 0, 0, 0], "edible-herb"),
						new XElement("Definition", new XElement("Bites", 1)));
					AttachSpiceHook(proto, "FoodPoison", foodHook);
				}
				if (type == 17 && kind > 0 && _initialLiquidContents.TryGetValue(proto.Id, out var contents))
				{
					var original = _db.Liquids.Local.FirstOrDefault(x => x.Id == contents.Liquid) ?? _db.Liquids.Single(x => x.Id == contents.Liquid);
					var poisoned = PoisonedLiquid(original, kind);
					_initialLiquidContents[proto.Id] = (poisoned.Id, contents.Amount);
					var initializer = _progs.Single(x => x.FunctionName == "ArmageddonWorldfileLiquidContents" + proto.Id);
					initializer.FunctionText = $"setliquid(@item, {poisoned.Id}, {contents.Amount.ToString("R", CultureInfo.InvariantCulture)})\nreturn true";
					foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
					foreach (var component in item.GameItemComponents)
					{
						var xml = XElement.Parse(component.Definition);
						foreach (var liquid in xml.Elements("Mix").Elements("Liquid").Where(x => (long?)x.Attribute("id") == original.Id))
							liquid.SetAttributeValue("id", poisoned.Id);
						component.Definition = xml.ToString();
					}
				}
			}
			if (type == 34) InstallAntidote(proto, source, values);
		}
		InstallPoisonVials();
	}

	private void InstallInitialWeaponPoison(GameItemProto proto, FutureProg delivery)
	{
		AddItemOnLoadProg(proto, "InitialWeaponPoison", $"setweaponpoison(@item, {delivery.Id})\nreturn true");
		foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
		{
			var xml = XElement.Parse(item.EffectData);
			xml.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "WeaponPoisonCoating"),
				new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect", new XElement("Mix"),
					new XElement("DeliveryProg", delivery.Id), new XElement("SingleUseOnWound", true))));
			item.EffectData = xml.ToString();
		}
	}

	private void InitialisePoisonRegisters(GameItemProto proto, int type, int kind, double gramsPerBite)
	{
		AddItemOnLoadProg(proto, "PoisonState", $"setregister @item \"{PoisonKindRegister}\" {kind}\nsetregister @item \"{PoisonTargetRegister}\" {type}\nsetregister @item \"{PoisonFoodDoseRegister}\" {gramsPerBite.ToString("R", CultureInfo.InvariantCulture)}\nreturn true");
		foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
		foreach (var (name, value) in new[] { (PoisonKindRegister, (double)kind), (PoisonTargetRegister, (double)type), (PoisonFoodDoseRegister, gramsPerBite) })
			_db.VariableValues.Add(new VariableValue { ReferenceTypeDefinition = ProgVariableTypes.Item.ToStorageString(), ReferenceId = item.Id,
				ReferenceProperty = name, ValueTypeDefinition = ProgVariableTypes.Number.ToStorageString(), ValueDefinition = new XElement("var", value).ToString() });
	}

	private Liquid PoisonedLiquid(Liquid original, int kind)
	{
		if (_poisonLiquids.TryGetValue((original.Id, kind), out var existing)) return existing;
		var clone = (Liquid)_db.Entry(original).CurrentValues.ToObject();
		clone.Id = Next<Liquid>(); clone.Name = $"armw.poison.{kind}.{original.Id}"; clone.CountAsId = original.Id;
		_poisonLiquids.Add((original.Id, kind), clone); _poisonLiquidBases[clone.Id] = original.Id;
		_db.Liquids.Add(clone);
		var litres = _db.UnitsOfMeasure.Single(x => x.Id == _settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
		var body = new StringBuilder();
		if (original.DraughtProgId is { } oldId)
		{
			var old = _db.FutureProgs.Local.FirstOrDefault(x => x.Id == oldId) ?? _db.FutureProgs.Single(x => x.Id == oldId);
			body.AppendLine($"@{old.FunctionName}(@actor, @item, @amount)");
		}
		body.AppendLine($"@{_proportionalPoisonProgs[kind].FunctionName}(@actor, @amount * {(0.06 / litres).ToString("R", CultureInfo.InvariantCulture)}, \"ingested\", true)");
		clone.DraughtProgId = Prog($"ArmageddonWorldfilePoisonDraught{original.Id}Type{kind}", body.ToString(), ProgVariableTypes.Void,
			("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item), ("amount", ProgVariableTypes.Number)).Id;
		foreach (var tag in original.LiquidsTags) clone.LiquidsTags.Add(new LiquidsTags { TagId = tag.TagId, LiquidId = clone.Id });
		if (original.StaleLiquidId is { } stale) clone.StaleLiquidId = PoisonedLiquid(_db.Liquids.Local.FirstOrDefault(x => x.Id == stale) ?? _db.Liquids.Single(x => x.Id == stale), kind).Id;
		if (original.SpoiledLiquidId is { } spoiled) clone.SpoiledLiquidId = PoisonedLiquid(_db.Liquids.Local.FirstOrDefault(x => x.Id == spoiled) ?? _db.Liquids.Single(x => x.Id == spoiled), kind).Id;
		Record($"poison.liquid.{original.Id}.{kind}", nameof(Liquid), clone.Id, null,
			new { OriginalLiquid = original.Id, PoisonIndex = kind, BaseGramsPerLitre = 0.06, Disposition = "Same beverage presentation/physics, native mixture transfer/dilution/freshness, separate poison admission prog; original drug and draught behaviour preserved." });
		return clone;
	}

	private void InstallAntidote(GameItemProto proto, ArmWorldItem source, int[] values)
	{
		if (values[1] is not (0 or 1)) { Adapt("antidote", source.Vnum, "Unimplemented source form is not given an invented injection action."); return; }
		var body = new StringBuilder();
		var cureKinds = values[0] is 3 or 9 or 10 ? new[] { 3, 9, 10 } : new[] { values[0] };
		foreach (var kind in cureKinds)
			if (_poisonDrugIds.TryGetValue(kind, out var drugId)) body.AppendLine($"purgedrugdosages(@actor, todrug({drugId}), \"\", {(values[1] == 0 ? "true" : "false")})");
		if (values[2] is >= 1 and <= 10) body.AppendLine($"@{_poisonDoseProgs[values[2]].FunctionName}(@actor, random(1, 3), \"ingested\", false)");
		if (values[1] == 0 && _items.TryGetValue(13248, out var empty))
			body.AppendLine($"deleteitem(@item)\nvar empty = loaditem({empty.Id})\nif (not(put(@empty, @actor.location)))\ndeleteitem(@empty)\nelse\nsilentget(@actor, @empty)\nend if");
		var cure = Prog("ArmageddonWorldfileAntidote" + proto.Id, body.ToString(), ProgVariableTypes.Void,
			("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item));
		var pill = Component("antidote." + proto.Id, "Pill", new XElement("Definition", new XElement("Drug", 0),
			new XElement("GramsPerPill", 0), new XElement("OnSwallowProg", cure.Id), new XElement("ConsumptionEmote", new XCData(
				values[1] == 0 ? "@ drink|drinks the contents of $0." : "@ eat|eats $0."))));
		AttachDeliveryComponent(proto, pill, new XElement("Definition"));
		var command = values[1] == 0 ? "drink" : "eat";
		var input = Prog("ArmageddonWorldfileAntidoteInput" + proto.Id, "if (@args.Count(word, true) != 1)\nreturn false\nend if\nif (targetitem(@actor, @actor.helditems, getindex(@args, 0)) != @item)\nreturn false\nend if\nswallowitem(@actor, @item)\nreturn true",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
			("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
		AttachSpiceHook(proto, "AntidoteInputHook", SpiceHook(command, input));
		Record("poison.antidote." + proto.Id, "NativePill", proto.Id, proto.RevisionNumber,
			new { SourceVnum = source.Vnum, CureIndex = values[0], Form = values[1], SideEffect = values[2], Disposition = "Native oral consumption and exact drug purge; Skellebain family cure; index zero intentionally cures nothing; vial produces available empty Vnum13248." });
	}

	private void AttachDeliveryComponent(GameItemProto proto, GameItemComponentProto component, XElement state)
	{
		Attach(proto, component);
		foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == proto.Id))
		{
			var instance = new GameItemComponent { Id = Next<GameItemComponent>(), GameItemId = item.Id, GameItemComponentProtoId = component.Id,
				GameItemComponentProtoRevision = component.RevisionNumber, Definition = state.ToString() };
			_db.GameItemComponents.Add(instance);
			item.GameItemComponents.Add(instance);
		}
	}
}
