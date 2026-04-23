#nullable enable
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellTransformFormEffect : SimpleSpellStatusEffectBase
{
	private long _priorBodyId;
	private long _formBodyId;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellTransformForm", (effect, owner) => new SpellTransformFormEffect(effect, owner));
	}

	public SpellTransformFormEffect(IPerceivable owner, IMagicSpellEffectParent parent, string formKey, long formBodyId,
		long priorBodyId, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		FormKey = formKey;
		_formBodyId = formBodyId;
		_priorBodyId = priorBodyId;
	}

	private SpellTransformFormEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		FormKey = root.Element("FormKey")?.Value ?? string.Empty;
		_formBodyId = long.Parse(root.Element("FormBodyId")?.Value ?? "0");
		_priorBodyId = long.Parse(root.Element("PriorBodyId")?.Value ?? "0");
	}

	public string FormKey { get; }
	public long PriorBodyId => _priorBodyId;
	public long FormBodyId => _formBodyId;

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("FormKey", FormKey),
			new XElement("FormBodyId", _formBodyId),
			new XElement("PriorBodyId", _priorBodyId)
		);
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character)
		{
			if (character.CurrentBody.Id != _formBodyId)
			{
				base.RemovalEffect();
				return;
			}

			if (character.EffectsOfType<SpellTransformFormEffect>()
			             .Any(x => !ReferenceEquals(x, this) &&
			                       x.Spell == Spell &&
			                       x.FormKey.EqualTo(FormKey)))
			{
				base.RemovalEffect();
				return;
			}

			if (_priorBodyId != 0 && _priorBodyId != character.CurrentBody.Id)
			{
				var priorForm = character.Forms.FirstOrDefault(x => x.Body.Id == _priorBodyId);
				if (priorForm is not null &&
				    character.CanSwitchBody(priorForm.Body, BodySwitchIntent.Scripted, out _))
				{
					character.SwitchToBody(priorForm.Body, BodySwitchIntent.Scripted);
					base.RemovalEffect();
					return;
				}
			}

			var fallbackForm = character.Forms
			                           .FirstOrDefault(x => x.Body != character.CurrentBody &&
			                                                character.CanSwitchBody(x.Body, BodySwitchIntent.Scripted,
				                                                out _));
			if (fallbackForm is not null)
			{
				character.SwitchToBody(fallbackForm.Body, BodySwitchIntent.Scripted);
			}
			else
			{
				character.Gameworld.SystemMessage(
					$"Character #{character.Id.ToString("N0")} could not revert from spell-granted form '{FormKey}'.",
					true
				);
			}
		}

		base.RemovalEffect();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically transformed into an alternate form.";
	}

	protected override string SpecificEffectType => "SpellTransformForm";
}
