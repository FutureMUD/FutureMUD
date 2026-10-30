#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public sealed class KeywordDescriptionsEffect : Effect, IKeywordDescriptionProvider
{
	private readonly List<KeywordDescription> _descriptions;
	public IReadOnlyCollection<KeywordDescription> KeywordDescriptions => _descriptions;

	public KeywordDescriptionsEffect(IPerceivable owner, IEnumerable<KeywordDescription> descriptions) : base(owner)
	{
		_descriptions = descriptions.ToList();
	}

	public KeywordDescriptionsEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		_descriptions = (effect.Element("Effect")?.Element("KeywordDescriptions")?.Elements("Entry") ?? [])
			.Select(x => new KeywordDescription((string?)x.Element("Keyword") ?? string.Empty,
				(string?)x.Element("Prose") ?? string.Empty))
			.Where(x => !string.IsNullOrWhiteSpace(x.Keywords) && !string.IsNullOrWhiteSpace(x.Description))
			.ToList();
	}

	protected override string SpecificEffectType => "KeywordDescriptions";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => "Saved keyword description entries";

	protected override XElement SaveDefinition() => new XElement("Effect",
		new XElement("KeywordDescriptions", _descriptions.Select(x => new XElement("Entry",
			new XElement("Keyword", x.Keywords), new XElement("Prose", x.Description)))));

	public static void InitialiseEffectType() =>
		RegisterFactory("KeywordDescriptions", (effect, owner) => new KeywordDescriptionsEffect(effect, owner));
}
