using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class NaturalAttackQualityMerit : CharacterMeritBase, INaturalAttackQualityMerit
{
	protected NaturalAttackQualityMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Verb = (MeleeWeaponVerb)int.Parse(definition.Attribute("verb")?.Value ?? "0");
		Boosts = int.Parse(definition.Attribute("boosts")?.Value ?? "0");
	}

	public MeleeWeaponVerb Verb { get; set; }
	public int Boosts { get; set; }

	#region Implementation of INaturalAttackQualityMerit

	public ItemQuality GetQuality(ItemQuality baseQuality)
	{
		return baseQuality.StageUp(Boosts);
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Natural Attack Quality",
			(merit, gameworld) => new NaturalAttackQualityMerit(merit, gameworld));
	}
}