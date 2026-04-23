#nullable enable
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellTransformFormEffect : SimpleSpellStatusEffectBase
{
	private long _priorBodyId;
	private long _formBodyId;
	private ForcedTransformationPriorityBand _priorityBand;
	private int _priorityOffset;
	private long _appliedAtUtcTicks;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellTransformForm", (effect, owner) => new SpellTransformFormEffect(effect, owner));
	}

	public SpellTransformFormEffect(IPerceivable owner, IMagicSpellEffectParent parent, string formKey, long formBodyId,
		long priorBodyId, ForcedTransformationPriorityBand priorityBand, int priorityOffset, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		_formBodyId = formBodyId;
		_priorBodyId = priorBodyId;
		_priorityBand = priorityBand;
		_priorityOffset = priorityOffset;
		_appliedAtUtcTicks = DateTime.UtcNow.Ticks;
	}

	private SpellTransformFormEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		FormKey = root.Element("FormKey")?.Value ?? string.Empty;
		_formBodyId = long.Parse(root.Element("FormBodyId")?.Value ?? "0");
		_priorBodyId = long.Parse(root.Element("PriorBodyId")?.Value ?? "0");
		if (int.TryParse(root.Element("PriorityBand")?.Value, out var priorityBand))
		{
			_priorityBand = (ForcedTransformationPriorityBand)priorityBand;
		}

		_ = int.TryParse(root.Element("PriorityOffset")?.Value, out _priorityOffset);
		_appliedAtUtcTicks = long.Parse(root.Element("AppliedAtUtcTicks")?.Value ?? "0");
	}

	public string FormKey { get; }
	public long PriorBodyId => _priorBodyId;
	public long FormBodyId => _formBodyId;
	public ForcedTransformationPriorityBand PriorityBand => _priorityBand;
	public int PriorityOffset => _priorityOffset;
	public long AppliedAtUtcTicks => _appliedAtUtcTicks;

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("FormBodyId", _formBodyId),
			new XElement("PriorBodyId", _priorBodyId),
			new XElement("PriorityBand", (int)_priorityBand),
			new XElement("PriorityOffset", _priorityOffset),
			new XElement("AppliedAtUtcTicks", _appliedAtUtcTicks)
		);
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is ICharacter character && character.OutputHandler is not null)
		{
			character.ReevaluateForcedBodyTransformation();
		}
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		(Owner as ICharacter)?.ReevaluateForcedBodyTransformation();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically transformed into an alternate form.";
	}

	protected override string SpecificEffectType => "SpellTransformForm";
}
