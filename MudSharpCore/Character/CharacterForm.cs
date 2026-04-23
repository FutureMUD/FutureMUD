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
		TraumaMode = BodySwitchTraumaMode.Automatic;
	}

	public CharacterForm(MudSharp.Models.CharacterBody form, IBody body, IFuturemud gameworld)
	{
		Body = body;
		Alias = form.Alias;
		SortOrder = form.SortOrder;
		TraumaMode = (BodySwitchTraumaMode)form.TraumaMode;
		TransformationEcho = form.TransformationEcho;
		AllowVoluntarySwitch = form.AllowVoluntarySwitch;
		CanVoluntarilySwitchProg = gameworld.FutureProgs.Get(form.CanVoluntarilySwitchProgId ?? 0);
		WhyCannotVoluntarilySwitchProg = gameworld.FutureProgs.Get(form.WhyCannotVoluntarilySwitchProgId ?? 0);
		CanSeeFormProg = gameworld.FutureProgs.Get(form.CanSeeFormProgId ?? 0);
	}

	public IBody Body { get; }
	public string Alias { get; set; }
	public int SortOrder { get; set; }
	public BodySwitchTraumaMode TraumaMode { get; set; }
	public string? TransformationEcho { get; set; }
	public bool AllowVoluntarySwitch { get; set; }
	public IFutureProg CanVoluntarilySwitchProg { get; set; }
	public IFutureProg WhyCannotVoluntarilySwitchProg { get; set; }
	public IFutureProg CanSeeFormProg { get; set; }

	public bool CanSee(ICharacter character)
	{
		return CanSeeFormProg?.ExecuteBool(character) ?? true;
	}

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
