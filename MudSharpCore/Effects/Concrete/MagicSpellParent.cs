using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.NPC;

namespace MudSharp.Effects.Concrete;

public class MagicSpellParent : Effect, IMagicSpellEffectParent
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("MagicSpellParent", (effect, owner) => new MagicSpellParent(effect, owner));
	}

	public MagicSpellParent(IPerceivable owner, IMagicSpell spell, ICharacter caster) : base(owner, null)
	{
		Spell = spell;
		_caster = caster;
		_casterId = caster.Id;
	}

	protected MagicSpellParent(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		Spell = Gameworld.MagicSpells.Get(long.Parse(trueRoot.Element("Spell").Value));
		_casterId = long.Parse(trueRoot.Element("Caster").Value);
		foreach (var element in trueRoot.Element("Children").Elements())
		{
			var child = (IMagicSpellEffect)LoadEffect(element, owner);
			child.ParentEffect = this;
			_spellEffects.Add(child);
			owner.AddEffect(child);
		}
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Affected by the {Spell.Name.Colour(Spell.School.PowerListColour)} spell.";
	}

	protected override string SpecificEffectType => "MagicSpellParent";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Spell", Spell.Id),
			new XElement("Caster", _casterId),
			new XElement("Children",
				from child in _spellEffects
				select child.SaveToXml(new Dictionary<IEffect, TimeSpan>())
			)
		);
	}

	public override void RemovalEffect()
	{
		foreach (var effect in _spellEffects)
		{
			Owner.RemoveEffect(effect, true);
		}
	}

	#endregion

	#region Implementation of IMagicSpellEffectParent

	public IMagicSpell Spell { get; set; }

	private long _casterId { get; set; }
	private ICharacter _caster;

	public ICharacter Caster
	{
		get
		{
			if (_caster == null)
			{
				_caster = Gameworld.TryGetCharacter(_casterId, true);
				if (!Gameworld.Actors.Has(_caster))
				{
					Gameworld.Add(_caster, _caster is INPC);
				}
			}

			return _caster;
		}
	}

	private readonly List<IMagicSpellEffect> _spellEffects = new();

	public void AddSpellEffect(IMagicSpellEffect effect)
	{
		_spellEffects.Add(effect);
	}

	public IEnumerable<IMagicSpellEffect> SpellEffects => _spellEffects;

	#endregion
}