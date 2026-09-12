using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

internal sealed class VancianTestClock : TimeProvider
{
	public DateTimeOffset Now { get; private set; } = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
	public override DateTimeOffset GetUtcNow() => Now;
	public void Advance(TimeSpan duration) => Now += duration;
}

internal sealed class MemoryVancianStore : IVancianStateStore
{
	public Dictionary<(long, long), VancianCapabilityState> States { get; } = [];
	public Dictionary<Guid, VancianOperation> Log { get; } = [];
	public int Writes { get; private set; }
	public Action<VancianOperation?>? AfterCommit { get; set; }
	public Action<VancianOperation>? BeforeRecord { get; set; }
	public VancianCapabilityState Read(long owner, long capability) => States.TryGetValue((owner, capability), out var state) ? state.Copy() : new() { OwnerId = owner, CapabilityId = capability };
	public void Commit(VancianCapabilityState state, long expectedVersion, VancianOperation? operation = null)
	{
		if (Read(state.OwnerId, state.CapabilityId).Version != expectedVersion) throw new InvalidOperationException("Stale version");
		state.Version = expectedVersion + 1; States[(state.OwnerId, state.CapabilityId)] = state.Copy(); Writes++;
		if (operation is not null) Record(operation);
		AfterCommit?.Invoke(operation);
	}
	public IReadOnlyList<VancianOperation> Operations(long owner, long? capability = null) => Log.Values.Where(x => x.OwnerId == owner && (!capability.HasValue || x.CapabilityId == capability)).OrderByDescending(x => x.CreatedUtc).ToArray();
	public IReadOnlyList<VancianOperation> ReservedOperations(long owner, long capability) => Operations(owner, capability).Where(x => x.Status == "Reserved").ToArray();
	public VancianOperation? Operation(Guid id) => Log.GetValueOrDefault(id);
	public bool HasUnresolved(long owner, long capability, params string[] kinds) => Log.Values.Any(x => x.OwnerId == owner && x.CapabilityId == capability &&
		(kinds.Length == 0 || kinds.Contains(x.Kind)) && (x.Status is "Pending" or "Invoking" or "NeedsReview" or "Committing" or "Reserved" ||
			x.Status == "Consumed" && x.Kind is "Cast" or "ScrollActivation"));
	public void Record(VancianOperation operation) { BeforeRecord?.Invoke(operation); Log[operation.Id] = operation; }
	public bool ItemConsumed(long itemId, Guid chargeId) => Log.TryGetValue(chargeId, out var operation) && operation.SourceItem == itemId && operation.Status is "Committing" or "Consumed" or "Completed" or "NeedsReview";
	public bool ClaimCharge(VancianOperation operation) => Log.TryAdd(operation.Id, operation with { Status = "Consumed" });
}

