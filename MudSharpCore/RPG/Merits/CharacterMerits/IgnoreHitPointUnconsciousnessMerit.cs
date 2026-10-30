using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.RPG.Merits.CharacterMerits;

/// <summary>
/// A marker merit used by health systems to suppress hit point unconsciousness while applicable.
/// </summary>
public sealed class IgnoreHitPointUnconsciousnessMerit : CharacterMeritBase, IIgnoreHitPointUnconsciousnessMerit
{
	public const string MeritTypeName = "IgnoreHitPointUnconsciousness";

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser(MeritTypeName,
			(merit, gameworld) => new IgnoreHitPointUnconsciousnessMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser(MeritTypeName,
			(gameworld, name) => new IgnoreHitPointUnconsciousnessMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp(MeritTypeName,
			"Allows a character to ignore hit point related unconsciousness while the merit applies",
			new IgnoreHitPointUnconsciousnessMerit().HelpText);
	}

	private IgnoreHitPointUnconsciousnessMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
	}

	private IgnoreHitPointUnconsciousnessMerit()
	{
	}

	private IgnoreHitPointUnconsciousnessMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, MeritTypeName, "@ ignore|ignores unconsciousness caused by hit point loss")
	{
		DoDatabaseInsert();
	}
}
