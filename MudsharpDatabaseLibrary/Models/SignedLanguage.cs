using System.Collections.Generic;

namespace MudSharp.Models;

public class SignedLanguage
{
	public SignedLanguage()
	{
		Varieties = new HashSet<SignedLanguageVariety>();
		ArticulationProfiles = new HashSet<SignedLanguageArticulationProfile>();
		MutualIntelligibilitiesListenerLanguage = new HashSet<SignedLanguageMutualIntelligibility>();
		MutualIntelligibilitiesTargetLanguage = new HashSet<SignedLanguageMutualIntelligibility>();
		CharactersSignedLanguages = new HashSet<CharactersSignedLanguage>();
		CharactersCurrentSignedLanguage = new HashSet<Character>();
	}

	public long Id { get; set; }
	public string Name { get; set; }
	public long DifficultyModelId { get; set; }
	public long LinkedTraitId { get; set; }
	public string UnknownLanguageDescription { get; set; }
	public double LanguageObfuscationFactor { get; set; }

	public virtual LanguageDifficultyModels DifficultyModel { get; set; }
	public virtual TraitDefinition LinkedTrait { get; set; }
	public virtual ICollection<SignedLanguageVariety> Varieties { get; set; }
	public virtual ICollection<SignedLanguageArticulationProfile> ArticulationProfiles { get; set; }
	public virtual ICollection<SignedLanguageMutualIntelligibility> MutualIntelligibilitiesListenerLanguage { get; set; }
	public virtual ICollection<SignedLanguageMutualIntelligibility> MutualIntelligibilitiesTargetLanguage { get; set; }
	public virtual ICollection<CharactersSignedLanguage> CharactersSignedLanguages { get; set; }
	public virtual ICollection<Character> CharactersCurrentSignedLanguage { get; set; }
}
