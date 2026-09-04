using MudSharp.Database;
using MudSharp.Framework.Save;

using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Work.Agriculture;

public class AgricultureFieldProfile : SaveableItem, IAgricultureFieldProfile
{
	private readonly Dictionary<AgricultureScoreType, int> _defaultScores = new();
	private readonly HashSet<AgricultureFieldUse> _allowedUses = new();

	public AgricultureFieldProfile(Models.AgricultureFieldProfile profile, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = profile.Id;
		_name = profile.Name;
		Description = profile.Description;
		LoadDefinition(profile.Definition);
	}

	public AgricultureFieldProfile(IFuturemud gameworld, string name, string description,
		IReadOnlyDictionary<AgricultureScoreType, int> scores,
		IEnumerable<AgricultureFieldUse> allowedUses)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		foreach (var score in scores)
		{
			_defaultScores[score.Key] = score.Value.ClampScore();
		}

		foreach (var use in allowedUses)
		{
			_allowedUses.Add(use);
		}

		using (new FMDB())
		{
			var dbitem = new Models.AgricultureFieldProfile
			{
				Name = Name,
				Description = Description,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AgricultureFieldProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureFieldProfile";
	public ProgVariableTypes Type => ProgVariableTypes.AgricultureFieldProfile;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"description" => new TextVariable(Description),
			"defaultscorecount" => new NumberVariable(_defaultScores.Count),
			"allowsfallow" => new BooleanVariable(AllowsUse(AgricultureFieldUse.Fallow)),
			"allowscrop" => new BooleanVariable(AllowsUse(AgricultureFieldUse.Crop)),
			"allowspasture" => new BooleanVariable(AllowsUse(AgricultureFieldUse.Pasture)),
			"allowswoodland" => new BooleanVariable(AllowsUse(AgricultureFieldUse.Woodland)),
			"allowsorchard" => new BooleanVariable(AllowsUse(AgricultureFieldUse.Orchard)),
			_ => throw new NotSupportedException($"Unsupported agriculture field profile property {property}.")
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.AgricultureFieldProfile,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["description"] = ProgVariableTypes.Text,
				["defaultscorecount"] = ProgVariableTypes.Number,
				["allowsfallow"] = ProgVariableTypes.Boolean,
				["allowscrop"] = ProgVariableTypes.Boolean,
				["allowspasture"] = ProgVariableTypes.Boolean,
				["allowswoodland"] = ProgVariableTypes.Boolean,
				["allowsorchard"] = ProgVariableTypes.Boolean
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The stable field-profile identity.",
				["name"] = "The field-profile name.",
				["description"] = "The builder-authored field-profile description.",
				["defaultscorecount"] = "The number of configured default agriculture scores.",
				["allowsfallow"] = "Whether fields using this profile can be fallow.",
				["allowscrop"] = "Whether fields using this profile can grow annual crops.",
				["allowspasture"] = "Whether fields using this profile can be pasture.",
				["allowswoodland"] = "Whether fields using this profile can be woodland.",
				["allowsorchard"] = "Whether fields using this profile can be orchard land."
			});
	}
	public string Description { get; private set; }
	public IReadOnlyDictionary<AgricultureScoreType, int> DefaultScores => _defaultScores;

	public bool AllowsUse(AgricultureFieldUse use)
	{
		return _allowedUses.Contains(use);
	}

	public void BuildingSetName(string name)
	{
		_name = name;
		Changed = true;
	}

	public void BuildingSetDescription(string description)
	{
		Description = description;
		Changed = true;
	}

	public void BuildingSetDefaultScore(AgricultureScoreType score, int value)
	{
		_defaultScores[score] = value.ClampScore();
		Changed = true;
	}

	public void BuildingSetAllowedUse(AgricultureFieldUse use, bool allowed)
	{
		if (allowed)
		{
			_allowedUses.Add(use);
		}
		else
		{
			_allowedUses.Remove(use);
		}

		Changed = true;
	}

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Profile");
		foreach (var score in root.LoadScores())
		{
			_defaultScores[score.Key] = score.Value;
		}

		foreach (var score in AgricultureScoreTypeExtensions.ActiveScoreTypes(Gameworld))
		{
			_defaultScores.TryAdd(score, 50);
		}

		foreach (var use in root.LoadUses())
		{
			_allowedUses.Add(use);
		}
	}

	private XElement SaveDefinition()
	{
		var root = AgricultureXmlExtensions.SaveScores("Profile", _defaultScores);
		root.SetAttributeValue("uses", _allowedUses.Select(x => x.ToString()).ListToCommaSeparatedValues());
		return root;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureFieldProfiles.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}
}
