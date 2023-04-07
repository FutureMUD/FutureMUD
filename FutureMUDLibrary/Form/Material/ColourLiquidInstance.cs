using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.Form.Material
{
    public class ColourLiquidInstance : LiquidInstance
    {
        public Colour.IColour Colour {get;}

        public ColourLiquidInstance(ILiquid liquid, Colour.IColour colour, double amount) {
            Liquid = liquid;
            Colour = colour;
            Amount = amount;
        }

        public ColourLiquidInstance(ColourLiquidInstance rhs) {
            Liquid = rhs.Liquid;
            Colour = rhs.Colour;
            Amount = rhs.Amount;
        }

        public ColourLiquidInstance(XElement root, IFuturemud gameworld) : base(root, gameworld) {
            Colour = gameworld.Colours.Get(long.Parse(root.Attribute("colour").Value));
        }

        public override bool CanMergeWith(LiquidInstance other)
        {
            return base.CanMergeWith(other) && other is ColourLiquidInstance cli && cli.Colour == Colour;
        }

        public override LiquidInstance SplitVolume(double volume)
        {
            Amount -= volume;
            return new ColourLiquidInstance(Liquid, Colour, volume);
        }

        public override LiquidInstance Copy() {
            return new ColourLiquidInstance(this);
        }

        public override XElement SaveToXml() {
            var root = base.SaveToXml();
            root.Add(new XAttribute("instancetype", "colour"));
            root.Add(new XAttribute("colour", Colour.Id));
            return root;
        }

        public override string LiquidDescription => string.Format(Liquid.MaterialDescription, Colour.Name);
        public override string LiquidLongDescription => string.Format(Liquid.Description, Colour.Name);
    }
}
