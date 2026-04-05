using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class OnFire : Effect
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("OnFire", (effect, owner) => new OnFire(effect, owner));
    }

    public static OnFire Apply(IPerceivable owner, IFireProfile profile)
    {
        OnFire existing = owner.EffectsOfType<OnFire>().FirstOrDefault();
        if (existing is not null)
        {
            existing.FireProfile = profile;
            owner.Reschedule(existing, profile.TickFrequency);
            return existing;
        }

        OnFire effect = new(owner, profile);
        owner.AddEffect(effect, profile.TickFrequency);
        return effect;
    }

    public OnFire(IPerceivable owner, IFireProfile profile, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
    {
        FireProfile = profile;
    }

    protected OnFire(XElement root, IPerceivable owner) : base(root, owner)
    {
        FireProfile = new FireProfile(root.Element("Effect")?.Element("FireProfile") ?? new XElement("FireProfile"), owner.Gameworld);
    }

    public IFireProfile FireProfile { get; set; }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Burning with {FireProfile.Name.ColourValue()}";
    }

    protected override string SpecificEffectType => "OnFire";
    public override bool SavingEffect => true;

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect", (FireProfile as FireProfile)?.SaveToXml() ?? new FireProfile(Gameworld).SaveToXml());
    }

    private bool CanSustainFire()
    {
        if (FireProfile.SelfOxidising)
        {
            return true;
        }

        return (Owner.Location?.Atmosphere as IGas)?.OxidationFactor >= FireProfile.MinimumOxidation;
    }

    private void ApplyThermalLoad()
    {
        if (Owner is not ICharacter ch)
        {
            return;
        }

        ThermalImbalance thermal = ch.CombinedEffectsOfType<ThermalImbalance>().FirstOrDefault();
        if (thermal is null)
        {
            thermal = new ThermalImbalance(ch);
            ch.AddEffect(thermal);
        }

        thermal.ImbalanceProgress += FireProfile.ThermalLoadPerTick;
    }

    private IEnumerable<IWound> ApplyBurnDamage()
    {
        switch (Owner)
        {
            case ICharacter ch:
                {
                    IBodypart bodypart = ch.Body.RandomBodyPartGeometry(Orientation.Centre, Alignment.Front, Body.Facing.Front, false);
                    return ch.PassiveSufferDamage(new Damage
                    {
                        ActorOrigin = ch,
                        Bodypart = bodypart,
                        DamageAmount = FireProfile.DamagePerTick,
                        PainAmount = FireProfile.PainPerTick,
                        StunAmount = FireProfile.StunPerTick,
                        ShockAmount = 0.0,
                        DamageType = FireProfile.DamageType,
                        AngleOfIncidentRadians = Math.PI * 0.5,
                        PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
                    });
                }
            case IGameItem item:
                return item.PassiveSufferDamage(new Damage
                {
                    ActorOrigin = null,
                    DamageAmount = FireProfile.DamagePerTick,
                    PainAmount = 0.0,
                    StunAmount = 0.0,
                    ShockAmount = 0.0,
                    DamageType = FireProfile.DamageType,
                    AngleOfIncidentRadians = Math.PI * 0.5,
                    PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
                });
            default:
                return Enumerable.Empty<IWound>();
        }
    }

    private void AttemptSpread()
    {
        if (Owner is not IPositionable positionable || !RandomUtilities.Roll(1.0, FireProfile.SpreadChance))
        {
            return;
        }

        foreach (IPerceivable target in positionable.LocalThingsAndProximities()
                                         .Where(x => x.Proximity <= Proximity.Immediate)
                                         .Select(x => x.Thing)
                                         .Where(x => x != Owner)
                                         .Take(2))
        {
            Apply(target, FireProfile);
        }
    }

    public override void ExpireEffect()
    {
        if (!CanSustainFire())
        {
            base.ExpireEffect();
            return;
        }

        ApplyBurnDamage().ProcessPassiveWounds();
        ApplyThermalLoad();
        AttemptSpread();
        Owner.Reschedule(this, FireProfile.TickFrequency);
    }
}
