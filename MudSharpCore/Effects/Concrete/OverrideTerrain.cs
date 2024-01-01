using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;

public class OverrideTerrain : Effect, IOverrideTerrain
{
	#region Static Initialisation
	public static void InitialiseEffectType()
	{
		RegisterFactory("OverrideTerrain", (effect, owner) => new OverrideTerrain(effect, owner));
	}
	#endregion

	#region Constructors
	public OverrideTerrain(IPerceivable owner, ITerrain terrain, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Terrain = terrain;
	}

	protected OverrideTerrain(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Terrain = Gameworld.Terrains.Get(long.Parse(root!.Element("Terrain")!.Value));
	}
	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Terrain", Terrain.Id)
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "OverrideTerrain";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Overriding the terrain type with {Terrain.Name.ColourForegroundCustom(Terrain.TerrainANSIColour)}.";
	}

	public override bool SavingEffect => true;
	#endregion

	#region Implementation of IOverrideTerrain

	/// <inheritdoc />
	public ITerrain Terrain { get; }

	#endregion
}