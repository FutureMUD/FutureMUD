using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp.Movement;

public class PartyMergeProposal : Proposal, IProposal
{
	public PartyMergeProposal(ICharacter applicant, ICharacter supplicant)
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
			Supplicant.OutputHandler.Send("You no longer have a party into which to be merged.");
			Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
			                             " no longer has a party into which your party may merge.");
			return;
		}

		if (Applicant.Party == null)
		{
			Applicant.OutputHandler.Send("You no longer have a party to be merged with that of " +
			                             Supplicant.HowSeen(Applicant) + ".");
			Supplicant.OutputHandler.Send(Applicant.HowSeen(Supplicant, true) +
			                              " no longer has a party to merge into yours.");
			return;
		}

		if (Applicant.Party == Supplicant.Party)
		{
			Supplicant.OutputHandler.Send("You have already merged your party with that person.");
			Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
			                             " has already merged their party with yours.");
			return;
		}

		foreach (var ch in Applicant.Party.Members)
		{
			ch.OutputHandler.Send("Your party has merged with that of " + Supplicant.HowSeen(ch) +
			                      ", who is now your leader.");
		}

		foreach (var ch in Supplicant.Party.Members)
		{
			ch.OutputHandler.Send(Applicant.HowSeen(ch) + " and all " + Applicant.ApparentGender(ch).Possessive() +
			                      " followers merge with your party.");
		}

		foreach (var ch in Applicant.Party.Members.ToList())
		{
			ch.LeaveParty(false);
			ch.JoinParty(party);
		}
	}

	public override void Reject(string message = "")
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
		                             " declines your request to merge your party with " +
		                             Supplicant.ApparentGender(Applicant).Possessive() + " party.");
		Supplicant.OutputHandler.Send("You decline " +
		                              Applicant.HowSeen(Supplicant, type: DescriptionType.Possessive) +
		                              " invitation to merge " + Applicant.ApparentGender(Supplicant).Possessive() +
		                              " party with your party.");
	}

	public override void Expire()
	{
		Applicant.OutputHandler.Send(Supplicant.HowSeen(Applicant, true) +
		                             " did not answer your request to merge your party with " +
		                             Supplicant.ApparentGender(Applicant).Possessive() + " party.");
		Supplicant.OutputHandler.Send(Applicant.HowSeen(Supplicant, true, DescriptionType.Possessive) +
		                              " offer to merge " + Applicant.ApparentGender(Supplicant).Possessive() +
		                              " party with your party has expired.");
	}

	public override string Describe(IPerceiver voyeur)
	{
		return Applicant.HowSeen(voyeur, true) + " has asked to merge parties with " +
		       Supplicant.HowSeen(voyeur, type: DescriptionType.Possessive) + " party.";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "party", "merge" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}