using MudSharp.Body;
using System.Collections.Generic;

namespace MudSharp.Effects.Interfaces;

public interface IOrganFunctionEffect : IEffectSubtype
{
    IEnumerable<(IOrganProto Organ, double Bonus)> OrganFunctionBonuses(IBody body);
}
