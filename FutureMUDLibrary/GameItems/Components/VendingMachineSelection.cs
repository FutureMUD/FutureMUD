using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.GameItems.Components
{
    public class VendingMachineSelection {
        public VendingMachineSelection() {
        }

        public IFuturemud Gameworld { get; set; }

        public VendingMachineSelection(XElement root, IFuturemud gameworld) {
            Gameworld = gameworld;
            Keyword = root.Element("Keyword").Value;
            Cost = decimal.Parse(root.Element("Cost").Value);
            Description = root.Element("Description").Value;
            Prototype = gameworld.ItemProtos.Get(long.Parse(root.Element("Prototype").Value));
            LoadArguments = root.Element("LoadArguments").Value;
            OnLoadProg = gameworld.FutureProgs.Get(long.Parse(root.Element("OnLoadProg").Value));
        }

        public string Keyword { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
        private long _prototypeId;
        public IGameItemProto Prototype
        {
            get => Gameworld.ItemProtos.Get(_prototypeId);
            set => _prototypeId = value?.Id ?? 0;
        }
        public string LoadArguments { get; set; }
        public IFutureProg OnLoadProg { get; set; }

        public XElement SaveSelection() {
            return new XElement("Selection",
                new XElement("Keyword", Keyword),
                new XElement("Cost", Cost),
                new XElement("Description", Description),
                new XElement("Prototype", _prototypeId),
                new XElement("LoadArguments", new XCData(LoadArguments)),
                new XElement("OnLoadProg", OnLoadProg?.Id ?? 0)
            );
        }
    }
}