using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Movement;

public class PartyJoinProposal : Proposal, IProposal
{
	public PartyJoinProposal(ICharacter applicant, ICharacter supplicant)
	{
		Applicant = applicant;
		Supplicant = supplicant;
	}

	public ICharacter Applicant { get; protected set; }
	public ICharacter Supplicant { get; protected set; }

	#region IProposal Members

	public override void Accept(string message = "")
	{
		var party = Supplicant.Party;
		if (party == null)
		{
			party = new Party(Supplicant);
			Supplicant.JoinParty(party);
		}

		if (Supplicant.Party == Applicant.Party)
		{
			Applicant.OutputHandler.Send("You are already in that party.");
			Supplicant.OutputHandler.Send("They are already in that party.");
			return;
		}

		if (Applicant.Party != null)
		{
			foreach (var ch in Applicant.Party.Members.Except(Applicant))
			{
				ch.OutputHandler.Send(Applicant.HowSeen(ch, true) + " leaves your party for " +
				                      Supplicant.HowSeen(ch, type: DescriptionType.Possessive) + ".");
			}

			Applicant.LeaveParty();
		}

		foreach (var ch in Supplicant.Party.Members)
		{
			ch.OutputHandler.Send(Applicant.HowSeen(ch, true) + " joins your party.");
		}

		Applicant.OutputHandler.Send("You join " + Supplicant.HowSeen(Applicant, type: DescriptionType.Possessive) +
		                             " party.");
		Applicant.JoinParty(party);
	}

	public override void Reject(string message = "")
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) + " declines your request to join " +
		                             Supplicant.ApparentGender(Applicant).Possessive() + " party.");
		Supplicant.OutputHandler.Send("You decline " +
		                              Applicant.HowSeen(Supplicant, type: DescriptionType.Possessive) +
		                              " invitation to join your party.");
	}

	public override void Expire()
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) + " did not answer your request to join " +
		                             Supplicant.ApparentGender(Applicant).Possessive() + " party.");
		Supplicant.OutputHandler.Send(Applicant.HowSeen(Supplicant, true, DescriptionType.Possessive) +
		                              " offer to join your party has expired.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return Applicant.HowSeen(voyeur, true) + " has asked to join " +
		       Supplicant.HowSeen(voyeur, type: DescriptionType.Possessive) + " party.";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "party", "join" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}