using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class OrganFunctionDrugEffect : Effect, IOrganFunctionEffect
{
    public OrganFunctionDrugEffect(IBody owner) : base(owner)
    {
    }

    protected override string SpecificEffectType => "OrganFunctionDrugEffect";

    public Dictionary<BodypartTypeEnum, double> OrganBonuses { get; } = new();

    public void SetBonuses(Dictionary<BodypartTypeEnum, double> bonuses)
    {
        OrganBonuses.Clear();
        foreach (var item in bonuses)
        {
            OrganBonuses[item.Key] = item.Value;
        }
    }

    public IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body)
    {
        foreach (var bonus in OrganBonuses)
        {
            foreach (var organ in body.Organs.Where(x => x.BodypartType == bonus.Key))
            {
                yield return (organ, bonus.Value);
            }
        }
    }

    public override string Describe(IPerceiver voyeur)
    {
        var organs = OrganBonuses.Select(x => x.Key.DescribeEnum().Pluralise().ColourValue()).ListToString();
        return $"Organ function modifier to {organs}";
    }
}
