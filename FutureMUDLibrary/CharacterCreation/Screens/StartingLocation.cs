using MudSharp.CharacterCreation.Roles;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.CharacterCreation.Screens
{
    public class StartingLocation
    {
        private IRoom _location;

        public string Name { get; set; }
        public string Blurb { get; set; }
        public IFutureProg FutureProg => Role.AvailabilityProg;
        public IRoom Location
        {
            get => _location; set
            {
                _location?.RoomProposedForDeletion -= LocationRoomProposedForDeletion;
                _location = value;
                if (_location is not null)
                {
                    _location.RoomProposedForDeletion -= LocationRoomProposedForDeletion;
                    _location.RoomProposedForDeletion += LocationRoomProposedForDeletion;
                }
            }
        }

        private void LocationRoomProposedForDeletion(IRoom room, Framework.ProposalRejectionResponse response)
        {
            response.RejectWithReason($"That room is the starting location for role #{Role.Id:N0} ({Role.Name.ColourName()})");
        }

        public IChargenRole Role { get; set; }
        public IFutureProg OnCommenceProg { get; set; }
    }
}
