#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Resources;
using MudSharp.PerceptionEngine;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Foraging;
using CompiledFutureProg = MudSharp.FutureProg.FutureProg;
using Db = MudSharp.Models;

namespace MudSharp.Testing.EnvironmentalMagic;

/// <summary>
/// Shared deterministic fixture for structural tests and opt-in measurements. Cells, overlays,
/// terrains, resources, generators, forage reads, compiled progs, coordinator and heartbeat are real.
/// Only the external world facade, save sink and explicit-operation store are in-memory substitutes.
/// </summary>
internal sealed class EnvironmentalMagicTestWorld : IDisposable
{
	public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
	public EnvironmentalMagicTestClock Clock { get; }
	public HeartbeatManager Heartbeat { get; }
	public Scheduler Scheduler { get; }
	public EnvironmentalMagicCoordinator Coordinator { get; }
	public EnvironmentalMagicTestOperationStore Operations { get; }
	public EnvironmentalMagicTestRegistry<ICell> Cells { get; } = new();
	public EnvironmentalMagicTestRegistry<IMagicResource> Resources { get; } = new();
	public EnvironmentalMagicTestRegistry<IMagicResourceRegenerator> Profiles { get; } = new();
	public EnvironmentalMagicTestRegistry<IFutureProg> Progs { get; } = new();
	public EnvironmentalMagicTestRegistry<IAgricultureField> Fields { get; } = new();
	public EnvironmentalMagicTestRegistry<ITerrain> Terrains { get; } = new();
	public RevisableAll<IForagableProfile> ForageProfiles { get; } = new();
	public Dictionary<long, Db.Cell> Models { get; } = new();
	public Mock<ISaveManager> Saves { get; } = new();
	public HashSet<ISaveable> PendingSaves { get; } = new();
	public long SaveRequests { get; private set; }
	public long Flushes { get; private set; }
	public long HeartbeatPumps { get; private set; }
	public Terrain Terrain { get; }
	public EnvironmentalMagicGenerator Profile { get; }
	public ICharacter Builder { get; }
	public IReadOnlyList<string> Messages => _messages;
	private readonly List<string> _messages = new();
	private readonly Mock<IRoom> _room = new();
	private readonly Mock<IZone> _zone = new();
	private readonly Mock<ICharacter> _builder = new();
	private readonly Mock<IOutputHandler> _builderOutput = new();
	private long _nextCellId;

