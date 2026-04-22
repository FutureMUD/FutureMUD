#nullable enable
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Character;

public class CharacterForm : ICharacterForm
{
	public CharacterForm(IBody body, string alias, int sortOrder = 0)
	{
		Body = body;
		Alias = alias;
		SortOrder = sortOrder;
	}

	public CharacterForm(MudSharp.Models.CharacterBody form, IBody body, IFuturemud gameworld)
	{
		Body = body;
		Alias = form.Alias;
		SortOrder = form.SortOrder;
		AllowVoluntarySwitch = form.AllowVoluntarySwitch;
		CanVoluntarilySwitchProg = gameworld.FutureProgs.Get(form.CanVoluntarilySwitchProgId ?? 0);
		WhyCannotVoluntarilySwitchProg = gameworld.FutureProgs.Get(form.WhyCannotVoluntarilySwitchProgId ?? 0);
	}

	public IBody Body { get; }
	public string Alias { get; set; }
	public int SortOrder { get; set; }
	public bool AllowVoluntarySwitch { get; set; }
	public IFutureProg CanVoluntarilySwitchProg { get; set; }
	public IFutureProg WhyCannotVoluntarilySwitchProg { get; set; }

	public bool CanSwitchVoluntarily(ICharacter character, out string whyNot)
	{
		if (!AllowVoluntarySwitch)
		{
			whyNot = "You cannot voluntarily switch to that form.";
			return false;
		}

		if (CanVoluntarilySwitchProg?.ExecuteBool(character) == false)
		{
			whyNot = WhyCannotVoluntarilySwitchProg?.Execute(character)?.ToString() ??
			         "You cannot voluntarily switch to that form right now.";
			return false;
		}

		whyNot = string.Empty;
		return true;
	}
}
