using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Health.Corpses;

public abstract class CorpseModel : FrameworkItem, ICorpseModel
{
	public override string FrameworkItemType => "CorpseModel";

	#region ICorpseModel Members

	public string Description { get; init; }

	public abstract string Describe(DescriptionType type, DecayState state,
		ICharacter originalCharacter, IPerceiver voyeur, double eatenPercentage);

	public abstract string DescribeSevered(DescriptionType type, DecayState state, ICharacter originalCharacter,
		IPerceiver voyeur, ISeveredBodypart part, double eatenPercentage);

	public abstract double DecayRate(ITerrain terrain);

	public abstract DecayState GetDecayState(double decayPoints);

	public abstract ISolid CorpseMaterial(double decayPoints);

	public double EdiblePercentage { get; set; }

	public virtual bool CreateCorpse => true;
	public virtual bool RetainItems => true;

	public string EatenShortDescription(double percentage)
	{
		if (percentage <= 0.005)
		{
			return string.Empty;
		}

		if (percentage <= 0.075)
		{
			return "slightly eaten";
		}

		if (percentage <= 0.15)
		{
			return "partially eaten";
		}

		if (percentage <= 0.3)
		{
			return "substantially eaten";
		}

		if (percentage <= 0.6)
		{
			return "largely eaten";
		}

		if (percentage <= 0.8)
		{
			return "extensively eaten";
		}

		if (percentage < 1.0)
		{
			return "almost totally devoured";
		}

		return "wholly devoured";
	}

	public string EatenDescription(double percentage)
	{
		if (percentage <= 0.005)
		{
			return string.Empty;
		}

		if (percentage <= 0.075)
		{
			return "A very small portion of the corpse has been eaten.".Colour(Telnet.Red);
		}

		if (percentage <= 0.15)
		{
			return "A small portion of the corpse has been eaten.".Colour(Telnet.Red);
		}

		if (percentage <= 0.3)
		{
			return "A substantial amount of the corpse has been eaten.".Colour(Telnet.Red);
		}

		if (percentage <= 0.6)
		{
			return "A large amount of the corpse has been eaten.".Colour(Telnet.Red);
		}

		if (percentage <= 0.8)
		{
			return "The majority of the edible portions of the corpse have been eaten.".Colour(Telnet.Red);
		}

		if (percentage < 1.0)
		{
			return "Almost every edible morsel on the corpse has been picked clean and devoured.".Colour(Telnet.Red);
		}

		return "Every edible morsel on the corpse has been completely devoured.".Colour(Telnet.Red);
	}

	#endregion
}