	public EnvironmentalMagicTestWorld(int count = 1, int outputs = 1, double activePercent = 100.0,
		string policy = "constant", EnvironmentalMagicOptions? options = null, bool missingBalances = false,
		bool start = true, EnvironmentalMagicTestClock? clock = null, EnvironmentalMagicTestOperationStore? operations = null)
	{
		Clock = clock ?? new EnvironmentalMagicTestClock();
		Operations = operations ?? new EnvironmentalMagicTestOperationStore();
		World.SetupGet(world => world.Cells).Returns(Cells);
		World.SetupGet(world => world.MagicResources).Returns(Resources);
		World.SetupGet(world => world.MagicResourceRegenerators).Returns(Profiles);
		World.SetupGet(world => world.FutureProgs).Returns(Progs);
		World.SetupGet(world => world.AgricultureFields).Returns(Fields);
		World.SetupGet(world => world.Terrains).Returns(Terrains);
		World.SetupGet(world => world.ForagableProfiles).Returns(ForageProfiles);
		World.SetupGet(world => world.HearingProfiles).Returns(new EnvironmentalMagicTestRegistry<IHearingProfile>());
		World.SetupGet(world => world.SaveManager).Returns(Saves.Object);
		World.Setup(world => world.SystemMessage(It.IsAny<string>(), It.IsAny<bool>()))
			.Callback<string, bool>((message, _) => _messages.Add(message));
		Saves.Setup(save => save.Add(It.IsAny<ISaveable>())).Callback<ISaveable>(item =>
		{
			SaveRequests++;
			PendingSaves.Add(item);
		});
		Saves.Setup(save => save.Flush()).Callback(() => Flushes++);
		Saves.Setup(save => save.Abort(It.IsAny<ISaveable>())).Callback<ISaveable>(item => PendingSaves.Remove(item));
		Heartbeat = new HeartbeatManager(World.Object);
		Scheduler = new Scheduler(Clock);
		World.SetupGet(world => world.HeartbeatManager).Returns(Heartbeat);
		World.SetupGet(world => world.Scheduler).Returns(Scheduler);
		var packages = new RevisableAll<ICellOverlayPackage>();
		var package = new Mock<ICellOverlayPackage>();
		package.SetupGet(value => value.Id).Returns(1);
		package.SetupGet(value => value.RevisionNumber).Returns(1);
		package.SetupGet(value => value.Status).Returns(RevisionStatus.Current);
		packages.Add(package.Object);
		World.SetupGet(world => world.CellOverlayPackages).Returns(packages);
		_room.SetupGet(room => room.Gameworld).Returns(World.Object);
		_room.SetupGet(room => room.Zone).Returns(_zone.Object);
		_room.SetupGet(room => room.Areas).Returns(Array.Empty<IArea>());
		_room.SetupGet(room => room.Cells).Returns(() => Cells);
		_zone.SetupGet(zone => zone.Gameworld).Returns(World.Object);
		World.Setup(world => world.Destroy(It.IsAny<ICell>())).Callback<ICell>(cell =>
		{
			Coordinator!.Unregister(cell);
			Cells.Remove(cell.Id);
		});
		_builder.SetupGet(actor => actor.Gameworld).Returns(World.Object);
		_builder.SetupGet(actor => actor.OutputHandler).Returns(_builderOutput.Object);
		_builder.Setup(actor => actor.GetFormat(It.IsAny<Type>())).Returns<Type>(CultureInfo.InvariantCulture.GetFormat);
		var account = new Mock<IAccount>();
		account.SetupGet(value => value.InnerLineFormatLength).Returns(100);
		_builder.SetupGet(actor => actor.Account).Returns(account.Object);
		Builder = _builder.Object;
		for (var i = 1; i <= outputs; i++)
		{
			Resources.Add(new SimpleMagicResource(new Db.MagicResource
			{
				Id = i, Name = $"Resource{i}", ShortName = $"R{i}", Type = "simple",
				MagicResourceType = (int)(MagicResourceType.LocationResource | MagicResourceType.PlayerResource | MagicResourceType.ItemResource),
				Definition = "<Definition/>"
			}, World.Object));
		}
		var definition = ProfileDefinition(outputs);
		if (policy is "native-yield" or "native-yield-two")
		{
			CreateForageProfile(1, 100.0, 10.0, policy == "native-yield-two");
			definition.Element("Inputs")!.Add(Input("native", "Forage", "herbs"));
			foreach (var output in definition.Element("Outputs")!.Elements()) output.SetElementValue("Maximum", "native");
		}
		else if (policy == "compiled-prog")
		{
			CompileProg(1, "if (@where.id > 0)\nreturn 80 + 20\nend if\nreturn 0");
			definition.Element("Inputs")!.Add(Input("policy", "Prog", "1"));
			foreach (var output in definition.Element("Outputs")!.Elements()) output.SetElementValue("Maximum", "policy");
		}
		Profile = AddProfile(1, definition);
		Terrain = AddTerrain(1, Profile.Id, policy is "native-yield" or "native-yield-two" ? 1 : 0);
		Coordinator = new EnvironmentalMagicCoordinator(World.Object, Clock, options ?? new EnvironmentalMagicOptions
		{
			SoftBudgetMilliseconds = 1000.0
		}, Operations);
		World.SetupGet(world => world.EnvironmentalMagic).Returns(Coordinator);
		for (var i = 0; i < count; i++)
		{
			CreateCell(missingBalances ? null : i < count * activePercent / 100.0 ? 0.0 : 100.0);
		}
		if (start) Start();
		ResetSavedFlags();
	}

	public void Start()
	{
		Heartbeat.StartHeartbeatTick();
		Coordinator.Initialise();
	}

	public Cell CreateCell(double? balance = 0.0, EnvironmentalMagicBindingMode binding = EnvironmentalMagicBindingMode.Inherit,
		long? profileId = null, long? id = null)
	{
		var cellId = id ?? ++_nextCellId;
		_nextCellId = Math.Max(_nextCellId, cellId);
		var model = new Db.Cell
		{
			Id = cellId, CurrentOverlayId = cellId, EnvironmentalMagicBindingMode = (int)binding,
			EnvironmentalMagicProfileId = profileId, EffectData = "<Effects/>"
		};
		model.CellOverlays.Add(new Db.CellOverlay
		{
			Id = cellId, CellId = cellId, CellName = $"Test Cell {cellId}", CellDescription = "Test environment.",
			TerrainId = Terrain.Id, CellOverlayPackageId = 1, CellOverlayPackageRevisionNumber = 1,
			AmbientLightFactor = 1.0
		});
		if (balance.HasValue)
			foreach (var resource in Resources)
				model.CellsMagicResources.Add(new Db.CellMagicResource { CellId = cellId, MagicResourceId = resource.Id, Amount = balance.Value });
		return LoadCell(model);
	}

