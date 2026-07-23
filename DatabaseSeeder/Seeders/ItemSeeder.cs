using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using Parlot.Fluent;
using System;
using System.Collections.Generic;
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
    #BRevolution#0 - The age of revolutions, roughly 1750 to 1850 CE
    #BModern#0 - The modern era, roughly 1850 CE to 1945 CE
    #BAtomic#0 - The atomic age, roughly 1945 CE to 1990 CE
    #BComputer#0 - Computer and digital age, roughly 1990 CE to present day


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
							case "revolution":
							case "modern":
							case "atomic":
							case "computer":
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
    public string FullDescription => @"This seeder sets up an item and craft package, to further simplify your building.

This comes with over 850 items and 171 crafts, which while not totally comprehensive, is enough to get you started and give you plenty of examples to copy off for your own building.

The items and crafts are fairly universal and of approximately medieval to reneissance level technology. You can simply not use the items that aren't appropriate for your world. You can also disable any crafts you're not interested in using.";

    private Dictionary<string, GameItemComponentProto> _components = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Tag> _tags = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Tag> _tagsByFullPath = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Material> _materials = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, Liquid> _liquids = new(StringComparer.InvariantCultureIgnoreCase);
    private Dictionary<string, FutureProg> _progs = new(StringComparer.InvariantCultureIgnoreCase);
    private DictionaryWithDefault<string, TraitDefinition> _traits = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, GameItemProto> _items = new(StringComparer.InvariantCultureIgnoreCase);
	private Dictionary<string, Craft> _craftsByNameAndCategory = new(StringComparer.OrdinalIgnoreCase);
    private long _nextId = 1;
	private bool _deferCraftProductSave;

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

        _components = _context.GameItemComponentProtos.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
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
        _materials = _context.Materials.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        _liquids = _context.Liquids.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        _nextId = _context.GameItemProtos.Any()
            ? _context.GameItemProtos.Max(x => x.Id) + 1
            : 1;
        _dbAccount = _context.Accounts.First();

        foreach (TraitDefinition trait in _context.TraitDefinitions)
        {
            _traits[trait.Name] = trait;
        }
		IndexStockSkillPackageTraitAliases();

        foreach (GameItemProto item in _context.GameItemProtos)
        {
            _items[item.ShortDescription] = item;
            if (!string.IsNullOrWhiteSpace(item.UniqueName))
            {
                _items[item.UniqueName] = item;
            }
        }

		_craftsByNameAndCategory = _context.Crafts.Local
			.AsEnumerable()
			.Concat(_context.Crafts.AsEnumerable())
			.GroupBy(x => CraftLookupKey(x.Name, x.Category), StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.OrderBy(craft => craft.Id).First(), StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        _context = context;
        _questionAnswers = questionAnswers;
        InitialiseDependencies();
        SeedReworkItems();
        SeedCrafts();
        _context.SaveChanges();

        return "The operation completed successfully.";
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
            context.Tags.All(x => x.Name != "Functions"))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        return ShouldSeedResult.ReadyToInstall;
    }

    /// <inheritdoc />
    public bool Enabled => false;

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

		if (_items.TryGetValue(stableReference, out var existing))
		{
			ApplyReworkItemMetadata(existing, stableReference, tagList, builderNotes);
			ApplyItemLifecycleSettings(existing, morphToUniqueReference, morphEmote, morphTimer, destroyedItemUniqueReference);
			return existing;
		}

		existing = _context!.GameItemProtos.Local
			.AsEnumerable()
			.FirstOrDefault(x => x.UniqueName?.Equals(stableReference, StringComparison.OrdinalIgnoreCase) == true) ??
				   _context.GameItemProtos.AsEnumerable()
					   .FirstOrDefault(x => x.UniqueName?.Equals(stableReference, StringComparison.OrdinalIgnoreCase) == true);
		if (existing is null && allowLegacyShortDescriptionMatch)
		{
			existing = _context.GameItemProtos.AsEnumerable()
				.FirstOrDefault(x =>
					string.IsNullOrWhiteSpace(x.UniqueName) &&
					x.ShortDescription.Equals(sdesc, StringComparison.OrdinalIgnoreCase));
		}
		if (existing is not null)
		{
			ApplyReworkItemMetadata(existing, stableReference, tagList, builderNotes);
			CacheReworkItem(stableReference, existing);
			ApplyItemLifecycleSettings(existing, morphToUniqueReference, morphEmote, morphTimer, destroyedItemUniqueReference);
			return existing;
		}

		GameItemProto dbitem = new()
		{
			Id = _nextId++,
			Name = noun.ToLowerInvariant(),
			UniqueName = GameItemProtoLookupExtensions.NormaliseUniqueName(stableReference),
			BuilderNotes = BuildReworkItemBuilderNotes(stableReference, tagList, builderNotes),
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
		foreach (string item in tagList)
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}

			if (!_tagsByFullPath.ContainsKey(item))
			{
				return null;
			}

			dbitem.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = dbitem,
				TagId = _tagsByFullPath[item].Id
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
		return dbitem;
	}

	private void CacheReworkItem(string stableReference, GameItemProto item)
	{
		_items[stableReference] = item;
		_items[item.ShortDescription] = item;
		if (!string.IsNullOrWhiteSpace(item.UniqueName))
		{
			_items[item.UniqueName] = item;
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

		return tagList;
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
		item.BuilderNotes = MergeBuilderNotes(
			item.BuilderNotes,
			BuildReworkItemBuilderNotes(stableReference, tags, builderNotes));
		ApplyReworkItemTags(item, tags);
	}

	private void ApplyReworkItemTags(GameItemProto item, IEnumerable<string> tags)
	{
		var existingTagIds = item.GameItemProtosTags
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


	private static bool HasAnyEra(string eras, params string[] eraKeys)
	{
		return eraKeys.Any(x => eras.Contains(x, StringComparison.InvariantCultureIgnoreCase));
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
			SeedSharedPreIndustrialBaselineItems();
		}

		if (eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase))
		{
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
		}

		if (eras.Contains("medieval", StringComparison.InvariantCultureIgnoreCase))
		{
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
			SeedMedievalComponentGapItems();
		}

		if (eras.Contains("renaissance", StringComparison.InvariantCultureIgnoreCase))
		{
			SeedRenaissanceItems();
		}

		if (eras.Contains("earlymodern", StringComparison.InvariantCultureIgnoreCase))
		{
			SeedEarlyModernItems();
		}

		if (eras.Contains("revolution", StringComparison.InvariantCultureIgnoreCase))
		{

		}

		if (eras.Contains("modern", StringComparison.InvariantCultureIgnoreCase))
		{

		}

		if (eras.Contains("atomic", StringComparison.InvariantCultureIgnoreCase))
		{

		}

		if (eras.Contains("computer", StringComparison.InvariantCultureIgnoreCase))
		{

		}

		SeedDocumentedClothingOutfitManifests(eras);
	}
}
