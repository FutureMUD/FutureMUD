#nullable enable

using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private enum AntiquityAdditionalNameForm
	{
		GivenPatronym,
		GivenFamilyGroup
	}

	private sealed record AntiquityAdditionalNameSeed(
		string Key,
		string NameCultureName,
		AntiquityAdditionalNameForm NameForm,
		string NameRegex,
		string PersonalNameDescription,
		string SecondNameDescription,
		string[] MaleGivenNames,
		string[] FemaleGivenNames,
		string[] MaleSecondNames,
		string[] FemaleSecondNames);

	private void SeedAntiquityAdditionalNameCultures()
	{
		foreach (AntiquityAdditionalNameSeed seed in AntiquityAdditionalNameSeeds)
		{
			ValidateAntiquityAdditionalNameSeed(seed);
			NameCulture nameCulture = AddNameCulture(
				seed.NameCultureName,
				seed.NameRegex,
				GetAntiquityAdditionalNameElements(seed),
				GetAntiquityAdditionalNamePatterns(seed.NameForm));

			SeedAntiquityAdditionalNameProfile(seed, nameCulture, Gender.Male, seed.MaleGivenNames, seed.MaleSecondNames);
			SeedAntiquityAdditionalNameProfile(seed, nameCulture, Gender.Female, seed.FemaleGivenNames, seed.FemaleSecondNames);
		}

		_context.SaveChanges();
	}

	private void SeedAntiquityAdditionalNameProfile(
		AntiquityAdditionalNameSeed seed,
		NameCulture nameCulture,
		Gender gender,
		IEnumerable<string> givenNames,
		IEnumerable<string> secondNames)
	{
		RandomNameProfile profile = AddRandomNameProfile($"{seed.NameCultureName} {gender.DescribeEnum()}", gender, nameCulture);
		AddRandomNameDice(profile, NameUsage.BirthName, "1");
		foreach (string name in givenNames)
		{
			AddRandomNameElement(profile, NameUsage.BirthName, name, 100);
		}

		NameUsage secondUsage = GetAntiquityAdditionalSecondUsage(seed.NameForm);
		AddRandomNameDice(profile, secondUsage, "1");
		foreach (string name in secondNames)
		{
			AddRandomNameElement(profile, secondUsage, name, 100);
		}

		_context.SaveChanges();
	}

	private static (string Name, int Minimum, int Maximum, string Description, NameUsage Usage)[]
		GetAntiquityAdditionalNameElements(AntiquityAdditionalNameSeed seed)
	{
		NameUsage secondUsage = GetAntiquityAdditionalSecondUsage(seed.NameForm);
		return
		[
			("Personal Name", 1, 1, seed.PersonalNameDescription, NameUsage.BirthName),
			(GetAntiquityAdditionalSecondElementLabel(secondUsage), 1, 1, seed.SecondNameDescription, secondUsage)
		];
	}

	private static (NameStyle Style, string Pattern, NameUsage[] Parameters)[]
		GetAntiquityAdditionalNamePatterns(AntiquityAdditionalNameForm form)
	{
		NameUsage secondUsage = GetAntiquityAdditionalSecondUsage(form);
		return
		[
			(NameStyle.GivenOnly, "{0}", [NameUsage.BirthName]),
			(NameStyle.SimpleFull, "{0} {1}", [NameUsage.BirthName, secondUsage]),
			(NameStyle.FullName, "{0} {1}", [NameUsage.BirthName, secondUsage]),
			(NameStyle.Affectionate, "{0}", [NameUsage.BirthName]),
			(NameStyle.SurnameOnly, "{0}", [secondUsage]),
			(NameStyle.FullWithNickname, "{0} {1}", [NameUsage.BirthName, secondUsage])
		];
	}

	private static NameUsage GetAntiquityAdditionalSecondUsage(AntiquityAdditionalNameForm form)
	{
		return form switch
		{
			AntiquityAdditionalNameForm.GivenPatronym => NameUsage.Patronym,
			AntiquityAdditionalNameForm.GivenFamilyGroup => NameUsage.FamilyGroupName,
			_ => throw new ArgumentOutOfRangeException(nameof(form), form, null)
		};
	}

	private static string GetAntiquityAdditionalSecondElementLabel(NameUsage usage)
	{
		return usage switch
		{
			NameUsage.Patronym => "Patronym or Parentage Name",
			NameUsage.FamilyGroupName => "Tribe, House or People Name",
			_ => usage.DescribeEnum()
		};
	}

	private static void ValidateAntiquityAdditionalNameSeed(AntiquityAdditionalNameSeed seed)
	{
		if (string.IsNullOrWhiteSpace(seed.NameCultureName) ||
			string.IsNullOrWhiteSpace(seed.NameRegex) ||
			string.IsNullOrWhiteSpace(seed.PersonalNameDescription) ||
			string.IsNullOrWhiteSpace(seed.SecondNameDescription))
		{
			throw new InvalidOperationException($"{seed.Key} is missing required name-culture data.");
		}

		ValidateAntiquityAdditionalNameList(seed.Key, "male personal", seed.MaleGivenNames, 50);
		ValidateAntiquityAdditionalNameList(seed.Key, "female personal", seed.FemaleGivenNames, 50);
		ValidateAntiquityAdditionalNameList(seed.Key, "male second", seed.MaleSecondNames, 20);
		ValidateAntiquityAdditionalNameList(seed.Key, "female second", seed.FemaleSecondNames, 20);
	}

	private static void ValidateAntiquityAdditionalNameList(
		string key,
		string label,
		IReadOnlyCollection<string> names,
		int minimum)
	{
		if (names.Count < minimum)
		{
			throw new InvalidOperationException($"{key} must define at least {minimum} {label} names; found {names.Count}.");
		}

		if (names.Any(string.IsNullOrWhiteSpace) || names.Any(x => x.Any(char.IsWhiteSpace)))
		{
			throw new InvalidOperationException($"{key} contains a blank or multi-token {label} name. Use hyphens for compounds.");
		}

		if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
		{
			throw new InvalidOperationException($"{key} contains duplicate {label} names.");
		}
	}

	private static readonly AntiquityAdditionalNameSeed[] AntiquityAdditionalNameSeeds =
	[
		new(
			"illyrian_pannonian",
			"Illyrian-Pannonian",
			AntiquityAdditionalNameForm.GivenPatronym,
			@"^(?<birthname>[\w'-]{2,}) (?<patronym>[\w'-]{2,})$",
			"Illyrian, Liburnian, Delmatae and Pannonian names often survive through Greek and Latin spellings, with short personal names and repeated dynastic or clan-associated elements. Common masculine examples include Bato, Gentius, Plator, Monunius, Longarus and Agron; common feminine examples include Teuta, Batea, Etuta, Triteuta, Andia and Dasa.",
			"The second element represents a parentage, house or prominent kin name in a Romanised spelling. Examples include Bato, Plator, Monunius, Gentius, Longarus, Agron, Teuta and Etuta.",
			[
				"Bato", "Baton", "Batas", "Batun", "Plator", "Plaetor", "Pletor", "Gentius", "Genthius", "Monunius",
				"Monounios", "Longarus", "Lydarus", "Bardylis", "Cleitus", "Kleitos", "Glaucias", "Agron", "Pleuratus", "Scerdilaidas",
				"Pinnes", "Epulon", "Verzo", "Testimos", "Dasius", "Dasmenus", "Andinus", "Epicadus", "Laedicalius", "Lavius",
				"Liccaius", "Messor", "Pantauchus", "Pines", "Skerdilaidas", "Teuticus", "Tritanus", "Grabos", "Pleurias", "Galaurus",
				"Pinnus", "Etleus", "Cinna", "Tato", "Temus", "Audata", "Caravantius", "Dardanos", "Daunus", "Volsus"
			],
			[
				"Teuta", "Etuta", "Triteuta", "Bateia", "Batuia", "Batea", "Birkenna", "Bircenna", "Etleua", "Andia",
				"Dasa", "Platona", "Platoria", "Monunia", "Genthia", "Teutana", "Batonis", "Batouna", "Batonia", "Pinnia",
				"Agrona", "Pleurata", "Glaucia", "Cleita", "Bardyla", "Longara", "Lydara", "Dardana", "Daunia", "Epicada",
				"Laedica", "Lavia", "Liccaia", "Teutilla", "Trita", "Testima", "Verzona", "Galaura", "Temusa", "Tatia",
				"Audata", "Caravantia", "Pinneta", "Monouna", "Dasia", "Batila", "Platina", "Gentiana", "Agronia", "Etutia"
			],
			[
				"Bato", "Plator", "Gentius", "Monunius", "Longarus", "Agron", "Pleuratus", "Bardylis", "Glaucias", "Pinnes",
				"Dasius", "Verzo", "Epulon", "Grabos", "Teuta", "Etuta", "Triteuta", "Batea", "Andia", "Dardanos"
			],
			[
				"Bato", "Plator", "Gentius", "Monunius", "Longarus", "Agron", "Pleuratus", "Bardylis", "Glaucias", "Pinnes",
				"Dasius", "Verzo", "Epulon", "Grabos", "Teuta", "Etuta", "Triteuta", "Batea", "Andia", "Dardanos"
			]),

		new(
			"thracian_dacian",
			"Thracian-Dacian",
			AntiquityAdditionalNameForm.GivenPatronym,
			@"^(?<birthname>[\w'-]{2,}) (?<patronym>[\w'-]{2,})$",
			"Thracian, Getic and Dacian names survive mostly through Greek and Latin forms, especially royal, noble and military names. Common masculine examples include Seuthes, Sitalces, Teres, Cotys, Burebista and Decebalus; common feminine examples include Bendis, Meda, Cotysa, Teria, Diza and Seutha.",
			"The second element represents a parentage, dynastic or house name in a Greek or Latin spelling. Examples include Teres, Cotys, Seuthes, Sitalces, Burebista, Decebalus, Dromichaetes and Rholes.",
			[
				"Seuthes", "Sitalces", "Teres", "Cotys", "Kotys", "Rhesus", "Sparadocus", "Zalmodegicus", "Dromichaetes", "Burebista",
				"Decebalus", "Deceneus", "Cotiso", "Comosicus", "Scorilo", "Duras", "Diurpaneus", "Diegis", "Oroles", "Rholes",
				"Bithus", "Mucapor", "Mucatralis", "Mucatra", "Auluporis", "Diza", "Dizapes", "Dentupes", "Zibelmius", "Amatokos",
				"Cersobleptes", "Sadocus", "Medocus", "Berisades", "Adaios", "Abrupolis", "Auluzon", "Bessos", "Bitus", "Brasus",
				"Cosingas", "Dadas", "Denzibalus", "Drenis", "Eptaporis", "Mokkos", "Mokaporis", "Mokazis", "Rascuporis", "Ziaelas"
			],
			[
				"Bendis", "Meda", "Cotysa", "Kotysa", "Teria", "Diza", "Seutha", "Sitalca", "Rhesa", "Dromichaeta",
				"Burebista", "Decebala", "Decenea", "Cotisona", "Comosica", "Scorila", "Dura", "Diegia", "Orola", "Rhola",
				"Bitha", "Mucapora", "Mucatralia", "Aulupora", "Dizapa", "Dentupa", "Zibelmia", "Amatoka", "Cersoblepta", "Sadoca",
				"Medoca", "Berisada", "Adaia", "Abrupola", "Auluzona", "Bessa", "Brasia", "Dada", "Denzibala", "Drenisa",
				"Eptapora", "Mokka", "Mokapora", "Mokazia", "Rascupora", "Ziaela", "Tarsa", "Zina", "Sura", "Brygia"
			],
			[
				"Teres", "Cotys", "Seuthes", "Sitalces", "Dromichaetes", "Burebista", "Decebalus", "Rholes", "Mucapor", "Zibelmius",
				"Amatokos", "Cersobleptes", "Medocus", "Berisades", "Duras", "Cotiso", "Scorilo", "Oroles", "Bessos", "Rascuporis"
			],
			[
				"Teres", "Cotys", "Seuthes", "Sitalces", "Dromichaetes", "Burebista", "Decebalus", "Rholes", "Mucapor", "Zibelmius",
				"Amatokos", "Cersobleptes", "Medocus", "Berisades", "Duras", "Cotiso", "Scorilo", "Oroles", "Bessos", "Rascuporis"
			]),

		new(
			"levantine",
			"Antiquity Levantine",
			AntiquityAdditionalNameForm.GivenPatronym,
			@"^(?<birthname>[\w'-]{2,}) (?<patronym>(?:bar|ben|bat|barat)-[\w'-]{2,})$",
			"Levantine names in this period are deliberately broad, drawing on Aramaic, Nabataean, Palmyrene, Phoenician and related Semitic naming habits as they appear in Greek and Latin contact zones. Common masculine examples include Abgar, Aretas, Malichus, Obodas, Rabbel and Odaenathus; common feminine examples include Zenobia, Shuqailat, Amatallat, Batnoam, Berenice and Salome.",
			"The second element is a patronym or parentage name. Masculine examples use bar- or ben- forms; feminine examples use bat- or barat- forms.",
			[
				"Abgar", "Aretas", "Malichus", "Obodas", "Rabbel", "Syllaeus", "Odaenathus", "Hairan", "Vaballathus", "Wahballat",
				"Zabdas", "Zabdila", "Malku", "Yarhai", "Moqimu", "Shaidu", "Aglibol", "Malakbel", "Barates", "Gaddai",
				"Neshra", "Abdai", "Abdastart", "Abdmilk", "Abdobodat", "Abdsamya", "Yedibel", "Mattai", "Nabatu", "Nashru",
				"Hannai", "Soados", "Mannai", "Themu", "Bolha", "Bargates", "Azizu", "Samsigeram", "Iamblichus", "Sohaemus",
				"Antiochus", "Mithridates", "Sampsiceramus", "Maanu", "Zabdibol", "Maliku", "Haidu", "Abdel", "Haram", "Qosnatan"
			],
			[
				"Zenobia", "Bathzabbai", "Shuqailat", "Shaqilat", "Hagiru", "Hagaru", "Amatallat", "Batnoam", "Berenice", "Salome",
				"Martha", "Mariamme", "Mariam", "Huldu", "Taimi", "Zabdila", "Baltha", "Soadia", "Malkuta", "Yarhaia",
				"Moqima", "Shaida", "Aglibola", "Malakbela", "Gaddaia", "Neshra", "Abdaia", "Abdastarta", "Abdmilka", "Abdobodata",
				"Abdsamya", "Yedibla", "Mattaia", "Nabatea", "Nashra", "Hannaia", "Thema", "Bargata", "Aziza", "Samsigerama",
				"Iambliche", "Sohaema", "Antiochia", "Maana", "Zabdibola", "Malikuta", "Haramia", "Qosnata", "Baalata", "Tadmora"
			],
			[
				"bar-Abgar", "bar-Aretas", "bar-Malichus", "bar-Obodas", "bar-Rabbel", "bar-Odaenathus", "bar-Hairan", "bar-Malku", "bar-Yarhai", "bar-Moqimu",
				"ben-Abgar", "ben-Aretas", "ben-Malichus", "ben-Obodas", "ben-Rabbel", "ben-Gaddai", "ben-Neshra", "ben-Mattai", "bar-Malakbel", "bar-Wahballat"
			],
			[
				"bat-Abgar", "bat-Aretas", "bat-Malichus", "bat-Obodas", "bat-Rabbel", "bat-Odaenathus", "bat-Hairan", "bat-Malku", "bat-Yarhai", "bat-Moqimu",
				"barat-Abgar", "barat-Aretas", "barat-Malichus", "barat-Obodas", "barat-Rabbel", "barat-Gaddai", "barat-Neshra", "barat-Mattai", "bat-Malakbel", "bat-Wahballat"
			]),

		new(
			"numidian_mauretanian",
			"Numidian-Mauretanian",
			AntiquityAdditionalNameForm.GivenFamilyGroup,
			@"^(?<birthname>[\w'-]{2,}) (?<familygroupname>[\w'-]{2,})$",
			"Numidian and Mauretanian names are drawn from the Libyco-Berber and North African royal and tribal naming horizon as it appears in Punic, Greek and Latin sources. Common masculine examples include Masinissa, Micipsa, Jugurtha, Adherbal, Hiempsal and Bocchus; feminine examples are necessarily broader and include Sophonisba, Eunoe, Masinissa-derived and tribal feminine forms suitable for the same setting.",
			"The second element represents a tribe, royal house, regional people or broad kin affiliation rather than a fixed hereditary surname. Examples include Massylii, Masaesyli, Gaetuli, Mauri, Musulamii and Cirta.",
			[
				"Masinissa", "Massinissa", "Micipsa", "Jugurtha", "Jugurthen", "Adherbal", "Hiempsal", "Gulussa", "Mastanabal", "Gaia",
				"Syphax", "Vermina", "Bocchus", "Bogud", "Juba", "Gauda", "Massiva", "Oxyntas", "Iampsas", "Lacumazes",
				"Masgava", "Misagenes", "Stembanos", "Arabion", "Capussa", "Mazetullus", "Naravas", "Iarbas", "Hiarbas", "Aedemon",
				"Tacfarinas", "Afer", "Iftas", "Masties", "Firmus", "Gildo", "Nubel", "Masuna", "Meztul", "Aksil",
				"Ayar", "Aderbal", "Ameqran", "Amestan", "Amezian", "Ammar", "Izem", "Massyle", "Mauri", "Numid"
			],
			[
				"Sophonisba", "Eunoe", "Massiva", "Masinissa", "Micipsa", "Jugurtha", "Adherbala", "Hiempsala", "Gulussa", "Mastanabala",
				"Gaia", "Syphaxa", "Vermina", "Boccha", "Boguda", "Juba", "Gauda", "Oxynta", "Iampsa", "Lacumaza",
				"Masgava", "Misagena", "Stembana", "Arabiona", "Capussa", "Mazetulla", "Narava", "Iarba", "Hiarba", "Aedemona",
				"Tacfarina", "Afera", "Iftasa", "Mastia", "Firma", "Gilda", "Nubela", "Masuna", "Meztula", "Aksila",
				"Ayara", "Aderbala", "Ameqrana", "Amestana", "Ameziana", "Ammara", "Izema", "Massyla", "Mauria", "Numida"
			],
			[
				"Massylii", "Masaesyli", "Gaetuli", "Mauri", "Musulamii", "Cinithii", "Nasamones", "Garamantes", "Autololes", "Mazices",
				"Bavares", "Quinquegentiani", "Cirta", "Iol", "Siga", "Thugga", "Lambaesis", "Aures", "Atlas", "Zama"
			],
			[
				"Massylii", "Masaesyli", "Gaetuli", "Mauri", "Musulamii", "Cinithii", "Nasamones", "Garamantes", "Autololes", "Mazices",
				"Bavares", "Quinquegentiani", "Cirta", "Iol", "Siga", "Thugga", "Lambaesis", "Aures", "Atlas", "Zama"
			])
	];
}
