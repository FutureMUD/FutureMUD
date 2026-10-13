#nullable enable
using MudSharp.Body;
using MudSharp.Body.Traits;
using System.Globalization;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.NPC.AI;

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
		if (ArmourConfiguration.FiniteBudget && owner is ICharacter character)
		{
			var budget = ArmourConfiguration.MaximumDamageAbsorbed.Evaluate(character);
			if (!double.IsFinite(budget) || budget < 0) throw new InvalidOperationException("Finite armour requires a nonnegative finite budget.");
			ArmourConfiguration.MaximumDamageAbsorbed = new TraitExpression(budget.ToString("R", CultureInfo.InvariantCulture), Gameworld);
		}
	}

	private SpellArmourProtectionEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		ArmourConfiguration = new MagicArmourConfiguration(effect, Gameworld);
		TotalDamageAbsorbed = double.Parse(effect.Element("TotalDamageAbsorbed")?.Value ?? "0", CultureInfo.InvariantCulture);
		if (ArmourConfiguration.FiniteBudget && owner is ICharacter character) ValidateFiniteState();
	}

	public MagicArmourConfiguration ArmourConfiguration { get; }
	public double TotalDamageAbsorbed { get; protected set; }

	public IDamage PreviewDamage(IDamage damage) => ArmourConfiguration.FiniteBudget ? ResolveFinite(damage, false) : Owner is ICharacter character
		? ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial, Quality, character, true).PassedOn
		: damage;

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
		if (ArmourConfiguration.FiniteBudget) return ResolveFinite(damage, true);
		if (Owner is not ICharacter character)
		{
			return damage;
		}

		(IDamage passOn, IDamage self) = ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial,
			Quality, character, true);
		TotalDamageAbsorbed += Math.Max(0.0, (self?.DamageAmount ?? 0.0) - (passOn?.DamageAmount ?? 0.0));
		CheckDamageAbsorbed();
		return passOn!;
	}

	public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		if (ArmourConfiguration.FiniteBudget) return ResolveFinite(damage, true);
		if (Owner is not ICharacter character)
		{
			return damage;
		}

		(IDamage passOn, IDamage self) = ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial,
			Quality, character, true);
		TotalDamageAbsorbed += Math.Max(0.0, (self?.DamageAmount ?? 0.0) - (passOn?.DamageAmount ?? 0.0));
		CheckDamageAbsorbed();
		return passOn!;
	}

	/// <summary>Budget measures native damage prevented, including dissipation. Pain/stun mitigation
	/// scales with the affordable fraction on the last hit; a partial hit keeps its original damage kind.</summary>
	private IDamage ResolveFinite(IDamage damage, bool commit)
	{
		if (Owner is not ICharacter character) return damage;
		// Validate persisted/runtime numbers before any policy callback or penetration roll.
		var maximum = ValidateFiniteState();
		var body = character.Body;
		var parent = ParentEffect;
		bool Attached() => parent is not null && ReferenceEquals(ParentEffect, parent) &&
			ReferenceEquals(parent.Owner, Owner) && Owner.Effects.Any(x => ReferenceEquals(x, this)) &&
			Owner.Effects.Any(x => ReferenceEquals(x, parent)) && parent.SpellEffects.Any(x => ReferenceEquals(x, this));
		bool Continue()
		{
			if (!Attached() || !CommandExecutionScope.TryContinue()) return false;
			damage.MutationGuard?.Validate(body, damage);
			if (!ReferenceEquals(character.Body, body) ||
				(damage.TargetBody is not null && !ReferenceEquals(damage.TargetBody, body)))
				throw new InvalidOperationException("The protected physical body changed during armour resolution.");
			return Attached();
		}
		if (!Continue()) return damage;
		var remaining = Math.Max(0, maximum - TotalDamageAbsorbed);
		if (remaining <= 0) return damage;
		var passed = ArmourType.AbsorbDamageViaSpell(damage, ArmourConfiguration.ArmourMaterial, Quality, character, true).PassedOn;
		// Native penetration/health checks can call user policy. Fence those callbacks before spending capacity.
		if (!Continue()) return damage;
		maximum = ValidateFiniteState();
		remaining = Math.Max(0, maximum - TotalDamageAbsorbed);
		if (remaining <= 0) return damage;
		var prevented = Math.Max(0, damage.DamageAmount - (passed?.DamageAmount ?? 0));
		var spent = Math.Min(remaining, prevented);
		var fraction = prevented > 0 ? spent / prevented : 1.0;
		var result = passed is null && fraction >= 1.0 ? null : new Damage(passed ?? damage)
		{
			TargetBody = damage.TargetBody,
			MutationGuard = damage.MutationGuard,
			VitalTransfer = damage.VitalTransfer,
			DamageType = fraction < 1.0 ? damage.DamageType : passed?.DamageType ?? damage.DamageType,
			DamageAmount = prevented > 0 ? damage.DamageAmount - spent : passed?.DamageAmount ?? 0,
			PainAmount = damage.PainAmount + ((passed?.PainAmount ?? 0) - damage.PainAmount) * fraction,
			StunAmount = damage.StunAmount + ((passed?.StunAmount ?? 0) - damage.StunAmount) * fraction
		};
		if (commit)
		{
			TotalDamageAbsorbed += spent;
			if (spent > 0) Changed = true;
			if (TotalDamageAbsorbed >= maximum) Owner.RemoveEffect(this, true);
		}
		return result!;
	}

	private double ValidateFiniteState()
	{
		// Finite children freeze this field to an invariant numeric literal at creation. Do not
		// reinterpret corrupted saved literals (e.g. NaN) as unresolved expression parameters.
		if (!double.TryParse(ArmourConfiguration.MaximumDamageAbsorbed.OriginalFormulaText,
			NumberStyles.Float, CultureInfo.InvariantCulture, out var maximum) ||
			!double.IsFinite(maximum) || maximum < 0 || !double.IsFinite(TotalDamageAbsorbed) || TotalDamageAbsorbed < 0)
			throw new InvalidOperationException("Finite armour requires finite nonnegative capacity and spent protection.");
		return maximum;
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
