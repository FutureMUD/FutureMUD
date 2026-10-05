#nullable enable

using System.Xml.Linq;
using MudSharp.Magic.Emotions;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

/// <summary>Independent editable definition; construction/paid runtime awaits main-owned hook allocation.</summary>
public static class ArmageddonStillAngerStock
{
	public const string Key = "arm.spell.still_anger";
	public const string Name = "Still Anger";
	public const string PrerequisiteKey = ArmageddonRousedFuryStock.Key;
	public const double PrerequisiteRaw = 80;
	public const int Opening = 30;
	public const double RawCap = 90;
	public const double BranchRaw = 80;
	public const double MinimumEnergy = 7;
	public const string LifetimeGroup = "armageddon.still_anger";
	public const int UnitSeconds = 600;
	public const int MaximumUnits = 24;
	public const string DurationFormula = "0";
	public const string NextKey = ArmageddonMendFleshStock.Key;
	public const double NextRawCap = 60;

	public static EmotionalStockProfile Profile(long opposedTraitId, double intensity, long eligibilityProgId,
		IEnumerable<KeyValuePair<int, Difficulty>> saves, bool breakOnAdmittedAttack = true) =>
		new(EmotionalSpellKind.Calm, LifetimeGroup, UnitSeconds, MaximumUnits, eligibilityProgId, intensity,
			opposedTraitId, 0, new(2, 1, 0), [], saves, breakOnAdmittedAttack);

	public static XElement Definition(long resourceId, long costExpressionId, EmotionalStockProfile profile)
	{
		if (profile.Kind != EmotionalSpellKind.Calm || resourceId <= 0 || costExpressionId <= 0)
			throw new ArgumentException("Select a Calm profile and existing resource/cost expression IDs.");
		return ArmageddonUtilityStock.Definition(Key, "character", resourceId, costExpressionId,
			profile.EligibilityProgId, Opening, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "sourcecalm"), profile.SaveToXml()));
	}
}
