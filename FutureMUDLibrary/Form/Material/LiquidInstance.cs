using MudSharp.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Form.Material
{
    public class LiquidInstance
    {
        public static LiquidInstance LoadInstance(XElement root, IFuturemud gameworld) {
            switch (root.Attribute("instancetype")?.Value ?? "none") {
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

        public virtual bool CanMergeWith(LiquidInstance other)
        {
            return Liquid == other.Liquid;
        }

        public virtual void MergeOtherIntoSelf(LiquidInstance other)
        {
            Amount += other.Amount;
        }

        public virtual LiquidInstance SplitVolume (double volume)
        {
            Amount -= volume;
            return new LiquidInstance
            {
                Liquid = Liquid,
                Amount = volume
            };
        }

        public virtual XElement SaveToXml()
        {
            return new XElement("Liquid",
                    new XAttribute("id", Liquid.Id),
                    new XAttribute("amount", Amount)
                );
        }

        public LiquidInstance() { }

        public LiquidInstance(XElement root, IFuturemud gameworld)
        {
            Liquid = gameworld.Liquids.Get(long.Parse(root.Attribute("id").Value));
            Amount = double.Parse(root.Attribute("amount").Value);
        }

        public LiquidInstance(LiquidInstance rhs)
        {
            Liquid = rhs.Liquid;
            Amount = rhs.Amount;
        }

        public virtual LiquidInstance Copy()
        {
            return new LiquidInstance(this);
        }
    }
}
