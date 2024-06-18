using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Effects.Concrete;
public class GraffitiEffect : Effect, IGraffitiEffect
{
	#region Static Initialisation
	public static void InitialiseEffectType()
	{
		RegisterFactory("GraffitiEffect", (effect, owner) => new GraffitiEffect(effect, owner));
	}
	#endregion

	#region Constructors
	public GraffitiEffect(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
	}

	protected GraffitiEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Writing = Gameworld.Writings.Get(long.Parse(root!.Element("Writing")!.Value));
		Drawing = Gameworld.Drawings.Get(long.Parse(root!.Element("Drawing")!.Value));
		LocaleDescription = root!.Element("LocaleDescription")!.Value;
	}
	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Writing", Writing?.Id ?? 0),
			new XElement("Drawing", Drawing?.Id ?? 0),
			new XElement("LocaleDescription", new XCData(LocaleDescription ?? string.Empty))
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "GraffitiEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return "An undescribed effect of type GraffitiEffect.";
	}

	public override bool SavingEffect => true;
	#endregion

	/// <inheritdoc />
	public IWriting Writing { get; private set; }

	/// <inheritdoc />
	public IDrawing Drawing { get; private set; }

	/// <inheritdoc />
	public string LocaleDescription { get; private set; }
}
