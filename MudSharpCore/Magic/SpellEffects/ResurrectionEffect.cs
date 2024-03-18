using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public class ResurrectionEffect : IMagicSpellEffectTemplate
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("resurrect", (root, spell) => new ResurrectionEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("resurrect", BuilderFactory);
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		return (new GlowEffect(new XElement("Effect",
			new XAttribute("type", "resurrect"),
			new XElement("HealWounds", "true"),
			new XElement("RestoreSevers", "true")), spell), string.Empty);
	}

	public IMagicSpell Spell { get; }
	public bool HealWounds { get; set; }
	public bool RestoreSevers { get; set; }

	protected ResurrectionEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		HealWounds = bool.Parse(root.Element("HealWounds").Value);
		RestoreSevers = bool.Parse(root.Element("RestoreSevers").Value);
	}

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld => Spell.Gameworld;

	#endregion

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "resurrect"),
			new XElement("HealWounds", HealWounds),
			new XElement("RestoreSevers", RestoreSevers)
		);
	}

	#endregion

	#region Implementation of IEditableItem

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "heal":
				return BuildingCommandHeal(actor);
			case "restore":
				return BuildingCommandRestore(actor);
		}

		actor.OutputHandler.Send(@"You can use the following options with this effect:

    #3heal#0 - toggles healing wounds on the corpse
	#3restore#0 - toggles restoring severed bodyparts on the corpse");
		return false;
	}

	private bool BuildingCommandHeal(ICharacter actor)
	{
		HealWounds = !HealWounds;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will {HealWounds.NowNoLonger()} heal all the wounds on the corpse.");
		return true;
	}

	private bool BuildingCommandRestore(ICharacter actor)
	{
		RestoreSevers = !RestoreSevers;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will {RestoreSevers.NowNoLonger()} restore all severed bodyparts on the corpse.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		return
			$"ResurrectEffect - Heal {HealWounds.ToColouredString()} - Restore Severs {RestoreSevers.ToColouredString()}";
	}

	#endregion

	#region Implementation of IMagicSpellEffectTemplate

	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent)
	{
		var corpse = (target as IGameItem)?.GetItemType<ICorpse>();
		if (corpse is null)
		{
			return null;
		}

		if (HealWounds)
		{
			corpse.OriginalCharacter.Body.CureAllWounds();
		}

		if (RestoreSevers)
		{
			corpse.OriginalCharacter.Body.RestoreAllBodypartsOrgansAndBones();
		}

		corpse.OriginalCharacter.Resurrect(caster.Location);
		corpse.OriginalCharacter.RoomLayer = caster.RoomLayer;
		corpse.Parent.Delete();
		return null;
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new ResurrectionEffect(SaveToXml(), Spell);
	}

	#endregion
}