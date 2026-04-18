using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class SupressWoundMessages : Effect
{
    public string TargetTookWoundsEmote { get; init; }

    public SupressWoundMessages(IPerceivable owner) : base(owner, null)
    {
    }

    public override string Describe(IPerceiver voyeur)
    {
        return "Supressing wound messages";
    }

    protected override string SpecificEffectType => "SupressWoundMessages";
}