	public Cell LoadCell(Db.Cell model)
	{
		var cell = new Cell(model, _room.Object);
		cell.PostLoadTasks(model);
		Cells.Add(cell);
		Models[cell.Id] = model;
		Operations.Models[cell.Id] = model;
		return cell;
	}

	public Terrain AddTerrain(long id, long? environmentalProfile, long forageProfile = 0)
	{
		var terrain = new Terrain(new Db.Terrain
		{
			Id = id, Name = $"Test Terrain {id}", TerrainBehaviourMode = "outdoors", MovementRate = 1.0,
			EnvironmentalMagicProfileId = environmentalProfile, ForagableProfileId = forageProfile
		}, World.Object);
		Terrains.Add(terrain);
		return terrain;
	}

	public EnvironmentalMagicGenerator AddProfile(long id, XElement definition)
	{
		var profile = (EnvironmentalMagicGenerator)BaseMagicResourceGenerator.LoadFromDatabase(new Db.MagicGenerator
		{
			Id = id, Name = $"Test Profile {id}", Type = "environmental", Definition = definition.ToString()
		}, World.Object);
		Profiles.Add(profile);
		return profile;
	}

	public CompiledFutureProg CompileProg(long id, string text, string name = "EnvironmentalTestPolicy")
	{
		CompiledFutureProg.Initialise();
		var prog = new CompiledFutureProg(World.Object, name + id, ProgVariableTypes.Number,
			new[] { Tuple.Create(ProgVariableTypes.Location, "where") }, text)
		{
			Id = id, StaticType = FutureProgStaticType.NotStatic
		};
		if (!prog.Compile()) throw new InvalidOperationException(prog.CompileError);
		Progs.Add(prog);
		return prog;
	}

	public ForagableProfile CreateForageProfile(long id, double maximum, double recovery, bool includeBerries = false)
	{
		var model = new Db.ForagableProfile
		{
			Id = id, Name = $"Test Forage {id}", RevisionNumber = 1,
			EditableItem = new Db.EditableItem { RevisionStatus = (int)RevisionStatus.Current }
		};
		model.ForagableProfilesMaximumYields.Add(new Db.ForagableProfilesMaximumYields { ForageType = "herbs", Yield = maximum });
		model.ForagableProfilesHourlyYieldGains.Add(new Db.ForagableProfilesHourlyYieldGains { ForageType = "herbs", Yield = recovery });
		if (includeBerries)
		{
			model.ForagableProfilesMaximumYields.Add(new Db.ForagableProfilesMaximumYields { ForageType = "berries", Yield = maximum });
			model.ForagableProfilesHourlyYieldGains.Add(new Db.ForagableProfilesHourlyYieldGains { ForageType = "berries", Yield = recovery });
		}
		var profile = new ForagableProfile(model, World.Object);
		ForageProfiles.Add(profile);
		return profile;
	}

	public void Tick(double seconds = 1.0)
	{
		Clock.Advance(TimeSpan.FromSeconds(seconds));
		Heartbeat.ManuallyFireHeartbeatSecond();
		HeartbeatPumps++;
		// Avoid retaining Moq's external-facade call history for the entire scale run.
		World.Invocations.Clear();
		_room.Invocations.Clear();
		_zone.Invocations.Clear();
		Saves.Invocations.Clear();
	}

	public void RunSeconds(int seconds)
	{
		for (var i = 0; i < seconds; i++) Tick();
	}

	public double Balance(int index = 0, long resourceId = 1) =>
		((Cell)Cells.At(index)).MagicResourceAmounts.GetValueOrDefault(Resources.Get(resourceId)!);

	public void Edit(string command)
	{
		if (!Profile.BuildingCommand(Builder, new StringStack(command)))
			throw new InvalidOperationException($"Rejected environmental test command: {command}");
	}

