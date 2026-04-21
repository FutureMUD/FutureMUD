#nullable enable
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellArmourProtectionEffect : SimpleSpellStatusEffectBase, IMagicArmour, IDescriptionAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellArmourProtection", (effect, owner) => new SpellArmourProtectionEffect(effect, owner));
	}

	public SpellArmourProtectionEffect(IPerceivable owner, IMagicSpellEffectParent parent,
		MagicArmourConfiguration armourConfiguration, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		ArmourConfiguration = new MagicArmourConfiguration(armourConfiguration);
	}

	private SpellArmourProtectionEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		ArmourConfiguration = new MagicArmourConfiguration(effect, Gameworld);
		TotalDamageAbsorbed = double.Parse(effect.Element("TotalDamageAbsorbed")?.Value ?? "0");
	}

	public MagicArmourConfiguration ArmourConfiguration { get; }
	public double TotalDamageAbsorbed { get; protected set; }

	protected override XElement SaveDefinition()
	{
		var root = SimpleSaveDefinition(new XElement("TotalDamageAbsorbed", TotalDamageAbsorbed));
		ArmourConfiguration.SaveToXml(root);
		return root;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Protected by spell-forged magical armour.";
	}

	protected override string SpecificEffectType => "SpellArmourProtection";

	private void CheckDamageAbsorbed()
	{
		if (Owner is not ICharacter character)
		{
			return;
		}

		var max = ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(character);
		if (max > 0.0 && TotalDamageAbsorbed >= max)
		{
			Owner.RemoveEffect(this, true);
		}
	}

	public IDamage SufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		if (Owner is not ICharacter character)
		{
			return damage;
		}

		(IDamage passOn, IDamage self) = ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial,
			Quality, character, true);
		TotalDamageAbsorbed += passOn?.DamageAmount ?? 0.0;
		CheckDamageAbsorbed();
		return self;
	}

	public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		if (Owner is not ICharacter character)
		{
			return damage;
		}

		(IDamage passOn, IDamage self) = ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial,
			Quality, character, true);
		TotalDamageAbsorbed += passOn?.DamageAmount ?? 0.0;
		CheckDamageAbsorbed();
		return self;
	}

	public void ProcessPassiveWound(IWound wound)
	{
	}

	public IArmourType ArmourType => ArmourConfiguration.ArmourType;
	public ItemQuality Quality => ArmourConfiguration.Quality;
	public string MagicArmourOriginDescription => $"{Spell.Name.Colour(Spell.School.PowerListColour)} Spell";

	public bool AppliesToPart(IBodypart bodypart)
	{
		return ArmourConfiguration.AppliesToBodypart(bodypart);
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		if (Owner is not ICharacter character || string.IsNullOrEmpty(ArmourConfiguration.FullDescriptionAddendum))
		{
			return string.Empty;
		}

		if (!ArmourConfiguration.ArmourCanBeObscuredByInventory ||
		    character.Body.ExposedBodyparts.All(x => !AppliesToPart(x)))
		{
			return new EmoteOutput(new Emote(ArmourConfiguration.FullDescriptionAddendum.SubstituteANSIColour(),
				character, character)).ParseFor(voyeur);
		}

		return string.Empty;
	}

	public bool PlayerSet => false;
}
