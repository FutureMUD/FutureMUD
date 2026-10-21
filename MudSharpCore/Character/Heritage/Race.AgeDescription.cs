#nullable enable

using MudSharp.FutureProg;

namespace MudSharp.Character.Heritage;

public partial class Race
{
	private long? _ageDescriptionProgId;

	public string DescribeAge(int ageInYears)
	{
		var fallback = AgeCategory(ageInYears).DescribeEnum(true).ToLowerInvariant();
		if (_ageDescriptionProgId is not { } id) return fallback;
		var prog = Gameworld.FutureProgs.Get(id);
		if (prog is null || prog.ReturnType != ProgVariableTypes.Text || !prog.MatchesParameters([ProgVariableTypes.Number]))
			return fallback;
		var text = prog.ExecuteString(ageInYears);
		return string.IsNullOrWhiteSpace(text) ? fallback : text;
	}

	private bool BuildingCommandAgeDescription(ICharacter actor, StringStack command)
	{
		var text = command.SafeRemainingArgument;
		if (text.EqualTo("none")) _ageDescriptionProgId = null;
		else
		{
			var prog = new ProgLookupFromBuilderInput(actor, text, ProgVariableTypes.Text, [ProgVariableTypes.Number]).LookupProg();
			if (prog is null) return false;
			_ageDescriptionProgId = prog.Id;
		}
		Changed = true;
		actor.OutputHandler.Send(_ageDescriptionProgId is { } id
			? $"Age descriptions now use prog #{id}. Mechanical lifecycle categories are unchanged."
			: "Age descriptions now use the mechanical lifecycle category.");
		return true;
	}
}
