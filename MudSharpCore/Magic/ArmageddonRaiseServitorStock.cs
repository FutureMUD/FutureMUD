#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Magic;

/// <summary>A bounded stock spell, independently installable through the normal spell builder.</summary>
public static class ArmageddonRaiseServitorStock
{
	public const string Key = "arm.spell.raise_servitor";
	public const string Name = "Raise Servitor";
	// codedump.c affect_to_char: time(NULL) + RT_ZAL_HOUR * duration - 1; RT_ZAL_HOUR = 600.
	// Fresh Animate Dead affects: charm level+6; summoned level+10. Native selected grade maps to level.
	public const string LifetimeSeconds = "600*(grade+10)-1";
	public const string ControlSeconds = "600*(grade+6)-1";
	public const string ControlEligibilitySource = "return lowercase(@corpse.location.terrain.name) != \"silt\" and lowercase(@corpse.location.terrain.name) != \"shallows\"";
	public static IReadOnlyList<string> Commands { get; } = Array.AsReadOnly(new[]
		{ "follow", "hit", "flee", "stop", "stand", "sit", "kneel", "emote", "get", "drop", "give", "wear", "remove", "wield", "unwield" });

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource)
	{
		if (world.SpellOwnedCorpseAnimations is null || !ReferenceEquals(school.Gameworld, world) ||
			world.Traits.Get(trait.Id) != trait || world.MagicResources.Get(resource.Id) != resource)
			throw new InvalidOperationException("Select a school, casting trait and resource in this world with durable corpse animation enabled.");
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(Name)))
			throw new InvalidOperationException("Raise Servitor already exists in that school; edit or clone it instead.");
		var yes = world.AlwaysTrueProg ?? throw new InvalidOperationException("The always-true support prog is unavailable.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == Name))
			throw new InvalidOperationException("A persisted Raise Servitor already exists in that school.");
		var spellRow = new Db.MagicSpell
		{
			Name = Name, MagicSchoolId = school.Id, CastingTraitDefinitionId = trait.Id, SpellKnownProgId = no.Id,
			Blurb = "Raise a dead body's servitor, with temporary creator command authority.",
			Description = "Animate a final-death corpse. The servitor follows you and accepts permitted in-character orders while your control lasts. " +
				"Control is not granted in Silt or Shallows terrain. Animation lasts longer than command authority. " +
				"When it ends, the same corpse and its possessions return. It collapses on restart or when the animated instance quits. " +
				"This stock spell uses native ordered combat; it does not assign a casting capability automatically.",
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			CastingEmote = "$0 call|calls a borrowed body into motion.", FailCastingEmote = "$0 fail|fails to stir the remains.",
			TargetEmote = "", TargetNullEmote = "The remains do not answer.", TargetResistedEmote = "",
			AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
		};
		FMDB.Context.MagicSpells.Add(spellRow); FMDB.Context.SaveChanges();
		Db.FutureProg Prog(string suffix, string text, params (ProgVariableTypes Type, string Name)[] parameters)
		{
			var row = new Db.FutureProg { FunctionName = $"armservitor_{spellRow.Id}_{suffix}", FunctionText = text,
				ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionComment = "Raise Servitor stock support; freely editable.",
				Category = "Magic", Subcategory = "Raise Servitor", Public = false };
			foreach (var (p, index) in parameters.Select((p, index) => (p, index)))
				row.FutureProgsParameters.Add(new() { ParameterIndex = index, ParameterName = p.Name, ParameterTypeDefinition = p.Type.ToStorageString() });
			var check = new FutureProg.FutureProg(row, world);
			if (!check.Compile()) throw new InvalidOperationException("Stock support prog did not compile: " + check.CompileError);
			FMDB.Context.FutureProgs.Add(row); return row;
		}
		var control = Prog("control", "return cancommandanimation(@servitor, @commander)",
			(ProgVariableTypes.Character, "servitor"), (ProgVariableTypes.Character, "commander"), (ProgVariableTypes.Text, "command"));
		var eligibility = Prog("terrain", ControlEligibilitySource, (ProgVariableTypes.Character, "caster"), (ProgVariableTypes.Item, "corpse"));
		var duration = new Db.TraitExpression { Name = $"Raise Servitor #{spellRow.Id} duration", Expression = LifetimeSeconds };
		var cost = new Db.TraitExpression { Name = $"Raise Servitor #{spellRow.Id} energy", Expression = "7*grade" };
		FMDB.Context.TraitExpressions.AddRange(duration, cost); FMDB.Context.SaveChanges();
		var commandAi = new Db.ArtificialIntelligence { Name = $"Raise Servitor #{spellRow.Id} orders", Type = "Commandable",
			Definition = new XElement("Definition", new XElement("CanCommandProg", control.Id), new XElement("WhyCannotCommandProg", 0),
				new XElement("CommandIssuedEmote", ""), new XElement("BannedCommands"),
				new XElement("IncludedCommands", Commands.Select(x => new XElement("Command", x)))).ToString() };
		var combatAi = new Db.ArtificialIntelligence { Name = $"Raise Servitor #{spellRow.Id} combat", Type = "CombatEnd",
			Definition = new XElement("Definition", new XElement("WillAcceptTruce", yes.Id), new XElement("WillAcceptTargetIncapacitated", yes.Id),
				new XElement("OnOfferedTruce", 0), new XElement("OnTargetIncapacitated", 0), new XElement("OnNoNaturalTargets", 0)).ToString() };
		FMDB.Context.ArtificialIntelligences.AddRange(commandAi, combatAi); FMDB.Context.SaveChanges();
		spellRow.EffectDurationExpressionId = duration.Id;
		spellRow.Definition = Definition(resource.Id, cost.Id, eligibility.Id, commandAi.Id, combatAi.Id).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();

		foreach (var row in new[] { control, eligibility })
		{
			var prog = new FutureProg.FutureProg(row, world); prog.Compile(); world.Add(prog);
		}
		world.Add(new TraitExpression(duration, world)); world.Add(new TraitExpression(cost, world));
		world.Add(ArtificialIntelligenceBase.LoadIntelligence(commandAi, world));
		world.Add(ArtificialIntelligenceBase.LoadIntelligence(combatAi, world));
		var result = new MagicSpell(spellRow, world); world.Add(result); return result;
	}

	internal static XElement Definition(long resource, long costExpression, long eligibility, long commands, long combat) => new("Definition",
		new XElement("StockIdentity", Key),
		new XElement("Trigger", new XAttribute("type", "item"), new XElement("MinimumPower", (int)SpellPower.ExtremelyWeak),
			new XElement("MaximumPower", (int)SpellPower.ExtremelyStrong), new XElement("TargetFilterProg", 0)),
		new XElement("Costs", new XElement("Cost", new XAttribute("resource", resource), new XAttribute("expression", costExpression))),
		new XElement("Effects", new XElement("Effect", new XAttribute("type", "animatecorpse"),
			new XElement("AllowPCs", true), new XElement("AllowNPCs", true), new XElement("AllowAdmins", false),
			new XElement("AllowFinal", true), new XElement("AllowSkeletal", false),
			new XElement("ArtificialIntelligences", new XElement("AI", commands), new XElement("AI", combat)),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"),
				new XElement("Family", "raise-servitor"), new XElement("Seconds", LifetimeSeconds), new XElement("FollowCaster", true),
				new XElement("Control", new XElement("Seconds", ControlSeconds), new XElement("EligibilityProg", eligibility))))),
		new XElement("CasterEffects"), new XElement("Plan"),
		new XElement("ControlledPower", new XAttribute("schema", 1), new XAttribute("version", 1),
			new XAttribute("overreachCost", 1.5), new XAttribute("overreachDifficulty", 1), new XAttribute("masteryChance", 0.25),
			new XAttribute("masterySeconds", 600), new XAttribute("skillSeconds", 60), new XAttribute("openingSkill", 30),
			new XElement("Efficiency", new XAttribute("type", "source"), new XAttribute("minimum", 7), new XAttribute("scale", 1)),
			new[] { 0, 20, 40, 55, 70, 85, 95 }.Select((skill, index) => new XElement("Grade", new XAttribute("number", index + 1),
				new XAttribute("power", (int)SpellPower.ExtremelyWeak + index), new XAttribute("skill", skill), new XAttribute("difficulty", index / 2))),
			new XElement("ScalarBindings")));
}
