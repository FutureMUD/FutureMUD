using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;

#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class EnvironmentalExposureSeeder
{
	private void InstallPreparations(bool fantasy)
	{
		var school = Owned("school:preparations", "preparations", new MagicSchool
		{
			Name = "Exposure Preparations", SchoolVerb = "exposurepreparation", SchoolAdjective = "alchemical", PowerListColour = "green"
		}, () => _context.MagicSchools.FirstOrDefault(x => x.Name == "Exposure Preparations"));
		var known = Owned("prog:preparation-not-castable", "preparations", new FutureProg
		{
			FunctionName = "ExposurePreparationIsNotCastable", FunctionText = "return false", ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionComment = "Reagent-delivered preparations grant no casting knowledge.", Category = "Magic", Subcategory = "Exposure", Public = false,
			FutureProgsParameters =
			[
				new FutureProgsParameter { ParameterIndex = 0, ParameterName = "character", ParameterType = (long)ProgVariableTypes.Character },
				new FutureProgsParameter { ParameterIndex = 1, ParameterName = "spell", ParameterType = (long)ProgVariableTypes.MagicSpell }
			]
		}, () => _context.FutureProgs.FirstOrDefault(x => x.FunctionName == "ExposurePreparationIsNotCastable"));
		if (school is null || known is null) return;
		if (!fantasy)
		{
			Preparation("alchemical burn salve", "A heat-resistant alchemical salve", false, false, ExposureRoute.LiquidContact | ExposureRoute.GasContact | ExposureRoute.AmbientHeat, "heat", DrugVector.Touched, SubstanceLifecycle.Maintained);
			Preparation("alchemical respirant draught", "An alchemical preparation for respiratory irritation", false, false, ExposureRoute.Inhalation, "chemical", DrugVector.Ingested, SubstanceLifecycle.Activation);
		}
		else
		{
			Preparation("elemental ward tincture", "A magical ward for exposed skin", true, false, ExposureRoute.LiquidContact | ExposureRoute.GasContact | ExposureRoute.AmbientHeat, "*", DrugVector.Touched, SubstanceLifecycle.Activation);
			Preparation("vessel ward tincture", "A magical preparation for an item's surfaces", true, true, ExposureRoute.LiquidContact | ExposureRoute.GasContact | ExposureRoute.AmbientHeat, "*", DrugVector.Touched, SubstanceLifecycle.Maintained);
			Preparation("breath ward draught", "A magical preparation for respiratory exposure", true, false, ExposureRoute.Inhalation, "*", DrugVector.Ingested, SubstanceLifecycle.Activation);
		}
		return;

		void Preparation(string name, string description, bool magical, bool item, ExposureRoute routes, string category, DrugVector vector, SubstanceLifecycle lifecycle)
		{
			var carrier = Liquid(name, magical ? "fantasy-preparations" : "natural-preparations");
			if (carrier is null) return;
			var spell = Owned("spell:" + name, "preparations", new MagicSpell
			{
				Name = description, Blurb = description, Description = description + ". Reduces injury to one quarter; does not stop physical corrosion, supply oxygen or protect possessions worn by a character.",
				MagicSchoolId = school.Id, SpellKnownProgId = known.Id, AppliedEffectsAreExclusive = true, MinimumSuccessThreshold = 1,
				CastingEmote = "", FailCastingEmote = "", TargetEmote = magical ? "A faint shimmer settles over $0." : "", TargetResistedEmote = "", TargetNullEmote = "",
				Definition = new XElement("Spell", new XElement("Trigger", new XAttribute("type", item ? "substanceitem" : "substancecharacter")),
					new XElement("Costs"), new XElement("Effects", new XElement("Effect", new XAttribute("type", "exposureresistance"),
						new XElement("Multiplier", 0.25), new XElement("Routes", (int)routes), new XElement("Category", category), new XElement("Part", 0))),
					new XElement("CasterEffects"), new XElement("Plan")).ToString()
			}, () => _context.MagicSpells.FirstOrDefault(x => x.Name == description));
			if (spell is null) return;
			Owned("substance:" + name, "preparations", new MagicalSubstance
			{
				Name = name,
				Definition = new XElement("Substance", new XAttribute("vectors", (int)vector), new XAttribute("power", (int)SpellPower.Standard),
					new XAttribute("reference", 0.01 / _litresPerBase), new XAttribute("clearance", 0.0001 / _litresPerBase),
					new XElement("Binding", new XAttribute("carrier", (int)SubstanceCarrier.Liquid), new XAttribute("id", carrier.Id), new XAttribute("quantity", 1)),
					new XElement("Entry", new XAttribute("key", StableId("preparation:" + name)), new XAttribute("spell", spell.Id),
						new XAttribute("lifecycle", (int)lifecycle), new XAttribute("pulse", (int)SubstancePulseMode.Presence), new XAttribute("stack", (int)SubstanceStacking.Aggregate),
						new XAttribute("scale", (int)SubstanceScaling.Duration), new XAttribute("minimum", 0.1), new XAttribute("maximum", 1),
						new XAttribute("duration", 300), new XAttribute("durationcap", 300), new XAttribute("interval", 10))).ToString()
			}, () => _context.MagicalSubstances.FirstOrDefault(x => x.Name == name));
		}
	}
}