internal sealed class VancianTestFixture
{
	public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<ICharacterInstance> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
	public Mock<IVancianMagicCapability> Capability { get; } = new();
	public Mock<IMagicSchool> School { get; } = new();
	public Mock<ITraitDefinition> Trait { get; } = new();
	public List<IMagicSpell> Spells { get; } = [];
	public List<VancianRepertoireDefinition> Rules { get; } = [];
	public List<VancianCastingAllowanceDefinition> Allowances { get; } = [];
	public Dictionary<string, long> Policies { get; } = [];
	public List<Mock<IFutureProg>> Progs { get; } = [];
	public MemoryVancianStore Store { get; } = new();
	public VancianTestClock Clock { get; } = new();
	public VancianMagicService Service { get; }
	public int Count { get; set; } = 2;
	public int Limit { get; set; } = 3;
	public bool BookAccessible { get; set; } = true;
	public bool HasCapability { get; set; } = true;
	public Action<IGameItemComponent>? Persisted { get; set; }
	public VancianTimedAction? Action { get; private set; }
	public VancianTestFixture(bool actualBooks = false)
	{
		School.SetupGet(x => x.Id).Returns(1); School.SetupGet(x => x.Name).Returns("Arcane"); School.SetupGet(x => x.SchoolVerb).Returns("arcane");
		Trait.SetupGet(x => x.Id).Returns(1);
		World.SetupGet(x => x.MagicSchools).Returns(Collection<IMagicSchool>(() => [School.Object]));
		World.SetupGet(x => x.MagicSpells).Returns(Collection(() => Spells));
		World.SetupGet(x => x.MagicCapabilities).Returns(Collection<IMagicCapability>(() => [Capability.Object]));
		World.SetupGet(x => x.Traits).Returns(Collection<ITraitDefinition>(() => [Trait.Object]));
		World.SetupGet(x => x.TraitExpressions).Returns(Collection<ITraitExpression>(() => []));
		World.SetupGet(x => x.FutureProgs).Returns(Collection(() => Progs.Select(x => x.Object)));
		Actor.SetupGet(x => x.Id).Returns(10); Actor.SetupGet(x => x.Name).Returns("Tester");
		Actor.SetupGet(x => x.GetObject).Returns(() => Actor.Object); Actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		Actor.SetupGet(x => x.Gameworld).Returns(World.Object); Actor.SetupGet(x => x.Identity).Returns(() => null!);
		Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		Actor.SetupGet(x => x.Combat).Returns(() => null!); Actor.SetupGet(x => x.Movement).Returns(() => null!);
		Actor.SetupGet(x => x.Capabilities).Returns(() => HasCapability ? [Capability.Object] : []);
		Actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns((Type type) => CultureInfo.InvariantCulture.GetFormat(type)!);
		Actor.SetupGet(x => x.ContextualItems).Returns([]);
		Actor.Setup(x => x.EffectsOfType<VancianTimedAction>()).Returns(() => Action is null ? [] : [Action]);
		Actor.Setup(x => x.AddEffect(It.IsAny<VancianTimedAction>(), It.IsAny<TimeSpan>())).Callback<MudSharp.Effects.IEffect, TimeSpan>((effect, _) => Action = (VancianTimedAction)effect);
		Actor.Setup(x => x.RemoveEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<bool>())).Callback<MudSharp.Effects.IEffect,bool>((effect, fire) =>
		{
			if (!ReferenceEquals(effect, Action)) return;
			Action = null;
			if (fire) effect.RemovalEffect();
		});
		Capability.SetupGet(x => x.Id).Returns(20); Capability.SetupGet(x => x.Name).Returns("Wizard"); Capability.SetupGet(x => x.School).Returns(School.Object);
		Capability.SetupGet(x => x.GetObject).Returns(() => Capability.Object); Capability.SetupGet(x => x.Type).Returns(ProgVariableTypes.MagicCapability);
		Capability.SetupGet(x => x.Gameworld).Returns(World.Object); Capability.SetupGet(x => x.Repertoires).Returns(() => Rules);
		Capability.SetupGet(x => x.Allowances).Returns(() => Allowances); Capability.SetupGet(x => x.PolicyProgs).Returns(() => Policies);
		Capability.SetupGet(x => x.BasePower).Returns(SpellPower.Standard); Capability.SetupGet(x => x.PowerStepPerSlotLevel).Returns(1);
		Capability.SetupGet(x => x.ReliableOutcome).Returns(Outcome.Pass); Capability.SetupGet(x => x.ScrollMinimumOutcome).Returns(Outcome.MinorPass);
		Capability.SetupGet(x => x.ScrollCheckTrait).Returns(Trait.Object); Capability.SetupGet(x => x.MaximumSavedLoadouts).Returns(10);
		Capability.SetupGet(x => x.RecoveryMode).Returns(VancianRecoveryMode.PreparationAction);
		Capability.SetupGet(x => x.PreparationDuration).Returns(TimeSpan.FromSeconds(5)); Capability.SetupGet(x => x.RequiredSleepDuration).Returns(TimeSpan.FromSeconds(10));
		Capability.Setup(x => x.ConfigurationErrors()).Returns([]);
		Policies["casterlevel"] = Prog("casterlevel", _ => 3).Object.Id;
		Policies["canchangeknown"] = Prog("canchangeknown", _ => true).Object.Id;
		var candidate = Prog("candidates", _ => true).Object.Id;
		var limit = Prog("limit", _ => Limit).Object.Id;
		var count = Prog("count", _ => Count).Object.Id;
		Rules.Add(new(Guid.NewGuid(), "known", "Known", 0, VancianRepertoireSource.Selected, 0, 6, candidate, limit));
		Allowances.Add(new(Guid.NewGuid(), "first", "First", 0, VancianAllowanceMode.Memorised, 1, [Rules[0].Key], 1, 0, 1, count));
		AddSpell(1, 0); AddSpell(2, 1); AddSpell(3, 1);
		Service = new(World.Object, Store, Clock, actualBooks ? null : (_, _, _) => BookAccessible, component => Persisted?.Invoke(component));
		var services = (System.Runtime.CompilerServices.ConditionalWeakTable<IFuturemud, VancianMagicService>)typeof(VancianMagicService)
			.GetField("Services", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!.GetValue(null)!;
		services.Add(World.Object, Service);
	}
	public Mock<IFutureProg> Prog(string signature, Func<object[], object?> result)
	{
		var prog = new Mock<IFutureProg>(); var id = Progs.Count + 1;
		prog.SetupGet(x => x.Id).Returns(id); prog.SetupGet(x => x.Parameters).Returns(VancianPolicy.Signatures[signature].Parameters);
		prog.SetupGet(x => x.ReturnType).Returns(VancianPolicy.Signatures[signature].Return);
		prog.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object[] args) => result(args)!);
		Progs.Add(prog); return prog;
	}
	public Mock<IMagicSpell> AddSpell(long id, int level)
	{
		var spell = new Mock<IMagicSpell>(); spell.SetupGet(x => x.Id).Returns(id); spell.SetupGet(x => x.Name).Returns($"Spell {id}");
		spell.SetupGet(x => x.School).Returns(School.Object); spell.SetupGet(x => x.SpellLevel).Returns(level); spell.SetupGet(x => x.ReadyForGame).Returns(true);
		var trigger = new Mock<ICastMagicTrigger>(); trigger.SetupGet(x => x.MinimumPower).Returns((SpellPower)0); trigger.SetupGet(x => x.MaximumPower).Returns(SpellPower.RecklesslyPowerful);
		spell.SetupGet(x => x.Trigger).Returns(trigger.Object); Spells.Add(spell.Object); return spell;
	}
	public VancianCapabilityState State => Service.State(Actor.Object, Capability.Object);
	public void Select(params long[] ids) => Assert.IsTrue(Service.CommitKnown(Actor.Object, Capability.Object, State.Version,
		new Dictionary<Guid, IReadOnlyList<long>> { [Rules[0].Key] = ids }).Success);
	public void Plan(params long[] spellIds)
	{
		Assert.IsTrue(Service.EditLoadout(Actor.Object, Capability.Object, "new", "daily").Success);
		for (var i = 0; i < spellIds.Length; i++)
		{
			var spell = Spells.Single(x => x.Id == spellIds[i]); var allowance = Allowances[0];
			var result = Service.EditLoadout(Actor.Object, Capability.Object, "assign", "daily", assignment: new(allowance.Key, allowance.StructuralVersion, i + 1, Rules[0].Key, spell.Id, allowance.SlotLevel!.Value, spell.SpellLevel, VancianPolicy.Power(Capability.Object, spell.SpellLevel, allowance.SlotLevel.Value)));
			Assert.IsTrue(result.Success, result.Message);
		}
		Assert.IsTrue(Service.SelectLoadout(Actor.Object, Capability.Object, "daily").Success);
	}
	public void Refresh()
	{
		var result = Service.RequestRefresh(Actor.Object, Capability.Object); Assert.IsTrue(result.Success, result.Message);
		Assert.IsNotNull(Action); var action = Action; Clock.Advance(TimeSpan.FromSeconds(5)); Action = null; action.ExpireEffect();
	}
	public static IUneditableAll<T> Collection<T>(Func<IEnumerable<T>> values) where T : class, IFrameworkItem
	{
		var collection = new Mock<IUneditableAll<T>>(); collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => values().FirstOrDefault(x => x.Id == id)!);
		collection.Setup(x => x.GetEnumerator()).Returns(() => values().GetEnumerator()); return collection.Object;
	}
}
