#nullable enable
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class CheckLearningGlobals : IDisposable
	{
		private readonly FieldInfo _overwork = typeof(Character).GetField("_overworkExpression", BindingFlags.NonPublic | BindingFlags.Static)!;
		private readonly FieldInfo _bonuses = typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.NonPublic | BindingFlags.Static)!;
		private readonly object? _oldOverwork;
		private readonly object? _oldBonuses;
		internal CheckLearningGlobals()
		{
			_oldOverwork = _overwork.GetValue(null); _oldBonuses = _bonuses.GetValue(null);
			_overwork.SetValue(null, new ExpressionEngine.Expression("0")); _bonuses.SetValue(null, 10);
		}
		public void Dispose() { _overwork.SetValue(null, _oldOverwork); _bonuses.SetValue(null, _oldBonuses); }
	}
	private sealed record CheckLearningReader(string Database, long Owner, long Trait, double Expected, string Case);
	private static int RunCheckLearningReader(string[] args)
	{
		Require(args.Length == 1, "Check learning reader needs one owned receipt.");
		var input = JsonSerializer.Deserialize<CheckLearningReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		using var db = NewIndependentContext(database.ConnectionString);
		var row = db.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == input.Owner && x.TraitDefinitionId == input.Trait);
		Require(row.Value == input.Expected, "Cold independent reader observed an uncommitted or missing skill gain.");
		Console.WriteLine($"ARMCheck-learning-reader={input.Case} passed raw:{row.Value} native-character-trait-row no-order-replay");
		return 0;
	}

	private static int RunOrderedNpcCheckLearning(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe,
		Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance, ICharacter, string> order)
	{
		using var globals = new CheckLearningGlobals();
		var world = host.Native.World;
		var definition = (SkillDefinition)world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var previousImprover = definition.Improver;
		var improver = new ClassicImprovement(world, new MudSharp.Models.Improver { Id = 99001, Name = "Check checkpoint use",
			Definition = "<Definition Chance='1' Expression='5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='60'/>" });
		var grantService = world.SpellOwnedCorpseAnimations!;
		var normal = new FutureProg(world, "armCheckNativeGain", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.Trait, "trait")], "return 5");
		var retire = new FutureProg(world, "armCheckNativeRetirement", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.Trait, "trait")],
			"var corpse = kill(@actor, true)\nreturn 5");
		Require(normal.Compile() && retire.Compile(), "Native numeric improvement progs did not compile.");
		var expr = new TraitExpression("1000+variable*0", world);
		var expressions = (All<ITraitExpression>)world.TraitExpressions;
		expr.Id = 99001; expressions.Add(expr);
		StandardCheck Check(CheckType type)
		{
			var template = new MudSharp.Models.CheckTemplate { Name = "Check native checkpoint", ImproveTraits = true, CanBranchIfTraitMissing = false };
			foreach (var difficulty in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
			return new StandardCheck(new MudSharp.Models.Check { Type = (int)type, TraitExpressionId = expr.Id,
				CheckTemplate = template, MaximumDifficultyForImprovement = (int)Difficulty.Impossible }, world);
		}
		var check = Check(CheckType.MeleeWeaponCheck);
		var castCheck = Check(CheckType.CastSpellCheck);
		CheckOutcome? latest = null;
		var change = string.Empty;
		var invoked = 0;
		var commands = NPCCommandTree.Instance.Commands;
		commands.Add(["checkpointlearning"], new Command<ICharacter>((actor, _) =>
		{
			++invoked;
			if (change == "defender") latest = check.Check(foe, Difficulty.Normal, definition);
			else if (change == "casting")
			{
				using var suppression = new CheckImprovementScope(actor);
				latest = castCheck.Check(actor, Difficulty.Normal, definition);
			}
			else latest = check.Check(actor, Difficulty.Normal, definition);
		}, states: CharacterState.Awake, name: "CheckpointLearning"));
		try
		{
			SetPrivateMember(definition, "Improver", improver);
			var configured = new HashSet<CommandableAI>();
			foreach (var scenario in new[] { "valid", "expire", "defender", "autonomous", "casting", "native-retire" })
			{
				change = scenario;
				if (scenario != "valid") animated = cast();
				var actor = animated;
				var ai = actor.AIs.OfType<CommandableAI>().Single();
				if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included checkpointlearning")), "Native command allowlist rejected fixture command.");
				var user = scenario == "defender" ? foe : actor;
				Require(user.SetTraitValue(definition, 40) && user.GetTrait(definition) is Skill, "Native canonical skill setup failed.");
				user.RemoveAllEffects<NoTraitGain>(fireRemovalAction: true);
				var owner = user.GetTrait(definition).Owner as ICharacter ?? throw new InvalidOperationException("Expected native canonical trait owner.");
				Require(scenario == "defender" || !ReferenceEquals(owner, actor), "Animated physical user must share the original canonical owner's skill.");
				void Expire()
				{
					var origin = grantService.CommandGrant(actor.InstanceId, caster.Id)!;
					var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
					var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
					clock.Advance(until - RuntimeClock.UtcNow);
					Require(!grantService.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Check callback did not expire only control.");
				}
				var calls = 0;
				var expiryProg = new Mock<IFutureProg>();
				expiryProg.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Callback(() => { ++calls; Expire(); })
					.Returns<object[]>(args => normal.ExecuteDouble(args));
				improver.ImprovementProg = scenario == "expire" ? expiryProg.Object : scenario == "native-retire" ? retire : normal;
				var before = invoked; latest = null;
				if (scenario == "autonomous")
				{
					Expire();
					((IControllable)actor).ExecuteCommand("checkpointlearning");
				}
				else order(actor, caster, "checkpointlearning");
				Require(invoked == before + 1 && latest is not null && latest.Outcome.IsPass(), "Native ordered scoring lost its rolled outcome or fixture dispatch.");
				var gained = scenario is "valid" or "defender" or "autonomous";
				var expected = gained ? 45.0 : 40.0;
				Require(user.TraitRawValue(definition) == expected && latest.ImprovedTraits.Count() == (gained ? 1 : 0), "Native learning wrote or reported a refused gain.");
				if (scenario == "expire") Require(calls == 1 && actor.EffectsOfType<NoTraitGain>().Count() == 1, "The committed cooldown before improvement prog was lost or duplicated.");
				if (scenario == "native-retire") Require(!actor.IsEmbodied && !grantService.CanCommand(actor.InstanceId, caster.Id), "Compiled improvement prog did not retire the exact physical actor.");
				world.SaveManager.Flush();
				RunItemReaderProcess(new CheckLearningReader(database.Name, owner.Id, definition.Id, expected, scenario), "--check-learning-reader");
				Console.WriteLine($"ARMCheck-learning={scenario} passed physical:{actor.InstanceId} canonical:{owner.Id} raw:{expected} reported:{latest.ImprovedTraits.Count()} rolled:{latest.Outcome} native-skill-save-cold-reader");
				if (scenario != "native-retire") Require(grantService.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
				restored(actor);
			}
		}
		finally { SetPrivateMember(definition, "Improver", previousImprover); }
		return 0;
	}
}
