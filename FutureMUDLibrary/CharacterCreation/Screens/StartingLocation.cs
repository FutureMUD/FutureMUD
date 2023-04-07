using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Construction;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation.Screens
{
    public class StartingLocation {
        public string Name { get; set; }
        public string Blurb { get; set; }
        public IFutureProg FutureProg => Role.AvailabilityProg;
        public ICell Location { get; set; }
        public IChargenRole Role { get; set; }
        public IFutureProg OnCommenceProg { get; set; }
    }
}
