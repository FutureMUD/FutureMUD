using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;
public class ItemSeeder : IDatabaseSeeder
{
	/// <inheritdoc />
	public IEnumerable<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => new List<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
	{

	};

	/// <inheritdoc />
	public int SortOrder => 400;

	/// <inheritdoc />
	public string Name => "Items";

	/// <inheritdoc />
	public string Tagline => "A default collection of items and crafts";

	/// <inheritdoc />
	public string FullDescription => @"This seeder sets up an item and craft package, to further simplify your building.";

	private Dictionary<string, MudSharp.Models.GameItemComponentProto> _components = new(StringComparer.InvariantCultureIgnoreCase);
	private Dictionary<string, MudSharp.Models.Tag> _tags = new(StringComparer.InvariantCultureIgnoreCase);
	private Dictionary<string, MudSharp.Models.Material> _materials = new(StringComparer.InvariantCultureIgnoreCase);
	private Dictionary<string, MudSharp.Models.Liquid> _liquids = new(StringComparer.InvariantCultureIgnoreCase);

	private FuturemudDatabaseContext? _context;
	private IReadOnlyDictionary<string, string>? _questionAnswers;

	private void InitialiseDependencies()
	{
		if (_context is null)
		{
			throw new ApplicationException("Context cannot be null at this point.");
		}

		_components = _context.GameItemComponentProtos.ToDictionary(x => x.Name, x => x);
		_tags = _context.Tags.ToDictionary(x => x.Name, x => x);
		_materials = _context.Materials.ToDictionary(x => x.Name, x => x);
		_liquids = _context.Liquids.ToDictionary(x => x.Name, x => x);
	}

	MudSharp.Models.GameItemProto CreateItem(string noun, string sdesc, string? ldesc, string fdesc, SizeCategory size, ItemQuality quality, double weightInGrams, decimal inherentCost, bool skinnable, bool hideFromPlayers, string material, IEnumerable<string> tags, IEnumerable<string> components)
	{
		var dbitem = new MudSharp.Models.GameItemProto
		{
			Name = noun,
			Keywords = null,
			MaterialId = _materials[material].Id,
			EditableItemId = 0,
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
		};
		foreach (var item in tags)
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}

			dbitem.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProto = dbitem,
				TagId = _tags[item].Id
			});
		}

		foreach (var item in components)
		{
			if (string.IsNullOrEmpty(item))
			{
				continue;
			}

			dbitem.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
			{
				GameItemProto = dbitem,
				GameItemComponent = _components[item]
			});
		}

		_context!.GameItemProtos.Add(dbitem);
		return dbitem;
	}

	/// <inheritdoc />
	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		_context = context;
		_questionAnswers = questionAnswers;
		InitialiseDependencies();



		return "The operation completed successfully.";
	}

	/// <inheritdoc />
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (context.GameItemComponentProtos.All(x => x.Name != "Container_Table") ||
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
}
