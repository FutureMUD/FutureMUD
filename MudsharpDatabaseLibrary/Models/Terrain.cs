using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
	public partial class Terrain
	{
		public Terrain()
		{
			CellOverlays = new HashSet<CellOverlay>();
			TerrainsRangedCovers = new HashSet<TerrainsRangedCovers>();
		}

		public string Name { get; set; }
		public long Id { get; set; }
		public double MovementRate { get; set; }
		public bool DefaultTerrain { get; set; }
		public string TerrainBehaviourMode { get; set; }
		public int HideDifficulty { get; set; }
		public int SpotDifficulty { get; set; }
		public double StaminaCost { get; set; }
		public long ForagableProfileId { get; set; }
		public double InfectionMultiplier { get; set; }
		public int InfectionType { get; set; }
		public int InfectionVirulence { get; set; }
		public long? AtmosphereId { get; set; }
		public string AtmosphereType { get; set; }
		public string TerrainEditorColour { get; set; }
		public string TerrainANSIColour { get; set; }
		public long? WeatherControllerId { get; set; }
		public int DefaultCellOutdoorsType { get; set; }
		public string TerrainEditorText { get; set; }
		public bool CanHaveTracks { get; set; }
		public double TrackIntensityMultiplierVisual { get; set; }
		public double TrackIntensityMultiplierOlfactory { get; set; }

		public virtual WeatherController WeatherController { get; set; }
		public virtual ICollection<CellOverlay> CellOverlays { get; set; }
		public virtual ICollection<TerrainsRangedCovers> TerrainsRangedCovers { get; set; }
	}
}
