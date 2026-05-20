using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

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

		foreach (AgricultureScoreType score in System.Enum.GetValues(typeof(AgricultureScoreType)))
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