	public Db.Cell Snapshot(Cell cell)
	{
		var model = Models[cell.Id];
		foreach (var resource in cell.MagicResourceAmounts.Keys)
			if (!model.CellsMagicResources.Any(value => value.MagicResourceId == resource.Id))
				model.CellsMagicResources.Add(new Db.CellMagicResource { CellId = cell.Id, MagicResourceId = resource.Id });
		cell.SaveMagic(model);
		typeof(Cell).GetMethod("SaveEnvironment", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(cell, new object[] { model });
		return model;
	}

	public void ResetSavedFlags()
	{
		foreach (var cell in Cells.Cast<Cell>())
		{
			cell.Changed = false;
			cell.ResourcesChanged = false;
			cell.YieldsChanged = false;
		}
		PendingSaves.Clear();
		SaveRequests = 0;
		Saves.Invocations.Clear();
	}

	public int SecondSubscriptions => InvocationCount(Heartbeat, "_secondHeartbeat");
	public int EnvironmentalDelegateCount => ((IDictionary)typeof(BaseMagicResourceGenerator)
		.GetField("_delegates", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(Profile)!).Count;
	public int SchedulerEntries
	{
		get
		{
			var heap = typeof(Scheduler).GetField("_schedules", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(Scheduler)!;
			return (int)heap.GetType().GetProperty("Count")!.GetValue(heap)!;
		}
	}
	public static int InvocationCount(object owner, string field) =>
		((Delegate?)owner.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(owner))?.GetInvocationList().Length ?? 0;

	public static XElement ProfileDefinition(int outputs = 1, string maximum = "basecapacity", string rate = "baserate") =>
		new("Definition", new XAttribute("version", 1), new XElement("PressureHalfLifeSeconds", 3600.0),
			new XElement("NaturalRepairPerMinute", 0.0),
			new XElement("Outputs", Enumerable.Range(1, outputs).Select(id => new XElement("Output",
				new XAttribute("resource", id), new XAttribute("basecapacity", 100.0), new XAttribute("baserate", 1.0),
				new XElement("Maximum", maximum), new XElement("Rate", rate)))), new XElement("Inputs"));
	public static XElement Input(string name, string kind, string source, double scale = 1.0) =>
		new("Input", new XAttribute("name", name), new XAttribute("kind", kind), new XAttribute("source", source), new XAttribute("scale", scale));
	public void Dispose() => Coordinator.Dispose();
}

internal sealed class EnvironmentalMagicTestClock : TimeProvider
{
	private long _ticks;
	private DateTimeOffset _utc = new(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);
	public int UtcReads { get; private set; }
	public TimeSpan UtcReadStep { get; set; }
	public override long TimestampFrequency => TimeSpan.TicksPerSecond;
	public override long GetTimestamp() => _ticks;
	public override DateTimeOffset GetUtcNow()
	{
		UtcReads++;
		var result = _utc;
		_utc += UtcReadStep;
		return result;
	}
	public void Advance(TimeSpan amount) { _ticks += amount.Ticks; _utc += amount; }
	public void SetUtcNow(DateTimeOffset value) => _utc = value;
}

internal sealed class EnvironmentalMagicTestOperationStore : IEnvironmentalMagicOperationStore
{
	public Dictionary<Guid, LandRejuvenationProgress> Treatments { get; } = new();
	public int TreatmentReads { get; private set; }
	public bool FailTreatmentSave { get; set; }
	public LandRejuvenationProgress? FindTreatment(Guid id)
	{
		TreatmentReads++;
		if (FailRead) throw new InvalidOperationException("Test treatment read failure");
		return Treatments.GetValueOrDefault(id);
	}
	public IReadOnlyList<LandRejuvenationProgress> TreatmentsFor(long cellId)
	{
		TreatmentReads++;
		if (FailRead) throw new InvalidOperationException("Test treatment read failure");
		return Treatments.Values.Where(x => x.CellId == cellId).ToArray();
	}
	public void SaveTreatment(LandRejuvenationProgress progress, long? expectedRevision)
	{
		if (FailTreatmentSave) throw new InvalidOperationException("Test treatment save failure");
		if (Treatments.GetValueOrDefault(progress.Id)?.Revision != expectedRevision || progress.Revision != (expectedRevision ?? -1) + 1)
			throw new InvalidOperationException("Test treatment checkpoint concurrency conflict");
		Treatments[progress.Id] = progress;
	}
	public void CommitRepair(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double> balances,
		LandRejuvenationProgress progress, long expectedRevision)
	{
		var before = Treatments[progress.Id];
		if (before.Revision != expectedRevision || before.PendingRequest != request) throw new InvalidOperationException("Prepared step mismatch");
		var loseAck = FailAfterCommit;
		FailAfterCommit = false;
		try
		{
			Commit(cell, request, result, state, atUtc, balances);
			Treatments[progress.Id] = progress;
		}
		finally { FailAfterCommit = loseAck; }
		if (loseAck) throw new InvalidOperationException("Test acknowledgement lost after atomic checkpoint");
	}
	public Dictionary<Guid, StoredEnvironmentalMagicOperation> Receipts { get; } = new();
	public Dictionary<long, EnvironmentalMagicState> PersistedStates { get; } = new();
	public Dictionary<long, IReadOnlyDictionary<long, double>> PersistedResourceAmounts { get; } = new();
	public Dictionary<long, Db.Cell> Models { get; } = new();
	public bool FailCommit { get; set; }
	public bool FailAfterCommit { get; set; }
	public bool FailAfterClaim { get; set; }
	public bool FailRead { get; set; }
	public Action<Cell>? BeforeCommit { get; set; }
	public int Reads { get; private set; }
	public int Commits { get; private set; }
	public StoredEnvironmentalMagicOperation? Find(Guid operationId)
	{
		Reads++;
		if (FailRead) throw new InvalidOperationException("Test persistence read failure");
		return Receipts.GetValueOrDefault(operationId);
	}
	public StoredEnvironmentalMagicState Load(Cell cell)
	{
		if (FailRead) throw new InvalidOperationException("Test persistence read failure");
		return new(PersistedStates.GetValueOrDefault(cell.Id, cell.EnvironmentState),
			PersistedResourceAmounts.GetValueOrDefault(cell.Id) ?? cell.MagicResourceAmounts.ToDictionary(value => value.Key.Id, value => value.Value),
			PersistedStates.ContainsKey(cell.Id));
	}
	public void Commit(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc,
		IReadOnlyDictionary<IMagicResource, double>? resourceAmounts = null)
	{
		BeforeCommit?.Invoke(cell);
		if (FailCommit) throw new InvalidOperationException("Test persistence failure");
		typeof(Cell).GetMethod("BeginEnvironmentalOperation", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(cell, new object[] { request.OperationId });
		if (FailAfterClaim) throw new InvalidOperationException("Test transaction rolled back after claiming operation");
		Commits++;
		Receipts.Add(request.OperationId, new StoredEnvironmentalMagicOperation(cell.Id, request, result));
		PersistedStates[cell.Id] = state;
		var balances = cell.MagicResourceAmounts.ToDictionary(value => value.Key.Id, value => value.Value);
		if (resourceAmounts is not null)
			foreach (var (resource, amount) in resourceAmounts) balances[resource.Id] = amount;
		PersistedResourceAmounts[cell.Id] = balances;
		if (Models.TryGetValue(cell.Id, out var model))
		{
			model.EnvironmentalState ??= new Db.CellEnvironmentalState { CellId = cell.Id, Cell = model };
			typeof(Cell).GetMethod("CopyEnvironmentState", BindingFlags.Static | BindingFlags.NonPublic)!
				.Invoke(null, new object[] { state, model.EnvironmentalState });
			foreach (var (resourceId, amount) in balances)
			{
				var row = model.CellsMagicResources.FirstOrDefault(value => value.MagicResourceId == resourceId);
				if (row is null)
				{
					row = new Db.CellMagicResource { CellId = cell.Id, MagicResourceId = resourceId };
					model.CellsMagicResources.Add(row);
				}
				row.Amount = amount;
			}
		}
		if (FailAfterCommit) throw new InvalidOperationException("Test acknowledgement lost after commit");
	}
}

/// <summary>Constant-time fake registry; enumeration can be forbidden after the owning load pass.</summary>
internal sealed class EnvironmentalMagicTestRegistry<T> : IUneditableAll<T> where T : class, IFrameworkItem
{
	private readonly Dictionary<long, T> _items = new();
	private readonly List<T> _order = new();
	public bool ForbidEnumeration { get; set; }
	public long Enumerations { get; private set; }
	public T At(int index) => _order[index];
	public void Add(T value)
	{
		if (_items.TryGetValue(value.Id, out var old)) _order.Remove(old);
		_items[value.Id] = value;
		_order.Add(value);
	}
	public void Remove(long id) { if (_items.Remove(id, out var value)) _order.Remove(value); }
	public bool Has(T value) => _items.GetValueOrDefault(value.Id) == value;
	public bool Has(long id) => _items.ContainsKey(id);
	public bool Has(string name) => _order.Any(item => item.Name.EqualTo(name));
	public T? Get(long id) => _items.GetValueOrDefault(id);
	public bool TryGet(long id, out T? result) => _items.TryGetValue(id, out result);
	public List<T> Get(string name) => _order.Where(item => item.Name.EqualTo(name)).ToList();
	public T? GetByName(string name) => _order.FirstOrDefault(item => item.Name.EqualTo(name));
	public T? GetByIdOrName(string value, bool permitAbbreviations = true) => long.TryParse(value, out var id) ? Get(id) : GetByName(value);
	public void ForEach(Action<T> action) { foreach (var item in _order) action(item); }
	public int Count => _items.Count;
	public IEnumerator<T> GetEnumerator()
	{
		if (ForbidEnumeration) throw new InvalidOperationException($"Unexpected full {typeof(T).Name} registry enumeration");
		Enumerations++;
		return _order.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
