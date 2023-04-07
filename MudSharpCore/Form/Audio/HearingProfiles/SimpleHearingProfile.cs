using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A SimpleHearingProfile is a class implementing IHearingProfile that provides a flat, constant difficulty for
///     various proximities for the location
/// </summary>
public class SimpleHearingProfile : HearingProfile
{
	private readonly Dictionary<Tuple<AudioVolume, Proximity>, Difficulty> _difficultyMap =
		new();

	private Difficulty _defaultDifficulty;

	public SimpleHearingProfile(MudSharp.Models.HearingProfile profile)
		: base(profile)
	{
	}

	public override string FrameworkItemType => "SimpleHearingProfile";

	public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
	{
		return _difficultyMap.ContainsKey(Tuple.Create(volume, proximity))
			? _difficultyMap[Tuple.Create(volume, proximity)]
			: _defaultDifficulty;
	}

	public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		var root = XElement.Parse(profile.Definition);
		var element = root.Element("DefaultDifficulty");
		if (element != null)
		{
			_defaultDifficulty = (Difficulty)Convert.ToInt32(element.Value);
		}
		else
		{
			_defaultDifficulty = Difficulty.Automatic;
		}

		element = root.Element("Difficulties");
		if (element != null)
		{
			foreach (var sub in element.Elements("Difficulty"))
			{
				_difficultyMap.Add(
					Tuple.Create((AudioVolume)Convert.ToInt32(sub.Attribute("Volume").Value),
						(Proximity)Convert.ToInt32(sub.Attribute("Proximity").Value)),
					(Difficulty)Convert.ToInt32(sub.Value));
			}
		}
	}
}