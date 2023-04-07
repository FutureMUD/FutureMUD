using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body
{
    public class BodypartInternalInfo
    {
        public BodypartInternalInfo(double hitChance, bool isPrimaryInternalLocation, string proximityGroup) {
            HitChance = hitChance;
            IsPrimaryInternalLocation = isPrimaryInternalLocation;
            ProximityGroup = proximityGroup;
        }

        public double HitChance { get; }
        public bool IsPrimaryInternalLocation { get;}
        public string ProximityGroup { get; }
    }
}
