#nullable enable

using System.Collections.Generic;

namespace MudSharp.Magic;

public static partial class PsionicPowerEmotes
{
	public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Combat { get; } =
		new Dictionary<string, IReadOnlyDictionary<string, string>>
		{
			["forcestrike"] = Attack("@ focus|focuses on $1, &0's muscles tightening."),
			["forcelance"] = Attack("@ fix|fixes &0's gaze on $1 and thrust|thrusts one hand forward."),
			["psychictrip"] = Attack("@ glance|glances sharply toward $1's feet.", "$1 lose|loses &1 footing under an abrupt, unseen pressure!"),
			["concussivepulse"] = Attack("@ tense|tenses, concentrating on $1.", "$1 stagger|staggers as an unseen force strikes &1!"),
			["repulse"] = Attack("@ spread|spreads &0's fingers toward $1.", "$1 are|is driven back from $0 by an unseen force!"),
			["wrench"] = Attack("@ curl|curls &0's fingers, &0's gaze fixed on $1.", "A weapon is wrenched from $1's grasp!"),
			["drawfoe"] = Attack("@ draw|draws one hand toward &0, watching $1.", "$1 are|is hauled into close quarters with $0!"),
			["breakhold"] = Attack("@ tense|tenses against $1's hold.", "$0 and $1 are forced apart, breaking their clinch!"),
			["kineticparry"] = Defense("@ steady|steadies &0's breathing and raise|raises one hand.", "$0 lower|lowers &0's hand as &0's concentration relaxes.", "$1's attack turns aside just short of $0.", "$1's attack forces its way through $0's defense."),
			["phantomdoubles"] = Defense("@ become|becomes still, &0's outline seeming briefly indistinct.", "$0's outline seems clear once more.", "$1's attack passes through a fleeting false image of $0!", "$1's attack finds $0 among the shifting images!"),
			["kineticbarrier"] = Defense("@ brace|braces &0's stance and focus|focuses intently.", "The pressure around $0 subsides.", "$1's attack meets an unseen resistance before reaching $0!", "$1's attack punches through the unseen resistance around $0!")
		};
	private static IReadOnlyDictionary<string, string> Attack(string echo, string? rider = null)
	{
		var result = new Dictionary<string, string> { ["AttackEmote"] = echo };
		if (rider is not null) result["RiderSuccess"] = rider;
		return result;
	}
	private static IReadOnlyDictionary<string, string> Defense(string begin, string end, string success, string fail) =>
		new Dictionary<string, string> { ["BeginEmote"] = begin, ["EndEmote"] = end, ["SuccessEmote"] = success, ["FailEmote"] = fail };
}
