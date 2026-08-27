using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using System.IO;

namespace MudSharp.Construction.Autobuilder.Areas;

public class AutobuilderAreaTerrainFeatureRectangle : AutobuilderAreaTerrainRectangle
{
    public new static void RegisterAutobuilderLoader()
    {
        AutobuilderFactory.RegisterLoader("terrain feature rectangle",
            (area, gameworld) => new AutobuilderAreaTerrainFeatureRectangle(area, gameworld));
        AutobuilderFactory.RegisterBuilderLoader("terrain feature rectangle",
            (gameworld, name) => new AutobuilderAreaTerrainFeatureRectangle(name, gameworld));
    }

    protected AutobuilderAreaTerrainFeatureRectangle(string name, IFuturemud gameworld, string type = null) : base(name,
        gameworld, type ?? "terrain feature rectangle")
    {
    }

    protected AutobuilderAreaTerrainFeatureRectangle(Models.AutobuilderAreaTemplate area, IFuturemud gameworld) : base(
        area, gameworld)
    {
    }

    protected override void SetupParameters()
    {
        base.SetupParameters();
        _parameters.Add(new AutobuilderCustomParameter
        {
            Gameworld = Gameworld,
            IsOptional = false,
            ParameterName = "featuresmask",
            MissingErrorMessage =
                "You must enter a mask of tag IDs for each location, separated by vertical lines (|) within the location and commas between the locations, starting from the bottom left corner of the rectangle and proceeding right and up.",
            TypeName = "feature mask",
            IsValidArgumentFunction = (arg, game, args) =>
            {
                int height = (int)args[0];
                int width = (int)args[1];
                return AutobuilderFeatureMask.TryParse(arg, height * width, game.Tags, out _, out _);
            },
            WhyIsNotValidArgumentFunction = (arg, game, args) =>
            {
                int height = (int)args[0];
                int width = (int)args[1];
                AutobuilderFeatureMask.TryParse(arg, height * width, game.Tags, out _, out string error);
                return error;
            },
            GetArgumentFunction = (arg, game) => AutobuilderFeatureMask.Parse(arg, game.Tags)
        });
    }

    public override IEnumerable<ICell> ExecuteTemplate(ICharacter builder, IEnumerable<object> arguments)
    {
        ICellOverlayPackage package = builder.CurrentOverlayPackage;
        List<object> argList = arguments.ToList();
        int height = (int)argList.ElementAt(0);
        int width = (int)argList.ElementAt(1);
        IAutobuilderRoom roomTemplate = (IAutobuilderRoom)argList.ElementAt(2);
        ITerrain[] terrainArg = (ITerrain[])argList.ElementAt(3);
        ITag[][] featureArg = (ITag[][])argList.ElementAt(4);

        ITerrain[,] terrains = new ITerrain[width, height];
        ITag[,][] features = new ITag[width, height][];
        int x = 0, y = 0;
        for (int i = 0; i < terrainArg.Length; i++)
        {
            terrains[x, y] = terrainArg[i];
            features[x, y] = featureArg[i];
            if (++x == width)
            {
                x = 0;
                y++;
            }
        }

        ICell[,] cells = new ICell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (terrains[i, j] == null)
                {
                    continue;
                }

                ITag[] tags = features[i, j];
                ICell cell = roomTemplate.CreateRoom(builder, terrains[i, j], false, tags,
                    tags.Select(x => x.Name).ToArray());
                cells[i, j] = cell;

            }
        }

		AutobuilderRectangleTopology.ConnectCells(builder, package, cells, ConnectCellsWithDiagonalExits);

        foreach (ICell cell in cells)
        {
            if (cell == null)
            {
                continue;
            }

            builder.Gameworld.ExitManager.UpdateCellOverlayExits(cell, cell.CurrentOverlay);
        }

        return cells.OfType<ICell>().ToList();
    }

    public override string Show(ICharacter builder)
    {
        return
            $"{$"Autobuilder Area Template #{Id} ({Name})".Colour(Telnet.Cyan)}\n\n{$"This autobuilder template will return a rectangular area of cells with height, width, terrain, room features and room template supplied by the builder. It also requires the builder to specify a matching mask of tag IDs to be applied to the generated rooms. This template {(ConnectCellsWithDiagonalExits ? "does" : "does not")} connect rooms diagonally.".Wrap(builder.InnerLineFormatLength)}";
    }

    public override IAutobuilderArea Clone(string newName)
    {
        using (new FMDB())
        {
            Models.AutobuilderAreaTemplate dbitem = new()
            {
                Name = newName,
                Definition = SaveToXml().ToString(),
                TemplateType = "terrain feature rectangle"
            };
            FMDB.Context.AutobuilderAreaTemplates.Add(dbitem);
            FMDB.Context.SaveChanges();
            return new AutobuilderAreaTerrainFeatureRectangle(dbitem, Gameworld);
        }
    }
}

public static class AutobuilderFeatureMask
{
	public static bool TryParse(string mask, int expectedCellCount, IEnumerable<ITag> tags,
		out ITag[][] result, out string error)
	{
		try
		{
			result = Parse(mask, expectedCellCount, tags);
			error = string.Empty;
			return true;
		}
		catch (InvalidDataException exception)
		{
			result = [];
			error = exception.Message;
			return false;
		}
	}

	public static ITag[][] Parse(string mask, IEnumerable<ITag> tags)
	{
		var entries = SplitEntries(mask);
		return Parse(entries, entries.Length, tags);
	}

	public static ITag[][] Parse(string mask, int expectedCellCount, IEnumerable<ITag> tags)
	{
		return Parse(SplitEntries(mask), expectedCellCount, tags);
	}

	private static ITag[][] Parse(IReadOnlyList<string> entries, int expectedCellCount, IEnumerable<ITag> tags)
	{
		ArgumentNullException.ThrowIfNull(tags);
		if (entries.Count != expectedCellCount)
		{
			throw new InvalidDataException("The feature mask must exactly match the size of the grid.");
		}

		var tagsById = tags
			.GroupBy(tag => tag.Id)
			.ToDictionary(group => group.Key, group => group.First());
		var result = new ITag[entries.Count][];
		for (var index = 0; index < entries.Count; index++)
		{
			if (string.IsNullOrWhiteSpace(entries[index]))
			{
				result[index] = [];
				continue;
			}

			var tagIds = entries[index]
				.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
				.Select(value => ParseTagId(value, index))
				.Distinct()
				.ToArray();
			var cellTags = new List<ITag>(tagIds.Length);
			foreach (var tagId in tagIds)
			{
				if (!tagsById.TryGetValue(tagId, out var tag))
				{
					throw new InvalidDataException($"Feature mask entry {index + 1} refers to unknown tag ID {tagId}.");
				}

				cellTags.Add(tag);
			}

			result[index] = cellTags.ToArray();
		}

		return result;
	}

	private static string[] SplitEntries(string mask) =>
		mask.Trim().Split(',', StringSplitOptions.None);

	private static long ParseTagId(string value, int entryIndex)
	{
		if (!long.TryParse(value, System.Globalization.NumberStyles.None,
			System.Globalization.CultureInfo.InvariantCulture, out long tagId) || tagId <= 0)
		{
			throw new InvalidDataException(
				$"Feature mask entry {entryIndex + 1} contains '{value}', which is not a positive tag ID.");
		}

		return tagId;
	}
}
