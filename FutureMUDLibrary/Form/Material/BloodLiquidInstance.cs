using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character.Heritage;

namespace MudSharp.Form.Material
{
    public class BloodLiquidInstance : LiquidInstance, IHaveFuturemud
    {
        private long _sourceCharacterId;
        private ICharacter _source;
        public long SourceId => _sourceCharacterId;

        public BloodLiquidInstance(ICharacter source, double amount)
        {
            Gameworld = source.Gameworld;
            _source = source;
            _sourceCharacterId = source?.Id ?? 0;
            Race = source.Race;
            BloodType = source.Body.Bloodtype;
            Liquid = source.Body.BloodLiquid;
            Amount = amount;
        }

        public BloodLiquidInstance(ICharacter source, IRace race, IBloodtype bloodtype, ILiquid liquid, IFuturemud gameworld, double amount)
        {
            Gameworld = gameworld;
            _source = source;
            _sourceCharacterId = source?.Id ?? 0;
            Race = race;
            BloodType = bloodtype;
            Liquid = liquid;
            Amount = amount;
        }

        public BloodLiquidInstance(XElement root, IFuturemud gameworld) : base(root, gameworld)
        {
            Gameworld = gameworld;
            _sourceCharacterId = long.Parse(root.Attribute("source").Value);
            Race = Gameworld.Races.Get(long.Parse(root.Attribute("race").Value));
            BloodType = Gameworld.Bloodtypes.Get(long.Parse(root.Attribute("bloodtype").Value));
        }

        public BloodLiquidInstance(BloodLiquidInstance rhs) : base(rhs) {
            Gameworld = rhs.Gameworld;
            _sourceCharacterId = rhs._sourceCharacterId;
            _source = rhs._source;
            Race = rhs.Race;
            BloodType = rhs.BloodType;
        }

        public ICharacter Source {
            get {
                if (_source == null && _sourceCharacterId != 0)
                {
                    _source = Gameworld.TryGetCharacter(_sourceCharacterId, true);
                }

                return _source;
            }
        }

        public IRace Race { get; set; }
        public IBloodtype BloodType { get; set; }

        public override bool CanMergeWith(LiquidInstance other)
        {
            return base.CanMergeWith(other) && other is BloodLiquidInstance bli && bli.Race == Race && bli.BloodType == BloodType && _sourceCharacterId == bli._sourceCharacterId;
        }

        public override LiquidInstance SplitVolume(double volume)
        {
            Amount -= volume;
            return new BloodLiquidInstance(Source, Race, BloodType, Liquid, Gameworld, volume);
        }

        #region Overrides of LiquidInstance

        public override LiquidInstance Copy() {
            return new BloodLiquidInstance(this);
        }

        #endregion

        public IFuturemud Gameworld { get; }

        public override XElement SaveToXml()
        {
            var root = base.SaveToXml();
            root.Add(new XAttribute("instancetype", "blood"));
            root.Add(new XAttribute("source", Source?.Id ?? 0));
            root.Add(new XAttribute("race", Race.Id));
            root.Add(new XAttribute("bloodtype", BloodType?.Id ?? 0));
            return root;
        }
    }
}
