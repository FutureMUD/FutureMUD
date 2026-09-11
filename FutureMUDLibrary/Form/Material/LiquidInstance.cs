using MudSharp.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Form.Material
{
    public class LiquidInstance
    {
        public static LiquidInstance LoadInstance(XElement root, IFuturemud gameworld)
        {
            switch (root.Attribute("instancetype")?.Value ?? "none")
            {
                case "blood":
                    return new BloodLiquidInstance(root, gameworld);
                case "colour":
                    return new ColourLiquidInstance(root, gameworld);
            }

            return new LiquidInstance(root, gameworld);
        }

        public virtual string LiquidDescription => Liquid.MaterialDescription;
        public virtual string LiquidLongDescription => Liquid.Description;

        public ILiquid Liquid { get; init; }
        public double Amount { get; set; }

        public System.Collections.Generic.Dictionary<long, MudSharp.Magic.SubstanceCharge> MagicalCharges { get; } = new();

        public void CopyMagicalChargesTo(LiquidInstance other)
        {
            foreach (var charge in MagicalCharges) other.MagicalCharges[charge.Key] = charge.Value.Copy();
        }

        public bool MagicalChargesMatch(LiquidInstance other) => MagicalCharges.Count == other.MagicalCharges.Count &&
            System.Linq.Enumerable.All(MagicalCharges, x => other.MagicalCharges.TryGetValue(x.Key, out var charge) && x.Value.CanMerge(charge));

        public virtual bool CanMergeWith(LiquidInstance other)
        {
            return Liquid == other.Liquid && MagicalChargesMatch(other);
        }

        public virtual void MergeOtherIntoSelf(LiquidInstance other)
        {
            Amount += other.Amount;
        }

        public virtual LiquidInstance SplitVolume(double volume)
        {
            Amount -= volume;
            var split = new LiquidInstance
            {
                Liquid = Liquid,
                Amount = volume
            };
            CopyMagicalChargesTo(split);
            return split;
        }

        public virtual XElement SaveToXml()
        {
            return new XElement("Liquid",
                    new XAttribute("id", Liquid.Id),
                    new XAttribute("amount", Amount),
                    System.Linq.Enumerable.Select(MagicalCharges, x => new XElement("Magic", new XAttribute("substance", x.Key), x.Value.Save()))
                );
        }

        public LiquidInstance() { }

        public LiquidInstance(XElement root, IFuturemud gameworld)
        {
            Liquid = gameworld.Liquids.Get(long.Parse(root.Attribute("id").Value));
            Amount = double.Parse(root.Attribute("amount").Value);
            foreach (var magic in root.Elements("Magic")) MagicalCharges[(long)magic.Attribute("substance")] = MudSharp.Magic.SubstanceCharge.Load(magic.Element("Charge"));
        }

        public LiquidInstance(LiquidInstance rhs)
        {
            Liquid = rhs.Liquid;
            Amount = rhs.Amount;
            rhs.CopyMagicalChargesTo(this);
        }

        public virtual LiquidInstance Copy()
        {
            return new LiquidInstance(this);
        }
    }
}
