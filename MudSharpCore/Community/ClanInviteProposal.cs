using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Community;

public class ClanInviteProposal : Proposal, IProposal
{
	public ICharacter Recruiter { get; set; }
	public ICharacter Recruit { get; set; }
	public IClan Clan { get; set; }
	public IRank Rank { get; set; }

	#region IProposal Members

	public override void Accept(string message = "")
	{
		var archived = Clan.Memberships.FirstOrDefault(x => x.MemberId == Recruit.Id && x.IsArchivedMembership);
		if (archived != null)
		{
			archived.IsArchivedMembership = false;
			archived.Rank = Rank;
			archived.Changed = true;
			Recruit.AddMembership(archived);
		}
		else
		{
			using (new FMDB())
			{
				var dbitem = new Models.ClanMembership
				{
					CharacterId = Recruit.Id,
					ClanId = Clan.Id,
					RankId = Rank.Id,
					PaygradeId = Rank.Paygrades.Any() ? Rank.Paygrades.First().Id : (long?)null,
					PersonalName = Recruit.CurrentName.SaveToXml().ToString(),
					JoinDate = Clan.Calendar.CurrentDate.GetDateString(),
					ManagerId = Recruiter.Id
				};
				FMDB.Context.ClanMemberships.Add(dbitem);
				FMDB.Context.SaveChanges();
				var newMembership = new ClanMembership(dbitem, Clan, Recruit.Gameworld);
				Recruit.AddMembership(newMembership);
				Clan.Memberships.Add(newMembership);
			}
		}

		Recruit.Gameworld.SystemMessage(
			$"{Recruiter.PersonalName.GetName(NameStyle.FullWithNickname)} recruits {Recruit.PersonalName.GetName(NameStyle.FullWithNickname)} into {Clan.FullName.TitleCase()} with a rank of {Rank.Title(Recruit).TitleCase()}.",
			true);

		var emote = new EmoteOutput(new Emote(
			$"@ recruit|recruits $0 into {Clan.FullName.TitleCase().Colour(Telnet.Green)}, with a rank of {Rank.Title(Recruit).TitleCase().Colour(Telnet.Green)}.",
			Recruiter, Recruit));
		Recruiter.OutputHandler.Send(emote);
		Recruit.OutputHandler.Send(emote);
	}

	public override void Reject(string message = "")
	{
		var emote = new EmoteOutput(new Emote(
			$"@ decline|declines $0's invitation to join {Clan.FullName.TitleCase().Colour(Telnet.Green)}.", Recruit,
			Recruiter));
		Recruiter.OutputHandler.Send(emote);
		Recruit.OutputHandler.Send(emote);
	}

	public override void Expire()
	{
		var emote = new EmoteOutput(new Emote(
			$"$0's offer to recruit @ into {Clan.FullName.TitleCase().Colour(Telnet.Green)} has expired.", Recruit,
			Recruiter));
		Recruiter.OutputHandler.Send(emote);
		Recruit.OutputHandler.Send(emote);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Recruiter.HowSeen(voyeur, true)} has invited {Recruit.HowSeen(voyeur)} to join {Clan.FullName.TitleCase().Colour(Telnet.Green)} with a rank of {Rank.Title(Recruit).TitleCase().Colour(Telnet.Green)}.";
	}

	#endregion

	#region IKeyworded Members

	protected static List<string> _keywords = new() { "clan", "invite" };
	public override IEnumerable<string> Keywords => _keywords;

	#endregion
}