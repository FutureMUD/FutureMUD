#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellMagicTagEffect : SimpleSpellStatusEffectBase, IMagicTagEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellMagicTag", (effect, owner) => new SpellMagicTagEffect(effect, owner));
	}

	public SpellMagicTagEffect(IPerceivable owner, IMagicSpellEffectParent parent, string tag, string value,
		IFutureProg? prog = null) : base(owner, parent, prog)
	{
		Tag = tag.Trim();
		Value = value;
	}

	private SpellMagicTagEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Tag = trueRoot?.Element("Tag")?.Value ?? string.Empty;
		Value = trueRoot?.Element("Value")?.Value ?? string.Empty;
	}

	public string Tag { get; }
	public string Value { get; }
	public ICharacter? Caster => ParentEffect?.Caster;

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("Tag", new XCData(Tag)),
			new XElement("Value", new XCData(Value))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Magic tag {Tag.ColourName()} = {Value.ColourValue()}";
	}

	protected override string SpecificEffectType => "SpellMagicTag";
}
