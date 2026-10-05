#nullable enable
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp.Magic;

/// <summary>Transactional construction of ordinary editable stock rows, not a casting implementation.</summary>
internal static class ArmageddonUtilityStock
{
	internal static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource,
		string name, string description, string durationFormula, double minimum, string emote,
		Func<long, long, long, XElement> definition, string? filterSource = null,
		ProgVariableTypes? targetType = null, IFutureProg? existingFilter = null)
	{
		targetType ??= ProgVariableTypes.Character;
		if (!ReferenceEquals(school.Gameworld, world) || world.Traits.Get(trait.Id) != trait ||
			world.MagicResources.Get(resource.Id) != resource || trait.TraitType != TraitType.Skill || trait.OwnerScope != TraitOwnerScope.Character)
			throw new InvalidOperationException("Select an existing school, character skill and magic resource in this world.");
		if (existingFilter is not null && (world.FutureProgs.Get(existingFilter.Id) != existingFilter ||
			existingFilter.ReturnType != ProgVariableTypes.Boolean || !existingFilter.MatchesParameters([targetType.Value, ProgVariableTypes.Character])))
			throw new InvalidOperationException("Select an existing boolean target-eligibility prog with (target, caster) parameters.");
		if (world.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(name)))
			throw new InvalidOperationException($"{name} already exists in that school; edit or clone it instead.");
		var no = world.AlwaysFalseProg ?? throw new InvalidOperationException("The always-false support prog is unavailable.");
		using var scope = FMDB.BeginIndependentScope(requireWrites: true);
		using var db = new FMDB(); using var transaction = FMDB.Context.Database.BeginTransaction();
		if (FMDB.Context.MagicSpells.Any(x => x.MagicSchoolId == school.Id && x.Name == name))
			throw new InvalidOperationException($"A persisted {name} already exists in that school.");
		var row = new Db.MagicSpell {
			Name = name, MagicSchoolId = school.Id, CastingTraitDefinitionId = trait.Id, SpellKnownProgId = no.Id,
			Blurb = description.Split('.').First() + ".", Description = description,
			CastingDifficulty = (int)Difficulty.Normal, MinimumSuccessThreshold = (int)Outcome.MinorPass,
			CastingEmote = emote, FailCastingEmote = "$0 fail|fails to shape the enchantment.", TargetEmote = "",
			TargetNullEmote = "The enchantment finds no suitable target.", TargetResistedEmote = "",
			AppliedEffectsAreExclusive = true, ScrollInscriptionAllowed = false, Definition = "<Definition />"
		};
		FMDB.Context.MagicSpells.Add(row); FMDB.Context.SaveChanges();
		Db.FutureProg? filter = null;
		if (filterSource is not null) {
			filter = new() { FunctionName = $"armutility_{row.Id}_eligibility", FunctionText = filterSource,
				ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionComment = "Editable stock terrain mapping; target then caster.", Category = "Magic", Subcategory = name, Public = false };
			filter.FutureProgsParameters.Add(new() { ParameterIndex=0, ParameterName="target", ParameterTypeDefinition=targetType.Value.ToStorageString() });
			filter.FutureProgsParameters.Add(new() { ParameterIndex=1, ParameterName="caster", ParameterTypeDefinition=ProgVariableTypes.Character.ToStorageString() });
			var check = new FutureProg.FutureProg(filter, world);
			if (!check.Compile()) throw new InvalidOperationException("Stock eligibility prog failed: " + check.CompileError);
			FMDB.Context.FutureProgs.Add(filter);
		}
		var duration = new Db.TraitExpression { Name=$"{name} #{row.Id} duration", Expression=durationFormula };
		var cost = new Db.TraitExpression { Name=$"{name} #{row.Id} energy", Expression=$"{minimum.ToString(System.Globalization.CultureInfo.InvariantCulture)}*grade" };
		FMDB.Context.TraitExpressions.AddRange(duration,cost); FMDB.Context.SaveChanges();
		row.EffectDurationExpressionId = duration.Id;
		row.Definition = definition(resource.Id,cost.Id,filter?.Id ?? existingFilter?.Id ?? 0).ToString();
		FMDB.Context.SaveChanges(); transaction.Commit();
		if (filter is not null) { var prog = new FutureProg.FutureProg(filter,world); prog.Compile(); world.Add(prog); }
		world.Add(new TraitExpression(duration,world)); world.Add(new TraitExpression(cost,world));
		var result = new MagicSpell(row,world); world.Add(result); return result;
	}

	internal static XElement Definition(string key, string trigger, long resource, long cost, long filter,
		int opening, double minimum, XElement effect) => new("Definition", new XElement("StockIdentity",key),
		new XElement("Trigger", new XAttribute("type",trigger), new XElement("MinimumPower",(int)SpellPower.ExtremelyWeak),
			new XElement("MaximumPower",(int)SpellPower.ExtremelyStrong), new XElement("TargetFilterProg",filter),
			trigger == "character" ? new XElement("CanTargetSelf",true) : null),
		new XElement("Costs",new XElement("Cost",new XAttribute("resource",resource),new XAttribute("expression",cost))),
		new XElement("Effects",effect),new XElement("CasterEffects"),new XElement("Plan"),
		new XElement("ControlledPower",new XAttribute("schema",1),new XAttribute("version",1),new XAttribute("overreachCost",1.5),
			new XAttribute("overreachDifficulty",1),new XAttribute("masteryChance",0.25),new XAttribute("masterySeconds",600),
			new XAttribute("skillSeconds",60),new XAttribute("openingSkill",opening),
			new XElement("Efficiency",new XAttribute("type","source"),new XAttribute("minimum",minimum),new XAttribute("scale",1)),
			new[] {0,20,40,55,70,85,95}.Select((skill,index)=>new XElement("Grade",new XAttribute("number",index+1),
				new XAttribute("power",(int)SpellPower.ExtremelyWeak+index),new XAttribute("skill",skill),new XAttribute("difficulty",index/2))),
			new XElement("ScalarBindings"), new XElement("Practice",new XAttribute("schema",1),new XAttribute("enabled",true),
				new XAttribute("difficulty",(int)Difficulty.Normal),new XAttribute("seconds",30),new XAttribute("energy",1),
				new XAttribute("speech",true),new XAttribute("hand",true),new XAttribute("movement",false),new XElement("Plan"))));
}
