#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public sealed partial class PsionicsSeeder : IDatabaseSeeder
{
	public string Name => "Psionics";
	public string Tagline => "Optional Basic and Advanced Psionics, with unassigned capabilities.";
	public string FullDescription => @"Installs two psionic schools for basic and advanced psionics, reminiscent of Armageddon's distinction between universal psionics and psionic class. 

Although this sets up a fully usable suite of powers, regenerators, and capabilities it does not make them accessible. You will need to decide how this applies in your world and wire them in.

Item/cell psychometry remains disabled until static configurable #3EnablePsychometricImpressions#0 is enabled. 
Also see #3VNPCWitnessReportDelaySeconds#0, which defaults to zero; changing to 120 seconds allows time for witness forgetting before reports arrive.";
	public int SortOrder => 304;
	public bool SafeToRunMoreThanOnce => true;
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
	[
		("install-psionics", "Install the optional Basic and Advanced Psionics schools? (yes/no)",
			(_, _) => true,
			(answer, _) => answer.Equals("yes", StringComparison.OrdinalIgnoreCase) || answer.Equals("no", StringComparison.OrdinalIgnoreCase)
				? (true, string.Empty) : (false, "Please answer yes or no."))
	];
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) => (!context.TraitDefinitions.Any(x => x.Type == 0) || !context.Races.Any(x => x.Name == "Human"))
		? ShouldSeedResult.PrerequisitesNotMet : context.MagicSchools.Any(x => x.Name == "Basic Psionics")
			? ShouldSeedResult.MayAlreadyBeInstalled : ShouldSeedResult.ReadyToInstall;

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (questionAnswers.TryGetValue("install-psionics", out var install) && install.Equals("no", StringComparison.OrdinalIgnoreCase))
			return "Psionics installation declined; no content was changed.";
		var human = context.Races.SingleOrDefault(x => x.Name == "Human")
			?? throw new InvalidOperationException("Psionics requires the organic Human race. Install the Human race foundation first.");
		using var transaction = context.Database.BeginTransaction();
		var preserved = new List<string>();
		FutureProg Prog(string suffix, ProgVariableTypes type, string body)
		{
			var name = "Psionics" + suffix;
			var existing = context.FutureProgs.SingleOrDefault(x => x.FunctionName == name);
			if (existing is not null)
			{
				if (existing.ReturnType != (long)type) throw new InvalidOperationException($"Conflicting prog identity: {name}.");
				return existing;
			}
			var prog = new FutureProg { FunctionName = name, FunctionText = body, ReturnType = (long)type,
				FunctionComment = "Stock psionics support. Reruns preserve edits.", Category = "Magic", Subcategory = "Psionics", AcceptsAnyParameters = true, Public = false };
			context.FutureProgs.Add(prog); context.SaveChanges(); return prog;
		}
		var yes = Prog("Allowed", ProgVariableTypes.Boolean, "return true").Id;
		var identity = Prog("TargetKnowsIdentity", ProgVariableTypes.Boolean, "return false").Id;
		var eligibility = Prog("TargetEligible", ProgVariableTypes.Boolean, "return true").Id;
		var no = Prog("NoAutomaticAccess", ProgVariableTypes.Boolean, "return false").Id;
		var error = Prog("Unavailable", ProgVariableTypes.Text, "return \"That psychic action is unavailable.\"").Id;
		var normal = Prog("NormalDifficulty", ProgVariableTypes.Text, "return \"Normal\"").Id;
		var cap = Prog("FocusCap", ProgVariableTypes.Number, $"return {PsionicStockContent.FocusCap}").Id;
		var resource = context.MagicResources.SingleOrDefault(x => x.Name == "Focus");
		if (resource is null)
		{
			if (context.MagicResources.Any(x => x.ShortName == "Focus")) throw new InvalidOperationException("Conflicting resource short name Focus.");
			resource = new MagicResource { Name = "Focus", ShortName = "Focus", Type = "simple", MagicResourceType = 1,
				BottomColour = "red", MidColour = "yellow", TopColour = "cyan", Definition = new XElement("Definition",
					new XElement("ResourceCapProg", cap), new XElement("ShouldStartWithResourceCharacterProg", no), new XElement("StartingResourceAmountCharacterProg", cap)).ToString() };
			context.MagicResources.Add(resource); context.SaveChanges();
		}
		else if (resource.Type != "simple" || resource.MagicResourceType != 1) throw new InvalidOperationException("Conflicting Focus resource identity.");
		var generator = context.MagicGenerators.SingleOrDefault(x => x.Name == "Psionic Focus Regeneration");
		if (generator is null)
		{
			generator = new MagicGenerator { Name = "Psionic Focus Regeneration", Type = "linear", Definition = new XElement("Definition",
				new XElement("WhichResource", resource.Id), new XElement("AmountPerMinute", PsionicStockContent.FocusPerMinute),
				new XElement("ConsciousOnly", true), new XElement("RestMultiplier", 2)).ToString() };
			context.MagicGenerators.Add(generator); context.SaveChanges();
		}
		var trait = context.TraitDefinitions.SingleOrDefault(x => x.Name == "Psionic Discipline");
		if (trait is null)
		{
			var template = context.TraitDefinitions.First(x => x.Type == 0);
			var expression = new TraitExpression { Name = "Psionic Discipline Cap", Expression = "100" };
			context.TraitExpressions.Add(expression); context.SaveChanges();
			trait = new TraitDefinition { Name = "Psionic Discipline", Alias = "psidiscipline", Type = 0, OwnerScope = template.OwnerScope,
				DecoratorId = template.DecoratorId, ImproverId = template.ImproverId, ExpressionId = expression.Id,
				TraitGroup = "Psionics", Hidden = false, ChargenBlurb = "Builder-assigned psychic discipline.", AvailabilityProgId = no,
				LearnableProgId = no, TeachableProgId = no, BranchMultiplier = 0, ValueExpression = "", ShowInScoreCommand = true };
			context.TraitDefinitions.Add(trait); context.SaveChanges();
		}
		else if (trait.Type != 0) throw new InvalidOperationException("Conflicting Psionic Discipline trait identity.");
		foreach (var basic in new[] { true, false })
		{
			var name = basic ? "Basic Psionics" : "Advanced Psionics";
			var verb = basic ? "psi" : "apsi";
			var school = context.MagicSchools.SingleOrDefault(x => x.Name == name);
			if (school is null)
			{
				if (context.MagicSchools.Any(x => x.SchoolVerb == verb)) throw new InvalidOperationException($"Conflicting school verb {verb}.");
				school = new MagicSchool { Name = name, SchoolVerb = verb, SchoolAdjective = "psychic", PowerListColour = "boldmagenta" };
				context.MagicSchools.Add(school); context.SaveChanges();
			}
			var powers = new List<(MagicPower Power, int Band)>();
			foreach (var stock in PsionicStockContent.Powers.Where(x => !basic || x.Basic))
			{
				var powerName = stock.Name;
				var legacyName = $"{name}: {stock.Verb}";
				var power = FindPower(context, school, stock.Verb, powerName, legacyName);
				if (power is null)
				{
					power = new MagicPower { Name = powerName, MagicSchoolId = school.Id, PowerModel = stock.Type, Blurb = stock.Help.Replace("{range}", basic ? "zone" : "shard"), ShowHelp = PsionicPlayerContent.Help(stock.Verb, verb, !basic),
						Definition = PsionicStockContent.Definition(stock, trait.Id, resource.Id, yes, no, error, normal, identity, eligibility, !basic).ToString() };
					context.MagicPowers.Add(power); context.SaveChanges();
				}
				else
				{
					if (power.PowerModel != stock.Type || power.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting power identity: {powerName}.");
					if (power.Name == legacyName) power.Name = powerName;
					if (power.Blurb == stock.LegacyHelp || power.Blurb == "Contact a nearby mind; disconnect ends the maintained link.")
						power.Blurb = stock.Help.Replace("{range}", basic ? "zone" : "shard");
					if (power.ShowHelp == stock.LegacyHelp || power.ShowHelp == "Contact a nearby mind; disconnect ends the maintained link.")
						power.ShowHelp = PsionicPlayerContent.Help(stock.Verb, verb, !basic);
					var definition = XElement.Parse(power.Definition);
					definition.SetElementValue("SeededIdentity", "psionics:" + stock.Verb);
					// Repair only known stock placeholders. Independently edited fields remain intact.
					var desired = PsionicStockContent.Definition(stock, trait.Id, resource.Id, yes, no, error, normal, identity, eligibility, !basic);
					foreach (var element in definition.Elements().ToList())
					{
						if (element.Value is "You feel a mental presence shift." or "The mental barrier shifts." or
						    "You cannot shape the mental impulse." or "You focus your psychic senses." or
						    "Your psychic senses subside." or "You fail to focus your senses." or
						    "You examine the boundaries of your own mind." or "Your mental presence has been noticed." or
						    "Your mental connection is expelled." or "You feel pressure against your connection." or
						    "You project the words: {0}" or "Your words fade unformed.")
							if (desired.Element(element.Name) is { } replacement) element.Value = replacement.Value;
					}
					if (stock.Type == "connectmind")
					{
						if ((string?)definition.Element("PowerDistance") == ((int)MagicPowerDistance.SameLocationOnly).ToString())
							definition.SetElementValue("PowerDistance", desired.Element("PowerDistance")!.Value);
						if ((long?)definition.Element("TargetEligibilityProg") == yes) definition.SetElementValue("TargetEligibilityProg", eligibility);
					}
					if (stock.Type is "connectmind" or "mindsay" && (long?)definition.Element("TargetCanSeeIdentityProg") == yes)
						definition.SetElementValue("TargetCanSeeIdentityProg", identity);
					power.Definition = definition.ToString();
					preserved.Add(powerName);
				}
				powers.Add((power, stock.Band));
			}
			if (!basic) powers.AddRange(InstallSpellPowers(context, school, trait, resource, yes, no, error, human.Id));
			if (!basic) powers.AddRange(InstallCombatPowers(context, school, trait, resource, yes, error));
			var capability = context.MagicCapabilities.SingleOrDefault(x => x.Name == name);
			if (capability is null)
			{
				context.MagicCapabilities.Add(new MagicCapability { Name = name, CapabilityModel = "skilllevel", PowerLevel = basic ? 1 : 2, MagicSchoolId = school.Id,
					Definition = new XElement("Definition", new XElement("ConcentrationTrait", trait.Id), new XElement("ConcentrationCapabilityExpression", "3"),
						new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators", new XElement("Regenerator", generator.Id)),
						powers.Select(x => new XElement("Power", new XAttribute("trait", trait.Id), new XAttribute("minvalue", x.Band), new XAttribute("power", x.Power.Id)))).ToString() });
			}
			else if (capability.CapabilityModel != "skilllevel" || capability.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting capability identity: {name}.");
			else
			{
				var definition = XElement.Parse(capability.Definition);
				foreach (var entry in powers.Where(x =>
					(string?)XElement.Parse(x.Power.Definition).Element("SeededIdentity") == "psionics:connectback" ||
					x.Power.Name.EndsWith(": connectback", StringComparison.Ordinal) ||
					PsionicStockContent.CombatPowers.Any(stock => x.Power.Name == $"{name}: {stock.Name}")))
					if (!definition.Elements("Power").Any(x => (long?)x.Attribute("power") == entry.Power.Id))
						definition.Add(new XElement("Power", new XAttribute("trait", trait.Id), new XAttribute("minvalue", entry.Band), new XAttribute("power", entry.Power.Id)));
				capability.Definition = definition.ToString();
			}
		}
		context.SaveChanges(); transaction.Commit();
		return $"Installed missing psionics definitions; preserved {preserved.Count} existing powers. No characters were granted access. Configure school access explicitly. Psychometric impressions and VNPC reporting delay were not enabled.";
	}

	private static MagicPower? FindPower(FuturemudDatabaseContext context, MagicSchool school, string verb, string name, string legacyName)
	{
		var candidates = context.MagicPowers.Where(x => x.MagicSchoolId == school.Id).ToList()
			.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || x.Name == legacyName ||
				(string?)XElement.Parse(x.Definition).Element("SeededIdentity") == "psionics:" + verb).ToList();
		if (candidates.Count > 1) throw new InvalidOperationException($"Conflicting psionics identities in {school.Name}: {name}.");
		return candidates.SingleOrDefault();
	}

	private static IEnumerable<(MagicPower Power, int Band)> InstallSpellPowers(FuturemudDatabaseContext context, MagicSchool school,
		TraitDefinition trait, MagicResource resource, long yes, long no, long error, long humanRaceId)
	{
		foreach (var (verb, effectType, band, cost, seconds) in PsionicStockContent.SpellPowers)
		{
			var name = $"Advanced Psionics: {verb}";
			var displayName = PsionicPlayerContent.Powers[verb].Name;
			var power = FindPower(context, school, verb, displayName, name);
			var spellId = power is null ? null : (long?)XElement.Parse(power.Definition).Element("Spell");
			var spell = spellId.HasValue ? context.MagicSpells.SingleOrDefault(x => x.Id == spellId.Value) : context.MagicSpells.SingleOrDefault(x => x.Name == name && x.MagicSchoolId == school.Id);
			if (spellId.HasValue && spell is null) throw new InvalidOperationException($"Missing backing spell for {displayName}: {spellId.Value}.");
			var newSpell = spell is null;
			if (spell is null)
			{
				TraitExpression Expression(string suffix, int value)
				{
					var expressionName = name + suffix;
					var expression = context.TraitExpressions.SingleOrDefault(x => x.Name == expressionName);
					if (expression is not null) return expression;
					expression = new TraitExpression { Name = expressionName, Expression = value.ToString(System.Globalization.CultureInfo.InvariantCulture) };
					context.TraitExpressions.Add(expression); context.SaveChanges(); return expression;
				}
				var duration = Expression(" Duration", seconds);
				var castingCost = Expression(" Cost", cost);
				var effect = new XElement("Effect", new XAttribute("type", effectType));
				if (verb == "project")
				{
					effect.Add(new XElement("Race", humanRaceId), new XElement("FormKey", "psionic-astral"));
				}
				if (verb == "illusion") effect.Add(new XElement("Description", "A faint, translucent shimmer veils this figure."), new XElement("AudienceScope", "Caster"), new XElement("OverrideKey", "psionic-example"));
				spell = new MagicSpell { Name = name, Blurb = PsionicPlayerContent.Powers[verb].Description, Description = PsionicPlayerContent.Help(verb, school.SchoolVerb, true),
					MagicSchoolId = school.Id, SpellKnownProgId = no, CastingTraitDefinitionId = trait.Id,
					CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
					ResistingDifficulty = verb == "possess" ? (int)Difficulty.Normal : null, ResistingTraitDefinitionId = verb == "possess" ? trait.Id : null,
					EffectDurationExpressionId = duration.Id, ExclusiveDelay = 5, NonExclusiveDelay = 15, AppliedEffectsAreExclusive = true,
					CastingEmote = PsionicPowerEmotes.Spells[verb]["CastingEmote"],
					FailCastingEmote = PsionicPowerEmotes.Spells[verb]["FailCastingEmote"],
					TargetEmote = PsionicPowerEmotes.Spells[verb]["TargetEmote"],
					TargetResistedEmote = PsionicPowerEmotes.Spells[verb]["TargetResistedEmote"],
					TargetNullEmote = PsionicPowerEmotes.Spells[verb]["TargetNullEmote"],
					Definition = new XElement("Spell", new XElement("Trigger", new XAttribute("type", verb == "possess" ? "character" : "self"),
						new XElement("MinimumPower", (int)SpellPower.Insignificant), new XElement("MaximumPower", (int)SpellPower.Insignificant),
						new XElement("CanTargetSelf", false), new XElement("TargetFilterProg", 0)),
						new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource.Id), new XAttribute("expression", castingCost.Id))),
						new XElement("Effects", effect), new XElement("CasterEffects"), new XElement("Plan")).ToString() };
				context.MagicSpells.Add(spell); context.SaveChanges();
			}
			else if (spell.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting spell identity {name}.");
			const string oldSpellDescription = "Uses ordinary spell targeting, materials, costs, resistance and cleanup. Power access controls spell knowledge.";
			if (spell.Description == oldSpellDescription)
			{
				if (verb == "project")
				{
					var definition = XElement.Parse(spell.Definition);
					var effect = definition.Element("Effects")?.Elements("Effect").FirstOrDefault(x => (string?)x.Attribute("type") == "astralprojection");
					if ((long?)effect?.Element("Race") == context.Races.OrderBy(x => x.Id).Select(x => x.Id).First())
						effect!.SetElementValue("Race", humanRaceId);
					spell.Definition = definition.ToString();
				}
				spell.Description = PsionicPlayerContent.Help(verb, school.SchoolVerb, true);
			}
			if (spell.Blurb == $"A finite psychic {verb} example.") spell.Blurb = PsionicPlayerContent.Powers[verb].Description;
			if (power is null)
			{
				power = new MagicPower { Name = displayName, MagicSchoolId = school.Id, PowerModel = "spellbacked", Blurb = PsionicPlayerContent.Powers[verb].Description,
					ShowHelp = PsionicPlayerContent.Help(verb, school.SchoolVerb, true),
					Definition = new XElement("Definition", new XElement("SeededIdentity", "psionics:" + verb), new XElement("Verb", verb), new XElement("Spell", spell.Id), new XElement("IsPsionic", true),
						new XElement("CanInvokePowerProg", yes), new XElement("WhyCantInvokePowerProg", error), new XElement("InvocationCosts")).ToString() };
				context.MagicPowers.Add(power); context.SaveChanges();
			}
			else if (power.PowerModel != "spellbacked" || power.MagicSchoolId != school.Id) throw new InvalidOperationException($"Conflicting power identity {name}.");
			else
			{
				if (power.Name == name) power.Name = displayName;
				if (power.Blurb == $"A finite psychic {verb} example.") power.Blurb = PsionicPlayerContent.Powers[verb].Description;
				if (power.ShowHelp == $"Use apsi {verb} insignificant followed by normal spell targeting arguments. The spell pays its own costs and maintains its effects.")
					power.ShowHelp = PsionicPlayerContent.Help(verb, school.SchoolVerb, true);
				var definition = XElement.Parse(power.Definition);
				definition.SetElementValue("SeededIdentity", "psionics:" + verb);
				power.Definition = definition.ToString();
			}
			if (newSpell)
			{
				var progName = "PsionicsKnows" + verb;
				var known = context.FutureProgs.SingleOrDefault(x => x.FunctionName == progName);
				if (known is null)
				{
					known = new FutureProg { FunctionName = progName, FunctionText = $"return HasMagicPower(@character, {power.Id})", ReturnType = (long)ProgVariableTypes.Boolean,
						FunctionComment = "Checks builder-granted capability access; grants nothing.", Category = "Magic", Subcategory = "Psionics", Public = false };
					known.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "character", ParameterType = (long)ProgVariableTypes.Character });
					known.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 1, ParameterName = "spell", ParameterType = (long)ProgVariableTypes.MagicSpell });
					context.FutureProgs.Add(known); context.SaveChanges();
				}
				spell.SpellKnownProgId = known.Id;
			}
			yield return (power, band);
		}
	}
}
