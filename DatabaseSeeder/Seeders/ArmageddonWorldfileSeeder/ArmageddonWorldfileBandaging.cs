#nullable enable

using System;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallBandaging()
	{
		var toolItems = _effectiveItems.Where(x => x.Value.Source.ItemType == 7 && x.Value.Values.Length > 1 && x.Value.Values[0] == 5).ToArray();
		if (toolItems.Length == 0) return;
		var skill = _db.TraitDefinitions.Where(x => x.Type == 0).AsEnumerable().SingleOrDefault(x =>
			x.Name.Equals("Bandage", StringComparison.OrdinalIgnoreCase))
			?? throw new InvalidOperationException("Armageddon bandaging requires the native Bandage skill.");

		_poisonDrugIds.TryGetValue(1, out var genericPoison);
		if (genericPoison == 0) throw new InvalidOperationException("Armageddon bandaging requires the imported generic poison drug.");
		var magicalPoison = OwnedBehaviourId("arm.spell.purge_toxin", nameof(MagicSpell)) is { } purgeSpell
			? (long?)XElement.Parse((_db.MagicSpells.Local.FirstOrDefault(x => x.Id == purgeSpell) ?? _db.MagicSpells.Single(x => x.Id == purgeSpell)).Definition)
				.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "removepoison").Element("Drug")!
			: null;
		var installedProgs = new System.Collections.Generic.List<long>();
		foreach (var (id, effective) in toolItems)
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			var toolHealing = effective.Values[1];
				var hasMagicalPoison = magicalPoison.HasValue
					? $"hasdrugdosage(@patient, todrug({magicalPoison.Value.ToString(CultureInfo.InvariantCulture)}), \"arm.effect.poison\")"
					: "false";
				var cureMagicalPoison = magicalPoison.HasValue
					? $"purgedrugdosages(@patient, todrug({magicalPoison.Value.ToString(CultureInfo.InvariantCulture)}), \"arm.effect.poison\")"
					: "false";
				var prog = Prog("ArmageddonWorldfileBandaging" + proto.Id.ToString(CultureInfo.InvariantCulture), $$"""
			if (not(@actor.helditems.Any(held, @held == @item)))
				return false
			end if
			if (@args.Count(word, true) == 0)
				send(@actor, "Use bandage <person>.")
				return true
			end if
			if (@args.Count(word, true) != 1)
				send(@actor, "Who do you want to bandage? Use bandage <person>.")
				return true
			end if
			var patient = targetcharacter(@actor, getindex(@args, 0))
			if (isnull(@patient) or @patient.isdead or not(@actor.isawake) or isincorporeal(@actor) or isincorporeal(@patient))
				send(@actor, "You cannot bandage that person right now.")
				return true
			end if
			var bandage = totrait({{skill.Id}})
			var learned = gettrait(@actor, @bandage)
			if (@learned <= 0)
				send(@actor, "You have no idea how to bandage anyone.")
				return true
			end if
			var hp = healthremaining(@patient, "health")
			var maximum = healthcapacity(@patient, "health")
			if ({{(toolHealing >= 8 ? "true" : "false")}} and @learned >= 50 and {{hasMagicalPoison}})
				if (check(@actor, @bandage, 5) > 0)
					var cured = {{cureMagicalPoison}}
					sendloc(@actor.location, "$0 skillfully draws magical poison from $1 with a bandage.", @actor, @patient)
					deleteitem(@item)
					return true
				end if
			end if
			if ({{(toolHealing >= 8 ? "true" : "false")}} and @learned >= 50 and hasdrugdosage(@patient, todrug({{genericPoison}}), ""))
				if (check(@actor, @bandage, 5) > 0)
					purgedrugdosages(@patient, todrug({{genericPoison}}), "")
					sendloc(@actor.location, "$0 skillfully draws poison from $1 with a bandage.", @actor, @patient)
					deleteitem(@item)
					return true
				end if
			end if
			if (@hp > (floor(@maximum / 2) + 10))
				send(@actor, "That person does not need bandaging.")
				return true
			end if
			if (@hp < max(-6, floor(@maximum / 2) - @learned))
				send(@actor, "Your bandaging skill is not sufficient for those wounds.")
				return true
			end if
			if (check(@actor, @bandage, 5) > 0)
				var low = {{toolHealing.ToString(CultureInfo.InvariantCulture)}}
				var high = floor(@learned / 3)
				if (@low == 0)
					low = -5
					high = 5
				end if
				var start = min(@low, @high)
				var finish = max(@low, @high)
				var healing = max(1, random(@start, @finish))
				sendloc(@actor.location, "$0 binds $1's wounds with a bandage.", @actor, @patient)
				adjusthitpoints(@patient, @healing)
			else
				var damage = random(1, 12)
				sendloc(@actor.location, "$0's attempt to bandage $1 makes the wounds worse.", @actor, @patient)
				adjusthitpoints(@patient, 0 - @damage)
			end if
			deleteitem(@item)
			return true
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character), ("item", ProgVariableTypes.Item),
				("command", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));
				AttachSpiceHook(proto, "BandagingHook", SpiceHook("bandage", prog));
				installedProgs.Add(prog.Id);
		}
		Record("combat.bandaging", nameof(FutureProg), installedProgs.First(), null, new
		{
			Skill = skill.Id, Progs = installedProgs, SourceToolVnums = toolItems.Select(x => x.Value.Source.Vnum).Distinct().ToArray(), SourceToolSubtype = 5,
			MagicalPoison = magicalPoison, GenericPoison = genericPoison,
			Disposition = "Source skill_bandage thresholds and magical-then-generic poison priority. Consumes each effective held subtype-5 tool on successful poison treatment, a healing attempt or failure; opening stock receives the same input hook. Random healing endpoints are normalized. Native HP adjustment owns consequences and death."
		});
	}

}
