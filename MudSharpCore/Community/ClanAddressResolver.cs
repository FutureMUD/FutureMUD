#nullable enable

using MudSharp.Character;

namespace MudSharp.Community;

public enum ClanAddressStatus { NoMatch, Text, Suppressed, Hidden, Error, Ambiguous }

public sealed record ClanAddressResult(ClanAddressStatus Status, string Text = "", string Source = "", string Detail = "")
{
	public bool StopsFallback => Status is not ClanAddressStatus.NoMatch and not ClanAddressStatus.Hidden;
}

/// <summary>Resolves direct address from an explicit speaker's viewpoint. Null speakers use public outsider/lower rules.</summary>
public static class ClanAddressResolver
{
	[ThreadStatic] private static int _depth;

	public static ClanAddressResult Resolve(ICharacter? speaker, ICharacter subject, IClan clan,
		IRank? rank = null, IAppointment? appointment = null, bool diagnostic = false)
	{
		if (_depth >= 32) return new(ClanAddressStatus.Error, Detail: "Recursive address predicates exceeded the limit.");
		_depth++;
		try
		{
			var member = Membership(subject, clan);
			if (member is null || clan.IsTemplate ||
			    rank is not null && member.Rank != rank ||
			    appointment is not null && !member.Appointments.Contains(appointment))
				return new(ClanAddressStatus.NoMatch, Detail: "The subject does not actively hold this source in a live clan.");
			if (speaker is not null && CharacterInstanceIdentityComparer.SameIdentity(speaker, subject))
				return new(ClanAddressStatus.NoMatch, Detail: "Self-address has no automatic form.");
			if (!diagnostic && speaker is not null &&
			    (!speaker.CanSee(subject) || subject.IdentityIsObscuredTo(speaker)))
				return new(ClanAddressStatus.Hidden, Detail: "The subject's identity is obscured or not visible.");
			var listener = speaker is null ? null : Membership(speaker, clan);
			var relation = listener is null ? ClanAddressRelation.Outsider :
				listener.Rank.RankNumber < member.Rank.RankNumber ? ClanAddressRelation.Lower :
				listener.Rank.RankNumber > member.Rank.RankNumber ? ClanAddressRelation.Higher : ClanAddressRelation.Peer;
			var permitted = diagnostic || listener?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers) == true;
			var result = ResolveSources(relation);
			return speaker is null && !result.StopsFallback ? ResolveSources(ClanAddressRelation.Lower) : result;

			ClanAddressResult ResolveSources(ClanAddressRelation relative)
			{
				if (appointment is not null) return Rules(appointment.FormsOfAddress, appointment.Name, relative);
				if (rank is not null) return Rules(rank.FormsOfAddress, rank.Name, relative);
				foreach (var group in member.Appointments.GroupBy(x => x.AddressPriority).OrderByDescending(x => x.Key))
				{
					var results = group.Select(x => Rules(x.FormsOfAddress, x.Name, relative)).ToList();
					var failure = results.FirstOrDefault(x => x.Status == ClanAddressStatus.Error);
					if (failure is not null) return failure;
					var matches = results.Where(x => x.StopsFallback).ToList();
					if (matches.Count == 0) continue;
					if (matches.Select(x => (x.Status, x.Text)).Distinct().Count() > 1)
						return new(ClanAddressStatus.Ambiguous, Detail: $"Conflicting address sources at priority {group.Key}: {string.Join(", ", matches.Select(x => x.Source))}.");
					return matches[0];
				}
				return Rules(member.Rank.FormsOfAddress, member.Rank.Name, relative);
			}

			ClanAddressResult Rules(IEnumerable<ClanAddressRule> rules, string source, ClanAddressRelation relative)
			{
				foreach (var rule in rules)
				{
					if ((!permitted && !rule.IsPublic) || (rule.Relations & relative) == 0) continue;
					if (rule.FutureProgId is { } id)
					{
						var prog = subject.Gameworld.FutureProgs.Get(id);
						if (prog is null || prog.ReturnType != ProgVariableTypes.Boolean || prog.AcceptsAnyParameters ||
						    !(prog.MatchesParameters([ProgVariableTypes.Character]) ||
						      prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character])))
							return new(ClanAddressStatus.Error, Source: source, Detail: $"Invalid predicate for rule {rule.Id}.");
						try
						{
							object[] args = prog.Parameters.Count() == 1 ? [subject] : [speaker!, subject];
							if (!prog.ExecuteWithStatus(out var value, args) || value is not bool applies)
								return new(ClanAddressStatus.Error, Source: source, Detail: $"Predicate failed for rule {rule.Id}.");
							if (!applies) continue;
						}
						catch (Exception)
						{
							return new(ClanAddressStatus.Error, Source: source, Detail: $"Predicate threw for rule {rule.Id}.");
						}
					}
					return new(rule.Text is null ? ClanAddressStatus.Suppressed : ClanAddressStatus.Text,
						rule.Text ?? "", source, $"Rule {rule.Id}; relation {relative}.");
				}
				return new(ClanAddressStatus.NoMatch, Source: source, Detail: $"No permitted matching rule for {relative}.");
			}
		}
		finally { _depth--; }
	}

	private static IClanMembership? Membership(ICharacter character, IClan clan) =>
		character.ClanMemberships.FirstOrDefault(x => x.Clan == clan && !x.IsArchivedMembership);

	/// <summary>The optional shared-clan full-look addition, with the same disclosure and formatting path for every caller.</summary>
	public static string LookHint(ICharacter viewer, ICharacter subject, IClan clan)
	{
		if (!clan.ShowFormsOfAddressInLook || !viewer.Gameworld.GetStaticBool("ClanSimilaritiesShowInLook") ||
		    Membership(viewer, clan)?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers) != true)
			return string.Empty;
		var result = Resolve(viewer, subject, clan);
		if (result.Status != ClanAddressStatus.Text) return string.Empty;
		var format = viewer.Gameworld.GetStaticString("ClanFormOfAddressLookFormat");
		if (string.IsNullOrWhiteSpace(format)) return string.Empty;
		var gender = subject.ApparentGender(viewer);
		return FormatLook(format, result.Text.ColourValue(), clan.Name.ColourName(), gender.Subjective(), gender.Objective(), gender.Possessive());
	}

	/// <summary>Formats the MUD-wide literal look template; only documented tokens are substituted.</summary>
	public static string FormatLook(string format, string address, string clan, string subjective, string objective, string possessive) =>
		System.Text.RegularExpressions.Regex.Replace(format, @"\{(address|clan|subjective|objective|possessive)\}",
			match => match.Groups[1].Value switch
			{
				"address" => address, "clan" => clan, "subjective" => subjective,
				"objective" => objective, "possessive" => possessive, _ => match.Value
			});
}
