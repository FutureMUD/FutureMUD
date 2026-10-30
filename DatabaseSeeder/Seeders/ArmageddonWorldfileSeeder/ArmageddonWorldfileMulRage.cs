#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private TraitDefinition? _willpowerSkill;

	private void InstallWillpowerSkill()
	{
		_willpowerSkill = _db.TraitDefinitions.SingleOrDefault(x => x.Type == 0 && x.Name == "Willpower");
		if (_willpowerSkill is not null) return;
		var cap = new TraitExpression { Id = Next<TraitExpression>(), Name = "Willpower Skill Cap", Expression = "100" };
		_db.TraitExpressions.Add(cap);
		_willpowerSkill = new TraitDefinition
		{
			Id = Next<TraitDefinition>(), Name = "Willpower", Alias = "willpower", Type = 0, OwnerScope = 1,
			DecoratorId = _settings.NativeBindings[ArmageddonNativeBinding.SkillDecorator],
			ImproverId = _settings.NativeBindings[ArmageddonNativeBinding.SkillImprover], ExpressionId = cap.Id,
			TraitGroup = "Innate", Hidden = true, ShowInScoreCommand = false, BranchMultiplier = 0,
			AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
			LearnableProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg],
			TeachableProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg],
			ChargenBlurb = "Resist an involuntary loss of self-control.", ValueExpression = "",
			TeachDifficulty = 7, LearnDifficulty = 7
		};
		_db.TraitDefinitions.Add(_willpowerSkill);
		Record("skill.willpower", nameof(TraitDefinition), _willpowerSkill.Id, null,
			new { SourceSkillIndex = 91, InitialPlayerValue = 5, Maximum = 100,
				Disposition = "Separate source-hidden innate skill. Native skill checks and learning apply; existing skills are never reset. NPC source Wisdom learning floors are retained." });
		Flush();
	}

	// constants.c wis_app[].learn, also used by init_psi_skills for NPC Willpower.
	internal static int SourceWisdomLearningValue(int wisdom)
	{
		if (wisdom >= 100) return 100;
		int[] values = [0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 38, 44, 50, 56, 62, 68, 74, 80, 86, 92];
		return values[Math.Clamp(wisdom, 0, values.Length - 1)];
	}

	private void InstallMulRage()
	{
		var mul = _db.Races.SingleOrDefault(x => x.Name == "Mul")
			?? throw new InvalidOperationException("Mul Rage requires the installed Mul race.");
		var willpower = _willpowerSkill ?? throw new InvalidOperationException("Mul Rage requires its separate Willpower skill.");
		var thodeliv = _db.SeederManagedRecords.Local.Single(x => x.StableKey == "spice.drug.147" &&
			x.Seeder == ArmageddonWorldfileSeeder.PackageName).LogicalId
			?? throw new InvalidOperationException("Mul Rage requires the imported Thodeliv drug.");
		var character = Prog("ArmageddonWorldfileMulRageCharacter", $$"""
			if (not(@actor.isawake) or isincorporeal(@actor) or isnull(@actor.location))
				return false
			end if
			var willpower = totrait({{willpower.Id}})
			if (not(@actor.skills.Any(skill, @skill == @willpower)))
				givetrait(@actor, @willpower, 5)
			end if
			if (@actor.race != torace({{mul.Id}}))
				return false
			end if
			if (hasdrugdosage(@actor, todrug({{thodeliv}}), ""))
				removecombatfrenzyeffect(@actor)
				return false
			end if
			if (not(hascombatfrenzyeffect(@actor)) and @actor.incombat)
				var remaining = healthremaining(@actor, "health")
				var maximum = healthcapacity(@actor, "health")
				var upper = max(0, floor(@remaining * 2000 / @maximum))
				if (random(0, @upper) == 0)
					if (check(@actor, @willpower, 5) < 0)
						var frenzy = addcombatfrenzyeffect(@actor, 600, 15, 15, 5, 4)
						if (not(isnull(@frenzy)))
							send(@actor, "You feel a deep rage boil to the surface, and bloodlust takes control.")
						end if
					end if
				end if
			end if
			if (not(hascombatfrenzyeffect(@actor)) or @actor.npc or @actor.incombat)
				return false
			end if
			var chosen = false
			foreach (target in @actor.location.characters)
				if (not(@chosen) and @target != @actor and not(isadmin(@target)) and cansee(@actor, @target))
					if (random(0, 3) == 0)
						var keyword = bestkeyword(@actor, @target)
						if (not(isnull(@keyword)))
							chosen = true
							force @actor ("kill " + @keyword)
						end if
					end if
				end if
			end foreach
			return @chosen
			""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		var controller = Prog("ArmageddonWorldfileMulRage", $$"""
			foreach (actor in loadedcharacters())
				@{{character.FunctionName}}(@actor)
			end foreach
			return true
			""", ProgVariableTypes.Boolean);
		var activeFrenzy = Prog("ArmageddonWorldfileMulRageApplicability",
			"return hascombatfrenzyeffect(@actor)", ProgVariableTypes.Boolean,
			("actor", ProgVariableTypes.Character));
		var rageMerit = new Merit
		{
			Id = Next<Merit>(), Name = "Mul Rage", Type = "IgnoreHitPointUnconsciousness", MeritType = 0, MeritScope = 2,
			Definition = new XElement("Merit", new XElement("ChargenAvailableProg", _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", activeFrenzy.Id), new XElement("ChargenBlurb", "Ignore hit point unconsciousness while in a combat frenzy."),
				new XElement("DescriptionText", "@ ignore|ignores hit point unconsciousness while consumed by combat rage.")).ToString()
		};
		_db.Merits.Add(rageMerit);
		if (!_db.RacesMerits.Any(x => x.RaceId == mul.Id && x.MeritId == rageMerit.Id))
			_db.RacesMerits.Add(new RacesMerits { RaceId = mul.Id, MeritId = rageMerit.Id });
		var schedule = ScheduleSourceWeatherUpdate(controller, "Armageddon Mul Rage and innate Willpower");
		Record("racial.mul-rage", nameof(ProgSchedule), schedule.Id, null, new
		{
			Race = mul.Id, Willpower = willpower.Id, SuppressingDrug = thodeliv, IgnoreHitPointUnconsciousnessMerit = rageMerit.Id,
			SourceRealSecondsPerCheck = 300,
			DurationRealSeconds = 600, NativeMinutesPerCheck = schedule.IntervalModifier,
			Disposition = "Source health-scaled random trigger and PC-only unallied aggression; normal native Willpower skill checks and learning. Thodeliv suppresses/cancels. Native stamina costs remain; no distinct daze state is invented."
		});
		ConfigureMulRageSpells(mul.Id, thodeliv);
	}

	private void ConfigureMulRageSpells(long raceId, long drugId)
	{
		foreach (var (key, fury) in new[] { ("arm.spell.roused_fury", true), ("arm.spell.still_anger", false) })
		{
			var id = OwnedBehaviourId(key, nameof(MagicSpell));
			if (id is null) continue;
			var spell = _db.MagicSpells.Single(x => x.Id == id.Value);
			var original = spell.Definition;
			spell.Definition = ConfigureMulRageSpellDefinition(original, fury, raceId, drugId);
			Record("racial.mul-rage.spell." + key, nameof(MagicSpell), spell.Id, null, new
			{
				StockKey = key, Race = raceId, SuppressingDrug = drugId, OriginalDefinition = original,
				Disposition = fury
					? "Ordinary Fury retained; eligible unsuppressed recipients additionally receive independent, additive-duration Combat Frenzy."
					: "Calm clears captured Combat Frenzy after successful Fury countering, even when the residual Calm is saved."
			});
		}
	}

	internal static string ConfigureMulRageSpellDefinition(string definition, bool fury, long raceId, long drugId)
	{
		if (raceId <= 0 || drugId <= 0) throw new ArgumentOutOfRangeException(nameof(raceId), "Existing race and drug IDs are required.");
		var root = XElement.Parse(definition);
		var profiles = root.Descendants("Effect").Where(x => (string?)x.Attribute("type") == (fury ? "sourcefury" : "sourcecalm"))
			.Elements("SourceProfile").ToArray();
		if (profiles.Length != 1) throw new InvalidOperationException("The installed emotional spell must contain exactly one native source profile.");
		var profile = profiles[0];
		if (fury)
		{
			profile.Elements("Frenzy").Remove();
			profile.Add(new XElement("Frenzy", new XAttribute("race", raceId), new XAttribute("suppressiondrug", drugId),
				new XAttribute("offence", 15), new XAttribute("defence", 15), new XAttribute("meleedamage", 5),
				new XAttribute("hunger", 4), new XAttribute("additive", true)));
		}
		else profile.SetAttributeValue("clearfrenzy", true);
		return root.ToString();
	}
}
