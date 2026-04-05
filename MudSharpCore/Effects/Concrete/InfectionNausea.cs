#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using System.Collections.Generic;

namespace MudSharp.Effects.Concrete;

public class InfectionNausea : Effect, ICauseDrugEffect
{
    public InfectionNausea(IBody owner, double intensity) : base(owner)
    {
        Intensity = intensity;
        owner.CheckDrugTick();
    }

    protected override string SpecificEffectType => "InfectionNausea";

    public double Intensity { get; set; }

    public IEnumerable<DrugType> AffectedDrugTypes => [DrugType.Nausea];

    public double AddedIntensity(ICharacter character, DrugType drugtype)
    {
        return drugtype == DrugType.Nausea ? Intensity : 0.0;
    }

    public override void RemovalEffect()
    {
        base.RemovalEffect();
        if (Owner is IBody body)
        {
            body.CheckDrugTick();
        }
    }

    public override string Describe(IPerceiver voyeur)
    {
        return $"Infection-driven nausea of intensity {Intensity:N2}.";
    }
}
