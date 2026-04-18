#nullable enable

using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class AntiInflammatoryTreatment : Effect, IPainReductionEffect, IPertainToBodypartEffect
{
    protected AntiInflammatoryTreatment(XElement effect, IPerceivable owner) : base(effect, owner)
    {
        XElement? root = effect.Element("Effect");
        Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(root!.Element("Bodypart")!.Value)) ??
                   throw new ApplicationException("AntiInflammatoryTreatment loaded with an invalid bodypart id.");
        PainReductionMultiplier = double.Parse(root.Element("PainReductionMultiplier")!.Value);
        FlatPainReductionAmount = double.Parse(root.Element("FlatPainReductionAmount")!.Value);
    }

    public AntiInflammatoryTreatment(IBody owner, IBodypart bodypart, double painReductionMultiplier,
        double flatPainReductionAmount) : base(owner)
    {
        Bodypart = bodypart;
        PainReductionMultiplier = painReductionMultiplier;
        FlatPainReductionAmount = flatPainReductionAmount;
    }

    public static void InitialiseEffectType()
    {
        RegisterFactory("AntiInflammatoryTreatment", (effect, owner) => new AntiInflammatoryTreatment(effect, owner));
    }

    public static void ApplyOrUpdate(IBody body, IBodypart bodypart, double painReductionMultiplier,
        double flatPainReductionAmount, TimeSpan duration)
    {
        AntiInflammatoryTreatment? effect = body.EffectsOfType<AntiInflammatoryTreatment>().FirstOrDefault(x => x.Bodypart == bodypart);
        if (effect == null)
        {
            body.AddEffect(new AntiInflammatoryTreatment(body, bodypart, painReductionMultiplier, flatPainReductionAmount),
                duration);
            return;
        }

        effect.PainReductionMultiplier = Math.Min(effect.PainReductionMultiplier, painReductionMultiplier);
        effect.FlatPainReductionAmount = Math.Max(effect.FlatPainReductionAmount, flatPainReductionAmount);
        effect.Changed = true;
        body.RescheduleIfLonger(effect, duration);
    }

    protected override string SpecificEffectType => "AntiInflammatoryTreatment";

    public IBodypart Bodypart { get; set; } = null!;
    public double PainReductionMultiplier { get; set; }
    public double FlatPainReductionAmount { get; set; }

    public override bool SavingEffect => true;

    public override bool Applies(object target)
    {
        return Bodypart == target && base.Applies(target);
    }

    protected override XElement SaveDefinition()
    {
        return new XElement("Effect",
            new XElement("Bodypart", Bodypart.Id),
            new XElement("PainReductionMultiplier", PainReductionMultiplier),
            new XElement("FlatPainReductionAmount", FlatPainReductionAmount)
        );
    }

    public override string Describe(IPerceiver voyeur)
    {
        return
            $"{Bodypart.FullDescription().ColourName()} has anti-inflammatory relief reducing pain to {PainReductionMultiplier:P0} with a flat reduction of {FlatPainReductionAmount.ToStringN2Colour(voyeur)}.";
    }
}
