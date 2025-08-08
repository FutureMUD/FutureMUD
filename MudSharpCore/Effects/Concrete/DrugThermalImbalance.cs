using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class DrugThermalImbalance : ThermalImbalance
{
    public static void InitialiseEffectType()
    {
        RegisterFactory("DrugThermalImbalance", (effect, owner) => new DrugThermalImbalance(effect, owner));
    }

    public DrugThermalImbalance(IPerceivable owner, IFutureProg prog = null) : base(owner, prog)
    {
    }

    protected DrugThermalImbalance(XElement root, IPerceivable owner) : base(root, owner)
    {
    }

    protected override string SpecificEffectType => "DrugThermalImbalance";
}
