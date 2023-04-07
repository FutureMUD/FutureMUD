using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Effects.Concrete;

public class ItemHidden : Effect, IItemHiddenEffect
{
	public List<long> OriginalWitnesses { get; } = new();

	public ItemHidden(IPerceivable owner, double effectiveHideSkill)
		: base(owner)
	{
		EffectiveHideSkill = effectiveHideSkill;
	}

	public ItemHidden(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		EffectiveHideSkill = double.Parse(effect.Element("Effect").Element("Skill").Value);
		OriginalWitnesses.AddRange(effect.Element("Effect").Elements("Witness").Select(x => long.Parse(x.Value)));
	}

	public double EffectiveHideSkill { get; set; }

	protected override string SpecificEffectType => "ItemHidden";

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new ItemHidden(newItem, EffectiveHideSkill);
		}

		return null;
	}

	public override bool SavingEffect => true;

	public override string Describe(IPerceiver voyeur)
	{
		return $"Item Hidden at {EffectiveHideSkill.ToString("N2", voyeur).Colour(Telnet.Green)} effective skill.";
	}

	public override PerceptionTypes Obscuring => PerceptionTypes.DirectVisual;

	public override bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		if (target is not IPerceiver voyeur || voyeur == Owner)
		{
			return false;
		}

		if (!base.Applies(target))
		{
			return false;
		}

		if (voyeur.AffectedBy<ISawHiddenItemEffect>(Owner) ||
		    flags.HasFlag(PerceiveIgnoreFlags.IgnoreHiding))
		{
			return false;
		}

		if (voyeur is IPerceivableHaveTraits perceiverHasTraits && !flags.HasFlag(PerceiveIgnoreFlags.IgnoreSpotting))
		{
			var outcome = Gameworld.GetCheck(CheckType.SpotStealthCheck)
			                       .Check(perceiverHasTraits, perceiverHasTraits.Location.SpotDifficulty(voyeur),
				                       Owner);
			if (outcome.Outcome.IsPass())
			{
				perceiverHasTraits.AddEffect(new SawHiddenItem(perceiverHasTraits, Owner), TimeSpan.FromSeconds(60));
				return false;
			}
		}

		return true;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Skill", EffectiveHideSkill),
			from witness in OriginalWitnesses select new XElement("Witness", witness));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("ItemHidden", (effect, owner) => new ItemHidden(effect, owner));
	}

	public override string ToString()
	{
		return "Item Hidden Effect";
	}

	public bool KnewOriginalHidingPlace(ICharacter actor)
	{
		return OriginalWitnesses.Contains(actor.Id);
	}
}