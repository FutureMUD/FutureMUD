using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder : IDatabaseSeeder
{
    /// <inheritdoc />
    public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => new List<(string Id, string Question,
            Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
    {
		("eras",
				@"The item seeder includes items from a variety of eras, but you may want to limit the selection to better fit your world. The options are:

    #BAntiquity#0 - Classical antiquity prior to the fall of rome, for europe and near east
    #BMedieval#0 - The medieval period, roughly 500 to 1400 CE
    #BRenaissance#0 - The renaissance period, roughly 1400 to 1600 CE
    #BEarlyModern#0 - The enlightenment and early modern period, roughly 1600 to 1750 CE

Later eras are intentionally unavailable until they have implemented manifest modules; selecting an era therefore always installs real content.


Please enter the eras that you want to be created, separated by spaces.

What is your choice? ", (context, answers) => true,
				(text, context) =>
				{
					string[] split = text.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string item in split) { switch (item.ToLowerInvariant())
						{
							case "antiquity":
							case "medieval":
							case "renaissance":
							case "earlymodern":
								continue;
							default:
								return (false,
									$"The option '{item.ToLowerInvariant()}' is not a valid era selection.");
						} } return (true, string.Empty);
				}
			),
	};

    /// <inheritdoc />
    public int SortOrder => 400;

    /// <inheritdoc />
    public string Name => "Items";

    /// <inheritdoc />
    public string Tagline => "A starter collection of items and crafts";

    /// <inheritdoc />
	public string FullDescription => BuildManifestBackedDescription();

	/// <inheritdoc />
	public bool SafeToRunMoreThanOnce => true;

    private Dictionary<string, GameItemComponentProto> _components = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Tag> _tags = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Tag> _tagsByFullPath = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Material> _materials = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Liquid> _liquids = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, FutureProg> _progs = new(StringComparer.InvariantCultureIgnoreCase);
    private DictionaryWithDefault<string, TraitDefinition> _traits = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, GameItemProto> _items = new(StringComparer.InvariantCultureIgnoreCase);
	private Dictionary<string, GameItemProto> _itemsByStableReference = new(StringComparer.OrdinalIgnoreCase);
	private Dictionary<long, GameItemProto> _itemsById = [];
	private Dictionary<long, string> _itemStableReferencesById = [];
	private Dictionary<string, IReadOnlyList<GameItemProto>> _legacyItemsByShortDescription =
		new(StringComparer.OrdinalIgnoreCase);
	private Dictionary<string, SeederManagedRecord> _managedRecordsByIdentity =
		new(StringComparer.OrdinalIgnoreCase);
	private Dictionary<string, Craft> _craftsByNameAndCategory = new(StringComparer.OrdinalIgnoreCase);
	private long _nextItemId = 1;
	private long _nextCraftId = 1;
	private long _nextCraftInputId = 1;
	private bool _deferCraftProductSave;
	private int _deferredCraftPersistenceCount;
	private Stopwatch? _progressStopwatch;
	private int _progressStage;
	private int _progressStageCount;

    private FuturemudDatabaseContext? _context;
    private IReadOnlyDictionary<string, string>? _questionAnswers;
    private readonly List<string> _missingTags = new();
    private Account _dbAccount = null!;
    private DateTime _now = DateTime.UtcNow;

    private void InitialiseDependencies()
    {
		if (_context is null)
		{
			throw new ApplicationException("Context cannot be null at this point.");
		}

		_managedRecordsByIdentity = _manifestCaptureOnly
			? new Dictionary<string, SeederManagedRecord>(StringComparer.OrdinalIgnoreCase)
			: _context.SeederManagedRecords
				.Where(x => x.Seeder == Name)
				.AsEnumerable()
				.GroupBy(x => ManagedRecordIdentity(x.EntityType, x.StableKey), StringComparer.OrdinalIgnoreCase)
				.ToDictionary(x => x.Key, x => x.OrderByDescending(record => record.AppliedAt).First(),
					StringComparer.OrdinalIgnoreCase);

		_components = _context.GameItemComponentProtos.Local
			.AsEnumerable()
			.Concat(_context.GameItemComponentProtos
				.Include(x => x.EditableItem)
				.AsEnumerable())
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => x.FirstOrDefault(y => y.EditableItem?.RevisionStatus == 4) ??
				     x.FirstOrDefault(y => y.EditableItem?.RevisionStatus is 1 or 2) ??
				     x.OrderByDescending(y => y.RevisionNumber).First(),
				StringComparer.OrdinalIgnoreCase);
        _tags = _context.Tags
            .AsEnumerable()
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.OrderBy(tag => tag.Id).First(), StringComparer.OrdinalIgnoreCase);
        var tagList = _context.Tags.ToList();
        var tagsById = tagList.ToDictionary(x => x.Id);
        Dictionary<long, string> fullPathCache = new();
        string BuildTagFullPath(Tag tag)
        {
            if (fullPathCache.TryGetValue(tag.Id, out var cached))
            {
                return cached;
            }

			Tag? parent = null;
            if (tag.ParentId is not null)
            {
                tagsById.TryGetValue(tag.ParentId.Value, out parent);
            }
            else if (tag.Parent is not null)
            {
                parent = tag.Parent;
            }

            var path = parent is null
                ? tag.Name
                : $"{BuildTagFullPath(parent)} / {tag.Name}";
            fullPathCache[tag.Id] = path;
            return path;
        }

        _tagsByFullPath = tagList
            .GroupBy(BuildTagFullPath, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.OrderBy(tag => tag.Id).First(), StringComparer.OrdinalIgnoreCase);
		var materialGroups = _context.Materials
			.AsEnumerable()
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		var liquidGroups = _context.Liquids
			.AsEnumerable()
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (!_manifestCaptureOnly)
		{
			var ambiguousMaterials = materialGroups.Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
			var ambiguousLiquids = liquidGroups.Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
			if (ambiguousMaterials.Length > 0 || ambiguousLiquids.Length > 0)
			{
				throw new InvalidOperationException(
					$"ItemSeeder preflight found ambiguous canonical identities. Materials: {ambiguousMaterials.ListToString()}; liquids: {ambiguousLiquids.ListToString()}.");
			}
		}

		_materials = materialGroups.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First(),
			StringComparer.OrdinalIgnoreCase);
		_liquids = liquidGroups.ToDictionary(x => x.Key, x => x.OrderBy(y => y.Id).First(),
			StringComparer.OrdinalIgnoreCase);
		_nextItemId = _context.GameItemProtos.Any()
            ? _context.GameItemProtos.Max(x => x.Id) + 1
            : 1;
        _dbAccount = _context.Accounts.First();

        foreach (TraitDefinition trait in _context.TraitDefinitions)
        {
            _traits[trait.Name] = trait;
        }
		IndexStockSkillPackageTraitAliases();

		var itemPrototypes = _context.GameItemProtos
			.Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosTags)
			.Include(x => x.GameItemProtosGameItemComponentProtos)
			.AsEnumerable()
			.ToArray();
		var activeStableReferenceConflicts = itemPrototypes
			.Where(x => !string.IsNullOrWhiteSpace(x.UniqueName) && x.EditableItem?.RevisionStatus is 1 or 2 or 4)
			.GroupBy(x => x.UniqueName, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Select(y => y.Id).Distinct().Count() > 1)
			.Select(x => $"{x.Key} ({string.Join(", ", x.Select(y => y.Id).Distinct().OrderBy(y => y))})")
			.ToArray();
		if (activeStableReferenceConflicts.Length > 0)
		{
			throw new InvalidOperationException(
				$"ItemSeeder preflight found stable references active on multiple logical IDs: {string.Join("; ", activeStableReferenceConflicts)}.");
		}

		_items = new Dictionary<string, GameItemProto>(StringComparer.InvariantCultureIgnoreCase);
		_itemsByStableReference = new(StringComparer.OrdinalIgnoreCase);
		_itemsById = [];
		_itemStableReferencesById = [];
		_legacyItemsByShortDescription = itemPrototypes
			.Where(x => string.IsNullOrWhiteSpace(x.UniqueName))
			.GroupBy(x => x.ShortDescription, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => (IReadOnlyList<GameItemProto>)x.ToArray(), StringComparer.OrdinalIgnoreCase);
		foreach (var group in itemPrototypes.GroupBy(x => x.Id))
		{
			var item = group.FirstOrDefault(x => x.EditableItem?.RevisionStatus == 4) ??
			           group.FirstOrDefault(x => x.EditableItem?.RevisionStatus is 1 or 2) ??
			           group.OrderByDescending(x => x.RevisionNumber).First();
			_itemsById[item.Id] = item;
			if (!string.IsNullOrWhiteSpace(item.UniqueName))
			{
				_items[item.UniqueName] = item;
				_itemsByStableReference[item.UniqueName] = item;
				_itemStableReferencesById[item.Id] = item.UniqueName;
			}
		}

		foreach (var group in itemPrototypes.GroupBy(x => x.ShortDescription, StringComparer.OrdinalIgnoreCase)
		         .Where(x => x.Select(y => y.Id).Distinct().Count() == 1))
		{
			var item = group.FirstOrDefault(x => x.EditableItem?.RevisionStatus == 4) ??
			           group.FirstOrDefault(x => x.EditableItem?.RevisionStatus is 1 or 2) ??
			           group.OrderByDescending(x => x.RevisionNumber).First();
			_items[group.Key] = item;
		}

		_craftsByNameAndCategory = _context.Crafts.Local
			.AsEnumerable()
			.Concat(_context.Crafts
				.Include(x => x.EditableItem)
				.Include(x => x.CraftPhases)
				.Include(x => x.CraftInputs)
				.Include(x => x.CraftTools)
				.Include(x => x.CraftProducts)
				.AsEnumerable())
			.GroupBy(x => CraftLookupKey(x.Name, x.Category), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.OrderBy(craft => craft.Id).First(), StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        _context = context;
		if (!_manifestCaptureOnly)
		{
			ItemSeederManifestCatalogue.LoadForRuntime();
		}

		var resolvedEras = ResolveSelectedEras(context, questionAnswers);
		_questionAnswers = new Dictionary<string, string>(questionAnswers, StringComparer.OrdinalIgnoreCase)
		{
			["eras"] = string.Join(" ", resolvedEras)
		};
		BeginProgressReporting(_questionAnswers);
		using var transaction = context.Database.IsRelational() ? context.Database.BeginTransaction() : null;
		try
		{
        RunSeedStage("Loading item prerequisites", InitialiseDependencies);
		if (!_manifestCaptureOnly && _questionAnswers.TryGetValue("eras", out var requestedEras) && HasAnyEra(requestedEras, "renaissance"))
		{
			RunSeedStage("Validating Renaissance military prerequisites", ValidateRenaissanceMilitaryPrerequisites);
		}
		if (!_manifestCaptureOnly && _questionAnswers.TryGetValue("eras", out requestedEras) &&
			HasAnyEra(requestedEras, "renaissance", "earlymodern"))
		{
			RunSeedStage("Validating Renaissance and Early Modern jewellery and door prerequisites",
				ValidateRenaissanceEarlyModernJewelleryDoorsPrerequisites);
			RunSeedStage("Validating historical medical prerequisites",
				() => ValidateHistoricalMedicalPrerequisites(requestedEras));
		}

        SeedReworkItems();
		if (_questionAnswers.TryGetValue("eras", out var selectedEras) &&
			HasAnyEra(selectedEras, "antiquity", "medieval", "renaissance", "earlymodern"))
		{
			RunSeedStage("Saving item changes before crafting", SaveItemChangesBeforeCrafting);
		}

		RunSeedStage("Creating crafting support progs", () =>
		{
			using var manifestModule = UseManifestModule("foundations");
			CreateProgs();
		});
        SeedCrafts();
		if (_questionAnswers.TryGetValue("eras", out var eras))
		{
			if (ParseVehicleEraTokens(eras).Count > 0)
			{
				RunSeedStage("Creating vehicle items and prototypes", () =>
				{
					using var manifestModule = UseManifestModule("vehicles", ParseVehicleEraTokens(eras).ToArray());
					SeedVehicleItemsAndPrototypes(eras);
				});
			}
		}
		RetireMissingManagedRecords();
		if (!_manifestCaptureOnly)
		{
			RunSeedStage("Saving item and craft changes", () => _context.SaveChanges());
		}
		if (_manifestCaptureOnly)
		{
			transaction?.Rollback();
		}
		else
		{
			transaction?.Commit();
		}
		Console.WriteLine($"[Item Seeder] Completed in {_progressStopwatch!.Elapsed.TotalSeconds:N1}s.");

		var summary = BuildManifestResultSummary();
		return string.IsNullOrWhiteSpace(summary)
			? "The operation completed successfully."
			: $"The operation completed successfully.{Environment.NewLine}{summary}";
		}
		catch
		{
			transaction?.Rollback();
			throw;
		}
    }

	private void BeginProgressReporting(IReadOnlyDictionary<string, string> questionAnswers)
	{
		_progressStopwatch = Stopwatch.StartNew();
		_progressStage = 0;
		_progressStageCount = 8; // Prerequisites, craft progs, the item flush, four craft batches, and the final save.

		if (!questionAnswers.TryGetValue("eras", out var eras))
		{
			return;
		}

		if (HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern"))
		{
			_progressStageCount++;
		}

		if (HasAnyEra(eras, "renaissance"))
		{
			_progressStageCount++;
		}

		if (HasAnyEra(eras, "renaissance", "earlymodern"))
		{
			_progressStageCount++;
		}

		if (HasAnyEra(eras, "medieval", "renaissance", "earlymodern"))
		{
			_progressStageCount++;
		}

		_progressStageCount += new[] { "antiquity", "medieval", "renaissance", "earlymodern" }
			.Count(era => eras.Contains(era, StringComparison.InvariantCultureIgnoreCase));

		if (HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern"))
		{
			_progressStageCount++;
		}

		if (ParseVehicleEraTokens(eras).Count > 0)
		{
			_progressStageCount++;
		}
	}

	private void RunSeedStage(string description, Action action)
	{
		if (_progressStopwatch is null)
		{
			action();
			return;
		}

		_progressStage++;
		var startingItemCount = CountNewEntities<GameItemProto>();
		var startingCraftCount = CountNewEntities<Craft>();
		var stageStopwatch = Stopwatch.StartNew();
		Console.WriteLine($"[Item Seeder] [{_progressStage}/{_progressStageCount}] {description}...");
		action();

		var newItemCount = CountNewEntities<GameItemProto>() - startingItemCount;
		var newCraftCount = CountNewEntities<Craft>() - startingCraftCount;
		var additions = new List<string>();
		if (newItemCount > 0)
		{
			additions.Add($"{newItemCount:N0} new item{(newItemCount == 1 ? string.Empty : "s")}");
		}

		if (newCraftCount > 0)
		{
			additions.Add($"{newCraftCount:N0} new craft{(newCraftCount == 1 ? string.Empty : "s")}");
		}

		var additionSummary = additions.Count == 0 ? string.Empty : $"; {additions.ListToString()}";
		Console.WriteLine(
			$"[Item Seeder] [{_progressStage}/{_progressStageCount}] Completed in {stageStopwatch.Elapsed.TotalSeconds:N1}s{additionSummary}.");
	}

	private int CountNewEntities<TEntity>() where TEntity : class
	{
		return _context!.ChangeTracker
			.Entries<TEntity>()
			.Count(x => x.State == EntityState.Added);
	}

	private void SaveItemChangesBeforeCrafting()
	{
		if (_manifestCaptureOnly)
		{
			return;
		}

		_context!.SaveChanges();
		DetachTrackedEntities(entity => entity is GameItemProto or
			GameItemComponent or
			GameItemProtosDefaultVariable or
			GameItemProtosGameItemComponentProtos or
			GameItemProtosOnLoadProgs or
			GameItemProtosTags or
			GameItemProtoExtraDescription or
			SeederManagedRecord or
			EditableItem);
	}

	private void DetachTrackedEntities(Func<object, bool> predicate)
	{
		foreach (var entry in _context!.ChangeTracker.Entries()
			         .Where(x => predicate(x.Entity))
			         .ToList())
		{
			entry.State = EntityState.Detached;
		}
	}

    /// <inheritdoc />
    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (context.GameItemComponentProtos.All(x => x.Name != "Container_Table") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Armor_Stand") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Weapon_Rack") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Cot_Surface") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Bed_Surface") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Couch_Surface") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Counter") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Bench_Surface") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Desk_Surface") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Wide_Shelves") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Open_Bin") ||
            context.GameItemComponentProtos.All(x => x.Name != "Container_Trunk") ||
            context.GameItemComponentProtos.All(x => x.Name != "TimePiece_Antiquity_Sundial") ||
            context.GameItemComponentProtos.All(x => x.Name != "WaterSource_Antiquity_PublicWell") ||
            context.GameItemComponentProtos.All(x => x.Name != "Dice_Antiquity_Knucklebones") ||
            context.GameItemComponentProtos.All(x => x.Name != "DragAid_Antiquity_FieldStretcher") ||
            context.GameItemComponentProtos.All(x => x.Name != "Locksmithing_Antiquity_BronzePoor") ||
            context.GameItemComponentProtos.All(x => x.Name != "ShopStall_Antiquity_OpenCounter") ||
            context.GameItemComponentProtos.All(x => x.Name != "MarketGoodWeight_Antiquity_StapleFood") ||
            context.GameItemComponentProtos.All(x => x.Name != "SealStamp_Antiquity_BronzeSignet") ||
            context.GameItemComponentProtos.All(x => x.Name != "Sealable_Envelope") ||
            context.GameItemComponentProtos.All(x => x.Name != "MeasuringInstrument_Antiquity_BalanceScale") ||
            context.GameItemComponentProtos.All(x => x.Name != "Insulation_Minor") ||
            context.GameItemComponentProtos.All(x => x.Name != "Destroyable_Misc") ||
            context.GameItemComponentProtos.All(x => x.Name != "Torch_Infinite") ||
			new[]
			{
				"Wear_Saddle", "Wear_Bridle", "Wear_Chanfron", "Wear_Criniere", "Wear_Croupiere",
				"Wear_Flanchards", "Wear_Peytral", "Wear_Caparison"
			}.Any(name => context.GameItemComponentProtos.All(x => x.Name != name)) ||
            context.Tags.All(x => x.Name != "Functions"))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

		return context.SeederManagedRecords.Any(x => x.Seeder == Name && !x.Retired)
			? ShouldSeedResult.MayAlreadyBeInstalled
			: ShouldSeedResult.ReadyToInstall;
    }

	/// <inheritdoc />
	public SeederAssessment AssessSeedData(FuturemudDatabaseContext context)
	{
		var missingPrerequisites = SeederMetadataRegistry.GetMetadata(this).Prerequisites
			.Where(x => !x.IsSatisfied(context))
			.Select(x => x.Description)
			.ToArray();
		if (missingPrerequisites.Length > 0)
		{
			return new SeederAssessment(SeederAssessmentStatus.Blocked,
				"Required ItemSeeder foundations are missing.", missingPrerequisites, [], []);
		}

		ItemSeederManifestDocument manifest;
		try
		{
			manifest = ItemSeederManifestCatalogue.LoadForRuntime();
		}
		catch (Exception exception)
		{
			return new SeederAssessment(SeederAssessmentStatus.Blocked,
				"The executable ItemSeeder manifest is missing or invalid.", [], [exception.Message], []);
		}

		var records = context.SeederManagedRecords
			.Where(x => x.Seeder == Name)
			.AsEnumerable()
			.ToArray();
		if (records.Length == 0)
		{
			return new SeederAssessment(SeederAssessmentStatus.ReadyToInstall,
				"No ItemSeeder provenance is installed.", [], [],
				[$"{manifest.Entries.Count:N0} manifest aggregates are available across four implemented eras."]);
		}

		var installedEras = records
			.Where(x => !x.Retired && ImplementedEraKeys.Contains(x.Module, StringComparer.OrdinalIgnoreCase))
			.Select(x => x.Module)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var expected = manifest.Entries
			.Where(x => x.EraAdmissions.Count == 0 || x.EraAdmissions.Any(installedEras.Contains))
			.ToDictionary(x => ManagedRecordIdentity(x.EntityType, x.StableKey), StringComparer.OrdinalIgnoreCase);
		var installed = records.ToDictionary(x => ManagedRecordIdentity(x.EntityType, x.StableKey),
			StringComparer.OrdinalIgnoreCase);
		var updateAvailable = records.Any(x => x.Retired) ||
		                      expected.Keys.Any(x => !installed.TryGetValue(x, out var record) || record.Retired) ||
		                      expected.Any(x => installed.TryGetValue(x.Key, out var record) &&
			                      !record.AppliedFingerprint.Equals(x.Value.Fingerprint, StringComparison.OrdinalIgnoreCase));
		return updateAvailable
			? new SeederAssessment(SeederAssessmentStatus.UpdateAvailable,
				"The installed ItemSeeder package has missing, retired, or revised manifest aggregates.", [], [],
				["A rerun will repair untouched stock and preserve customized aggregates."])
			: new SeederAssessment(SeederAssessmentStatus.InstalledCurrent,
				"Installed ItemSeeder provenance matches the current manifest.", [], [],
				["Rerun to add another implemented era; installed eras are never removed."]);
	}

    /// <inheritdoc />
    public bool Enabled => true;

	GameItemProto? CreateItem(string stableReference,
												  string noun,
												  string sdesc,
												  string? ldesc,
												  string fdesc,
												  SizeCategory size,
												  ItemQuality quality,
												  double weightInGrams,
												  decimal inherentCost,
												  bool skinnable,
												  bool hideFromPlayers,
												  string material,
												  IEnumerable<string> tags,
												  IEnumerable<string> components,
												  string? morphToUniqueReference,
												  string? morphEmote,
												  TimeSpan? morphTimer,
												  string? destroyedItemUniqueReference,
												  string? builderNotes = null,
												  bool allowLegacyShortDescriptionMatch = true)
	{
		var tagList = BuildReworkItemTagList(tags);
		var componentList = components as IReadOnlyCollection<string> ?? components.ToArray();
		var definition = BuildItemManifestDefinition(
			stableReference,
			noun,
			sdesc,
			ldesc,
			fdesc,
			(int)size,
			(int)quality,
			weightInGrams,
			inherentCost,
			skinnable,
			hideFromPlayers,
			material,
			tagList,
			componentList,
			morphToUniqueReference,
			morphEmote,
			morphTimer,
			destroyedItemUniqueReference);
		var lifecycleDependencies = new[] { morphToUniqueReference, destroyedItemUniqueReference }
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => $"item:{x}")
			.ToArray();
		var manifestEntry = RegisterManifestAggregate(
			"item",
			stableReference,
			definition,
			lifecycleDependencies,
			morphToUniqueReference is null && destroyedItemUniqueReference is null
				? ItemSeederOwnershipPolicy.StockAggregate
				: ItemSeederOwnershipPolicy.RequiredRelationship);
		if (_manifestCaptureOnly)
		{
			if (!_materials.TryGetValue(definition.Material, out var capturedMaterial))
			{
				capturedMaterial = new Material
				{
					Id = -(_materials.Count + 1L),
					Name = definition.Material,
					MaterialDescription = definition.Material
				};
				_materials[definition.Material] = capturedMaterial;
			}

			var capturedItem = new GameItemProto
			{
				Id = _nextItemId++,
				RevisionNumber = 0,
				Name = definition.Noun,
				UniqueName = GameItemProtoLookupExtensions.NormaliseUniqueName(stableReference),
				ShortDescription = definition.ShortDescription,
				LongDescription = definition.LongDescription,
				FullDescription = definition.FullDescription,
				Keywords = definition.Keywords,
				MaterialId = capturedMaterial.Id,
				Size = definition.Size,
				Weight = definition.WeightInGrams,
				BaseItemQuality = definition.Quality,
				CostInBaseCurrency = definition.Cost,
				PermitPlayerSkins = definition.Skinnable,
				IsHiddenFromPlayers = definition.HiddenFromPlayers,
				MorphTimeSeconds = definition.MorphTimeSeconds,
				MorphEmote = definition.MorphEmote,
				BuilderNotes = null
			};
			CacheReworkItem(stableReference, capturedItem);
			return capturedItem;
		}

		var managedRecord = FindManagedRecord(manifestEntry.EntityType, manifestEntry.StableKey);
		var existing = FindItemByStableReference(stableReference);
		if (existing is null && allowLegacyShortDescriptionMatch)
		{
			existing = FindExactLegacyItemMatch(stableReference, sdesc, definition);
		}

		if (existing is not null)
		{
			if (_manifestCaptureOnly)
			{
				CacheReworkItem(stableReference, existing);
				return existing;
			}

			if (managedRecord is not null && managedRecord.LogicalId is not null && managedRecord.LogicalId != existing.Id)
			{
				IncrementManifestResult(manifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"ItemSeeder ownership conflict for item:{stableReference}: provenance names logical ID {managedRecord.LogicalId:N0}, but the active stable reference resolves to {existing.Id:N0}.");
			}

			var liveDefinition = BuildLiveItemManifestDefinition(existing, stableReference);
			var liveFingerprint = ItemSeederManifestCatalogue.Fingerprint(liveDefinition);
			if (managedRecord is null && !liveFingerprint.Equals(manifestEntry.Fingerprint, StringComparison.OrdinalIgnoreCase))
			{
				IncrementManifestResult(manifestEntry.Module, x => x with { Blocked = x.Blocked + 1 });
				throw new InvalidOperationException(
					$"Unmanaged item conflict for stable reference '{stableReference}'. The uniquely matched record does not have the stock signature and will not be claimed or overwritten.");
			}

			if (managedRecord is not null &&
			    !liveFingerprint.Equals(managedRecord.AppliedFingerprint, StringComparison.OrdinalIgnoreCase))
			{
				if (IsRepairableMissingItemStock(liveDefinition, definition))
				{
					ApplyItemManifestDefinition(existing, definition, tagList, componentList, builderNotes);
					CacheReworkItem(stableReference, existing);
					ApplyItemLifecycleSettings(existing, morphToUniqueReference, morphEmote, morphTimer, destroyedItemUniqueReference);
					RecordAppliedManifestEntry(manifestEntry, existing.Id, existing.RevisionNumber);
					IncrementManifestResult(manifestEntry.Module, x => x with { Updated = x.Updated + 1 });
					return existing;
				}

				MarkManifestAggregateCustomized(manifestEntry.EntityType, manifestEntry.StableKey);
				IncrementManifestResult(manifestEntry.Module, x => x with { Customized = x.Customized + 1 });
				CacheReworkItem(stableReference, existing);
				return existing;
			}

			var changed = !liveFingerprint.Equals(manifestEntry.Fingerprint, StringComparison.OrdinalIgnoreCase);
			ApplyItemManifestDefinition(existing, definition, tagList, componentList, builderNotes);
			CacheReworkItem(stableReference, existing);
			ApplyItemLifecycleSettings(existing, morphToUniqueReference, morphEmote, morphTimer, destroyedItemUniqueReference);
			RecordAppliedManifestEntry(manifestEntry, existing.Id, existing.RevisionNumber);
			IncrementManifestResult(manifestEntry.Module,
				x => changed ? x with { Updated = x.Updated + 1 } : x with { Unchanged = x.Unchanged + 1 });
			return existing;
		}

		GameItemProto dbitem = new()
		{
			Id = _nextItemId++,
			Name = noun.ToLowerInvariant(),
			UniqueName = GameItemProtoLookupExtensions.NormaliseUniqueName(stableReference),
			BuilderNotes = null,
			Keywords = new ExplodedString(sdesc.Strip_A_An()).Words.Distinct().ListToCommaSeparatedValues(" "),
			MaterialId = _materials[material].Id,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _dbAccount.Id,
				BuilderDate = _now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = _dbAccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = _now
			},
			RevisionNumber = 0,
			Size = (int)size,
			Weight = weightInGrams,
			ReadOnly = false,
			LongDescription = ldesc,
			BaseItemQuality = (int)quality,
			ShortDescription = sdesc,
			FullDescription = fdesc,
			PermitPlayerSkins = skinnable,
			CostInBaseCurrency = inherentCost,
			IsHiddenFromPlayers = hideFromPlayers,
			MorphTimeSeconds = 0,
			MorphEmote = "$0 $?1|morphs into $1|decays into nothing$.",
		};
		var addedTagIds = new HashSet<long>();
		foreach (string item in tagList.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}

			if (!_tagsByFullPath.ContainsKey(item))
			{
				return null;
			}

			var tagId = _tagsByFullPath[item].Id;
			if (!addedTagIds.Add(tagId))
			{
				continue;
			}

			dbitem.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = dbitem,
				TagId = tagId
			});
		}

		foreach (string item in componentList)
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}

			if (!_components.ContainsKey(item))
			{
				return null;
			}

			dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = dbitem,
				GameItemComponent = _components[item]
			});
		}

		_context!.GameItemProtos.Add(dbitem);
		CacheReworkItem(stableReference, dbitem);
		ApplyItemLifecycleSettings(dbitem, morphToUniqueReference, morphEmote, morphTimer, destroyedItemUniqueReference);
		RecordAppliedManifestEntry(manifestEntry, dbitem.Id, dbitem.RevisionNumber);
		IncrementManifestResult(manifestEntry.Module, x => x with { Inserted = x.Inserted + 1 });
		return dbitem;
	}

	private GameItemProto? FindItemByStableReference(string stableReference)
	{
		return _itemsByStableReference.GetValueOrDefault(stableReference);
	}

	private GameItemProto? FindExactLegacyItemMatch(
		string stableReference,
		string shortDescription,
		ItemManifestDefinition definition)
	{
		var candidates = _legacyItemsByShortDescription
			.GetValueOrDefault(shortDescription, [])
			.Where(x => x.ShortDescription.Equals(shortDescription, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		var expectedFingerprint = ItemSeederManifestCatalogue.Fingerprint(definition);
		var exactMatches = candidates
			.Where(x => ItemSeederManifestCatalogue.Fingerprint(BuildLiveItemManifestDefinition(x, stableReference))
				.Equals(expectedFingerprint, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (candidates.Length > 1)
		{
			throw new InvalidOperationException(
				$"Legacy item adoption for '{stableReference}' is ambiguous: {candidates.Length:N0} unmanaged records use the stock short description. Adoption requires one unique candidate with the complete stock signature.");
		}

		if (exactMatches.Length == 1)
		{
			return exactMatches[0];
		}

		if (candidates.Length > 0)
		{
			throw new InvalidOperationException(
				$"Unmanaged legacy item conflict for '{stableReference}': {candidates.Length:N0} record(s) use the stock short description but none has the complete stock signature. No record was claimed or overwritten.");
		}

		return null;
	}

	private void ApplyItemManifestDefinition(
		GameItemProto item,
		ItemManifestDefinition definition,
		IEnumerable<string> tags,
		IEnumerable<string> components,
		string? builderNotes)
	{
		item.Name = definition.Noun;
		item.UniqueName = GameItemProtoLookupExtensions.NormaliseUniqueName(definition.StableReference);
		item.Keywords = definition.Keywords;
		item.MaterialId = _materials[definition.Material].Id;
		item.Size = definition.Size;
		item.Weight = definition.WeightInGrams;
		item.ReadOnly = false;
		item.LongDescription = definition.LongDescription;
		item.BaseItemQuality = definition.Quality;
		item.ShortDescription = definition.ShortDescription;
		item.FullDescription = definition.FullDescription;
		item.PermitPlayerSkins = definition.Skinnable;
		item.CostInBaseCurrency = definition.Cost;
		item.IsHiddenFromPlayers = definition.HiddenFromPlayers;
		ApplyReworkItemMetadata(item, definition.StableReference, tags, builderNotes);
		var existingComponents = item.GameItemProtosGameItemComponentProtos
			.Select(x => (x.GameItemComponentProtoId, x.GameItemComponentRevision))
			.ToHashSet();
		foreach (var componentName in components.Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			var component = _components[componentName];
			if (!existingComponents.Add((component.Id, component.RevisionNumber)))
			{
				continue;
			}

			item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = item,
				GameItemComponent = component,
				GameItemComponentProtoId = component.Id,
				GameItemComponentRevision = component.RevisionNumber
			});
		}
	}

	private void CacheReworkItem(string stableReference, GameItemProto item)
	{
		_items[stableReference] = item;
		_itemsByStableReference[stableReference] = item;
		_itemsById[item.Id] = item;
		_items[item.ShortDescription] = item;
		if (!string.IsNullOrWhiteSpace(item.UniqueName))
		{
			_items[item.UniqueName] = item;
			_itemStableReferencesById[item.Id] = item.UniqueName;
		}
	}

	private static readonly (string Token, string Culture)[] ReworkStableReferenceCultureTokens =
	[
		("_early_anglo_saxon_", "Early Anglo-Saxon/Insular"),
			("_anglo_danish_", "Late Anglo-Saxon/Anglo-Danish"),
			("_norse_", "Norse"),
			("_norman_", "Norman/Angevin"),
			("_high_british_", "High Medieval Britain/Marcher"),
			("_gaelic_", "Gaelic/Welsh/Highland"),
			("_carolingian_", "Carolingian/Frankish"),
			("_capetian_", "Capetian/Low Countries"),
			("_german_hre_", "German/HRE/Alpine-North Italian"),
			("_iberian_christian_", "Iberian Christian"),
			("_andalusi_", "al-Andalus/Maghreb"),
			("_byzantine_", "Byzantine"),
			("_abbasid_", "Abbasid/Persianate"),
			("_fatimid_", "Fatimid Egypt/Ifriqiya"),
			("_seljuk_ayyubid_", "Seljuk/Ayyubid/early Mamluk"),
			("_rus_novgorod_", "Kyivan Rus/Novgorod"),
			("_steppe_turkic_", "Steppe Turkic/Cuman/Mongol-adjacent"),
			("_song_china_", "Song China"),
			("_hellenic_", "Hellenic"),
			("_roman_", "Roman"),
			("_italic_", "Italic/Roman"),
			("_celtic_", "Celtic"),
			("_germanic_", "Germanic"),
			("_punic_", "Punic"),
			("_phoenician_", "Punic/Phoenician"),
			("_persian_", "Persian"),
			("_median_", "Persian/Median"),
			("_egyptian_", "Egyptian"),
			("_kushite_", "Kushite"),
			("_nubian_", "Kushite/Nubian"),
			("_etruscan_", "Etruscan"),
			("_anatolian_", "Anatolian"),
			("_scythian_", "Scythian-Sarmatian"),
			("_sarmatian_", "Scythian-Sarmatian"),
			("_steppe_", "Scythian-Sarmatian"),
			("_renaissance_italian_", "Renaissance Italian"),
			("_renaissance_iberian_", "Renaissance Iberian"),
			("_renaissance_french_", "Renaissance French/Low Countries"),
			("_renaissance_english_", "Tudor/Elizabethan English"),
			("_renaissance_german_hre_", "Renaissance German/HRE"),
			("_renaissance_ottoman_", "Ottoman"),
			("_renaissance_safavid_", "Safavid/Persianate"),
			("_renaissance_mughal_", "Mughal/Indo-Persian"),
			("_renaissance_ming_", "Ming China"),
			("_renaissance_joseon_", "Joseon Korea"),
			("_renaissance_japanese_", "Muromachi/Sengoku/Momoyama Japan"),
			("_renaissance_west_african_", "West African"),
			("_renaissance_mesoamerican_", "Mesoamerican"),
			("_renaissance_andean_", "Andean"),
			("_renaissance_colonial_", "Early Colonial/Contact Zone"),
			("_earlymodern_british_", "Early Modern British"),
			("_earlymodern_french_", "Early Modern French"),
			("_earlymodern_dutch_", "Early Modern Dutch/Low Countries"),
			("_earlymodern_spanish_", "Early Modern Spanish"),
			("_earlymodern_portuguese_", "Early Modern Portuguese"),
			("_earlymodern_german_", "Early Modern German/HRE"),
			("_earlymodern_ottoman_", "Ottoman"),
			("_earlymodern_safavid_", "Safavid/Persianate"),
			("_earlymodern_mughal_", "Mughal/Indo-Persian"),
			("_earlymodern_qing_", "Qing China"),
			("_earlymodern_edo_", "Edo Japan"),
			("_earlymodern_joseon_", "Joseon Korea"),
			("_earlymodern_colonial_", "Colonial/Contact Zone"),
			("_earlymodern_atlantic_", "Atlantic World"),
			("_preindustrial_", "Shared Pre-Industrial")
	];

	private static readonly (string Token, string Status)[] ReworkStableReferenceStatusTokens =
	[
		("_peasant_", "Peasant"),
			("_artisan_", "Artisan"),
			("_merchant_", "Merchant/Burgher"),
			("_noble_", "Noble/Court"),
			("_clergy_", "Clergy/Monastic"),
			("_military_", "Military")
	];

	private static readonly (string SourceRoot, string FunctionalTag)[] ReworkFunctionalTagMappings =
	[
		("Market / Professional Tools", "Functions / Tools"),
			("Market / Military Goods", "Functions / Military Equipment"),
			("Market / Military Goods / Weapons", "Functions / Military Equipment / Military Weapons"),
			("Market / Military Goods / Ammunition", "Functions / Military Equipment / Military Ammunition"),
			("Market / Military Goods / Armour", "Functions / Military Equipment / Military Armour"),
			("Market / Military Goods / Armour / Shields", "Functions / Military Equipment / Military Armour / Military Shields"),
			("Market / Household Goods", "Functions / Household Items"),
			("Market / Household Goods / Simple Furniture", "Functions / Household Items / Household Furniture"),
			("Market / Household Goods / Standard Furniture", "Functions / Household Items / Household Furniture"),
			("Market / Household Goods / Luxury Furniture", "Functions / Household Items / Household Furniture"),
			("Market / Household Goods / Simple Decorations", "Functions / Household Items / Household Decorations"),
			("Market / Household Goods / Standard Decorations", "Functions / Household Items / Household Decorations"),
			("Market / Household Goods / Luxury Decorations", "Functions / Household Items / Household Decorations"),
			("Market / Household Goods / Simple Wares", "Functions / Household Items / Household Wares"),
			("Market / Household Goods / Standard Wares", "Functions / Household Items / Household Wares"),
			("Market / Household Goods / Luxury Wares", "Functions / Household Items / Household Wares"),
			("Market / Religious Goods", "Functions / Household Items / Household Religious Items"),
			("Market / Lighting", "Functions / Household Items / Household Lighting"),
			("Market / Domestic Heating", "Functions / Household Items / Household Heating"),
			("Market / Construction Materials", "Functions / Household Items / Household Construction Materials"),
			("Market / Writing Materials", "Functions / Writing Goods"),
			("Materials / Writing Product", "Functions / Writing Goods")
	];

	private IReadOnlyCollection<string> BuildReworkItemTagList(IEnumerable<string> tags)
	{
		var tagList = new List<string>();

		void AddTag(string tag, bool requireKnownTag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				return;
			}

			var trimmedTag = tag.Trim();
			if (requireKnownTag && !_tagsByFullPath.ContainsKey(trimmedTag))
			{
				return;
			}

			if (tagList.Any(x => x.Equals(trimmedTag, StringComparison.InvariantCultureIgnoreCase)))
			{
				return;
			}

			tagList.Add(trimmedTag);
		}

		foreach (var tag in tags)
		{
			AddTag(tag, false);
			foreach (var functionalTag in InferReworkFunctionalTags(tag))
			{
				AddTag(functionalTag, true);
			}
		}

		return RemoveRedundantParentTags(tagList);
	}

	private static IReadOnlyCollection<string> RemoveRedundantParentTags(IEnumerable<string> tags)
	{
		var distinctTags = tags
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim())
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray();
		return distinctTags
			.Where(candidate => !distinctTags.Any(other =>
				!candidate.Equals(other, StringComparison.InvariantCultureIgnoreCase) &&
				other.StartsWith($"{candidate} /", StringComparison.InvariantCultureIgnoreCase)))
			.ToArray();
	}

	private static IEnumerable<string> InferReworkFunctionalTags(string tag)
	{
		foreach (var (sourceRoot, functionalTag) in ReworkFunctionalTagMappings)
		{
			if (ReworkTagPathMatchesRoot(tag, sourceRoot))
			{
				yield return functionalTag;
			}
		}
	}

	private static bool ReworkTagPathMatchesRoot(string tagPath, string root)
	{
		return tagPath.Equals(root, StringComparison.OrdinalIgnoreCase) ||
			   tagPath.StartsWith($"{root} /", StringComparison.OrdinalIgnoreCase);
	}

	private void ApplyReworkItemMetadata(GameItemProto item,
									 string stableReference,
									 IEnumerable<string> tags,
									 string? builderNotes)
	{
		item.UniqueName = string.IsNullOrWhiteSpace(item.UniqueName)
			? GameItemProtoLookupExtensions.NormaliseUniqueName(stableReference)
			: item.UniqueName;
		item.BuilderNotes = RemoveSeededBuilderNotes(item.BuilderNotes, stableReference, tags, builderNotes);
		ApplyReworkItemTags(item, tags);
	}

	private static string? RemoveSeededBuilderNotes(
		string? existingNotes,
		string stableReference,
		IEnumerable<string> tags,
		string? builderNotes)
	{
		if (string.IsNullOrWhiteSpace(existingNotes))
		{
			return null;
		}

		var seededLines = BuildReworkItemBuilderNotes(stableReference, tags, builderNotes)
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim())
			.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
		var retainedLines = existingNotes
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim())
			.Where(x => !seededLines.Contains(x))
			.ToArray();
		return retainedLines.Length == 0 ? null : string.Join(Environment.NewLine, retainedLines);
	}

	private void ApplyReworkItemTags(GameItemProto item, IEnumerable<string> tags)
	{
		var desiredPaths = tags
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Trim())
			.ToArray();
		var existingTagIds = item.GameItemProtosTags
			.Select(x => x.TagId)
			.ToHashSet();
		var redundantParentIds = _tagsByFullPath
			.Where(x => existingTagIds.Contains(x.Value.Id) && desiredPaths.Any(desired =>
				desired.StartsWith($"{x.Key} /", StringComparison.InvariantCultureIgnoreCase)))
			.Select(x => x.Value.Id)
			.ToHashSet();
		foreach (var obsoleteTag in item.GameItemProtosTags
			         .Where(x => redundantParentIds.Contains(x.TagId))
			         .ToArray())
		{
			item.GameItemProtosTags.Remove(obsoleteTag);
			_context?.GameItemProtosTags.Remove(obsoleteTag);
		}

		existingTagIds = item.GameItemProtosTags
			.Select(x => x.TagId)
			.ToHashSet();

		foreach (var tag in tags)
		{
			if (string.IsNullOrWhiteSpace(tag) ||
				!_tagsByFullPath.TryGetValue(tag, out var dbtag) ||
				!existingTagIds.Add(dbtag.Id))
			{
				continue;
			}

			item.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = item,
				TagId = dbtag.Id
			});
		}
	}

	private static string BuildReworkItemBuilderNotes(string stableReference, IEnumerable<string> tags, string? builderNotes)
	{
		var notes = new List<string>
			{
				$"Stock unique reference: {stableReference}."
			};

		var cultures = GetReworkItemCultureContexts(stableReference);
		if (cultures.Count > 0)
		{
			notes.Add($"Cultures: {string.Join(", ", cultures)}.");
		}

		var statuses = GetReworkItemStatusContexts(stableReference);
		if (statuses.Count > 0)
		{
			notes.Add($"Status/role: {string.Join(", ", statuses)}.");
		}

		if (stableReference.StartsWith("historic_", StringComparison.InvariantCultureIgnoreCase))
		{
			notes.Add("Shared scope: cross-era historic foundation.");
		}

		var category = GetReworkItemBuilderCategory(stableReference, tags);
		if (!string.IsNullOrWhiteSpace(category))
		{
			notes.Add($"Seeder category: {category}.");
		}

		if (!string.IsNullOrWhiteSpace(builderNotes))
		{
			notes.Add(builderNotes.Trim());
		}

		return string.Join("\n", notes);
	}

	private static string? MergeBuilderNotes(string? existingNotes, string? additionalNotes)
	{
		if (string.IsNullOrWhiteSpace(additionalNotes))
		{
			return string.IsNullOrWhiteSpace(existingNotes) ? null : existingNotes.Trim();
		}

		var trimmedAdditional = additionalNotes.Trim();
		if (string.IsNullOrWhiteSpace(existingNotes))
		{
			return trimmedAdditional;
		}

		var trimmedExisting = existingNotes.Trim();
		var newLines = trimmedAdditional
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim())
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Where(x => !trimmedExisting.Contains(x, StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		return newLines.Count == 0
			? trimmedExisting
			: $"{trimmedExisting}\n{string.Join("\n", newLines)}";
	}

	private static IReadOnlyList<string> GetReworkItemCultureContexts(string stableReference)
	{
		var cultures = new List<string>();

		void AddCulture(string culture)
		{
			if (!cultures.Any(x => x.Equals(culture, StringComparison.InvariantCultureIgnoreCase)))
			{
				cultures.Add(culture);
			}
		}

		void AddIfStableReferenceIn(IReadOnlyDictionary<string, string> stableReferences, string culture)
		{
			if (stableReferences.ContainsKey(stableReference))
			{
				AddCulture(culture);
			}
		}

		AddIfStableReferenceIn(HellenicAntiquityClothingStableReferences, "Hellenic");
		AddIfStableReferenceIn(EgyptianAntiquityClothingStableReferences, "Egyptian");
		AddIfStableReferenceIn(RomanAntiquityClothingStableReferences, "Roman");
		AddIfStableReferenceIn(CelticAntiquityClothingStableReferences, "Celtic");
		AddIfStableReferenceIn(GermanicAntiquityClothingStableReferences, "Germanic");
		AddIfStableReferenceIn(KushiteAntiquityClothingStableReferences, "Kushite");
		AddIfStableReferenceIn(PunicAntiquityClothingStableReferences, "Punic");
		AddIfStableReferenceIn(PersianAntiquityClothingStableReferences, "Persian");
		AddIfStableReferenceIn(EtruscanAntiquityClothingStableReferences, "Etruscan");
		AddIfStableReferenceIn(AnatolianAntiquityClothingStableReferences, "Anatolian");
		AddIfStableReferenceIn(ScythianSarmatianAntiquityClothingStableReferences, "Scythian-Sarmatian");

		foreach (var culture in AntiquityFoodCultures)
		{
			if (stableReference.StartsWith($"antiquity_food_{culture.Key}_", StringComparison.InvariantCultureIgnoreCase))
			{
				AddCulture(culture.Display);
			}
		}

		foreach (var (token, culture) in ReworkStableReferenceCultureTokens)
		{
			if (stableReference.StartsWith("medieval_", StringComparison.InvariantCultureIgnoreCase) &&
				token.Equals("_steppe_", StringComparison.Ordinal))
			{
				continue;
			}

			if (stableReference.Contains(token, StringComparison.InvariantCultureIgnoreCase))
			{
				AddCulture(culture);
			}
		}

		if (stableReference.StartsWith("preindustrial_", StringComparison.InvariantCultureIgnoreCase))
		{
			AddCulture("Shared Pre-Industrial");
		}

		return cultures;
	}

	private static IReadOnlyList<string> GetReworkItemStatusContexts(string stableReference)
	{
		var statuses = new List<string>();

		foreach (var (token, status) in ReworkStableReferenceStatusTokens)
		{
			if (!stableReference.Contains(token, StringComparison.InvariantCultureIgnoreCase) ||
				statuses.Any(x => x.Equals(status, StringComparison.InvariantCultureIgnoreCase)))
			{
				continue;
			}

			statuses.Add(status);
		}

		return statuses;
	}

	private static string? GetReworkItemBuilderCategory(string stableReference, IEnumerable<string> tags)
	{
		var tagList = tags.ToList();
		bool HasTagText(string text)
		{
			return tagList.Any(x => x.Contains(text, StringComparison.InvariantCultureIgnoreCase));
		}

		if (stableReference.StartsWith("historic_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "shared historic foundation stock";
		}

		if (stableReference.StartsWith("preindustrial_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "shared pre-industrial foundation stock";
		}

		if (stableReference.StartsWith("renaissance_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "renaissance era stock";
		}

		if (stableReference.StartsWith("earlymodern_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "early modern era stock";
		}

		if (stableReference.StartsWith("primary_production_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "primary production tools and site-prop stock";
		}

		if (stableReference.StartsWith("medieval_food_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval food and beverage stock";
		}

		if (stableReference.StartsWith("medieval_clothing_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval clothing stock";
		}

		if (stableReference.StartsWith("medieval_writing_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_trade_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval writing and administration stock";
		}

		if (stableReference.StartsWith("medieval_medical_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval medical and apothecary stock";
		}

		if (stableReference.StartsWith("medieval_jewellery_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_devotional_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_offering_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval jewellery and devotional stock";
		}

		if (stableReference.StartsWith("medieval_military_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_weapon_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_shield_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval equipment and military stock";
		}

		if (stableReference.StartsWith("medieval_household_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval furniture, container, and household stock";
		}

		if (stableReference.StartsWith("medieval_textile_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_leather_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_metal_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval repair-kit stock";
		}

		if (stableReference.StartsWith("medieval_surveyor_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval writing and administration stock";
		}

		if (stableReference.StartsWith("medieval_music_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_game_", StringComparison.InvariantCultureIgnoreCase) ||
			stableReference.StartsWith("medieval_horse_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "medieval component-gap prop stock";
		}

		if (stableReference.StartsWith("antiquity_food_", StringComparison.InvariantCultureIgnoreCase))
		{
			return "antiquity food and beverage stock";
		}

		if (stableReference.StartsWith("jewellery_", StringComparison.InvariantCultureIgnoreCase) ||
			HasTagText("Jewellery"))
		{
			return "antiquity jewellery stock";
		}

		if (HasTagText("Professional Tools"))
		{
			return "antiquity tool or workshop support stock";
		}

		if (HasTagText("Military Goods"))
		{
			return "antiquity military stock";
		}

		if (HasTagText("Furniture"))
		{
			return "antiquity furniture stock";
		}

		if (HasTagText("Food and Drink"))
		{
			return "antiquity food and drink stock";
		}

		return null;
	}

	internal static string BuildReworkItemBuilderNotesForTesting(string stableReference,
																 IEnumerable<string> tags,
																 string? builderNotes = null)
	{
		return BuildReworkItemBuilderNotes(stableReference, tags, builderNotes);
	}

	internal static IReadOnlyList<string> InferReworkFunctionalTagsForTesting(IEnumerable<string> tags)
	{
		return tags
			.SelectMany(InferReworkFunctionalTags)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToList();
	}

	internal static IReadOnlyCollection<string> RemoveRedundantParentTagsForTesting(IEnumerable<string> tags)
	{
		return RemoveRedundantParentTags(tags);
	}

	internal GameItemProto? CreateReworkItemForTesting(FuturemudDatabaseContext context,
																	   string stableReference,
																	   string noun,
																	   string shortDescription,
																	   string material,
																	   string? builderNotes = null,
																	   IEnumerable<string>? tags = null)
	{
		if (!ReferenceEquals(_context, context))
		{
			_context = context;
			InitialiseDependencies();
		}

		return CreateItem(
			stableReference,
			noun,
			shortDescription,
			null,
			"A test item.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1.0,
			1.0M,
			false,
			false,
			material,
			tags ?? [],
			[],
			null,
			null,
			null,
			null,
			builderNotes);
	}

	private void ApplyItemLifecycleSettings(GameItemProto item,
											string? morphToUniqueReference,
											string? morphEmote,
											TimeSpan? morphTimer,
											string? destroyedItemUniqueReference)
	{
		if (string.IsNullOrWhiteSpace(morphToUniqueReference) &&
			string.IsNullOrWhiteSpace(morphEmote) &&
			morphTimer is null &&
			string.IsNullOrWhiteSpace(destroyedItemUniqueReference))
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(morphToUniqueReference) &&
			_items.TryGetValue(morphToUniqueReference, out var morphItem))
		{
			item.MorphGameItemProtoId = morphItem.Id;
		}

		if (morphTimer is not null)
		{
			item.MorphTimeSeconds = (int)morphTimer.Value.TotalSeconds;
		}

		if (!string.IsNullOrWhiteSpace(morphEmote))
		{
			item.MorphEmote = morphEmote;
		}

		if (!string.IsNullOrWhiteSpace(destroyedItemUniqueReference) &&
			_items.TryGetValue(destroyedItemUniqueReference, out var destroyedItem))
		{
			item.OnDestroyedGameItemProtoId = destroyedItem.Id;
		}
	}


	private static readonly string[] ImplementedEraKeys = ["antiquity", "medieval", "renaissance", "earlymodern"];

	private static IReadOnlyCollection<string> ParseEraTokens(string? eras)
	{
		return (eras ?? string.Empty)
			.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(x => x.ToLowerInvariant())
			.Where(x => ImplementedEraKeys.Contains(x, StringComparer.OrdinalIgnoreCase))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => Array.IndexOf(ImplementedEraKeys, x))
			.ToArray();
	}

	private IReadOnlyCollection<string> ResolveSelectedEras(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var eras = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		if (questionAnswers.TryGetValue("eras", out var requested))
		{
			eras.UnionWith(ParseEraTokens(requested));
		}

		if (!_manifestCaptureOnly)
		{
			eras.UnionWith(ParseEraTokens(SeederAnswerMemory.GetLatestSeederAnswer(context, Name, "eras")));
			foreach (var module in context.SeederManagedRecords
			         .Where(x => x.Seeder == Name && !x.Retired)
			         .Select(x => x.Module)
			         .Distinct()
			         .AsEnumerable())
			{
				if (ImplementedEraKeys.Contains(module, StringComparer.OrdinalIgnoreCase))
				{
					eras.Add(module);
				}
			}
		}

		if (eras.Count == 0)
		{
			throw new InvalidOperationException("At least one implemented ItemSeeder era must be selected.");
		}

		return ImplementedEraKeys.Where(eras.Contains).ToArray();
	}

	private static bool HasAnyEra(string eras, params string[] eraKeys)
	{
		var selected = ParseEraTokens(eras);
		return eraKeys.Any(x => selected.Contains(x, StringComparer.OrdinalIgnoreCase));
	}

	public void SeedReworkItems()
	{
		if (_questionAnswers?.TryGetValue("eras", out var eras) != true ||
			string.IsNullOrWhiteSpace(eras))
		{
			return;
		}

		if (HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern"))
		{
			RunSeedStage("Creating shared pre-industrial foundations", () =>
			{
				using var manifestModule = UseManifestModule("shared-preindustrial", "antiquity", "medieval", "renaissance", "earlymodern");
				SeedSharedPreIndustrialBaselineItems();
				SeedSharedPreIndustrialFoodFoundation();
			});
		}

		if (HasAnyEra(eras, "medieval", "renaissance", "earlymodern"))
		{
			RunSeedStage("Creating shared food catalogue and leisure items", () =>
			{
				using var manifestModule = UseManifestModule("shared-preindustrial", "medieval", "renaissance", "earlymodern");
				SeedSharedPreIndustrialFoodCatalogue();
				SeedSharedPreIndustrialLeisureItems();
			});
		}

		if (eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase))
		{
			RunSeedStage("Creating antiquity items", () =>
			{
				using var manifestModule = UseManifestModule("antiquity", "antiquity");
				SeedAntiquityClothing();
				SeedAntiquityHouseholdCraftTools();
				SeedAntiquityWritingImplementsAndDocuments();
				SeedAntiquityMedicalItems();
				SeedAntiquityJewellery();
				SeedAntiquityArmour();
				SeedAntiquityContainers();
				SeedAntiquityDoorsAndLocks();
				SeedAntiquityRepairKits();
				SeedAntiquityHouseholdFurniture();
				SeedAntiquityWeaponsShieldsAccessories();
				SeedAntiquityApiaryItems();
				SeedAntiquityFoodAndBeverageItems();
				SeedAntiquityComponentGapItems();
			});
		}

		if (eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase))
		{
			RunSeedStage("Creating medieval items", () =>
			{
				using var manifestModule = UseManifestModule("medieval", "medieval");
				SeedMedievalClothing();
				SeedMedievalHouseholdCraftTools();
				SeedMedievalWritingAdministrationAndDocuments();
				SeedMedievalMedicalAndApothecaryItems();
				SeedMedievalJewelleryAndDevotionalGoods();
				SeedMedievalArmour();
				SeedMedievalContainers();
				SeedMedievalDoorsLocksAndStrongboxes();
				SeedMedievalRepairKits();
				SeedMedievalHouseholdFurniture();
				SeedMedievalWeaponsShieldsAccessories();
				SeedMedievalFoodAndBeverageItems();
				SeedMedievalFoodCatalogue();
				SeedMedievalFoodProductionFoundationItems();
				SeedMedievalComponentGapItems();
			});
		}

		if (eras.Contains("renaissance", StringComparison.InvariantCultureIgnoreCase))
		{
			RunSeedStage("Creating renaissance items", () =>
			{
				using var manifestModule = UseManifestModule("renaissance", "renaissance");
				SeedRenaissanceItems();
			});
		}

		if (eras.Contains("earlymodern", StringComparison.InvariantCultureIgnoreCase))
		{
			RunSeedStage("Creating early modern items", () =>
			{
				using var manifestModule = UseManifestModule("earlymodern", "earlymodern");
				SeedEarlyModernItems();
			});
		}

		if (HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern"))
		{
			RunSeedStage("Creating documented clothing outfits", () =>
			{
				using var manifestModule = UseManifestModule("outfits", "antiquity", "medieval", "renaissance", "earlymodern");
				SeedDocumentedClothingOutfitManifests(eras);
			});
		}
	}
}
