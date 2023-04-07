using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Groups;

public class GameItemGroupDeletionProposal : Proposal
{
	public GameItemGroupDeletionProposal(ICharacter proponent, IGameItemGroup group)
	{
		Proponent = proponent;
		Group = group;
	}

	public ICharacter Proponent { get; set; }
	public IGameItemGroup Group { get; set; }

	public override IEnumerable<string> Keywords => new[]
	{
		"delete",
		"itemgroup",
		"group"
	};

	public override void Accept(string message = "")
	{
		if (Group == null || !Proponent.Gameworld.ItemGroups.Has(Group))
		{
			Proponent.Send("It looks like that item group has already been deleted.");
			return;
		}

		Proponent.Send("You delete item group {0} (#{1:N0})", Group.Name.TitleCase().Colour(Telnet.Green), Group.Id);
		Group.Delete();
	}

	public override void Reject(string message = "")
	{
		if (Group != null)
		{
			Proponent.Send("You decide not to delete item group {0} (#{1:N0})",
				Group.Name.TitleCase().Colour(Telnet.Green), Group.Id);
		}
	}

	public override void Expire()
	{
		if (Group != null)
		{
			Proponent.Send("You decide not to delete item group {0} (#{1:N0})",
				Group.Name.TitleCase().Colour(Telnet.Green), Group.Id);
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Attempting to delete item group {Group.Name.TitleCase().Colour(Telnet.Green)} (#{Group.Id:N0})";
	}
}