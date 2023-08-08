using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.CharacterCreation.Screens
{
	public class StartingLocation
	{
		private ICell _location;

		public string Name { get; set; }
		public string Blurb { get; set; }
		public IFutureProg FutureProg => Role.AvailabilityProg;
		public ICell Location
		{
			get => _location; set
			{
				if (_location is not null)
				{
					_location.CellProposedForDeletion -= LocationCellProposedForDeletion;
				}
				_location = value;
				if (_location is not null)
				{
					_location.CellProposedForDeletion -= LocationCellProposedForDeletion;
					_location.CellProposedForDeletion += LocationCellProposedForDeletion;
				}
			}
		}

		private void LocationCellProposedForDeletion(ICell cell, Framework.ProposalRejectionResponse response)
		{
			response.RejectWithReason($"That room is the starting location for role #{Role.Id:N0} ({Role.Name.ColourName()})");
		}

		public IChargenRole Role { get; set; }
		public IFutureProg OnCommenceProg { get; set; }
	}
}
