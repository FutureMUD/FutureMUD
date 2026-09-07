using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.CharacterCreation.Screens;

/// <summary>Shared mandatory -> groups -> existing open/boost screen adapter.</summary>
public sealed class SkillGroupScreen : IChargenScreen
{
	private readonly IChargen _chargen;
	private readonly Func<IChargenScreen> _openFactory;
	private readonly SkillGroupResolver _resolver;
	private IChargenScreen? _open;
	private string? _currentKey;
	private bool _reviewGroups;
	private string _definitionStamp = "";
	private SkillGroupScreen(IChargen chargen, IFutureProg? freeSkills, Func<IChargenScreen> openFactory)
	{
		_chargen = chargen;
		_openFactory = openFactory;
		_resolver = new SkillGroupResolver(chargen, freeSkills);
	}

	public static IChargenScreen Wrap(IChargen chargen, IFutureProg? freeSkills, Func<IChargenScreen> openFactory) =>
		chargen.SkillClaims?.Initialised == true || (chargen.Gameworld.ChargenSkillSelectionGroups?.Any(x => x.Enabled && !x.Retired) ?? false)
			? new SkillGroupScreen(chargen, freeSkills, openFactory) : openFactory();

	private bool GroupPhase => !_resolver.Complete || (_reviewGroups && _resolver.Groups.Count > 0);
	private IChargenSkillSelectionGroup? Current => _resolver.Groups.FirstOrDefault(x => x.StableKey == _currentKey) ??
		_resolver.Groups.FirstOrDefault(x => !_chargen.SkillClaims.Groups[x.StableKey].Complete) ?? _resolver.Groups.FirstOrDefault();
	public ChargenStage AssociatedStage => ChargenStage.SelectSkills;
	public ChargenScreenState State => GroupPhase ? ChargenScreenState.Incomplete : Open.State;
	public IChargenScreen NextScreen => Open.NextScreen;
	private IChargenScreen Open
	{
		get
		{
			if (_open is not null) return _open;
			_open = _openFactory();
			_resolver.CaptureOrdinary();
			return _open;
		}
	}

	public string Display()
	{
		Revalidate();
		if (!GroupPhase) return Open.Display();
		var sb = new StringBuilder();
		sb.AppendLine("Mandatory skills: " + _chargen.SkillClaims.Mandatory.Select(x => _chargen.Gameworld.Traits.Get(x)?.Name ?? "Missing skill").ListToString());
		foreach (var error in _resolver.Errors) sb.AppendLine(error.ColourError());
		foreach (var change in _resolver.Changes.Distinct()) sb.AppendLine(change);
		var group = Current;
		if (group is null) return sb.ToString();
		sb.AppendLine(group.Name.ColourName());
		sb.AppendLine(group.Description.SubstituteANSIColour().Wrap(_chargen.Account.InnerLineFormatLength));
		var claim = _chargen.SkillClaims.Groups.GetValueOrDefault(group.StableKey);
		if (claim is null) return sb.ToString();
		sb.AppendLine(group.MinimumPicks == group.MaximumPicks
			? $"Choose exactly {group.MinimumPicks.ToString("N0", _chargen.Account)} skills."
			: $"Choose {group.MinimumPicks.ToString("N0", _chargen.Account)} to {group.MaximumPicks.ToString("N0", _chargen.Account)} skills.");
		sb.AppendLine($"Selected: {claim.Skills.Count.ToString("N0", _chargen.Account)}; remaining capacity: {(group.MaximumPicks - claim.Skills.Count).ToString("N0", _chargen.Account)}.");
		foreach (var skill in _resolver.Candidates.GetValueOrDefault(group.StableKey) ?? [])
		{
			var owner = _resolver.Groups.FirstOrDefault(x => _chargen.SkillClaims.Groups[x.StableKey].Skills.Contains(skill.Id));
			var status = owner == group ? (claim.Credits.Contains(skill.Id) ? " [known credit selected]" : " [selected]") :
				owner is not null ? $" [owned by {owner.Name}]" : _chargen.SkillClaims.Mandatory.Contains(skill.Id) ? " [known; explicit credit available]" :
				_chargen.SkillClaims.Ordinary.Contains(skill.Id) ? " [ordinary; pick to reallocate base cost]" : "";
			sb.AppendLine($"  {skill.Name}{status}".Wrap(_chargen.Account.InnerLineFormatLength));
		}
		sb.AppendLine("Commands: groups, group <name|number>, pick <skill>, unpick <skill>, help <skill>, done, skip, back");
		return sb.ToString();
	}

	public string HandleCommand(string command)
	{
		Revalidate();
		var ss = new StringStack(command);
		var verb = ss.PopSpeech().ToLowerInvariant();
		if (!GroupPhase && verb is not "groups" and not "group")
		{
			var result = Open.HandleCommand(command);
			_resolver.CaptureOrdinary();
			Save();
			return result;
		}
		_reviewGroups = true;
		if (verb == "groups") return string.Join("\n", _resolver.Groups.Select((x, i) => $"{i + 1}. {x.Name}: {x.MinimumPicks}-{x.MaximumPicks}; {(_chargen.SkillClaims.Groups[x.StableKey].Complete ? "complete" : "pending")}"));
		if (verb == "group")
		{
			var name = ss.SafeRemainingArgument;
			var matches = _resolver.Groups.Where((x, i) => (i + 1).ToString() == name || x.Name.EqualTo(name)).ToList();
			if (matches.Count != 1) return "No unique group matches that name or number.";
			_currentKey = matches[0].StableKey;
			return Display();
		}
		var group = Current;
		if (group is null) return Display();
		if (verb == "back")
		{
			_currentKey = _resolver.Groups[Math.Max(0, _resolver.Groups.IndexOf(group) - 1)].StableKey;
			return Display();
		}
		if (verb is "done" or "skip")
		{
			var error = _resolver.Finish(group);
			if (error is not null) return error + "\n" + Display();
			_currentKey = null;
			_reviewGroups = !_resolver.Complete;
			_open = null;
			Save();
			return Display();
		}
		if (verb is "pick" or "unpick" or "help")
		{
			var name = ss.SafeRemainingArgument;
			var matches = group.Members.Where(x => x.Id.ToString() == name || x.Name.EqualTo(name)).ToList();
			if (matches.Count != 1) return "Use the full skill name or ID; the match must be unique.";
			var skill = matches[0];
			if (verb == "help") return _chargen.Gameworld.Helpfiles.FirstOrDefault(x => x.Name.EqualTo(skill.Name))?.DisplayHelpFile(_chargen) ?? "There is no helpfile for that skill.";
			if (verb == "pick")
			{
				var error = _resolver.Pick(group, skill);
				if (error is not null) return error;
			}
			else _resolver.Unpick(group, skill);
			_open = null;
			Save();
		}
		return Display();
	}
	private void Save() { if (_chargen is Chargen chargen) chargen.SaveSkillClaims(); }
	private void Revalidate()
	{
		_resolver.Revalidate();
		var stamp = string.Join(",", _chargen.SkillClaims.Mandatory.Concat(_chargen.SkillClaims.GroupSkills).Order()) + ":" +
			string.Join(",", _resolver.Groups.Select(x => $"{x.StableKey}:{x.Revision}"));
		if (stamp != _definitionStamp) _open = null;
		_definitionStamp = stamp;
	}
}
