using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

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
        foreach (KeyValuePair<BodypartTypeEnum, double> item in bonuses)
        {
            OrganBonuses[item.Key] = item.Value;
        }
    }

    public IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body)
    {
        foreach (KeyValuePair<BodypartTypeEnum, double> bonus in OrganBonuses)
        {
            foreach (IOrganProto organ in body.Organs.Where(x => x.BodypartType == bonus.Key))
            {
                yield return (organ, bonus.Value);
            }
        }
    }

    public override string Describe(IPerceiver voyeur)
    {
        string organs = OrganBonuses.Select(x => x.Key.DescribeEnum().Pluralise().ColourValue()).ListToString();
        return $"Organ function modifier to {organs}";
    }
}
