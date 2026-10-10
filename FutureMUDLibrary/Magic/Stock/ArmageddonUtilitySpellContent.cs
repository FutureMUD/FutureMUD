#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;
namespace MudSharp.Magic;

/// <summary>Pure row and definition contribution shared by runtime builders and the optional installer.</summary>
public sealed record ArmageddonUtilitySpellContent(string Key, string Name, string Description,
	string DurationFormula, double MinimumEnergy, string Emote, Func<long, long, long, XElement> BuildDefinition,
	string? EligibilitySource = null, ProgVariableTypes? TargetType = null,
	long? ResistingTraitId = null, Difficulty? ResistingDifficulty = null)
{
	public Db.MagicSpell SpellRow(long school, long trait, long knownProg) => new()
	{
		Name = Name, MagicSchoolId = school, CastingTraitDefinitionId = trait, SpellKnownProgId = knownProg,
		Blurb = Description.Split('.').First() + ".", Description = Description,
		CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
		ResistingTraitDefinitionId = ResistingTraitId, ResistingDifficulty = (int?)ResistingDifficulty,
		CastingEmote = Emote, FailCastingEmote = "$0 fail|fails to shape the enchantment.", TargetEmote = "",
		TargetNullEmote = "The enchantment finds no suitable target.", TargetResistedEmote = "",
		AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
	};
	public Db.TraitExpression DurationRow(long spell) => new() { Name = $"{Name} #{spell} duration", Expression = DurationFormula };
	public Db.TraitExpression CostRow(long spell) => new() { Name = $"{Name} #{spell} energy", Expression = $"{MinimumEnergy.ToString(CultureInfo.InvariantCulture)}*grade" };
	public Db.FutureProg? EligibilityRow(long spell)
	{
		if (EligibilitySource is null) return null;
		var row = new Db.FutureProg
		{
			FunctionName = $"armutility_{spell}_eligibility", FunctionText = EligibilitySource,
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionComment = "Editable stock terrain mapping; target then caster.",
			Category = "Magic", Subcategory = Name, Public = false
		};
		row.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "target", ParameterTypeDefinition = (TargetType ?? ProgVariableTypes.Character).ToStorageString() });
		row.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		return row;
	}
	public static XElement Definition(string key, string trigger, long resource, long cost, long filter,
		int opening, double minimum, XElement effect) => new("Definition", new XElement("StockIdentity", key),
		new XElement("Trigger", new XAttribute("type", trigger), new XElement("MinimumPower", (int)SpellPower.ExtremelyWeak),
			new XElement("MaximumPower", (int)SpellPower.ExtremelyStrong), new XElement("TargetFilterProg", filter),
			trigger == "character" ? new XElement("CanTargetSelf", true) : null),
		new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource), new XAttribute("expression", cost))),
		new XElement("Effects", effect), new XElement("CasterEffects"), new XElement("Plan"),
		new XElement("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", 1), new XAttribute("overreachCost", 1.5),
			new XAttribute("overreachDifficulty", 1), new XAttribute("masteryChance", 0.25), new XAttribute("masterySeconds", 600),
			new XAttribute("skillSeconds", 60), new XAttribute("openingSkill", opening),
			new XElement("Efficiency", new XAttribute("type", "source"), new XAttribute("minimum", minimum), new XAttribute("scale", 1)),
			new[] { 0, 20, 40, 55, 70, 85, 95 }.Select((skill, index) => new XElement("Grade", new XAttribute("number", index + 1),
				new XAttribute("power", (int)SpellPower.ExtremelyWeak + index), new XAttribute("skill", skill), new XAttribute("difficulty", index / 2))),
			new XElement("ScalarBindings"), new XElement("Practice", new XAttribute("schema", 1), new XAttribute("enabled", true),
				new XAttribute("difficulty", (int)Difficulty.Normal), new XAttribute("seconds", 30), new XAttribute("energy", 1),
				new XAttribute("speech", true), new XAttribute("hand", true), new XAttribute("movement", false), new XElement("Plan"))));
}
