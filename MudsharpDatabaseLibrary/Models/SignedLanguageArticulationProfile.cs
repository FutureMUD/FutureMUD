using System.Collections.Generic;

namespace MudSharp.Models;

public class SignedLanguageArticulationProfile
{
	public SignedLanguageArticulationProfile()
	{
		Requirements = new HashSet<SignedLanguageArticulationRequirement>();
	}

	public long Id { get; set; }
	public long SignedLanguageId { get; set; }
	public long BodyPrototypeId { get; set; }
	public string Name { get; set; }

	public virtual SignedLanguage SignedLanguage { get; set; }
	public virtual BodyProto BodyPrototype { get; set; }
	public virtual ICollection<SignedLanguageArticulationRequirement> Requirements { get; set; }
}
