namespace MudSharp.Models;

public class SignedLanguageArticulationRequirement
{
	public long ArticulationProfileId { get; set; }
	public long BodypartShapeId { get; set; }
	public int MinimumCount { get; set; }
	public int PreferredCount { get; set; }

	public virtual SignedLanguageArticulationProfile ArticulationProfile { get; set; }
	public virtual BodypartShape BodypartShape { get; set; }
}
