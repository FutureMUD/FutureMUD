#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureLanguageSourceBinding(string SourcePack, string SourceName, string CanonicalKey);

/// <summary>
/// Explicit crosswalk of the retained historical generators. An unlisted label stays source-qualified;
/// modern labels never supply a historical-stage binding. These are implementation bindings, not new research.
/// </summary>
public static class CultureToolkitLanguageBindings
{
	public static readonly IReadOnlyDictionary<string, string> SourceModuleFirstEra = new Dictionary<string, string>
	{
		["earthantiquity"] = "antiquity", ["earthdarkagesandmedieval"] = "darkages",
		["earthrenaissanceeurope"] = "medieval", ["earthrenaissanceworldexpansion"] = "renaissance"
	};
	public static IReadOnlyList<CultureLanguageSourceBinding> All { get; } = Parse("""
earthantiquity|Latin|latin
earthantiquity|Attic Greek|greek.ancient
earthantiquity|Koine Greek|greek.koine
earthantiquity|Aramaic|aramaic
earthantiquity|Arabic|arabic.old
earthantiquity|Coptic|coptic
earthantiquity|Hebrew|hebrew
earthdarkagesandmedieval|Latin|latin
earthdarkagesandmedieval|Medieval Greek|greek.medieval
earthdarkagesandmedieval|Koine Greek|greek.koine
earthdarkagesandmedieval|Mashriqi Arabic|arabic.mashriqi
earthdarkagesandmedieval|Quranic Arabic|arabic.classical
earthdarkagesandmedieval|Persian|persian.new
earthdarkagesandmedieval|Old Norse|norse.old
earthdarkagesandmedieval|Oghuz Turkic|turkic.oghuz
earthdarkagesandmedieval|Old Hungarian|hungarian
earthdarkagesandmedieval|Old East Slavic|slavic.old-east
earthdarkagesandmedieval|Church Slavonic|church-slavonic
earthdarkagesandmedieval|Cuman-Kipchak|turkic.kipchak
earthdarkagesandmedieval|Old Norman|norman.old
earthdarkagesandmedieval|Anglo-Norman|french.anglonorman
earthdarkagesandmedieval|Old French|french.old
earthdarkagesandmedieval|Middle English|english.middle
earthdarkagesandmedieval|Medieval Gaelic|gaelic.medieval
earthdarkagesandmedieval|Old High German|german.old-high
earthdarkagesandmedieval|Old Low Franconian|franconian.old-low
earthdarkagesandmedieval|Middle High German|german.middle-high
earthdarkagesandmedieval|Old English|english.old
earthdarkagesandmedieval|Middle Low German|german.middle-low
earthdarkagesandmedieval|Old Castilian|castilian.old
earthdarkagesandmedieval|Old Leonese|leonese
earthdarkagesandmedieval|Old Aragonese|aragonese
earthdarkagesandmedieval|Galician-Portuguese|galician-portuguese
earthdarkagesandmedieval|Old Catalan|catalan
earthdarkagesandmedieval|Andalusi Arabic|arabic.andalusi
earthrenaissanceeurope|Latin|latin
earthrenaissanceeurope|Aragonese|aragonese
earthrenaissanceeurope|Leonese|leonese
earthrenaissanceeurope|Galician|galician
earthrenaissanceeurope|Wallachian|romanian
earthrenaissanceeurope|English|english.middle
earthrenaissanceeurope|High German|german.early-new-high
earthrenaissanceeurope|Low German|german.middle-low
earthrenaissanceeurope|Dutch|dutch
earthrenaissanceeurope|Frisian|frisian
earthrenaissanceeurope|Yiddish|yiddish
earthrenaissanceeurope|French|french.middle
earthrenaissanceeurope|Swedish|swedish
earthrenaissanceeurope|Norwegian|norwegian
earthrenaissanceeurope|Icelandic|icelandic
earthrenaissanceeurope|Danish|danish
earthrenaissanceeurope|Russian|russian
earthrenaissanceeurope|Ruthenian|ruthenian
earthrenaissanceeurope|Church Slavonic|church-slavonic
earthrenaissanceeurope|Polish|polish
earthrenaissanceeurope|Slovak|slovak
earthrenaissanceeurope|Occitan|occitan
earthrenaissanceeurope|Czech|czech
earthrenaissanceeurope|Prussian|old-prussian
earthrenaissanceeurope|Lithuanian|lithuanian
earthrenaissanceeurope|Latvian|latvian
earthrenaissanceeurope|Slovenian|slovene
earthrenaissanceeurope|Bulgarian|bulgarian
earthrenaissanceeurope|Quranic Arabic|arabic.classical
earthrenaissanceeurope|Mahgrebi Arabic|arabic.maghrebi
earthrenaissanceeurope|Mashriqi Arabic|arabic.mashriqi
earthrenaissanceeurope|Italian|italian
earthrenaissanceeurope|Aramaic|aramaic
earthrenaissanceeurope|Hebrew|hebrew
earthrenaissanceeurope|Greek|greek.medieval
earthrenaissanceeurope|Koine Greek|greek.koine
earthrenaissanceeurope|Persian|persian.new
earthrenaissanceeurope|Armenian|armenian
earthrenaissanceeurope|Coptic|coptic
earthrenaissanceeurope|Irish Gaelic|irish
earthrenaissanceeurope|Scottish Gaelic|scottish-gaelic
earthrenaissanceeurope|Welsh|welsh
earthrenaissanceeurope|Manx|source.earthrenaissanceeurope.language.Manx
earthrenaissanceeurope|Breton|breton
earthrenaissanceeurope|Cornish|cornish
earthrenaissanceeurope|Turkish|turkish.ottoman
earthrenaissanceeurope|Finnish|finnish
earthrenaissanceeurope|Hungarian|hungarian
earthrenaissanceeurope|Estonian|estonian
earthrenaissanceeurope|Venetian|venetian
earthrenaissanceeurope|Basque|basque
earthrenaissanceeurope|Albanian|albanian
earthrenaissanceeurope|Sicilian|sicilian
earthrenaissanceeurope|Castilian|castilian
earthrenaissanceworldexpansion|Armenian|armenian
earthrenaissanceworldexpansion|Kurmanji Kurdish|kurdish.kurmanji
earthrenaissanceworldexpansion|Gorani|kurdish.gorani
earthrenaissanceworldexpansion|Tashelhit|amazigh.tashelhit
earthrenaissanceworldexpansion|Tarifit|amazigh.tarifit
earthrenaissanceworldexpansion|Kabyle|amazigh.kabyle
earthrenaissanceworldexpansion|Mashriqi Arabic|arabic.mashriqi
earthrenaissanceworldexpansion|Mahgrebi Arabic|arabic.maghrebi
earthrenaissanceworldexpansion|Quranic Arabic|arabic.classical
earthrenaissanceworldexpansion|Coptic|coptic
earthrenaissanceworldexpansion|Persian|persian.new
earthrenaissanceworldexpansion|Georgian|georgian
earthrenaissanceworldexpansion|Sanskrit|source.earthdarkagesandmedieval.language.Sanskrit
""");

	private static readonly IReadOnlyDictionary<(string Pack, string Name), string> Keys =
		All.ToDictionary(x => (x.SourcePack, x.SourceName), x => x.CanonicalKey);

	public static string Key(string sourcePack, string sourceName) =>
		Keys.GetValueOrDefault((sourcePack, sourceName)) ?? $"source.{sourcePack}.language.{sourceName}";

	private static CultureLanguageSourceBinding[] Parse(string text) => text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
		.Select(x => x.Trim().Split('|')).Select(x => new CultureLanguageSourceBinding(x[0], x[1], x[2])).ToArray();
}
