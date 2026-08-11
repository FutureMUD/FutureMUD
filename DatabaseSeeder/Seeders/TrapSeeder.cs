#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Traps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Installs the stock trap skill, check mappings, and deliberately conservative example trap templates.
/// The package owns only records with the Stock Trap prefix and is safe to rerun.
/// </summary>
public sealed class TrapSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Stock Trap - ";
	private const string TrapComponentRoot = "Trap Components";

	public bool SafeToRunMoreThanOnce => true;
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

	public int SortOrder => 205;
	public string Name => "Trap System Starter Pack";
	public string Tagline => "Adds trap checks, component tags, a Traps skill, and safe example templates";
	public string FullDescription =>
		"Installs the Traps skill, all trap-specific checks, physical component tags, and idempotent stock templates for mechanical, magical, and natural traps. The templates are conservative examples and can be copied and tuned by builders.";

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.CheckTemplates.Any() || !context.TraitDecorators.Any() ||
		    !context.Improvers.Any() || !context.FutureProgs.Any() || !context.Liquids.Any() || !context.Gases.Any())
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		return context.TrapTemplates.Any(x => x.Name.StartsWith(StockPrefix))
			? ShouldSeedResult.MayAlreadyBeInstalled
			: ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		using var transaction = context.Database.BeginTransaction();
		try
		{
			var traps = EnsureTrapsSkill(context);
			EnsureChecks(context, traps);
			var tags = EnsureComponentTags(context);
			EnsureTemplates(context, tags);
			context.SaveChanges();
			transaction.Commit();
			return "Installed or refreshed the stock trap skill, checks, physical component tags, and templates.";
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private static TraitDefinition EnsureTrapsSkill(FuturemudDatabaseContext context)
	{
		var existing = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Traps");
		if (existing is not null)
		{
			return existing;
		}

		var expression = context.TraitExpressions.FirstOrDefault(x => x.Name == "Traps Skill Cap");
		if (expression is null)
		{
			expression = new TraitExpression
			{
				Name = "Traps Skill Cap",
				Expression = "70"
			};
			context.TraitExpressions.Add(expression);
		}

		var alwaysTrue = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue");
		var alwaysFalse = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysFalse");
		var decorator = context.TraitDecorators.FirstOrDefault(x => x.Name == "General Skill") ??
		                context.TraitDecorators.First();
		var improver = context.Improvers.FirstOrDefault(x => x.Name == "Skill Improver") ?? context.Improvers.First();

		var skill = new TraitDefinition
		{
			Name = "Traps",
			Alias = "traps",
			Type = (int)TraitType.Skill,
			OwnerScope = (int)TraitOwnerScope.Body,
			TraitGroup = "Survival",
			DecoratorId = decorator.Id,
			ImproverId = improver.Id,
			Expression = expression,
			AvailabilityProg = alwaysTrue,
			LearnableProg = alwaysTrue,
			TeachableProg = alwaysFalse,
			TeachDifficulty = (int)Difficulty.Normal,
			LearnDifficulty = (int)Difficulty.Normal,
			Hidden = false,
			ChargenBlurb = "The ability to set, spot, avoid, and disarm traps.",
			BranchMultiplier = 1.0,
			DerivedType = 0,
			ValueExpression = string.Empty
		};
		context.TraitDefinitions.Add(skill);
		context.SaveChanges();
		return skill;
	}

	private static void EnsureChecks(FuturemudDatabaseContext context, TraitDefinition traps)
	{
		var template = context.CheckTemplates.FirstOrDefault(x => x.Name == "Skill Check") ??
		               context.CheckTemplates.First();
		foreach (var checkType in new[]
		         {
			         CheckType.SetTrapCheck,
			         CheckType.SpotTrapCheck,
			         CheckType.SearchForTrapCheck,
			         CheckType.AvoidTrapCheck,
			         CheckType.DisarmTrapCheck,
			         CheckType.DispelTrapCheck,
			         CheckType.EscapeTrapCheck
		         })
		{
			var expressionName = $"{checkType.DescribeEnum(true)} Formula";
			var expression = context.TraitExpressions.FirstOrDefault(x => x.Name == expressionName);
			if (expression is null)
			{
				expression = new TraitExpression { Name = expressionName, Expression = $"traps:{traps.Id}" };
				context.TraitExpressions.Add(expression);
			}
			else
			{
				expression.Expression = $"traps:{traps.Id}";
			}

			var check = context.Checks.FirstOrDefault(x => x.Type == (int)checkType);
			if (check is null)
			{
				context.Checks.Add(new Check
				{
					Type = (int)checkType,
					CheckTemplateId = template.Id,
					TraitExpression = expression,
					MaximumDifficultyForImprovement = (int)Difficulty.Impossible
				});
			}
			else
			{
				check.CheckTemplateId = template.Id;
				check.TraitExpression = expression;
				check.MaximumDifficultyForImprovement = (int)Difficulty.Impossible;
			}
		}

		context.SaveChanges();
	}

	private static IReadOnlyDictionary<string, Tag> EnsureComponentTags(FuturemudDatabaseContext context)
	{
		var names = new[]
		{
			"Tripwire Trigger", "Signal Trap Trigger", "Signal Trap Payload", "Explosive Trap Payload", "Pressure Trap Mechanism",
			"Openable Trap Trigger", "Liquid Trap Payload", "Needle Trap Mechanism", "Bear Trap Mechanism",
			"Gas Trap Payload"
		};
		var functions = EnsureTag(context, "Functions", null);
		var root = EnsureTag(context, TrapComponentRoot, functions);
		foreach (var name in names)
		{
			EnsureTag(context, name, root);
		}
		context.SaveChanges();
		return context.Tags.Where(x => names.Contains(x.Name)).ToDictionary(x => x.Name);
	}

	private static Tag EnsureTag(FuturemudDatabaseContext context, string name, Tag? parent)
	{
		var tag = context.Tags.FirstOrDefault(x => x.Name == name);
		if (tag is null)
		{
			tag = new Tag { Name = name, Parent = parent };
			context.Tags.Add(tag);
			context.SaveChanges();
			return tag;
		}
		tag.Parent = parent;
		return tag;
	}

	private static void EnsureTemplates(FuturemudDatabaseContext context, IReadOnlyDictionary<string, Tag> tags)
	{
		var accountId = context.Accounts.OrderBy(x => x.Id).First().Id;
		var now = DateTime.UtcNow;
		var nextId = context.TrapTemplates
			.Select(x => x.Id)
			.AsEnumerable()
			.DefaultIfEmpty(0L)
			.Max() + 1L;
		var liquidId = context.Liquids.Select(x => (long?)x.Id).FirstOrDefault() ?? 0L;
		var gasId = context.Gases.Select(x => (long?)x.Id).FirstOrDefault() ?? 0L;
		var spellId = context.MagicSpells.Select(x => (long?)x.Id).FirstOrDefault() ?? 0L;

		EnsureTemplate(context, ref nextId, accountId, now, "Tripwire Alarm",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Safe,
				Trigger(TrapTriggerType.ExitTraversal),
				[Component(tags["Tripwire Trigger"], TrapComponentRole.Trigger, 85.0), Component(tags["Signal Trap Payload"], TrapComponentRole.Payload, 95.0)],
				Payload(TrapPayloadType.EmitSignal, ("targetitem", "0"), ("value", "1"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Tripwire Explosive",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Risky,
				Trigger(TrapTriggerType.ExitTraversal),
				[Component(tags["Tripwire Trigger"], TrapComponentRole.Trigger, 85.0), Component(tags["Explosive Trap Payload"], TrapComponentRole.Payload, 0.0)],
				Payload(TrapPayloadType.DetonateItem)));
		EnsureTemplate(context, ref nextId, accountId, now, "Pressure Plate",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Risky,
				Trigger(TrapTriggerType.CellEntry),
				[Component(tags["Pressure Trap Mechanism"], TrapComponentRole.TriggerAndPayload, 70.0)],
				Payload(TrapPayloadType.DirectDamage, ("damage", "8"), ("damagetype", "Crushing"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Trapped Chest Liquid Splash",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Safe,
				Trigger(TrapTriggerType.Openable),
				[Component(tags["Openable Trap Trigger"], TrapComponentRole.Trigger, 90.0), Component(tags["Liquid Trap Payload"], TrapComponentRole.Payload, 60.0)],
				Payload(TrapPayloadType.LiquidDischarge, ("liquid", liquidId.ToString()), ("amount", "0.25"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Trapped Chest Needle",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Risky,
				Trigger(TrapTriggerType.Openable),
				[Component(tags["Needle Trap Mechanism"], TrapComponentRole.TriggerAndPayload, 65.0)],
				Payload(TrapPayloadType.DirectDamage, ("damage", "3"), ("damagetype", "Piercing"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Bear Trap",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Risky,
				Trigger(TrapTriggerType.Proximity),
				[Component(tags["Bear Trap Mechanism"], TrapComponentRole.TriggerAndPayload, 80.0)],
				Payload(TrapPayloadType.DirectDamage, ("damage", "10"), ("damagetype", "Piercing")),
				Payload(TrapPayloadType.Restraint, ("duration", "00:00:30"), ("description", "caught in a bear trap"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Spider Web",
			Definition(TrapSourceKind.Natural, TrapDisarmPolicy.Safe,
				Trigger(TrapTriggerType.Proximity),
				[],
				Payload(TrapPayloadType.Restraint, ("duration", "00:00:20"), ("description", "entangled in sticky webbing"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Magical Glyph",
			Definition(TrapSourceKind.Magical, TrapDisarmPolicy.Dispellable,
				Trigger(TrapTriggerType.CellEntry),
				[],
				spellId > 0
					? Payload(TrapPayloadType.CastSpell, ("spell", spellId.ToString()), ("power", "Standard"))
					: Payload(TrapPayloadType.DirectDamage, ("damage", "5"), ("damagetype", "Electrical"))));
		EnsureTemplate(context, ref nextId, accountId, now, "Gas Release",
			Definition(TrapSourceKind.Mechanical, TrapDisarmPolicy.Safe,
				Trigger(TrapTriggerType.Openable),
				[Component(tags["Openable Trap Trigger"], TrapComponentRole.Trigger, 90.0), Component(tags["Gas Trap Payload"], TrapComponentRole.Payload, 50.0)],
				Payload(TrapPayloadType.GasCloud, ("gas", gasId.ToString()), ("duration", "00:00:30"), ("dose", "0.01"))));
	}

	private static void EnsureTemplate(FuturemudDatabaseContext context, ref long nextId, long accountId, DateTime now,
		string name, string definition)
	{
		var fullName = $"{StockPrefix}{name}";
		var template = context.TrapTemplates
			.OrderBy(x => x.Id)
			.ThenBy(x => x.RevisionNumber)
			.FirstOrDefault(x => x.Name == fullName);
		if (template is null)
		{
			template = new TrapTemplate
			{
				Id = nextId++,
				RevisionNumber = 0,
				Name = fullName,
				Definition = definition,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = (int)RevisionStatus.Current,
					BuilderAccountId = accountId,
					ReviewerAccountId = accountId,
					BuilderDate = now,
					ReviewerDate = now,
					BuilderComment = "Stock trap seeder template.",
					ReviewerComment = "Stock trap seeder template."
				}
			};
			context.TrapTemplates.Add(template);
			return;
		}

		template.Definition = definition;
		template.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
		template.EditableItem.ReviewerDate ??= now;
	}

	private static string Definition(TrapSourceKind source, TrapDisarmPolicy disarm, XElement trigger,
		IEnumerable<XElement> components,
		params XElement[] payloads)
	{
		return new XElement("TrapTemplate",
			new XAttribute("source", source),
			new XAttribute("disarm", disarm),
			new XAttribute("lifecycle", TrapLifecyclePolicy.Indefinite),
			new XAttribute("charges", 1),
			new XAttribute("cooldown", TimeSpan.Zero),
			new XElement("Triggers", trigger),
			new XElement("Payloads", payloads),
			new XElement("Components", components)).ToString();
	}

	private static XElement Trigger(TrapTriggerType type, params (string Name, string Value)[] parameters) =>
		new("Trigger",
			new XAttribute("type", type),
			parameters.Select(x => new XElement("Parameter", new XAttribute("name", x.Name), new XCData(x.Value))));

	private static XElement Payload(TrapPayloadType type, params (string Name, string Value)[] parameters) =>
		new("Payload",
			new XAttribute("type", type),
			new XAttribute("delay", TimeSpan.Zero),
			new XAttribute("target", TrapTargetSelector.Triggerer),
			parameters.Select(x => new XElement("Parameter", new XAttribute("name", x.Name), new XCData(x.Value))));

	private static XElement Component(Tag tag, TrapComponentRole role, double recoveryChance) =>
		new("Component",
			new XAttribute("tag", tag.Id),
			new XAttribute("role", role),
			new XAttribute("recovery", recoveryChance),
			new XAttribute("qualityweight", 1.0));
}
