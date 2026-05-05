#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using System.Linq;

namespace MudSharp.Magic;

public enum PsionicActivityKind
{
	Magical = 0,
	Psychic = 1
}

public readonly record struct PsionicActivity(
	ICharacter Source,
	IMagicPower Power,
	PsionicActivityKind Kind,
	string Description);

public static class PsionicActivityNotifier
{
	public static void Notify(ICharacter source, IMagicPower power, string description)
	{
		var activity = new PsionicActivity(source, power, power.IsPsionic ? PsionicActivityKind.Psychic : PsionicActivityKind.Magical,
			description);

		foreach (var effect in source.Gameworld.Characters.SelectMany(x => x.EffectsOfType<PsionicSensitivityEffect>()).ToList())
		{
			effect.NotifyActivity(activity);
		}
	}
}
