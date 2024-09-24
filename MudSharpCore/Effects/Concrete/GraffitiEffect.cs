using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction;
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
	public GraffitiEffect(IPerceivable owner, CompositeWriting graffiti, RoomLayer layer, string localeDescription) : base(owner, null)
	{
		_writingId = graffiti.Id;
		_writing = graffiti;
		Layer = layer;
		LocaleDescription = localeDescription;
	}

	protected GraffitiEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		_writingId = long.Parse(root!.Element("Writing")!.Value);
		Layer = (RoomLayer)int.Parse(root!.Element("RoomLayer").Value);
		LocaleDescription = root!.Element("LocaleDescription")!.Value;
	}
	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Writing", _writingId),
			new XElement("RoomLayer", (int)Layer),
			new XElement("LocaleDescription", new XCData(LocaleDescription ?? string.Empty))
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "GraffitiEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return 
			!string.IsNullOrEmpty(LocaleDescription) ?
			$"{Writing?.DescribeInLook(voyeur as ICharacter) ?? "an unknown graffiti".ColourError()} on {LocaleDescription.ColourValue()}" :
			$"{Writing?.DescribeInLook(voyeur as ICharacter) ?? "an unknown graffiti".ColourError()}";
	}

	public override bool SavingEffect => true;
	#endregion

	private readonly long _writingId;
	private CompositeWriting _writing;
	/// <inheritdoc />
	public IGraffitiWriting Writing => _writing ??= Gameworld.Writings.Get(_writingId) as CompositeWriting;

	/// <inheritdoc />
	public string LocaleDescription { get; }
	public RoomLayer Layer { get; }
	public bool IsJustDrawing => !_writing.LanguageRegex.IsMatch(_writing.Text);

	/// <inheritdoc />
	public IEnumerable<string> Keywords => new ExplodedString(Writing?.DescribeInLook(null).Strip_A_An() ?? "").Words;

	/// <inheritdoc />
	public IEnumerable<string> GetKeywordsFor(IPerceiver voyeur)
	{
		return new ExplodedString(Writing?.DescribeInLook(voyeur as ICharacter).Strip_A_An() ?? "").Words;
	}
}
