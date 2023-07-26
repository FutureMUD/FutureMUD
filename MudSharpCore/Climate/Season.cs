using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Framework;

namespace MudSharp.Climate;

public class Season : FrameworkItem, ISeason
{
	public Season(MudSharp.Models.Season season, [NotNull] Futuremud gameworld)
	{
		_id = season.Id;
		_name = season.Name;
		DisplayName = season.DisplayName;
		SeasonGroup = season.SeasonGroup;
		CelestialDayOnset = season.CelestialDayOnset;
		Celestial = gameworld.CelestialObjects.Get(season.CelestialId);
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Season";

	#endregion

	public string DisplayName { get; protected set; }
	public string SeasonGroup { get; protected set; }

	public int CelestialDayOnset { get; protected set; }
	public ICelestialObject Celestial { get; protected set; }
}