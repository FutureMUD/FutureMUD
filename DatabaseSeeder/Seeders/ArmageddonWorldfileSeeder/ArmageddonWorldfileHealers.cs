#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly string[] HealerVenoms = ["Cobra Venom", "Adder Venom", "Rattlesnake Venom", "Viper Venom",
		"Mamba Venom", "Coral Snake Venom", "Moccasin Venom", "Jellyfish Venom", "Bee Venom", "Wasp Venom",
		"Hornet Venom", "Spider Venom", "Tarantula Venom", "Scorpion Venom"];

	private void InstallHealerSpecials()
	{
		var kelwins = _source.Npcs.Where(x => HasSpecial(x, "bw_kelwin_cure")).Select(x => x.Vnum).ToArray();
		var halflings = _source.Npcs.Where(x => HasSpecial(x, "halfling_healer")).Select(x => x.Vnum).ToArray();
		if (kelwins.Length == 0 && halflings.Length == 0) return;
		var poison = OwnedBehaviourId("arm.effect.poison", nameof(Drug))
			?? throw new InvalidOperationException("Native healer services require the installed package-owned toxin.");
		if (halflings.Length > 0) InstallHalflingHealer(halflings, poison);
		if (kelwins.Length == 0) return;
		if (!_clans.TryGetValue(25, out var blackwing)) throw new InvalidOperationException("Kelwin requires the imported Blackwing clan.");
		var waterId = OwnedBehaviourId("arm.capability.water", nameof(MagicCapability))
			?? throw new InvalidOperationException("Kelwin requires installed native Water casting.");
		var mendId = OwnedBehaviourId("arm.spell.mend_flesh", nameof(MagicSpell))
			?? throw new InvalidOperationException("Kelwin requires the existing full native Mend Flesh spell.");
		var purgeId = OwnedBehaviourId("arm.spell.purge_toxin", nameof(MagicSpell))
			?? throw new InvalidOperationException("Kelwin requires the existing native Purge Toxin spell.");
		var originalCure = _db.MagicSpells.Single(x => x.Id == purgeId);
		var cure = (MagicSpell)_db.Entry(originalCure).CurrentValues.ToObject();
		cure.Id = Next<MagicSpell>(); cure.Name = "Cleanse Venom";
		cure.SpellKnownProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		cure.Blurb = "Purge the configured magical toxin and explicitly selected native venoms.";
		var venoms = _db.Drugs.Where(x => HealerVenoms.Contains(x.Name)).OrderBy(x => x.Id).ToArray();
		cure.Description = "The native Purge Toxin spell with an explicit additional venom list. Alcohol, medicines and other drugs are unaffected. " + string.Join(", ", venoms.Select(x => x.Name));
		var cureXml = XElement.Parse(cure.Definition);
		var removal = cureXml.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "removepoison");
		removal.Element("AdditionalDrugs")?.Remove();
		if (venoms.Length > 0) removal.Add(new XElement("AdditionalDrugs", venoms.Select(x => new XElement("Drug", x.Id))));
		cure.Definition = cureXml.ToString(); _db.MagicSpells.Add(cure);
		Record("npc-service.kelwin.cure", nameof(MagicSpell), cure.Id, null,
			new { NativeBasis = purgeId, AdditionalDrugs = venoms.Select(x => new { x.Id, x.Name }),
				AbsentOptionalVenoms = HealerVenoms.Except(venoms.Select(x => x.Name)).ToArray() });

		var water = _db.MagicCapabilities.Single(x => x.Id == waterId);
		var capability = (MagicCapability)_db.Entry(water).CurrentValues.ToObject();
		capability.Id = Next<MagicCapability>(); capability.Name = "Blackwing Healer";
		var definition = XElement.Parse(capability.Definition);
		var casting = definition.Element("Casting") ?? throw new InvalidOperationException("Water casting has no native policy.");
		var admissions = casting.Elements("Admission").ToArray();
		XElement Admission(long sourceId, long spellId)
		{
			var admission = new XElement(admissions.Single(x => (long)x.Attribute("spell")! == sourceId));
			admission.Elements().Remove(); admission.SetAttributeValue("key", Guid.NewGuid());
			admission.SetAttributeValue("spell", spellId); admission.SetAttributeValue("starting", true);
			admission.SetAttributeValue("min", 1); admission.SetAttributeValue("max", 1);
			return admission;
		}
		var mendAdmission = Admission(mendId, mendId); var cureAdmission = Admission(purgeId, cure.Id);
		casting.Elements().Remove(); casting.SetAttributeValue("identity", Guid.NewGuid());
		casting.SetAttributeValue("enabled", true); casting.SetAttributeValue("startingVersion", 1);
		casting.SetAttributeValue("enrolOnClassSelection", false);
		casting.Add(mendAdmission, cureAdmission);
		definition.Element("Powers")?.Remove(); definition.Element("Gathering")?.Remove();
		capability.Definition = definition.ToString(); _db.MagicCapabilities.Add(capability);
		var merit = new Merit { Id = Next<Merit>(), Name = "Blackwing Healer", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
				new XElement("ChargenBlurb", "The configured Blackwing healer's native spell route."),
				new XElement("DescriptionText", "@ practice|practices the healing arts."),
				new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() };
		_db.Merits.Add(merit);
		var reserve = (long)casting.Attribute("reserve")!;
		foreach (var vnum in kelwins)
		{
			foreach (var id in _npcFamilyIds.GetValueOrDefault(vnum, [_npcs[vnum].Id]))
			{
				var template = _db.NpcTemplates.Local.Single(x => x.Id == id); var xml = XElement.Parse(template.Definition);
				if (xml.Element("Merits") is null) xml.Add(new XElement("Merits"));
				xml.Element("Merits")!.Add(new XElement("Merit", merit.Id)); template.Definition = xml.ToString();
			}
			var onload = _progs.Single(x => x.FunctionName == "ArmageddonWorldfileNpcCash" + vnum);
			var lastReturn = onload.FunctionText.LastIndexOf("return true", StringComparison.Ordinal);
			onload.FunctionText = onload.FunctionText[..lastReturn] + $"if (enrolchannelcasting(@npc, tomagiccapability({capability.Id}), \"Source Blackwing healer service\"))\nsetmagicresource(@npc, {reserve}, magicresourcecapacity(@npc, {reserve}))\nend if\nreturn true";
		}
		Record("npc-service.kelwin.casting", nameof(MagicCapability), capability.Id, null,
			new { Merit = merit.Id, NativeHealingSpell = mendId, NativeCureSpell = cure.Id, Reserve = reserve,
				Disposition = "Private NPC-only native Water route with two grade-one starting admissions. Existing spell definitions, skill openings, costs, checks and reserve regeneration are reused; no player routes are changed. New NPCs enrol once and begin with their native reserve capacity; reload does not refill survivors." });
		var body = $"""
var healer = tocharacter(@npc)
if (@args.Count(word, true) < 2)
	return false
end if
if (targetcharacter(@actor, getindex(@args, 0)) != @healer)
	return false
end if
var service = lowercase(getindex(@args, 1))
if (@service != "heal" and @service != "cure")
	return false
end if
if (@args.Count(word, true) < 3)
	force @healer ("say " + @service + " whom?")
	return true
end if
var patient = @actor
var whom = getindex(@args, 2)
if (lowercase(@whom) != "me")
	patient = targetcharacter(@healer, @whom)
end if
if (isnull(@patient))
	force @healer ("say " + @service + " whom?")
	return true
end if
if (not(isclanmember(@patient, toclan({blackwing.Id}))))
	force @healer "say I will only aid those of our tribe."
	return true
end if
var spell = tomagicspell({mendId})
if (@service == "heal")
	if (healthremaining(@patient, "health") > 20)
		force @healer "say Your wounds are not serious enough. I must conserve my energy."
		return true
	end if
else
	spell = tomagicspell({cure.Id})
	if (not(haspurgeablepoison(@patient, @spell)))
		force @healer "say You are not poisoned."
		return true
	end if
end if
var target = bestkeyword(@healer, @patient)
if (isnull(@target))
	return true
end if
force @healer "say I will try."
var result = channelspell(@healer, tomagiccapability({capability.Id}), @spell, 1, false, @target)
if (not(startswith(@result, "Succeeded")))
	force @healer "say I cannot help just now."
end if
return true
""";
		var prog = NpcServiceProg("ArmageddonWorldfileKelwinHealer", body);
		InstallNpcInputHook("Blackwing Healing", "ask", prog, kelwins);
		Flush();
	}

	private FutureProg NpcServiceProg(string name, string text) => Prog(name, text, ProgVariableTypes.Boolean,
		("actor", ProgVariableTypes.Character), ("npc", ProgVariableTypes.Perceivable),
		("input", ProgVariableTypes.Text), ("args", ProgVariableTypes.Text | ProgVariableTypes.Collection));

	private void InstallHalflingHealer(int[] sources, long poison)
	{
		if (!_clans.TryGetValue(9, out var clan)) throw new InvalidOperationException("Halfling healing requires the imported clan 9.");
		var races = _db.Races.AsEnumerable().Where(x => x.Name.Split(" (", StringSplitOptions.None)[0].Equals("Halfling", StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToArray();
		var race = races.Length == 0 ? "false" : string.Join(" or ", races.Select(id => $"@actor.race.id == {id}"));
		var prog = NpcServiceProg("ArmageddonWorldfileHalflingHealer", $"""
var healer = tocharacter(@npc)
if (not(@healer.isawake) or @healer.incombat)
	return false
end if
if (not(isclanmember(@actor, toclan({clan.Id}))))
	return false
end if
if (not({race}))
	return false
end if
if (@args.Count(word, true) != 2)
	return false
end if
if (targetcharacter(@actor, getindex(@args, 0)) != @healer or lowercase(getindex(@args, 1)) != "poison")
	return false
end if
if (not(hasdrugdosage(@actor, todrug({poison}), "arm.effect.poison")))
	force @healer "say You are not afflicted."
	return true
end if
send(@actor, "$0 burn|burns a small stick and bathe|bathes you in the smoke.", @healer)
purgedrugdosages(@actor, todrug({poison}), "arm.effect.poison")
force @healer "say Be healed, and peace to you."
return true
""");
		InstallNpcInputHook("Halfling Poison Treatment", "ask", prog, sources);
		Record("npc-service.halfling-healer", nameof(FutureProg), prog.Id, null,
			new { Sources = sources, Drug = poison, Family = "arm.effect.poison", Disposition = "Source self-only Halfling/clan 9 smoke treatment. Explicit magical-toxin family selection preserves mundane doses, other families and unrelated effects; no casting cost or new spell is invented for this source direct cure." });
	}
}
