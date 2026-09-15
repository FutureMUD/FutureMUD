#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Magic;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicFutureProgFunctionTests
{
	[TestMethod]
	public void MagicFutureProgFunctions_RegisterExpectedSurface()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var names = FutureProg.GetFunctionCompilerInformations()
		                      .Where(x => x.Category.EqualTo("Magic"))
		                      .Select(x => x.FunctionName.ToLowerInvariant())
		                      .ToHashSet();

		foreach (var expected in new[]
		         {
			         "magicresourcelevel",
			         "setmagicresource",
			         "addmagicresource",
			         "subtractmagicresource",
			         "magiccapabilities",
			         "knownspells",
			         "castablespells",
			         "castablespellsnow",
			         "cancastspell",
			         "cancastspellnow",
			         "activespells",
			         "activespelleffects",
			         "spellremainingduration",
			         "spellduration",
			         "setspellduration",
			         "addspellduration",
			         "subtractspellduration",
			         "removespell",
			         "canmagicgather",
			         "beginmagicgather",
			         "completemagicgather",
			         "cancelmagicgather",
			         "magicgatherstatus"
		         })
		{
			Assert.IsTrue(names.Contains(expected), $"Missing FutureProg magic function {expected}.");
		}
	}

	[TestMethod]
	public void GatheringFutureProgFunctions_CompileWithTheirExactDeclaredTypes()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		foreach (MagicGatheringFunction.Contract contract in MagicGatheringFunction.Contracts)
		{
			Tuple<ProgVariableTypes, string>[] parameters = contract.Parameters
				.Select((type, index) => Tuple.Create(type, $"arg{index}"))
				.ToArray();
			string arguments = string.Join(", ", parameters.Select(x => $"@{x.Item2}"));
			string body = contract.ReturnType == ProgVariableTypes.Void
				? $"{contract.Name}({arguments})\nreturn"
				: $"return {contract.Name}({arguments})";
			var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, $"gather_{contract.Name}",
				contract.ReturnType, parameters, body);
			Assert.IsTrue(prog.Compile(), $"{contract.Name}: {prog.CompileError}");
		}
	}

	[TestMethod]
	public void BeginMagicGatherFunction_RoutesACompiledInvocationThroughTheGatheringService()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		Mock<IFuturemud> gameworld = new();
		Mock<IMagicGatheringService> service = new();
		Mock<ICharacter> actor = new();
		Mock<IMagicGatheringCapability> capability = new();
		gameworld.SetupGet(x => x.MagicGathering).Returns(service.Object);
		service.Setup(x => x.Begin(actor.Object, capability.Object, "draw", 1.0))
			.Returns(new MagicGatheringResult(false, "A shared action blocker refused gathering."));

		IFunction function = Compile("beginmagicgather", gameworld.Object,
			Constant(actor.Object, ProgVariableTypes.Character),
			Constant(capability.Object, ProgVariableTypes.MagicCapability),
			Constant("draw", ProgVariableTypes.Text),
			Constant(1.0M, ProgVariableTypes.Number));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(string.Empty, function.Result.GetObject);
		service.Verify(x => x.Begin(actor.Object, capability.Object, "draw", 1.0), Times.Once,
			"Compiled begin calls must share the same service gate as player commands.");
	}

	[TestMethod]
	public void BeginMagicGatherFunction_RespectsTheSharedActionBlockerGate()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		using MagicGatheringServiceTests.GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		fixture.ActorEffects.Add(MagicGatheringServiceTests.BlockingEffect(fixture.Actor.Object, "general",
			"You are already occupied."));

		IFunction function = Compile("beginmagicgather", fixture.World.World.Object,
			Constant(fixture.Actor.Object, ProgVariableTypes.Character),
			Constant(fixture.Capability.Object, ProgVariableTypes.MagicCapability),
			Constant("draw", ProgVariableTypes.Text),
			Constant(1.0M, ProgVariableTypes.Number));

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(string.Empty, function.Result.GetObject);
		Assert.AreEqual(1, fixture.ActorEffects.Count, "The real service must not schedule its gathering action past a blocker.");
		Assert.AreEqual(0, fixture.ReceiptCount);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
	}

	[TestMethod]
	public void MagicResourceFunctions_SetAddSubtractClampAndReturnCurrentLevel()
	{
		var resource = CreateFrameworkItemMock<IMagicResource>(1, "Essence");
		resource.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100.0);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.MagicResources).Returns(CreateCollectionMock(resource.Object).Object);
		var haver = new TestMagicResourceHaver();
		var variables = new Mock<IVariableSpace>().Object;

		var set = Compile("setmagicresourcelevel", gameworld.Object,
			Constant(haver, ProgVariableTypes.MagicResourceHaver),
			Constant("Essence", ProgVariableTypes.Text),
			Constant(150.0M, ProgVariableTypes.Number));

		Assert.AreEqual(StatementResult.Normal, set.Execute(variables));
		Assert.AreEqual(100.0M, set.Result.GetObject);

		var subtract = Compile("subtractmagicresource", gameworld.Object,
			Constant(haver, ProgVariableTypes.MagicResourceHaver),
			Constant(1.0M, ProgVariableTypes.Number),
			Constant(25.0M, ProgVariableTypes.Number));

		Assert.AreEqual(StatementResult.Normal, subtract.Execute(variables));
		Assert.AreEqual(75.0M, subtract.Result.GetObject);
		Assert.AreEqual(75.0, haver.MagicResourceAmounts[resource.Object]);
	}

	[TestMethod]
	public void CanCastSpellFunctions_UseGeneralAndCurrentSpellChecks()
	{
		var character = new Mock<ICharacter>();
		var spell = new Mock<IMagicSpell>();
		var trigger = new Mock<ICastMagicTrigger>();
		var variables = new Mock<IVariableSpace>().Object;

		spell.Setup(x => x.CharacterKnowsSpell(character.Object)).Returns(true);
		spell.SetupGet(x => x.ReadyForGame).Returns(true);
		spell.SetupGet(x => x.Trigger).Returns(trigger.Object);
		spell.Setup(x => x.CharacterCanCast(character.Object, character.Object)).Returns(true);

		var general = Compile("cancastspell", FutureProgTestBootstrap.Gameworld,
			Constant(character.Object, ProgVariableTypes.Character),
			Constant(spell.Object, ProgVariableTypes.MagicSpell));
		var now = Compile("cancastspellnow", FutureProgTestBootstrap.Gameworld,
			Constant(character.Object, ProgVariableTypes.Character),
			Constant(spell.Object, ProgVariableTypes.MagicSpell));

		Assert.AreEqual(StatementResult.Normal, general.Execute(variables));
		Assert.AreEqual(true, general.Result.GetObject);
		Assert.AreEqual(StatementResult.Normal, now.Execute(variables));
		Assert.AreEqual(true, now.Result.GetObject);
	}

	[TestMethod]
	public void SpellDurationFunctions_QueryRescheduleAndRemoveMatchingParents()
	{
		var gameworld = new Mock<IFuturemud>();
		var character = new Mock<ICharacter>();
		var spell = CreateFrameworkItemMock<IMagicSpell>(10, "Ward");
		var caster = new Mock<ICharacter>();
		caster.SetupGet(x => x.Id).Returns(25);
		character.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		character.SetupGet(x => x.Id).Returns(26);
		character.SetupGet(x => x.FrameworkItemType).Returns("Character");
		spell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var parent = new MagicSpellParent(character.Object, spell.Object, caster.Object);
		character.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
		         .Returns<Predicate<MagicSpellParent>>(predicate => new[] { parent }.Where(x => predicate(x)));
		character.Setup(x => x.ScheduledDuration(parent)).Returns(TimeSpan.FromSeconds(12));
		var variables = new Mock<IVariableSpace>().Object;

		var duration = Compile("spellremainingduration", gameworld.Object,
			Constant(character.Object, ProgVariableTypes.Character),
			Constant(spell.Object, ProgVariableTypes.MagicSpell));
		var set = Compile("setspellduration", gameworld.Object,
			Constant(character.Object, ProgVariableTypes.Character),
			Constant(spell.Object, ProgVariableTypes.MagicSpell),
			Constant(TimeSpan.FromSeconds(30), ProgVariableTypes.TimeSpan));
		var remove = Compile("removespell", gameworld.Object,
			Constant(character.Object, ProgVariableTypes.Character),
			Constant(spell.Object, ProgVariableTypes.MagicSpell));

		Assert.AreEqual(StatementResult.Normal, duration.Execute(variables));
		Assert.AreEqual(TimeSpan.FromSeconds(12), duration.Result.GetObject);
		Assert.AreEqual(StatementResult.Normal, set.Execute(variables));
		Assert.AreEqual(1.0M, set.Result.GetObject);
		character.Verify(x => x.Reschedule(parent, TimeSpan.FromSeconds(30)), Times.Once);
		Assert.AreEqual(StatementResult.Normal, remove.Execute(variables));
		Assert.AreEqual(1.0M, remove.Result.GetObject);
		character.Verify(x => x.RemoveEffect(parent, true), Times.Once);
	}

	private static IFunction Compile(string name, IFuturemud gameworld, params IFunction[] parameters)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var types = parameters.Select(x => x.ReturnType).ToList();
		var compiler = FutureProg.GetFunctionCompilerInformations()
		                         .Single(x => x.FunctionName.EqualTo(name) &&
		                                      x.Parameters.SequenceEqual(types, FutureProgVariableComparer.Instance));

		return compiler.CompilerFunction(parameters.ToList(), gameworld);
	}

	private static IFunction Constant(object value, ProgVariableTypes type)
	{
		return new ConstantFunctionStub(new ObjectVariable(value, type));
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var collection = new Mock<IUneditableAll<T>>();
		collection.SetupGet(x => x.Count).Returns(() => list.Count);
		collection.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => list.FirstOrDefault(x => x.Id == id));
		collection.Setup(x => x.GetByName(It.IsAny<string>())).Returns<string>(name =>
			list.FirstOrDefault(x => x.Name.EqualTo(name)));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns<string, bool>((value, _) =>
			long.TryParse(value, out var id)
				? list.FirstOrDefault(x => x.Id == id)
				: list.FirstOrDefault(x => x.Name.EqualTo(value)));
		return collection;
	}

	private static Mock<T> CreateFrameworkItemMock<T>(long id, string name) where T : class, IFrameworkItem
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return mock;
	}

	private sealed class ConstantFunctionStub : IFunction
	{
		public ConstantFunctionStub(IProgVariable result)
		{
			Result = result;
			ReturnType = result.Type;
		}

		public IProgVariable Result { get; private set; }
		public ProgVariableTypes ReturnType { get; }
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;
		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}

	private sealed class ObjectVariable : IProgVariable
	{
		public ObjectVariable(object value, ProgVariableTypes type)
		{
			GetObject = value;
			Type = type;
		}

		public ProgVariableTypes Type { get; }
		public object GetObject { get; }
		public IProgVariable GetProperty(string property) => throw new NotSupportedException();
	}

	private sealed class TestMagicResourceHaver : IHaveMagicResource
	{
		private readonly DoubleCounter<IMagicResource> _resources = new();

		public IEnumerable<IMagicResource> MagicResources => _resources.Keys;
		public IReadOnlyDictionary<IMagicResource, double> MagicResourceAmounts => _resources;
		public IEnumerable<IMagicResourceRegenerator> MagicResourceGenerators => Enumerable.Empty<IMagicResourceRegenerator>();
		public bool CanUseResource(IMagicResource resource, double amount) => _resources[resource] >= amount;
		public bool UseResource(IMagicResource resource, double amount)
		{
			if (!CanUseResource(resource, amount))
			{
				_resources[resource] = 0;
				return false;
			}

			_resources[resource] -= amount;
			return true;
		}

		public void AddResource(IMagicResource resource, double amount)
		{
			_resources[resource] = Math.Max(0.0, Math.Min(resource.ResourceCap(this), _resources[resource] + amount));
		}

		public void AddMagicResourceGenerator(IMagicResourceRegenerator generator)
		{
		}

		public void RemoveMagicResourceGenerator(IMagicResourceRegenerator generator)
		{
		}
	}
}
