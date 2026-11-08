#nullable enable

using System;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallLegacyUntal()
	{
		if (_db.Languages.Any(x => x.Name == "Untal")) return;
		var basis = _db.Languages.SingleOrDefault(x => x.Name == "Bendune")
			?? throw new InvalidOperationException("Legacy Untal requires the installed native language foundations.");
		var trait = _db.TraitDefinitions.SingleOrDefault(x => x.Name == "Untal" && x.Type == 0);
		if (trait is null)
		{
			var template = _db.TraitDefinitions.Single(x => x.Id == basis.LinkedTraitId);
			trait = (TraitDefinition)_db.Entry(template).CurrentValues.ToObject();
			trait.Id = Next<TraitDefinition>(); trait.Name = "Untal"; trait.Alias = "untal";
			trait.AvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
			trait.ChargenBlurb = "The cryptic Untal language of the historical worldfile setting.";
			_db.TraitDefinitions.Add(trait);
		}
		var language = (Language)_db.Entry(basis).CurrentValues.ToObject();
		language.Id = Next<Language>(); language.Name = "Untal"; language.LinkedTraitId = trait.Id;
		language.UnknownLanguageDescription = "a cryptic tongue";
		_db.Languages.Add(language);
		var accentBasis = _db.Accents.Where(x => x.LanguageId == basis.Id).OrderBy(x => x.Id).FirstOrDefault()
			?? throw new InvalidOperationException("Legacy Untal requires an installed native accent template.");
		var accent = (Accent)_db.Entry(accentBasis).CurrentValues.ToObject();
		accent.Id = Next<Accent>(); accent.LanguageId = language.Id; accent.Name = "Untal";
		accent.Suffix = "with an Untal accent"; accent.VagueSuffix = "with an unfamiliar accent";
		accent.Description = "An accent from the historical Untal language."; accent.Group = "Legacy Untal";
		accent.ChargenAvailabilityProgId = _settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg];
		_db.Accents.Add(accent);
		Record("language.legacy-untal", nameof(Language), language.Id, null, new
		{ Trait = trait.Id, Accent = accent.Id, Disposition = "Approved historical language support for source inscriptions and explicit NPC language data. Native language/skill/difficulty mechanics; no automatic culture/character grants or new writing system. Existing native Untal is reused unchanged when present." });
		_counts["legacy languages"] = 1;
		Flush();
	}
}
