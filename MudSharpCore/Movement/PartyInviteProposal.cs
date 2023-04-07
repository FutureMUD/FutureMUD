using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Movement;

public class PartyInviteProposal : Proposal, IProposal
{
	public PartyInviteProposal(ICharacter applicant, ICharacter supplicant)
	{
		Applicant = applicant;
		Supplicant = supplicant;
	}

	public ICharacter Applicant { get; protected set; }
	public ICharacter Supplicant { get; protected set; }

	#region IProposal Members

	public override void Accept(string message = "")
	{
		var party = Applicant.Party;
		if (party == null)
		{
			party = new Party(Applicant);
			Applicant.JoinParty(party);
		}

		if (Supplicant.Party == Applicant.Party)
		{
			Supplicant.OutputHandler.Send("You are already in that party.");
			return;
		}

		if (Supplicant.Party != null)
		{
			foreach (var ch in Supplicant.Party.Members.Except(Supplicant))
			{
				ch.OutputHandler.Send(Supplicant.HowSeen(ch, true) + " leaves your party for " +
				                      Applicant.HowSeen(ch, type: DescriptionType.Possessive) + ".");
			}

			Supplicant.LeaveParty();
		}

		foreach (var ch in Applicant.Party.Members)
		{
			ch.OutputHandler.Send(Supplicant.HowSeen(ch, true) + " joins your party.");
		}

		Supplicant.OutputHandler.Send("You join " + Applicant.HowSeen(Supplicant, type: DescriptionType.Possessive) +
		                              " party.");
		Supplicant.JoinParty(party);
	}

	public override void Reject(string message = "")
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
		                             " declines your invitation to join your party.");
		Supplicant.OutputHandler.Send("You decline " +
		                              Applicant.HowSeen(Supplicant, type: DescriptionType.Possessive) +
		                              " invitation to join " + Applicant.ApparentGender(Supplicant).Possessive() +
		                              " party.");
	}

	public override void Expire()
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
		                             " did not answer your invitation to join your party.");
		Supplicant.OutputHandler.Send(Applicant.HowSeen(Supplicant, true, DescriptionType.Possessive) +
		                              " offer to join " + Applicant.ApparentGender(Supplicant).Possessive() +
		                              " party has expired.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return Applicant.HowSeen(voyeur, true) + " has invited " + Supplicant.HowSeen(voyeur) + " to join " +
		       Applicant.ApparentGender(voyeur).Possessive() + " party.";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "party", "invite" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}