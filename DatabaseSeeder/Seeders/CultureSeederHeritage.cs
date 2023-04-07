using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private readonly Dictionary<string, FutureProg> _cultureProgs =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, Culture> _cultures = new(StringComparer.OrdinalIgnoreCase);
	private readonly Dictionary<string, Ethnicity> _ethnicities = new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, FutureProg> _ethnicProgs =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, FutureProg> _raceProgs =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, Race> _races = new(StringComparer.OrdinalIgnoreCase);

	public void AddEthnicity(Race race, string name, string group, string bloodGroup,
		double tempFloor = 0.0, double tempCeiling = 0.0, string subgroup = "", FutureProg? available = null,
		string description = "")
	{
		var ethnicity = new Ethnicity
		{
			Name = name,
			ParentRace = race,
			ChargenBlurb = description,
			EthnicGroup = group,
			EthnicSubgroup = subgroup,
			PopulationBloodModel = _bloodModels[bloodGroup],
			TolerableTemperatureFloorEffect = tempFloor,
			TolerableTemperatureCeilingEffect = tempCeiling,
			AvailabilityProg = available ?? _alwaysTrueProg
		};
		_ethnicities[name] = ethnicity;
		_context.Ethnicities.Add(ethnicity);
		_context.SaveChanges();

		var prog = new FutureProg
		{
			FunctionName = $"IsEthnicity{name.CollapseString()}",
			FunctionComment = $"Determines whether someone is the {name} ethnicity",
			FunctionText = $"return @ch.Ethnicity == ToEthnicity({ethnicity.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Ethnicity",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(prog);
		_context.SaveChanges();
		_ethnicProgs[name] = prog;
	}

	public void AddEthnicityVariable(string ethnicity, string feature, string profile)
	{
		if (feature.Equals("Humanoid Frame", StringComparison.InvariantCultureIgnoreCase) &&
		    !_definitions.ContainsKey("Humanoid Frame"))
			feature = "Frame";

		if (!_definitions.Any(x => x.Key.Equals(feature, StringComparison.OrdinalIgnoreCase)))
		{
#if DEBUG
			throw new ApplicationException($"Unknown definition {feature}");
#endif
			return;
		}

		if (profile.Equals("All Eye Shapes", StringComparison.OrdinalIgnoreCase) &&
		    _profiles.ContainsKey("all eye sapes")) profile = "All Eye Sapes";

		if (!_profiles.ContainsKey(profile))
		{
#if DEBUG
			throw new ApplicationException($"Unknown definition {feature}");
#endif
			return;
		}

		var foundProfile = _profiles[profile];

		if (_ethnicities[ethnicity].EthnicitiesCharacteristics
		    .Any(x => x.CharacteristicDefinition == _definitions[feature])) return;
		_context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = _ethnicities[ethnicity],
			CharacteristicDefinition = _definitions[feature],
			CharacteristicProfile = _profiles[profile.ToLowerInvariant()]
		});
	}

	public void AddCulture(string name, string nameCulture, string description, FutureProg? available = null,
		FutureProg? skillProg = null, Calendar? calendar = null)
	{
		var culture = new Culture
		{
			Name = name,
			Description = description,
			TolerableTemperatureCeilingEffect = 0.0,
			TolerableTemperatureFloorEffect = 0.0,
			AvailabilityProg = available ?? _alwaysTrueProg,
			PrimaryCalendarId = calendar?.Id ?? 1,
			SkillStartingValueProg = skillProg ?? _skillStartProg,
			PersonWordFemale = "Woman",
			PersonWordIndeterminate = "Person",
			PersonWordMale = "Man",
			PersonWordNeuter = "Person"
		};
		_context.Cultures.Add(culture);
		var nc = _context.NameCultures.First(x => x.Name == nameCulture);
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = nc, Gender = (short)Gender.Male });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = nc, Gender = (short)Gender.Female });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = nc, Gender = (short)Gender.Neuter });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = nc, Gender = (short)Gender.NonBinary });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = nc, Gender = (short)Gender.Indeterminate });

		_context.SaveChanges();
		_cultures[name] = culture;

		var prog = new FutureProg
		{
			FunctionName = $"IsCulture{name.CollapseString()}",
			FunctionComment = $"Determines whether someone is the {name} culture",
			FunctionText = $"return @ch.Culture == ToCulture({culture.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Culture",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(prog);
		_context.SaveChanges();
		_cultureProgs[name] = prog;
	}

	public void AddCulture(string name, string nameCultureMale, string nameCultureFemale, string description,
		FutureProg? available = null, FutureProg? skillProg = null,
		Calendar? calendar = null)
	{
		var culture = new Culture
		{
			Name = name,
			Description = description,
			TolerableTemperatureCeilingEffect = 0.0,
			TolerableTemperatureFloorEffect = 0.0,
			AvailabilityProg = available ?? _alwaysTrueProg,
			PrimaryCalendarId = calendar?.Id ?? 1,
			SkillStartingValueProg = skillProg ?? _skillStartProg,
			PersonWordFemale = "Woman",
			PersonWordIndeterminate = "Person",
			PersonWordMale = "Man",
			PersonWordNeuter = "Person"
		};
		_context.Cultures.Add(culture);
		var ncMale = _context.NameCultures.First(x => x.Name == nameCultureMale);
		var ncFemale = _context.NameCultures.First(x => x.Name == nameCultureFemale);
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = ncMale, Gender = (short)Gender.Male });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = ncFemale, Gender = (short)Gender.Female });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = ncMale, Gender = (short)Gender.Neuter });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = ncFemale, Gender = (short)Gender.NonBinary });
		culture.CulturesNameCultures.Add(new CulturesNameCultures
			{ Culture = culture, NameCulture = ncMale, Gender = (short)Gender.Indeterminate });

		_context.SaveChanges();
		_cultures[name] = culture;

		var prog = new FutureProg
		{
			FunctionName = $"IsCulture{name.CollapseString()}",
			FunctionComment = $"Determines whether someone is the {name} culture",
			FunctionText = $"return @ch.Culture == ToCulture({culture.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Culture",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(prog);
		_context.SaveChanges();
		_cultureProgs[name] = prog;
	}


	public void SeedModernHeritage()
	{
		AddEthnicity(_humanRace, "Germanic", "Western European", "O-A High Negative", 0, 0,
			description:
			"The Germanic peoples are an Indo-European ethno-linguistic group found in Britain, France, Germany and the Low Countries. They are typically characteristised by fair skin, fair hair and light eyes.");
		AddEthnicity(_humanRace, "Italic", "Mediterranian", "O-A High Negative", 0, 0,
			description:
			"The Italic peoples are an Indo-European ethno-linguistic group with their origins on the Italian peninsula. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Hispanic", "Mediterranian", "O-A High Negative", 0, 0,
			description:
			"The Hispanic peoples are an Indo-European ethno-linguistic group with their origins on the Iberian Peninsula. Due to extensive colonisation of the Americas, many American ethnic groups are ultimately Hispanic as well. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Celtic", "Western European", "Majority O Minor A", 0, 0,
			description:
			"The Celtic peoples are an Indo-European ethno-linguistic group predominantly found in the British Isles and France. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Slavic", "Eastern European", "O-A High Negative", 0, 0,
			description:
			"The Slavic peoples are an Indo-European ethno-linguistic group found in Central Europe, Eastern Europe and Central Asia. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Greek", "Mediterranian", "O-A High Negative", 0, 0,
			description:
			"The Greek peoples are an Indo-European ethno-linguistic group found to some extent most everywhere in the eastern Mediterranian (though predominantly Greece, Anatolia and Southern Italy). They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Turkish", "Middle Eastern", "A Dominant", 0, 0,
			description:
			"The Turkish peoples are an Indo-European ethno-linguistic group found predominantly in Turkey and Central Asia. They are typically characterised by dark olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Arabic", "Middle Eastern", "Majority O Minor A", 0, 0,
			description:
			"The Arabic peoples are an Indo-European ethno-linguistic group found predominantly in the Arabian Peninsula, North Africa and the Horn of Africa. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Persian", "Middle Eastern", "Majority O Minor A", 0, 0,
			description:
			"The Persian peoples are an Indo-European ethno-linguistic group found in Central Asia and the Middle East. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Scandanavian", "Western European", "A Dominant", 0, 0,
			description:
			"The Scandanavian peoples are an Indo-European ethno-linguistic group found in Northern Europe. They are typically characterised by very fair skin, fair hair and light eyes.");
		AddEthnicity(_humanRace, "North African", "African", "Majority O", 0, 0,
			description:
			"The North African peoples are an Afro-Asiatic ethno-linguistic group found in North Africa. They emerged as a distinct group from Sub-Saharan Africans due to the isolating effect of the desert. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Bantu", "African", "Majority O", 0, 0,
			description:
			"The Bantu peoples are a Sub-Saharan ethno-linguistic family originating in West Africa, but found across the vast majority of the continent. They are characterised by dark brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Khoisan", "African", "Majority O", 0, 0,
			description:
			"The Khoisan peoples are a Sub-Saharan ethno-linguistic group found mainly in Southern Africa. They are characterised by dark brown skin, dark hair and dark, hooded eyes.");
		AddEthnicity(_humanRace, "Swahili", "African", "Majority O", 0, 0,
			description:
			"The Swahili peoples are a Sub-Saharan ethno-linguistic group found along the eastern coast of Africa, ethnically related to the Bantu and Arabic peoples. They are characterised by brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Polynesian", "Oceanic", "Majority O", 0, 0,
			description:
			"The Polynesian peoples are an Oceanic ethno-linguistic group found across the eastern and southern pacific. They are characterised by light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Austronesian", "Oceanic", "B Dominant", 0, 0,
			description:
			"The Austronesian peoples are found mostly around the Indian Ocean, including the Madagascans, Taiwanese, Malaysians, Fillipinos and Indonesians. They are characterised by light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Melanesian", "Oceanic", "Majority O", 0, 0,
			description:
			"The Melanesian peoples are an Oceanic ethno-linguistic group found in Papua New Guinea, Fiji, Vanuatu and other nearby islands. They are characterised by brown skin, dark hair and dark eyes, although a significant number also have blonde hair.");
		AddEthnicity(_humanRace, "Dravidian", "Indian", "B Dominant", 0, 0,
			description:
			"The Dravidian peoples are an Indo-European ethno-linguistic group found in southern India and Sri Lanka. They are typically characterised by dark brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Indo-Aryan", "Indian", "B Dominant", 0, 0,
			description:
			"The Indo-Aryan peoples are an Indo-European ethno-linguistic group found in northern India, Bangladesh and Pakistan. They are typically characterised by light brown or brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Afro-Carribean", "African", "Majority O", 0, 0,
			description:
			"The Afro-Carribean peoples are the descendents of African slaves brought to the Carribean since European colonisation of the area. They come from a wide variety of Sub-Saharan African ethnicities and many have mixed heritage with European or Native American influences. Because of this, they have significant variety of physical characteristics. Skin colour ranges from light brown to dark brown, hair colour tends to be dark but both light and dark eyes do occur.");
		AddEthnicity(_humanRace, "Afro-American", "African", "Majority O", 0, 0,
			description:
			"The African-American peoples are the descendents of African slaves brought to North America since European colonisation of the area. They come from a wide variety of Sub-Saharan African ethnicities and many have mixed heritage with European or Native American influences. Because of this, they have significant variety of physical characteristics. Skin colour ranges from light brown to dark brown, hair colour tends to be dark but both light and dark eyes do occur.");
		AddEthnicity(_humanRace, "Eskimo", "Aboriginal", "A Dominant", 0, 0,
			description:
			"The Eskimo are an ethno-linguistic group who have traditionally inhabited the northern polar region.They are typically characterised by dark golden or reddish toned skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Amerindian", "Aboriginal", "Majority O Minor A", 0, 0,
			description:
			"The Amerindian are a North American ethno-linguistic family representing a number of distinct cultural groups in the United States and Canada. There is considerable variation in physical characteristics, but they tend to be characterised by golden, reddish toned or brown skin, and generally have dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Mesoamerican", "Aboriginal", "Majority O", 0, 0,
			description:
			"The Mesoamericans are a Central American ethno-linguistic family representing a number of distinct cultural groups in Mexico and Central America. They tend to be characterised by reddish toned or light brown skin, and generally have dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Andean", "Aboriginal", "Overwhelmingly O", 0, 0,
			description:
			"The Andeans are a South American ethno-linguistic family representing a number of distinct cultural groups in South America. They tend to be characterised by brown or olive skin, dark hair, and light or dark eyes.");
		AddEthnicity(_humanRace, "Mongolian", "East Asian", "B Dominant", 0, 0,
			description:
			"The Mongolians are an ethno-linguistic family representing both the peoples of Mongolia and many minority populations across Russia, China and other Northern Asian countries. They tend to be characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Han", "East Asian", "B Dominant", 0, 0,
			description:
			"The Han are an ethnic group native to east Asia, predominantly China, and are the most numerous ethnic group in the entire world, with a large diaspora. They tend to be characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Yamato", "East Asian", "B Dominant", 0, 0,
			description:
			"The Yamato are the predominant ethno-linguistic group in Japan. They are characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Korean", "East Asian", "B Dominant", 0, 0,
			description:
			"The Korean are an ethno-linguistic group found predominantly in the Korean peninsula and Manchuria. They are characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Tibetan", "East Asian", "B Dominant", 0, 0,
			description:
			"The Tibetans are an ethnic group found in areas of China, Indian, Nepal, and Bhutan. They are characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Uyghur", "East Asian", "A Dominant", 0, 0,
			description:
			"The Uyghur are an ethnic group of Turkic origin that live in Eastern and Central Asia (primarily China and the various Turkic states in Central Asia). They are characterised by fair or golden skin tones, dark hair and dark eyes (although fair haired Uyghur are not unheard of).");
		AddEthnicity(_humanRace, "Ainu", "Aboriginal", "B Dominant", 0, 0,
			description:
			"The Ainu are a small ethnic group found in Northern Japan and parts of Russia. They are characterised by fair or golden skin tones, dark hair and dark eyes. In particular, their men are renowned for their thick facial hair.");
		AddEthnicity(_humanRace, "Aboriginal Australian", "Aboriginal", "Majority O Minor A", 0, 0,
			description:
			"The Aboriginal Australians are an ethno-linguistic group found in and around Australia. They are characterised by dark brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Austro-Asiatic", "Aboriginal", "B Dominant", 0, 0,
			description:
			"The Austro-Asiatics are an ethno-linguistic group located in South East Asia, predominently Myanmar and Cambodia. They are characterised by golden or light brown skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Tai-Kadai", "East Asian", "B Dominant", 0, 0,
			description:
			"The Tai-Kadai are an ethno-linguistic group located in South East Asia, predominently Thailand and Laos. They are characterised by golden skin tones, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Anglo-Saxon", "Western European", "Majority O Minor A", 0, 0,
			description:
			"The Anglo-Saxon peoples are an Indo-European ethno-linguistic group found in the British Isles and various former British Colonies such as the USA, Canada, South Africa and Australia. They are typically characteristised by fair skin, fair hair and light eyes.");

		AddEthnicityVariable("Aboriginal Australian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Aboriginal Australian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Aboriginal Australian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Aboriginal Australian", "Ears", "All Ears");
		AddEthnicityVariable("Aboriginal Australian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Aboriginal Australian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Aboriginal Australian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Aboriginal Australian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Aboriginal Australian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Aboriginal Australian", "Nose", "All Noses");
		AddEthnicityVariable("Aboriginal Australian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Aboriginal Australian", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Afro-American", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Afro-American", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Afro-American", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Afro-American", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Afro-American", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Afro-American", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Afro-American", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Afro-American", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Afro-American", "Nose", "All Noses");
		AddEthnicityVariable("Afro-American", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Afro-American", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Afro-Carribean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Afro-Carribean", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Afro-Carribean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Afro-Carribean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Afro-Carribean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Afro-Carribean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Afro-Carribean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Afro-Carribean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Afro-Carribean", "Nose", "All Noses");
		AddEthnicityVariable("Afro-Carribean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Afro-Carribean", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Ainu", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Ainu", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Ainu", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Ainu", "Ears", "All Ears");
		AddEthnicityVariable("Ainu", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Ainu", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Ainu", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Ainu", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Ainu", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Ainu", "Nose", "All Noses");
		AddEthnicityVariable("Ainu", "Ears", "All Ears");
		AddEthnicityVariable("Ainu", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Ainu", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Amerindian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Amerindian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Amerindian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Amerindian", "Ears", "All Ears");
		AddEthnicityVariable("Amerindian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Amerindian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Amerindian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Amerindian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Amerindian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Amerindian", "Nose", "All Noses");
		AddEthnicityVariable("Amerindian", "Ears", "All Ears");
		AddEthnicityVariable("Amerindian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Amerindian", "Skin Colour", "redbrown_skin");
		AddEthnicityVariable("Andean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Andean", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Andean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Andean", "Ears", "All Ears");
		AddEthnicityVariable("Andean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Andean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Andean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Andean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Andean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Andean", "Nose", "All Noses");
		AddEthnicityVariable("Andean", "Ears", "All Ears");
		AddEthnicityVariable("Andean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Andean", "Skin Colour", "redbrown_skin");
		AddEthnicityVariable("Anglo-Saxon", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Anglo-Saxon", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Anglo-Saxon", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Anglo-Saxon", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Anglo-Saxon", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Anglo-Saxon", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Anglo-Saxon", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Anglo-Saxon", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Anglo-Saxon", "Nose", "All Noses");
		AddEthnicityVariable("Anglo-Saxon", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Anglo-Saxon", "Skin Colour", "fair_skin");
		AddEthnicityVariable("Arabic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Arabic", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Arabic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Arabic", "Ears", "All Ears");
		AddEthnicityVariable("Arabic", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Arabic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Arabic", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Arabic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Arabic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Arabic", "Nose", "All Noses");
		AddEthnicityVariable("Arabic", "Ears", "All Ears");
		AddEthnicityVariable("Arabic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Arabic", "Skin Colour", "swarthy_skin");
		AddEthnicityVariable("Austro-Asiatic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Austro-Asiatic", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Austro-Asiatic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Austro-Asiatic", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Austro-Asiatic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Austro-Asiatic", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Austro-Asiatic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Austro-Asiatic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Austro-Asiatic", "Nose", "All Noses");
		AddEthnicityVariable("Austro-Asiatic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Austro-Asiatic", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Austronesian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Austronesian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Austronesian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Austronesian", "Ears", "All Ears");
		AddEthnicityVariable("Austronesian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Austronesian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Austronesian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Austronesian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Austronesian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Austronesian", "Nose", "All Noses");
		AddEthnicityVariable("Austronesian", "Ears", "All Ears");
		AddEthnicityVariable("Austronesian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Austronesian", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Bantu", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Bantu", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Bantu", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Bantu", "Ears", "All Ears");
		AddEthnicityVariable("Bantu", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Bantu", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Bantu", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Bantu", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Bantu", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Bantu", "Nose", "All Noses");
		AddEthnicityVariable("Bantu", "Ears", "All Ears");
		AddEthnicityVariable("Bantu", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Bantu", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Celtic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Celtic", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Celtic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Celtic", "Ears", "All Ears");
		AddEthnicityVariable("Celtic", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Celtic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Celtic", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Celtic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Celtic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Celtic", "Nose", "All Noses");
		AddEthnicityVariable("Celtic", "Ears", "All Ears");
		AddEthnicityVariable("Celtic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Celtic", "Skin Colour", "fair_skin");
		AddEthnicityVariable("Dravidian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Dravidian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Dravidian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Dravidian", "Ears", "All Ears");
		AddEthnicityVariable("Dravidian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dravidian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Dravidian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dravidian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Dravidian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Dravidian", "Nose", "All Noses");
		AddEthnicityVariable("Dravidian", "Ears", "All Ears");
		AddEthnicityVariable("Dravidian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Dravidian", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Eskimo", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Eskimo", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Eskimo", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Eskimo", "Ears", "All Ears");
		AddEthnicityVariable("Eskimo", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Eskimo", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Eskimo", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Eskimo", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Eskimo", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Eskimo", "Nose", "All Noses");
		AddEthnicityVariable("Eskimo", "Ears", "All Ears");
		AddEthnicityVariable("Eskimo", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Eskimo", "Skin Colour", "redbrown_skin");
		AddEthnicityVariable("Germanic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Germanic", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Germanic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Germanic", "Ears", "All Ears");
		AddEthnicityVariable("Germanic", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Germanic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Germanic", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Germanic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Germanic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Germanic", "Nose", "All Noses");
		AddEthnicityVariable("Germanic", "Ears", "All Ears");
		AddEthnicityVariable("Germanic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Germanic", "Skin Colour", "fair_skin");
		AddEthnicityVariable("Greek", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Greek", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Greek", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Greek", "Ears", "All Ears");
		AddEthnicityVariable("Greek", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Greek", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Greek", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Greek", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Greek", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Greek", "Nose", "All Noses");
		AddEthnicityVariable("Greek", "Ears", "All Ears");
		AddEthnicityVariable("Greek", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Greek", "Skin Colour", "fair_olive_skin");
		AddEthnicityVariable("Han", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Han", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Han", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Han", "Ears", "All Ears");
		AddEthnicityVariable("Han", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Han", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Han", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Han", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Han", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Han", "Nose", "All Noses");
		AddEthnicityVariable("Han", "Ears", "All Ears");
		AddEthnicityVariable("Han", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Han", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Hispanic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Hispanic", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Hispanic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Hispanic", "Ears", "All Ears");
		AddEthnicityVariable("Hispanic", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Hispanic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Hispanic", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Hispanic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Hispanic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Hispanic", "Nose", "All Noses");
		AddEthnicityVariable("Hispanic", "Ears", "All Ears");
		AddEthnicityVariable("Hispanic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Hispanic", "Skin Colour", "fair_olive_skin");
		AddEthnicityVariable("Indo-Aryan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Indo-Aryan", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Indo-Aryan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Indo-Aryan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Indo-Aryan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Indo-Aryan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Indo-Aryan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Indo-Aryan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Indo-Aryan", "Nose", "All Noses");
		AddEthnicityVariable("Indo-Aryan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Indo-Aryan", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Italic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Italic", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Italic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Italic", "Ears", "All Ears");
		AddEthnicityVariable("Italic", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Italic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Italic", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Italic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Italic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Italic", "Nose", "All Noses");
		AddEthnicityVariable("Italic", "Ears", "All Ears");
		AddEthnicityVariable("Italic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Italic", "Skin Colour", "fair_olive_skin");
		AddEthnicityVariable("Khoisan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Khoisan", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Khoisan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Khoisan", "Ears", "All Ears");
		AddEthnicityVariable("Khoisan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Khoisan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Khoisan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Khoisan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Khoisan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Khoisan", "Nose", "All Noses");
		AddEthnicityVariable("Khoisan", "Ears", "All Ears");
		AddEthnicityVariable("Khoisan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Khoisan", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Korean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Korean", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Korean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Korean", "Ears", "All Ears");
		AddEthnicityVariable("Korean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Korean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Korean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Korean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Korean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Korean", "Nose", "All Noses");
		AddEthnicityVariable("Korean", "Ears", "All Ears");
		AddEthnicityVariable("Korean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Korean", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Melanesian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Melanesian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Melanesian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Melanesian", "Ears", "All Ears");
		AddEthnicityVariable("Melanesian", "Facial Hair Colour", "black_brown_blonde_grey_hair");
		AddEthnicityVariable("Melanesian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Melanesian", "Hair Colour", "black_brown_blonde_grey_hair");
		AddEthnicityVariable("Melanesian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Melanesian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Melanesian", "Nose", "All Noses");
		AddEthnicityVariable("Melanesian", "Ears", "All Ears");
		AddEthnicityVariable("Melanesian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Melanesian", "Skin Colour", "swarthy_skin");
		AddEthnicityVariable("Mesoamerican", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Mesoamerican", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Mesoamerican", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Mesoamerican", "Ears", "All Ears");
		AddEthnicityVariable("Mesoamerican", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mesoamerican", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Mesoamerican", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mesoamerican", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Mesoamerican", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Mesoamerican", "Nose", "All Noses");
		AddEthnicityVariable("Mesoamerican", "Ears", "All Ears");
		AddEthnicityVariable("Mesoamerican", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Mesoamerican", "Skin Colour", "redbrown_skin");
		AddEthnicityVariable("Mongolian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Mongolian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Mongolian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Mongolian", "Ears", "All Ears");
		AddEthnicityVariable("Mongolian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mongolian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Mongolian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mongolian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Mongolian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Mongolian", "Nose", "All Noses");
		AddEthnicityVariable("Mongolian", "Ears", "All Ears");
		AddEthnicityVariable("Mongolian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Mongolian", "Skin Colour", "golden_skin");
		AddEthnicityVariable("North African", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("North African", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("North African", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("North African", "Ears", "All Ears");
		AddEthnicityVariable("North African", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("North African", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("North African", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("North African", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("North African", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("North African", "Nose", "All Noses");
		AddEthnicityVariable("North African", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("North African", "Skin Colour", "swarthy_skin");
		AddEthnicityVariable("Persian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Persian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Persian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Persian", "Ears", "All Ears");
		AddEthnicityVariable("Persian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Persian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Persian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Persian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Persian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Persian", "Nose", "All Noses");
		AddEthnicityVariable("Persian", "Ears", "All Ears");
		AddEthnicityVariable("Persian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Persian", "Skin Colour", "fair_olive_skin");
		AddEthnicityVariable("Polynesian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Polynesian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Polynesian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Polynesian", "Ears", "All Ears");
		AddEthnicityVariable("Polynesian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Polynesian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Polynesian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Polynesian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Polynesian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Polynesian", "Nose", "All Noses");
		AddEthnicityVariable("Polynesian", "Ears", "All Ears");
		AddEthnicityVariable("Polynesian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Polynesian", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Scandanavian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Scandanavian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Scandanavian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Scandanavian", "Ears", "All Ears");
		AddEthnicityVariable("Scandanavian", "Facial Hair Colour", "blonde_red_grey_hair");
		AddEthnicityVariable("Scandanavian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Scandanavian", "Hair Colour", "blonde_red_grey_hair");
		AddEthnicityVariable("Scandanavian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Scandanavian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Scandanavian", "Nose", "All Noses");
		AddEthnicityVariable("Scandanavian", "Ears", "All Ears");
		AddEthnicityVariable("Scandanavian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Scandanavian", "Skin Colour", "fair_skin");
		AddEthnicityVariable("Slavic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Slavic", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Slavic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Slavic", "Ears", "All Ears");
		AddEthnicityVariable("Slavic", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Slavic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Slavic", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Slavic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Slavic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Slavic", "Nose", "All Noses");
		AddEthnicityVariable("Slavic", "Ears", "All Ears");
		AddEthnicityVariable("Slavic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Slavic", "Skin Colour", "fair_olive_skin");
		AddEthnicityVariable("Swahili", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Swahili", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Swahili", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Swahili", "Ears", "All Ears");
		AddEthnicityVariable("Swahili", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Swahili", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Swahili", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Swahili", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Swahili", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Swahili", "Nose", "All Noses");
		AddEthnicityVariable("Swahili", "Ears", "All Ears");
		AddEthnicityVariable("Swahili", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Swahili", "Skin Colour", "dark_skin");
		AddEthnicityVariable("Tai-Kadai", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Tai-Kadai", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Tai-Kadai", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Tai-Kadai", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Tai-Kadai", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Tai-Kadai", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Tai-Kadai", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Tai-Kadai", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Tai-Kadai", "Nose", "All Noses");
		AddEthnicityVariable("Tai-Kadai", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Tai-Kadai", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Tibetan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Tibetan", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Tibetan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Tibetan", "Ears", "All Ears");
		AddEthnicityVariable("Tibetan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Tibetan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Tibetan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Tibetan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Tibetan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Tibetan", "Nose", "All Noses");
		AddEthnicityVariable("Tibetan", "Ears", "All Ears");
		AddEthnicityVariable("Tibetan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Tibetan", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Turkish", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Turkish", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Turkish", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Turkish", "Ears", "All Ears");
		AddEthnicityVariable("Turkish", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Turkish", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Turkish", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Turkish", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Turkish", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Turkish", "Nose", "All Noses");
		AddEthnicityVariable("Turkish", "Ears", "All Ears");
		AddEthnicityVariable("Turkish", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Turkish", "Skin Colour", "olive_skin");
		AddEthnicityVariable("Uyghur", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Uyghur", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Uyghur", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Uyghur", "Ears", "All Ears");
		AddEthnicityVariable("Uyghur", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Uyghur", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Uyghur", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Uyghur", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Uyghur", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Uyghur", "Nose", "All Noses");
		AddEthnicityVariable("Uyghur", "Ears", "All Ears");
		AddEthnicityVariable("Uyghur", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Uyghur", "Skin Colour", "golden_skin");
		AddEthnicityVariable("Yamato", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Yamato", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Yamato", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Yamato", "Ears", "All Ears");
		AddEthnicityVariable("Yamato", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Yamato", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Yamato", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Yamato", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Yamato", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Yamato", "Nose", "All Noses");
		AddEthnicityVariable("Yamato", "Ears", "All Ears");
		AddEthnicityVariable("Yamato", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Yamato", "Skin Colour", "golden_skin");
		_context.SaveChanges();

		AddCulture("Western Upper Class", "Western European",
			"The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
		AddCulture("Western Middle Class", "Western European",
			"The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
		AddCulture("Western Working Class", "Western European",
			"The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
		AddCulture("Western Rural Poor", "Western European",
			"The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

		AddCulture("Soviet Upper Class", "Eastern European",
			"The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
		AddCulture("Soviet Middle Class", "Eastern European",
			"The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
		AddCulture("Soviet Working Class", "Eastern European",
			"The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
		AddCulture("Soviet Peasant Class", "Eastern European",
			"The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

		AddCulture("East Asian Upper Class", "Given and Family",
			"The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
		AddCulture("East Asian Middle Class", "Given and Family",
			"The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
		AddCulture("East Asian Working Class", "Given and Family",
			"The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
		AddCulture("East Asian Rural Poor", "Given and Family",
			"The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");

		AddCulture("Third World Upper Class", "Given and Family",
			"The upper class are those with sufficient means to support themselves and their lifestyle without the need to hold employment. They may have investments, money-making assets or simply enormous cash reserves. The upper class are typically the cultural and political elite.");
		AddCulture("Third World Middle Class", "Given and Family",
			"The middle class are typically well educated and relatively affluent, often making up the bulk of professional workers such as doctors, lawyers, engineers and scientists. They usually have some savings or non-liquid assets, and may be politically active.");
		AddCulture("Third World Working Class", "Given and Family",
			"The working class are those who work for a living in the cities of the world. They are factory workers, construction workers, utility workers and service workers. Their livelihoods are often insecure because they depend on their jobs and have little savings or assets.");
		AddCulture("Third World Rural Poor", "Given and Family",
			"The rural poor are often amongst the poorest people in society, but they have some advantages that the urban poor (or working class) do not. They often own their land and homes, meagre though they might be, and they often have access to supplementary off-market commodities such as food, goods and basic services from their fellow rural poor.");
	}

	public void SeedMedievalHeritage()
	{
	}

	public void SeedRomanHeritage()
	{
		SeedRomanHeritageEthnicities();
		SeedRomanHeritageCultures();
	}

	private void SeedRomanHeritageEthnicities()
	{
		AddEthnicity(_humanRace, "Roman", "Italian", "O-A High Negative", 0, 0,
			description:
			"This ethnicity is for people born in the city of Rome itself; true born children of the eternal city. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Etruscan", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Etruscans are a people in north western Italia just north of Rome, with a proud history and unique language and culture. Etruscans are citizens of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Sabine", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Sabines are an Italic people who live in the Mountains to the east and north of Rome. They have long been citizens of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Oscan", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Oscans are an Italic people who live in the southeastern portion of mainland Italia. Their culture is known for lascivious festivals, games and plays. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Umbrian", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Umbrians are an Italic people in the central portion of Italia, south of Rome. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Samnite", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Samnites are a tribal, pastoral people who long resisted the Romans until conquered by Sulla. They have a distinctive culture, and are known to be very superstituous. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Latin", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Latins are of broadly the same tribal origins as the Romans, but lacking the prestige of being born in the eternal city. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Sardinian", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Sardinians are a largely rural people that live on the island of Sardinia. They have at various times been part of Carthaginian and now Roman empires. They have a distinctive culture and language but are thought of as quite rustic. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Corsican", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Corsicans are a largely rural people that live on the island of Corsica. They have at various times been part of Carthaginian and now Roman empires. They have a distinctive culture and language but are thought of as quite rustic. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Sicilian", "Italian", "O-A High Negative", 0, 0,
			description:
			"The Sicilians are the inhabitants of the province of Sicily. This island has long been a melting pot of Greek, Italian and Carthaginian peoples and cultures and is rich, fertile and very prosperous. They are typically characterised by olive skin, dark hair and dark eyes.");

		AddEthnicity(_humanRace, "Cisalpine Gaul", "Celtic", "Majority O Minor A", subgroup: "Cisalpine Gaul",
			tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Cisalpine gauls are gauls that live on the Italian side of the Alps. They include tribes such as the Ligones, Senones, Boii, Cenomani, Insubres, Lepontii, Taurini and Salassi. Most of these peoples are thoroughly romanised with few living in traditional Celtic manner. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Alpine Gaul", "Celtic", "Majority O Minor A", subgroup: "Cisalpine Gaul",
			tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Alpine gauls live in the Alps, between Italia and Gaul. They include tribes such as the Helvetii and Alobroges. They are a fierce people with a history of rebellion and defiance. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Aquitaine Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
			tempFloor: 0, tempCeiling: 0,
			description:
			"The Aquitanian Gauls are Transalpine gauls from Southwestern Gaul, bordering Iberia. Their lands are not especially fertile but productive silver and gold mines have driven urbanisation and trade in this area. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Celtic Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
			tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Celtic Gauls live in the province of Gallia Lugdunensis, also known as Celtica, in central Gaul. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Belgican Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
			tempFloor: 0, tempCeiling: 0,
			description:
			"The Belgica Gauls live in Belgica, in Northern Gaul. They include tribes like the Treveri, Mediomatrici, Leuci, Sequani and the eponymous Belgae. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Narbonese Gaul", "Celtic", "Majority O Minor A", subgroup: "Transalpine Gaul",
			tempFloor: 0, tempCeiling: 0,
			description:
			"The Narbonese Gauls live in Gallia Narbonensis, the original province of Roman Transalpine Gaul. It had strong influence from a Greek colony Massalia and has been under Roman rule long enough to become fairly romanised. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Iberian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Iberians are a celtic people who live in the east of Iberia. They have long lived alongside Phoenicians, Greeks, Carthaginians and now Romans in their coastal cities. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Lusitanian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Lusitanians are a celtic people who live in the far west of the Iberian peninusla. Their lands are rich in gold and silver but this has been more of a curse than a blessing to their people, as they have fallen under the rule of successive empires who rule them brutally and export their wealth elsewhere. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Gallaecian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Gallacians are a celtic people who live in the northwest of the Iberian peninsular. They were a wealthy tribe and fierce allies of Carthage, but have been much reduced in both wealth and prestige under Roman rule. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Turdetanian", "Celtic", "Majority O Minor A", subgroup: "Iberian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Turdetanians are a celtic people who live in the far south of the Iberian peninsular in the province of Hispania Baetica. Roman historian Strabo described them as the most civilised of all the peoples of Iberia, and they had long lived in an ordered society governed by written laws, with much cultural contact with the Greeks and Carthaginians. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Brittanic", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Brittanians are a celtic people who live in the province of Brittania. They are culturally close to to the Gauls but have a distinct language and religious structure. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Hibernian", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Hibernians are a celtic people who live on the island of Hibernia, to the west of Brittania. While they have contact with the Romans, they are beyond their Imperium. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");
		AddEthnicity(_humanRace, "Caledonian", "Celtic", "Majority O Minor A", subgroup: "Insular", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Caledonians are a celtic people who belong to a tribal confederation north of Brittania that has come together to resist Roman expansion. They are a fierce and proud people and are considered by the Romans to be too warlike to occupy directly, so far from Rome. They are typically characterised by fair skin, fair hair (particularly red or brown hair) and light eyes.");

		AddEthnicity(_humanRace, "Ingvaeon", "Germanic", "O-A High Negative", 0, 0,
			description:
			"The Ingvaeones are a West Germanic cultural group living in the Northern Germania along the North Sea coast in the areas of Jutland, Holstein, and Frisia. Tribes in this area include the Angles, Frisii, Chauci, Saxons, and Jutes. They are typically characteristised by fair skin, fair hair and light eyes.");
		AddEthnicity(_humanRace, "Istvaeon", "Germanic", "O-A High Negative", 0, 0,
			description:
			"The Istvaeones are a West Germanic cultural group that is defined more by what they are not than what they are - generally speaking they are any group that is not Ingvaeones or Irminon. They occupy the eastern bank of the Rhine river in the more northerly parts of Germania. They are typically characteristised by fair skin, fair hair and light eyes.");
		AddEthnicity(_humanRace, "Irminon", "Germanic", "O-A High Negative", 0, 0,
			description:
			"The Irminon are a West Germanic cultural group that lives in the interior of Germania, around the Elbe river. Notable tribes include the Chatti, Cherusci and Hermunduri They are typically characteristised by fair skin, fair hair and light eyes.");
		AddEthnicity(_humanRace, "Suebi", "Germanic", "O-A High Negative", 0, 0,
			description:
			"The Suebi are a West Germanic cultural group that lives on the farthest side of Germania from the Rhine, and included such tribes as the Marcomanni, Semnones, Langobards and Angles. They are typically characteristised by fair skin, fair hair and light eyes.");

		AddEthnicity(_humanRace, "Illyrian", "Illyrian", "O-A High Negative", 0, 0,
			description:
			"The Illyrians are a broad group of many different tribes in the province of Illyricum. They generally identify as their particular tribal grouping, but the Greeks and Romans both refer to them all by the grouping of Illyrian. They have been under Roman rule for a long time, but prior to Roman subjugation they had a reputation as excellent pirates and mercenaries. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Pannonian", "Illyrian", "O-A High Negative", 0, 0,
			description:
			"The Pannonians are an admixture of Illyrians and Celts, forming a distinct tribal identity. Pannonia is north of Dalmatia and borders the Danube, and so is an important frontier of the Roman empire. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Liburnian", "Illyrian", "O-A High Negative", 0, 0,
			description:
			"The Liburnians occupy a small portion of the northwestern coast of Illyrium in the Adriatic sea. They are a strong seafaring peoples and maintain large fleets of fishing and trading vessels, as well as being major contributors to Roman naval efforts. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Venetian", "Illyrian", "O-A High Negative", 0, 0,
			description:
			"The Venetians are an Illyrian-related tribe that occupy the western and northern coasts of the Adriatic north of Italia. They have been under Roman rule for a long time and are mostly latinised in the urban areas, but are still most famous for their fabulous horses. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Dacian", "Thracian", "O-A High Negative", 0, 0,
			description:
			"The Dacians, also called the Getae by the Greeks, are a barbarian people that live in and around the Carpathain mountains. In recent memory they had a kingdom that occupied most of Illyricum, but it has long since fallen. They are typically characterised by fair skin, fair hair and either light or dark eyes.");
		AddEthnicity(_humanRace, "Thracian", "Thracian", "O-A High Negative", 0, 0,
			description:
			"The Thracians are a warlike barbarian people found both to the North of Greece and Macedon and on Anatolia as well. Their culture has a strong tradition of tattooing amongst both men and women. They are also known for being unsophisticated but having excellent music. They are typically characterised by fair skin, fair hair and either light or dark eyes.");

		AddEthnicity(_humanRace, "Scythian", "Scythian", "Majority O Minor A", 0, 0,
			description:
			"The Scythians are a renowned equestrian people originally from central asia who migrated into the Pontic Steppes and all around the Black Sea. They are divided into both sedentary populations of hellenised settlements, mostly along the silk road, and the larger bulk of patriarchal, nomadic pastoralists. The Scythians ride horses without saddles or stirrups. They are typically characterised by olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Sarmatian", "Scythian", "Majority O Minor A", 0, 0,
			description:
			"The Sarmatians are a large confederation of broadly Scythian tribes that nonetheless have a distinct cultural identity and exist in conflict with the traditional Scythian rulers. The Sarmatians are a more egalitarian society on gender grounds than the Scythians in general and women have more freedom and rights. The Sarmatians also ride horses with a saddle and stirrups. They are typically characterised by olive skin, dark hair and dark eyes.");

		AddEthnicity(_humanRace, "Achaean", "Hellenic", "O-A High Negative", subgroup: "Central Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Achaeans were one of the four major tribes into which the Greeks divided themselves (along with the Aeolians, Ionians and Dorians). According to the foundation myth formalized by Hesiod, their name comes from Achaeus, the mythical founder of the Achaean tribe, who was supposedly one of the sons of Xuthus, and brother of Ion, the founder of the Ionian tribe. Xuthus was in turn the son of Hellen, the mythical patriarch of the Greek (Hellenic) nation.\n\nHistorically, the members of the Achaean tribe inhabited the region of Achaea in the northern Peloponnese. The Achaeans played an active role in the Greek colonization of southern Italy, founding the city of Kroton in 710 BC. The city was to gain fame later as the place where the Pythagorean School was founded. Unlike the other major tribes (Ionians, Dorians and Aeolians), the Achaeans did not have a separate dialect in the Classical period, instead using a form of Doric.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Aeolian", "Hellenic", "O-A High Negative", subgroup: "Central Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Aeolians were one of the four major tribes in which Greeks divided themselves in the ancient period (along with the Achaeans, Dorians and Ionians). Their name mythologically derives from Aeolus, the mythical ancestor of the Aeolians and son of Hellen, the mythical patriarch of the Greek nation. The dialect of ancient Greek they spoke is referred to as Aeolic.\n\nOriginating in Thessaly, a part of which was called Aeolis, the Aeolians often appear as the most numerous amongst the other Hellenic tribes of early times. The Boeotians, a subgroup of the Aeolians, were driven from Thessaly by the Thessalians and moved their location to Boeotia. Aeolian peoples were spread in many other parts of Greece such as Aetolia, Locris, Corinth, Elis and Messinia. During the Dorian invasion, Aeolians from Thessaly fled across the Aegean Sea to the island of Lesbos and the region of Aeolis, called as such after them, in Asia Minor.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Ionian", "Hellenic", "O-A High Negative", subgroup: "Eastern Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Ionians were one of the four major tribes that the Greeks considered themselves to be divided into during the ancient period; the other three being the Dorians, Aeolians, and Achaeans. The Ionian dialect was one of the three major linguistic divisions of the Hellenic world, together with the Dorian and Aeolian dialects.\n\nAccording to the foundation myth the Ionians were named after Ion, son of Xuthus, who lived in the north Peloponnesian region of Aigialeia. When the Dorians invaded the Peloponnese they expelled the Achaeans from the Argolid and Lacedaemonia. The displaced Achaeans moved into Aigialeia (thereafter known as Achaea), in turn expelling the Ionians from Aigialeia. The Ionians moved to Attica and mingled with the local population of Attica, and many later emigrated to the coast of Asia Minor founding the historical region of Ionia.\n\nUnlike the austere and militaristic Dorians, the Ionians are renowned for their love of philosophy, art, democracy, and pleasure – Ionian traits that were most famously expressed by the Athenians.\n\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Dorian", "Hellenic", "O-A High Negative", subgroup: "Western Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Dorians were one of the four major ethnic groups into which the Hellenes (or Greeks) of Classical Greece divided themselves (along with the Aeolians, Achaeans, and Ionians). They are almost always referred to as just \"the Dorians\", as they are called in the earliest literary mention of them in the Odyssey, where they already can be found inhabiting the island of Crete.\\n\\nThey were diverse in way of life and social organization, varying from the populous trade center of the city of Corinth, known for its ornate style in art and architecture, to the isolationist, military state of Sparta. And yet, all Hellenes knew which localities were Dorian, and which were not. Dorian states at war could more likely, but not always, count on the assistance of other Dorian states. Dorians were distinguished by the Doric Greek dialect and by characteristic social and historical traditions.\\n\\nThey are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Italiote", "Hellenic", "O-A High Negative", subgroup: "Italian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"Italiote Greeks are those who live in the Greek-ethnicity city states in former Magna Graecia, Sicilia, and northern Italia. Once powerful city states in their own right, they have long since been subsumed into the Roman Imperium but they do retain a distinct cultural identity. They are mostly of Dorian Greek descent. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Cypriot", "Hellenic", "O-A High Negative", subgroup: "Aegean Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"Cypriots are natives of the island of Cyprus. They are descended from an Achaean diaspora. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Cretan", "Hellenic", "O-A High Negative", subgroup: "Aegean Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"Cretans are natives of the island of Crete. They are a mixture of a pre-hellenic Minoan culture as well as later Greek immigrants to the island. They are a proud people with the cultural memory of two great empires - the Minoans and the Mycenaens. Now subjects of the Roman empire, they have nonetheless flourished as a centre of eastern Mediterranean commerce. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Macedonian", "Hellenic", "O-A High Negative", subgroup: "Western Greek", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Macedonians are a branch of Dorian Greeks that nonetheless have a distinct cultural identity, most notably because of their association with Alexander the Great. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Phrygian", "Hellenic", "O-A High Negative", subgroup: "Asian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Phrygians are Hellenised peoples who live in central Anatolia, east of Greece proper. The Trojans were a tribe of Phrygians. These people spent a long period of time under Persian rule, before being conquered by Alexander the Great, then the Seleucids, then most recently Rome. They are typically characterised by fair to olive skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Anatolian", "Hellenic", "O-A High Negative", subgroup: "Asian", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Anatolians are a broad grouping of Hellenised peoples in coastal and eastern Anatolia. They descend from a variety of ancient kingdoms but now mostly draw their identity from the city states to which they belong. They are typically characterised by fair to olive skin, dark hair and dark eyes.");

		AddEthnicity(_humanRace, "Punic", "Punic", "Majority O", 0, 0,
			description:
			"The term Punic is used to refer to those Phoenician people of the western Mediterranean who descend from Carthage proper, Roman Provincia Africanus. Until recently, their people were the preeminent empire in the Mediterranean and an ancient enemy of Rome. Now, though much reduced, they still play a prominent role in the empire as traders and sailors. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Libyan", "Punic", "Majority O", 0, 0,
			description:
			"The Libyans are a Punic people in the regions of North Africa to the Southeast of Carthage, including Tripolitania and Cyrenaica. They are used to intermixing with Greek city states, although they maintain a seperate Punic identity. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Mauretanian", "Punic", "Majority O", 0, 0,
			description:
			"The Mauretanians are a Punic people in most of the western parts of North Africa, to the west of Numidia, and sometimes including the southern parts of Hispania. They have a great deal of cultural contact with their nomadic cousins in the more southerly regions and less contact with Greek city states. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Numidian", "Punic", "Majority O", 0, 0,
			description:
			"The Numidians are a people in North Africa to the west of Carthage. They were a client state and ally of Carthage but during the Second Punic War they managed to unify into an independent Kingdom. They then became a client state of Rome until they were later annexed. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Egyptian", "Egyptian", "Majority O", 0, 0,
			description:
			"The Egyptians are the native peoples of the land of Egypt; once a powerful empire that was one of the world's pre-eminent empires for thousands of years but now ruled by a Hellenised ruling class called the Ptolemaics. They are characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Berber", "North African", "Majority O", 0, 0,
			description:
			"The Berbers are the tribal, nomadic peoples of the interior of the province of Mauretania. They are related to most of the more civilised, sedentary Punic peoples on the coast but maintain a distinct cultural identity, even under Roman rule. They are characterised by olive or light brown skin, dark hair and light eyes.");

		AddEthnicity(_humanRace, "Arabian", "Levantine", "Majority O Minor A", 0, 0,
			description:
			"The Arabians are a broad grouping of both settled and nomadic peoples that live on the Arabian peninsular and in the Kingdom of Nabatea. Their peninsular is the gateway to the Indes and an enormous volume of trade passes through their lands, which are exceptionally wealthy on account of the commerce. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Syrian", "Levantine", "Majority O Minor A", 0, 0,
			description:
			"The Syrians are the Phoenician inhabitants of the Roman province of Syria, which sits south and east of Asia Minor and north and east of Egypt. The province is fabulously wealthy, with the trade of east and west coming through here. The Syrians are only recently additions to the Roman Empire, conquered by Pompey Magnus. The Syrian people are known for (to the Romans) exotic religious and cultural practices, and there is a great deal of fascination with the 'oriental'. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Judean", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Judeans are a broad ethnic group including Judeans and Israelites. They have a strong sense of distinct cultural identity from their neighbours and follow a monotheistic god called Yahweh. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Samaritan", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Samaritans are the people of the Kingdom of Samaria, akin to the Judeans but distinct from them. Unlike their neighbours they had gone through a period of Hellenisation after being Seleucid clients, though they retained their monotheistic religion. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");
		AddEthnicity(_humanRace, "Gallilean", "Levantine", "Majority O Minor A", subgroup: "Semitic", tempFloor: 0,
			tempCeiling: 0,
			description:
			"The Gallileans are a Semitic people to the north of the Israelites. Of the various Semitic tribes, they are the most influenced by the Phoenician culture and also tend to be the most tolerant of other religions. They are typically characterised by olive or light brown skin, dark hair and dark eyes.");


		AddEthnicityVariable("Cisalpine Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Cisalpine Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Cisalpine Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Cisalpine Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Cisalpine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Cisalpine Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Cisalpine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Cisalpine Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Cisalpine Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Cisalpine Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Cisalpine Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Cisalpine Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Alpine Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Alpine Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Alpine Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Alpine Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Alpine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Alpine Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Alpine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Alpine Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Alpine Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Alpine Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Alpine Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Alpine Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Aquitaine Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Aquitaine Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Aquitaine Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Aquitaine Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Aquitaine Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Aquitaine Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Aquitaine Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Aquitaine Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Aquitaine Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Aquitaine Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Aquitaine Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Aquitaine Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Celtic Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Celtic Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Celtic Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Celtic Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Celtic Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Celtic Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Celtic Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Celtic Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Celtic Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Celtic Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Celtic Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Celtic Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Belgican Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Belgican Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Belgican Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Belgican Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Belgican Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Belgican Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Belgican Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Belgican Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Belgican Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Belgican Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Belgican Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Belgican Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Narbonese Gaul", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Narbonese Gaul", "Ears", "All Ears");
		AddEthnicityVariable("Narbonese Gaul", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Narbonese Gaul", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Narbonese Gaul", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Narbonese Gaul", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Narbonese Gaul", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Narbonese Gaul", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Narbonese Gaul", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Narbonese Gaul", "Nose", "All Noses");
		AddEthnicityVariable("Narbonese Gaul", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Narbonese Gaul", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Iberian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Iberian", "Ears", "All Ears");
		AddEthnicityVariable("Iberian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Iberian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Iberian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Iberian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Iberian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Iberian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Iberian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Iberian", "Nose", "All Noses");
		AddEthnicityVariable("Iberian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Iberian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Lusitanian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Lusitanian", "Ears", "All Ears");
		AddEthnicityVariable("Lusitanian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Lusitanian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Lusitanian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Lusitanian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Lusitanian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Lusitanian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Lusitanian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Lusitanian", "Nose", "All Noses");
		AddEthnicityVariable("Lusitanian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Lusitanian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Gallaecian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Gallaecian", "Ears", "All Ears");
		AddEthnicityVariable("Gallaecian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Gallaecian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Gallaecian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Gallaecian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Gallaecian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Gallaecian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Gallaecian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Gallaecian", "Nose", "All Noses");
		AddEthnicityVariable("Gallaecian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Gallaecian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Turdetanian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Turdetanian", "Ears", "All Ears");
		AddEthnicityVariable("Turdetanian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Turdetanian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Turdetanian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Turdetanian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Turdetanian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Turdetanian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Turdetanian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Turdetanian", "Nose", "All Noses");
		AddEthnicityVariable("Turdetanian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Turdetanian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Brittanic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Brittanic", "Ears", "All Ears");
		AddEthnicityVariable("Brittanic", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Brittanic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Brittanic", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Brittanic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Brittanic", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Brittanic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Brittanic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Brittanic", "Nose", "All Noses");
		AddEthnicityVariable("Brittanic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Brittanic", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Hibernian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Hibernian", "Ears", "All Ears");
		AddEthnicityVariable("Hibernian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Hibernian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Hibernian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Hibernian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Hibernian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Hibernian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Hibernian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Hibernian", "Nose", "All Noses");
		AddEthnicityVariable("Hibernian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Hibernian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Caledonian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Caledonian", "Ears", "All Ears");
		AddEthnicityVariable("Caledonian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Caledonian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Caledonian", "Facial Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Caledonian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Caledonian", "Hair Colour", "brown_blonde_red_grey_hair");
		AddEthnicityVariable("Caledonian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Caledonian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Caledonian", "Nose", "All Noses");
		AddEthnicityVariable("Caledonian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Caledonian", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Roman", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Roman", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Roman", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Roman", "Ears", "All Ears");
		AddEthnicityVariable("Roman", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Roman", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Roman", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Roman", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Roman", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Roman", "Nose", "All Noses");
		AddEthnicityVariable("Roman", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Roman", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Etruscan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Etruscan", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Etruscan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Etruscan", "Ears", "All Ears");
		AddEthnicityVariable("Etruscan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Etruscan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Etruscan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Etruscan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Etruscan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Etruscan", "Nose", "All Noses");
		AddEthnicityVariable("Etruscan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Etruscan", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Sabine", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Sabine", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Sabine", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Sabine", "Ears", "All Ears");
		AddEthnicityVariable("Sabine", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sabine", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Sabine", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sabine", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Sabine", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Sabine", "Nose", "All Noses");
		AddEthnicityVariable("Sabine", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Sabine", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Oscan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Oscan", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Oscan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Oscan", "Ears", "All Ears");
		AddEthnicityVariable("Oscan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Oscan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Oscan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Oscan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Oscan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Oscan", "Nose", "All Noses");
		AddEthnicityVariable("Oscan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Oscan", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Umbrian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Umbrian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Umbrian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Umbrian", "Ears", "All Ears");
		AddEthnicityVariable("Umbrian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Umbrian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Umbrian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Umbrian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Umbrian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Umbrian", "Nose", "All Noses");
		AddEthnicityVariable("Umbrian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Umbrian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Samnite", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Samnite", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Samnite", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Samnite", "Ears", "All Ears");
		AddEthnicityVariable("Samnite", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Samnite", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Samnite", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Samnite", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Samnite", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Samnite", "Nose", "All Noses");
		AddEthnicityVariable("Samnite", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Samnite", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Latin", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Latin", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Latin", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Latin", "Ears", "All Ears");
		AddEthnicityVariable("Latin", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Latin", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Latin", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Latin", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Latin", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Latin", "Nose", "All Noses");
		AddEthnicityVariable("Latin", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Latin", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Sardinian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Sardinian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Sardinian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Sardinian", "Ears", "All Ears");
		AddEthnicityVariable("Sardinian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sardinian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Sardinian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sardinian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Sardinian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Sardinian", "Nose", "All Noses");
		AddEthnicityVariable("Sardinian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Sardinian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Corsican", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Corsican", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Corsican", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Corsican", "Ears", "All Ears");
		AddEthnicityVariable("Corsican", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Corsican", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Corsican", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Corsican", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Corsican", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Corsican", "Nose", "All Noses");
		AddEthnicityVariable("Corsican", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Corsican", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Sicilian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Sicilian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Sicilian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Sicilian", "Ears", "All Ears");
		AddEthnicityVariable("Sicilian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sicilian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Sicilian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sicilian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Sicilian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Sicilian", "Nose", "All Noses");
		AddEthnicityVariable("Sicilian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Sicilian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Ingvaeon", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Ingvaeon", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Ingvaeon", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Ingvaeon", "Ears", "All Ears");
		AddEthnicityVariable("Ingvaeon", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Ingvaeon", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Ingvaeon", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Ingvaeon", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Ingvaeon", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Ingvaeon", "Nose", "All Noses");
		AddEthnicityVariable("Ingvaeon", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Ingvaeon", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Istvaeon", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Istvaeon", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Istvaeon", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Istvaeon", "Ears", "All Ears");
		AddEthnicityVariable("Istvaeon", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Istvaeon", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Istvaeon", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Istvaeon", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Istvaeon", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Istvaeon", "Nose", "All Noses");
		AddEthnicityVariable("Istvaeon", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Istvaeon", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Irminon", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Irminon", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Irminon", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Irminon", "Ears", "All Ears");
		AddEthnicityVariable("Irminon", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Irminon", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Irminon", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Irminon", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Irminon", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Irminon", "Nose", "All Noses");
		AddEthnicityVariable("Irminon", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Irminon", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Suebi", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Suebi", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Suebi", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Suebi", "Ears", "All Ears");
		AddEthnicityVariable("Suebi", "Facial Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Suebi", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Suebi", "Hair Colour", "brown_blonde_grey_hair");
		AddEthnicityVariable("Suebi", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Suebi", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Suebi", "Nose", "All Noses");
		AddEthnicityVariable("Suebi", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Suebi", "Skin Colour", "fair_skin");

		AddEthnicityVariable("Achaean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Achaean", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Achaean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Achaean", "Ears", "All Ears");
		AddEthnicityVariable("Achaean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Achaean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Achaean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Achaean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Achaean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Achaean", "Nose", "All Noses");
		AddEthnicityVariable("Achaean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Achaean", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Aeolian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Aeolian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Aeolian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Aeolian", "Ears", "All Ears");
		AddEthnicityVariable("Aeolian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Aeolian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Aeolian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Aeolian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Aeolian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Aeolian", "Nose", "All Noses");
		AddEthnicityVariable("Aeolian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Aeolian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Ionian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Ionian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Ionian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Ionian", "Ears", "All Ears");
		AddEthnicityVariable("Ionian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Ionian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Ionian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Ionian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Ionian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Ionian", "Nose", "All Noses");
		AddEthnicityVariable("Ionian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Ionian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Dorian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Dorian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Dorian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Dorian", "Ears", "All Ears");
		AddEthnicityVariable("Dorian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dorian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Dorian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dorian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Dorian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Dorian", "Nose", "All Noses");
		AddEthnicityVariable("Dorian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Dorian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Italiote", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Italiote", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Italiote", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Italiote", "Ears", "All Ears");
		AddEthnicityVariable("Italiote", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Italiote", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Italiote", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Italiote", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Italiote", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Italiote", "Nose", "All Noses");
		AddEthnicityVariable("Italiote", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Italiote", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Cypriot", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Cypriot", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Cypriot", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Cypriot", "Ears", "All Ears");
		AddEthnicityVariable("Cypriot", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Cypriot", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Cypriot", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Cypriot", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Cypriot", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Cypriot", "Nose", "All Noses");
		AddEthnicityVariable("Cypriot", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Cypriot", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Cretan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Cretan", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Cretan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Cretan", "Ears", "All Ears");
		AddEthnicityVariable("Cretan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Cretan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Cretan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Cretan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Cretan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Cretan", "Nose", "All Noses");
		AddEthnicityVariable("Cretan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Cretan", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Macedonian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Macedonian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Macedonian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Macedonian", "Ears", "All Ears");
		AddEthnicityVariable("Macedonian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Macedonian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Macedonian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Macedonian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Macedonian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Macedonian", "Nose", "All Noses");
		AddEthnicityVariable("Macedonian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Macedonian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Phrygian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Phrygian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Phrygian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Phrygian", "Ears", "All Ears");
		AddEthnicityVariable("Phrygian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Phrygian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Phrygian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Phrygian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Phrygian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Phrygian", "Nose", "All Noses");
		AddEthnicityVariable("Phrygian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Phrygian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Anatolian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Anatolian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Anatolian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Anatolian", "Ears", "All Ears");
		AddEthnicityVariable("Anatolian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Anatolian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Anatolian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Anatolian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Anatolian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Anatolian", "Nose", "All Noses");
		AddEthnicityVariable("Anatolian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Anatolian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Illyrian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Illyrian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Illyrian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Illyrian", "Ears", "All Ears");
		AddEthnicityVariable("Illyrian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Illyrian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Illyrian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Illyrian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Illyrian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Illyrian", "Nose", "All Noses");
		AddEthnicityVariable("Illyrian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Illyrian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Pannonian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Pannonian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Pannonian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Pannonian", "Ears", "All Ears");
		AddEthnicityVariable("Pannonian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Pannonian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Pannonian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Pannonian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Pannonian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Pannonian", "Nose", "All Noses");
		AddEthnicityVariable("Pannonian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Pannonian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Liburnian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Liburnian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Liburnian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Liburnian", "Ears", "All Ears");
		AddEthnicityVariable("Liburnian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Liburnian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Liburnian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Liburnian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Liburnian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Liburnian", "Nose", "All Noses");
		AddEthnicityVariable("Liburnian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Liburnian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Venetian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Venetian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Venetian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Venetian", "Ears", "All Ears");
		AddEthnicityVariable("Venetian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Venetian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Venetian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Venetian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Venetian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Venetian", "Nose", "All Noses");
		AddEthnicityVariable("Venetian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Venetian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Dacian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Dacian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Dacian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Dacian", "Ears", "All Ears");
		AddEthnicityVariable("Dacian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dacian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Dacian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Dacian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Dacian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Dacian", "Nose", "All Noses");
		AddEthnicityVariable("Dacian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Dacian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Thracian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Thracian", "Eye Colour", "brown_green_blue_eyes");
		AddEthnicityVariable("Thracian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Thracian", "Ears", "All Ears");
		AddEthnicityVariable("Thracian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Thracian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Thracian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Thracian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Thracian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Thracian", "Nose", "All Noses");
		AddEthnicityVariable("Thracian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Thracian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Punic", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Punic", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Punic", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Punic", "Ears", "All Ears");
		AddEthnicityVariable("Punic", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Punic", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Punic", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Punic", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Punic", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Punic", "Nose", "All Noses");
		AddEthnicityVariable("Punic", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Punic", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Libyan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Libyan", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Libyan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Libyan", "Ears", "All Ears");
		AddEthnicityVariable("Libyan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Libyan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Libyan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Libyan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Libyan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Libyan", "Nose", "All Noses");
		AddEthnicityVariable("Libyan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Libyan", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Mauretanian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Mauretanian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Mauretanian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Mauretanian", "Ears", "All Ears");
		AddEthnicityVariable("Mauretanian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mauretanian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Mauretanian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Mauretanian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Mauretanian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Mauretanian", "Nose", "All Noses");
		AddEthnicityVariable("Mauretanian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Mauretanian", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Numidian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Numidian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Numidian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Numidian", "Ears", "All Ears");
		AddEthnicityVariable("Numidian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Numidian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Numidian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Numidian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Numidian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Numidian", "Nose", "All Noses");
		AddEthnicityVariable("Numidian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Numidian", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Egyptian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Egyptian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Egyptian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Egyptian", "Ears", "All Ears");
		AddEthnicityVariable("Egyptian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Egyptian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Egyptian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Egyptian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Egyptian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Egyptian", "Nose", "All Noses");
		AddEthnicityVariable("Egyptian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Egyptian", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Berber", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Berber", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Berber", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Berber", "Ears", "All Ears");
		AddEthnicityVariable("Berber", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Berber", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Berber", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Berber", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Berber", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Berber", "Nose", "All Noses");
		AddEthnicityVariable("Berber", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Berber", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Scythian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Scythian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Scythian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Scythian", "Ears", "All Ears");
		AddEthnicityVariable("Scythian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Scythian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Scythian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Scythian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Scythian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Scythian", "Nose", "All Noses");
		AddEthnicityVariable("Scythian", "Ears", "All Ears");
		AddEthnicityVariable("Scythian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Scythian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Sarmatian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Sarmatian", "Eye Colour", "brown_green_eyes");
		AddEthnicityVariable("Sarmatian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Sarmatian", "Ears", "All Ears");
		AddEthnicityVariable("Sarmatian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sarmatian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Sarmatian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Sarmatian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Sarmatian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Sarmatian", "Nose", "All Noses");
		AddEthnicityVariable("Sarmatian", "Ears", "All Ears");
		AddEthnicityVariable("Sarmatian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Sarmatian", "Skin Colour", "fair_olive_skin");

		AddEthnicityVariable("Arabian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Arabian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Arabian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Arabian", "Ears", "All Ears");
		AddEthnicityVariable("Arabian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Arabian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Arabian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Arabian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Arabian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Arabian", "Nose", "All Noses");
		AddEthnicityVariable("Arabian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Arabian", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Syrian", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Syrian", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Syrian", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Syrian", "Ears", "All Ears");
		AddEthnicityVariable("Syrian", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Syrian", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Syrian", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Syrian", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Syrian", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Syrian", "Nose", "All Noses");
		AddEthnicityVariable("Syrian", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Syrian", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Judean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Judean", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Judean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Judean", "Ears", "All Ears");
		AddEthnicityVariable("Judean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Judean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Judean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Judean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Judean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Judean", "Nose", "All Noses");
		AddEthnicityVariable("Judean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Judean", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Samaritan", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Samaritan", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Samaritan", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Samaritan", "Ears", "All Ears");
		AddEthnicityVariable("Samaritan", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Samaritan", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Samaritan", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Samaritan", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Samaritan", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Samaritan", "Nose", "All Noses");
		AddEthnicityVariable("Samaritan", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Samaritan", "Skin Colour", "swarthy_skin");

		AddEthnicityVariable("Gallilean", "Distinctive Feature", "All Distinctive Features");
		AddEthnicityVariable("Gallilean", "Eye Colour", "brown_blue_eyes");
		AddEthnicityVariable("Gallilean", "Eye Shape", "All Eye Shapes");
		AddEthnicityVariable("Gallilean", "Ears", "All Ears");
		AddEthnicityVariable("Gallilean", "Facial Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Gallilean", "Facial Hair Style", "All Facial Hair Styles");
		AddEthnicityVariable("Gallilean", "Hair Colour", "black_brown_grey_hair");
		AddEthnicityVariable("Gallilean", "Hair Style", "All Hair Styles");
		AddEthnicityVariable("Gallilean", "Humanoid Frame", "All Frames");
		AddEthnicityVariable("Gallilean", "Nose", "All Noses");
		AddEthnicityVariable("Gallilean", "Person Word", "Weighted Person Words");
		AddEthnicityVariable("Gallilean", "Skin Colour", "swarthy_skin");
	}

	private void SeedRomanHeritageCultures()
	{
		AddCulture("Patrician", "Roman",
			@"Patricians are those Romans who are descended from one of the original 100 ""Fathers"" (Patres) who were founding members of the first senate of Rome, appointed by Romulus himself.

These Romans enjoy privileges that cannot be bought; the right to hold certain high offices like Consul and Censor, as well as membership in one of Rome's many priesthoods. Depending on the era, Plebians (non-Patrician citizens) have the right to hold some of these offices but as Patricians hold a near-monopoly on power in the Roman Senate, they do tend to keep all but the most prominent Plebians out of power.

The children of a marriage between a Patrician and a Plebian are considered Patricians; accordingly, there are strong social taboos and even laws against the intermarriage of the classes. Nonetheless, Patrician families generally have the power to make such things happen if it is their desire to do so.");
		AddCulture("Roman-Born Citizen", "Roman",
			@"Those born within the territory of the eternal city but not descending from the Patrician class are the Roman-Born Plebians. While not as privileged as the Patricians, the Plebians have significant rights of their own and even many powers and privileges that non-Romans cannot earn by any means.

Plebians have the right to vote in the Tribal assemblies and also elect Tribunes, who have the power of veto in the Senate. They are also entitled to a dole that takes care of most of their daily needs - depending on the era, the dole might be grain, bread, pork, olive oil and wine.

Plebians can be elected into most of the lower magistrate positions on the Cursus Honorum, and are also entitled to join the Roman Legions.");
		AddCulture("Latin Ally", "Roman",
			@"The Latin allies are other Latin-speaking Itallic peoples who are not considered part of Rome proper. This includes many prominent cities like Neapolis, Asculum, Mediolanium, Ariminium and Capua, and all the subject tribal groupings like Samnites, Oscans, Umbrians, Etruscans, Sabines, Sardinians, and Siccilians.

By the late republic these peoples are considered citizens of Rome, although they generally do not participate in the governance of the Imperium as a whole - only rarely are their citizens allowed to join the Cursus Honorum or the Senatorial classes.

Still, Latin Allies enjoy many privileges that others do not - for example, Roman Citizens do not pay many taxes, and generally have immunity from capital punishment (being able to choose exile instead).");
		AddCulture("Provincial Elite", "Roman",
			@"The Roman Imperium is a sprawling, multi-ethnic empire of enormous scope, and although the Romans certainly reserve all the best positions such as governorships and other magistrates for themselves, there is still a great need for the cooperation, financial support and organisation of local elites.

These local elites are typically given a form of Roman Citizenship and Romanise to some extent, but they retain important cultural ties with the local population. They can thus be relied upon to conduct important state business of the governor's behalf - a most profitable endeavour.

Provincial elite walk a delicate balance between pleasing their ever-changing retinue of Roman masters and keeping the locals from revolting.");
		AddCulture("Romanised Barbarian", "Roman",
			@"Within the borders of the Roman Empire, as well as in the neighbouring client kingdoms, there is a great tendency for people to ""Romanise"", which means to adopt the Roman language, laws and way of life. The Romans themselves greatly encourage this process, and many Romanised Barbarians find increased opportunities for economic advancement and participation in the Empire.

There is a certain stigma associated with Romanising amongst the more traditional elements of the Barbarian society and they will often face some ostracism from their un-Romanised kin and exclusion from traditional cultural institutions - but this is the point.

Romanised Barbarians adopt Roman names and depending on the era they may be able to achieve some form of citizenship.");
		AddCulture("Unassimilated Gaul", "Gaulish Male", "Gaulish Female", @"");
		AddCulture("Unassimilated Punic", "Phoenician", @"");
		AddCulture("Unassimilated German", "Germanic Male", "Germanic Female", @"");
		AddCulture("Hellenic", "Hellenic", @"");
		AddCulture("Syrian", "Hellenic", @"");
		AddCulture("Jewish", "Jewish Male", "Jewish Female", @"");
		AddCulture("Freed Slave", "Roman", @"");
		AddCulture("Slave", "Slave", @"");
		
	}

	private (Liquid BloodLiquid, Liquid SweatLiquid, Material
		DriedBlood, Material DriedSweat) CreateBloodAndSweat(string racialDescriptor)
	{
		var driedBlood = new Material
		{
			Name = $"Dried {racialDescriptor} Blood",
			MaterialDescription = "dried blood",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 4,
			ResidueDesc = "It is covered in {0}dried blood",
			ResidueColour = "red",
			Absorbency = 0
		};
		_context.Materials.Add(driedBlood);
		var blood = new Liquid
		{
			Name = $"{racialDescriptor} Blood",
			Description = "blood",
			LongDescription = "a virtually opaque dark red fluid",
			TasteText = "It has a sharply metallic, umami taste",
			VagueTasteText = "It has a metallic taste",
			SmellText = "It has a metallic, coppery smell",
			VagueSmellText = "It has a faintly metallic smell",
			TasteIntensity = 200,
			SmellIntensity = 10,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.8,
			DrinkSatiatedHoursPerLitre = 6,
			FoodSatiatedHoursPerLitre = 4,
			CaloriesPerLitre = 800,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "bold red",
			DampDescription = "It is damp with blood",
			WetDescription = "It is wet with blood",
			DrenchedDescription = "It is drenched with blood",
			DampShortDescription = "(blood damp)",
			WetShortDescription = "(bloody)",
			DrenchedShortDescription = "(blood drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedBlood
		};
		_context.Liquids.Add(blood);

		var driedSweat = new Material
		{
			Name = $"Dried {racialDescriptor} Sweat",
			MaterialDescription = "dried sweat",
			Density = 1520,
			Organic = true,
			Type = 0,
			BehaviourType = 19,
			ThermalConductivity = 0.2,
			ElectricalConductivity = 0.0001,
			SpecificHeatCapacity = 420,
			IgnitionPoint = 555.3722,
			HeatDamagePoint = 412.0389,
			ImpactFracture = 1000,
			ImpactYield = 1000,
			ImpactStrainAtYield = 2,
			ShearFracture = 1000,
			ShearYield = 1000,
			ShearStrainAtYield = 2,
			YoungsModulus = 0.1,
			SolventId = 1,
			SolventVolumeRatio = 3,
			ResidueDesc = "It is covered in {0}dried sweat",
			ResidueColour = "yellow",
			Absorbency = 0
		};
		_context.Materials.Add(driedSweat);
		var sweat = new Liquid
		{
			Name = $"{racialDescriptor} Sweat",
			Description = "sweat",
			LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
			TasteText = "It tastes like a pungent, salty lick of someone's underarms",
			VagueTasteText = "It tastes very unpleasant, like underarm stench",
			SmellText = "It has the sharp, pungent smell of body odor",
			VagueSmellText = "It has the sharp, pungent smell of body odor",
			TasteIntensity = 200,
			SmellIntensity = 200,
			AlcoholLitresPerLitre = 0,
			WaterLitresPerLitre = 0.95,
			DrinkSatiatedHoursPerLitre = 5,
			FoodSatiatedHoursPerLitre = 0,
			CaloriesPerLitre = 0,
			Viscosity = 1,
			Density = 1,
			Organic = true,
			ThermalConductivity = 0.609,
			ElectricalConductivity = 0.005,
			SpecificHeatCapacity = 4181,
			FreezingPoint = -20,
			BoilingPoint = 100,
			DisplayColour = "yellow",
			DampDescription = "It is damp with sweat",
			WetDescription = "It is wet and smelly with sweat",
			DrenchedDescription = "It is soaking wet and smelly with sweat",
			DampShortDescription = "(sweat-damp)",
			WetShortDescription = "(sweaty)",
			DrenchedShortDescription = "(sweat-drenched)",
			SolventId = 1,
			SolventVolumeRatio = 5,
			InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
			ResidueVolumePercentage = 0.05,
			DriedResidue = driedSweat
		};
		_context.Liquids.Add(sweat);
		_context.SaveChanges();

		return (blood, sweat, driedBlood, driedSweat);
	}

	public void SeedMiddleEarthHeritage()
	{
		var body = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
		var humanoid = _context.Races.First(x => x.Name == "Organic Humanoid");
		var personDef = _context.CharacteristicDefinitions.First(x => x.Name == "Person Word");

		_context.FutureProgs.First(x => x.FunctionName == "MaximumHeightChargen").FunctionText =
			@"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
    if (@ch.Ethnicity.Group == ""Edain"")
      if (@ch.Gender == ToGender(""Male""))
        // 7'2""
        return 218.5
      else
        // 6'11""
        return 211
      end if
    end if
    if (@ch.Gender == ToGender(""Male""))
      // 6'9""
      return 205.75
    else
      // 6'7""
      return 200.5
    end if
  case (ToRace(""Elf""))
    if (@ch.Gender == ToGender(""Male""))
      // 6'9""
      return 205.75
    else
      // 6'7""
      return 200.5
    end if
  case (ToRace(""Dwarf""))
    if (@ch.Gender == ToGender(""Male""))
      // 4'0""
      return 121
    else
      // 3'11""
      return 119
    end if
  case (ToRace(""Hobbit""))
    if (@ch.Gender == ToGender(""Male""))
      // 4'0""
      return 121
    else
      // 3'11""
      return 119
    end if
  case (ToRace(""Orc""))
    if (@ch.Gender == ToGender(""Male""))
      // 6'0""
      return 181
    else
      // 6'0""
      return 181
    end if
  case (ToRace(""Troll""))
    if (@ch.Gender == ToGender(""Male""))
      // 15'
      return 510
    else
      // 15'
      return 510
    end if
end switch
return 200";
		_context.FutureProgs.First(x => x.FunctionName == "MaximumWeightChargen").FunctionText =
			@"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
    bmi = 50
  case (ToRace(""Elf""))
    bmi = 30
  case (ToRace(""Hobbit""))
    bmi = 60
  case (ToRace(""Dwarf""))
    bmi = 55
  case (ToRace(""Orc""))
    bmi = 60
  case (ToRace(""Troll""))
    bmi = 60
  default
    bmi = 50
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000";
		_context.FutureProgs.First(x => x.FunctionName == "MinimumHeightChargen").FunctionText =
			@"// You will need to expand this as you add new playable races
switch (@ch.Race)
  case (ToRace(""Human""))
    if (@ch.Ethnicity.Group == ""Edain"")
      if (@ch.Gender == ToGender(""Male""))
        // 5'10""
        return 178
      else
        // 5'8""
        return 173
      end if
    end if
    if (@ch.Gender == ToGender(""Male""))
      // 5'0""
      return 152
    else
      // 4'10""
      return 147
    end if
  case (ToRace(""Elf""))
    if (@ch.Gender == ToGender(""Male""))
      // 5'10""
      return 178
    else
      // 5'8""
      return 173
    end if
  case (ToRace(""Dwarf""))
    if (@ch.Gender == ToGender(""Male""))
      // 3'2""
      return 97
    else
      // 3'1""
      return 94
    end if
  case (ToRace(""Hobbit""))
    if (@ch.Gender == ToGender(""Male""))
      // 3'0""
      return 91
    else
      // 3'0""
      return 89
    end if
  case (ToRace(""Orc""))
    if (@ch.Gender == ToGender(""Male""))
      // 4'6""
      return 137
    else
      // 4'6""
      return 137
    end if
  case (ToRace(""Troll""))
    if (@ch.Gender == ToGender(""Male""))
      // 10'
      return 300
    else
      // 10'
      return 300
    end if
end switch
return 100";
		_context.FutureProgs.First(x => x.FunctionName == "MinimumWeightChargen").FunctionText =
			@"// You will need to expand this as you add new playable races
var bmi as number
switch (@ch.Race)
  case (ToRace(""Human""))
    bmi = 16
  case (ToRace(""Elf""))
    bmi = 12
  case (ToRace(""Hobbit""))
    bmi = 18
  case (ToRace(""Dwarf""))
    bmi = 20
  case (ToRace(""Orc""))
    bmi = 15
  case (ToRace(""Troll""))
    bmi = 20
  default
    bmi = 16
end switch
return (((@ch.Height / 100) ^ 2) * @bmi) * 1000";

		FutureProg CreateVariantAgeProg(FutureProg baseProg,
			Race race)
		{
			var prog = new FutureProg
			{
				FunctionName = baseProg.FunctionName.Replace("Humanoid", race.Name.CollapseString()),
				AcceptsAnyParameters = baseProg.AcceptsAnyParameters,
				StaticType = baseProg.StaticType,
				ReturnType = baseProg.ReturnType,
				Category = baseProg.Category,
				Subcategory = baseProg.Subcategory,
				FunctionComment = baseProg.FunctionComment.Replace("humanoid", race.Name),
				FunctionText = baseProg.FunctionText.Replace("ToRace(\"Humanoid\")", $"ToRace(\"{race.Name}\")")
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterType = 8200,
				ParameterName = "ch"
			});
			_context.FutureProgs.Add(prog);
			return prog;
		}

		var isBabyProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidBaby");
		var isToddlerProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidToddler");
		var isBoyProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidBoy");
		var isGirlProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidGirl");
		var isChildProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidChild");
		var isYouthProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYouth");
		var isYoungManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYoungMan");
		var isYoungWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidYoungWoman");
		var isAdultManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdultMan");
		var isAdultWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdultWoman");
		var isAdultProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidAdult");
		var isOldManProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldMan");
		var isOldWomanProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldWoman");
		var isOldPersonProg = _context.FutureProgs.First(x => x.FunctionName == "IsHumanoidOldPerson");

		#region Elves

		var canSelectElfProg = new FutureProg
		{
			FunctionName = "CanSelectElfRace",
			Category = "Character",
			Subcategory = "Race",
			FunctionComment = "Determines if the character select the elf race in chargen.",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = @"// You might consider checking RPP
return true"
		};
		canSelectElfProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectElfProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(canSelectElfProg);
		var elfAttributeProg = new FutureProg
		{
			FunctionName = "ElfBonusAttributes",
			Category = "Character",
			Subcategory = "Attributes",
			FunctionComment =
				"This prog is called for each attribute for elves at chargen time and the resulting value is applied as a modifier to that attribute.",
			ReturnType = (long)FutureProgVariableTypes.Number,
			StaticType = 0,
			FunctionText = @"// You might consider a switch on attributes like so:
//
// switch (@trait.Name)
//   case (""Dexterity"")
//     return 2
// end switch
return 0"
		};
		elfAttributeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = elfAttributeProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(elfAttributeProg);
		_context.SaveChanges();

		var (elfBlood, elfSweat, driedElfBlood, driedElfSweat) = CreateBloodAndSweat("Elven");

		var elfRace = new Race
		{
			Name = "Elf",
			Description = @"Elves are a immortal race of beings who are highly skilled in magic and crafting. They are tall and slender, with pointed ears and an innate connection to the natural world. Elves are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art. They are also skilled warriors, but prefer to use their weapons only as a last resort. 

Elves are divided into several different clans, each with their own distinct culture and way of life. The most well-known of these clans are the Vanyar, the Noldor, and the Teleri. The Vanyar are the fairest and most powerful of all the Elves, and are closely associated with the Valar, the angelic beings who serve the creator god of the universe. The Noldor are the most intellectually curious of the Elves, and are known for their prowess in magic and crafting. The Teleri are the most elusive and reclusive of the clans, and are associated with the sea.",
			AllowedGenders = _humanRace.AllowedGenders,
			ParentRace = humanoid,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 2.0,
			CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
			DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NaturalArmourQuality = 3,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SweatRateInLitresPerMinute = 0.1,
			SizeStanding = 6,
			SizeProne = 6,
			SizeSitting = 6,
			CommunicationStrategyType = "humanoid",
			DefaultHandedness = 1,
			HandednessOptions = "1 3",
			MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
			MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = -20,
			TemperatureRangeCeiling = 50,
			BodypartSizeModifier = 0,
			BodypartHealthMultiplier = 1,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			ChildAge = 8,
			YouthAge = 20,
			YoungAdultAge = 50,
			AdultAge = 200,
			ElderAge = 1000,
			VenerableAge = 3000,
			AttributeBonusProg = elfAttributeProg,
			AvailabilityProg = canSelectElfProg,
			BaseBody = body,
			BloodLiquid = elfBlood,
			BloodModel = humanoid.BloodModel,
			NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
			NaturalArmourType = humanoid.NaturalArmourType,
			RaceButcheryProfile = null,
			SweatLiquid = elfSweat
		};
		_context.Races.Add(elfRace);

		var elfMaleHWModel = new HeightWeightModel
		{
			Name = "Elf Male",
			MeanHeight = 187,
			MeanBmi = 21,
			StddevHeight = 7.6,
			StddevBmi = 1.2,
			Bmimultiplier = 0.1
		};
		_context.Add(elfMaleHWModel);
		var elfFemaleHWModel = new HeightWeightModel
		{
			Name = "Elf Female",
			MeanHeight = 180,
			MeanBmi = 21.5,
			StddevHeight = 5,
			StddevBmi = 1.2,
			Bmimultiplier = 0.1
		};
		_context.Add(elfFemaleHWModel);
		elfRace.DefaultHeightWeightModelMale = elfMaleHWModel;
		elfRace.DefaultHeightWeightModelFemale = elfFemaleHWModel;
		elfRace.DefaultHeightWeightModelNonBinary = elfFemaleHWModel;
		elfRace.DefaultHeightWeightModelNeuter = elfMaleHWModel;
		_races.Add("Elf", elfRace);
		_context.SaveChanges();

		var isElfProg = new FutureProg
		{
			FunctionName = "IsRaceElf",
			FunctionComment = "Determines whether someone is the Elf race",
			FunctionText = $"return @ch.Race == ToRace({elfRace.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isElfProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isElfProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isElfProg);
		_context.SaveChanges();
		_raceProgs["Elf"] = isElfProg;

		var isElfBabyProg = CreateVariantAgeProg(isBabyProg, elfRace);
		var isElfToddlerProg = CreateVariantAgeProg(isToddlerProg, elfRace);
		var isElfBoyProg = CreateVariantAgeProg(isBoyProg, elfRace);
		var isElfGirlProg = CreateVariantAgeProg(isGirlProg, elfRace);
		var isElfChildProg = CreateVariantAgeProg(isChildProg, elfRace);
		var isElfYouthProg = CreateVariantAgeProg(isYouthProg, elfRace);
		var isElfYoungManProg = CreateVariantAgeProg(isYoungManProg, elfRace);
		var isElfYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, elfRace);
		var isElfAdultManProg = CreateVariantAgeProg(isAdultManProg, elfRace);
		var isElfAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, elfRace);
		var isElfAdultPersonProg = CreateVariantAgeProg(isAdultProg, elfRace);
		var isElfOldManProg = CreateVariantAgeProg(isOldManProg, elfRace);
		var isElfOldWomanProg = CreateVariantAgeProg(isOldWomanProg, elfRace);
		var isElfOldPersonProg = CreateVariantAgeProg(isOldPersonProg, elfRace);
		var elfPersonValues = new List<(CharacteristicValue Value, double Weight)>();
		var nextId = _context.CharacteristicValues.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

		void AddElfPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++,
				Definition = personDef,
				Name = name,
				Value = basic,
				AdditionalValue = "",
				Default = false,
				Pluralisation = 0,
				FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			elfPersonValues.Add((pw, weight));
		}

		AddElfPersonWord("elven baby", "elven baby", isElfBabyProg, 1.0);
		AddElfPersonWord("elven infant", "elven baby", isElfBabyProg, 1.0);
		AddElfPersonWord("elven tot", "elven baby", isElfBabyProg, 1.0);
		AddElfPersonWord("elven toddler", "elven toddler", isElfToddlerProg, 5.0);
		AddElfPersonWord("elf", "elf", isElfAdultPersonProg, 5.0);
		AddElfPersonWord("elven person", "elf", isElfAdultPersonProg, 5.0);
		AddElfPersonWord("elf person", "elf", isElfAdultPersonProg, 1.0);
		AddElfPersonWord("elven individual", "elf", isElfAdultPersonProg, 1.0);
		AddElfPersonWord("male elf", "elf", isElfAdultManProg, 1.0);
		AddElfPersonWord("elven man", "elf", isElfAdultManProg, 5.0);
		AddElfPersonWord("elf man", "elf", isElfAdultManProg, 1.0);
		AddElfPersonWord("female elf", "elf", isElfAdultWomanProg, 1.0);
		AddElfPersonWord("elven woman", "elf", isElfAdultWomanProg, 5.0);
		AddElfPersonWord("elf woman", "elf", isElfAdultWomanProg, 1.0);
		AddElfPersonWord("elven lady", "elf", isElfAdultWomanProg, 3.0);
		AddElfPersonWord("elf lady", "elf", isElfAdultWomanProg, 1.0);
		AddElfPersonWord("elven lad", "elven boy", isElfBoyProg, 1.0);
		AddElfPersonWord("elf boy", "elven boy", isElfBoyProg, 3.0);
		AddElfPersonWord("elven boy", "elven boy", isElfBoyProg, 3.0);
		AddElfPersonWord("elven child", "elven child", isElfChildProg, 3.0);
		AddElfPersonWord("elven youth", "elven youth", isElfYouthProg, 3.0);
		AddElfPersonWord("elven adolescent", "elven youth", isElfYouthProg, 1.0);
		AddElfPersonWord("elven youngster", "elven youth", isElfYouthProg, 1.0);
		AddElfPersonWord("elven juvenile", "elven youth", isElfYouthProg, 1.0);
		AddElfPersonWord("elven kid", "elven child", isElfChildProg, 1.0);
		AddElfPersonWord("young elven boy", "elven boy", isElfBoyProg, 1.0);
		AddElfPersonWord("young elf", "elf", isElfYoungManProg, 3.0);
		AddElfPersonWord("elven youngster", "elf", isElfYoungManProg, 1.0);
		AddElfPersonWord("elven lass", "elven girl", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("elf lass", "elven girl", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("elven girl", "elven girl", isElfGirlProg, 3.0);
		AddElfPersonWord("elf girl", "elven girl", isElfGirlProg, 3.0);
		AddElfPersonWord("young elven girl", "elven girl", isElfGirlProg, 1.0);
		AddElfPersonWord("young elf girl", "elven girl", isElfGirlProg, 1.0);
		AddElfPersonWord("young elven woman", "elven girl", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("young elf", "elven girl", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("elf maiden", "elven elf", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("elven maiden", "elven elf", isElfYoungWomanProg, 1.0);
		AddElfPersonWord("old elf", "old elf", isElfOldPersonProg, 1.0);
		AddElfPersonWord("old elven woman", "old elf", isElfOldWomanProg, 1.0);
		AddElfPersonWord("elderly elven woman", "old elf", isElfOldWomanProg, 1.0);
		AddElfPersonWord("old elven man", "old elf", isElfOldManProg, 1.0);
		AddElfPersonWord("elderly elven man", "old elf", isElfOldManProg, 1.0);
		AddElfPersonWord("elderly elf", "old elf", isElfOldPersonProg, 1.0);
		_context.SaveChanges();

		var elfPersons = new CharacteristicProfile
		{
			Name = "Elf Person Words",
			Definition = new XElement("Values",
				from item in elfPersonValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words applicable to the elven race",
			TargetDefinition = personDef
		};
		_context.CharacteristicProfiles.Add(elfPersons);
		_profiles[elfPersons.Name] = elfPersons;
		_context.SaveChanges();

		void AddElfEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
			string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(elfRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? canSelectElfProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Elf Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddElfEthnicity("Noldor",
			"The Noldor are often called \"High Elves\" ostensibly because they are considered to be the most noble of the Quendi (Elves) in Middle-Earth. In reality, they are so named because they are the only Elves living in Endor who have ever resided in the Blessed Realm of Aman across the sea. This exalted status is accentuated by their close ties with the Valar, a relationship which accounts for their unique cultural and linguistic roots.\r\n\r\nOf all the Elves of Middle-Earth, the Noldor are the most ordered. While their brethren are content to wander or mark time in quiet diffusion, the Noldor seek to build communities and states in beautiful, guarded places. ",
			"Tatyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Black_Brown_Blonde_Grey_Hair", "fair_skin");
		AddElfEthnicity("Vanyar",
			"The Vanyar, also called the Fair Elves and Light-elves, were the first and smallest of the Kindreds of the Eldar. Under the leadership of Ingwë, the Vanyar were the first to set forth on the Great Journey and reach the shores of Belegaer; they sailed to Aman on the first voyage of Tol Eressëa and remained there permanently. Very few, if any, Vanyar remained in or returned to Middle-earth after the Great Journey.\r\n\r\nThe Vanyar were the fairest of all the Elves, hence their name \"Fair Elves\". Unlike other kindreds attracted to the sea or the building and forging of things, the Vanyar culture seemed to revolve about the Valar and Valinor. This is probably the reason why they chose to stay in Valinor proper, centered around Taniquetil, the seat of the rulers of Arda, and loved the light of Valinor best of all the other places in Aman. They greatly valued the wisdom of the Valar and were thus favored by Manwë and Varda, and always distrusted by Melkor. They were known to have had the greatest skill in poetry of all Elves, likened to Manwë, who loved them for it. Their hair was golden, and their banners were white.",
			"Minyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Blonde_Grey_Hair", "fair_skin", false);
		AddElfEthnicity("Teleri",
			"The Teleri (meaning \"Those who come last\") were the third of the Elf clans who came to Aman. Those who came to Aman became known as the Falmari. They were the ancestors of the Valinorean Teleri, and the Sindar, Laiquendi, and Nandor of Middle-earth.\r\n\r\nThe Teleri refused to join the Noldor in leaving Valinor, and many of them were cruelly slain in the Kinslaying at their chief city of Alqualondë, or Swan Harbour, when they refused to supply the exiles with their swan ships. For this reason few or none of the Teleri joined the host of the Valar which set out to capture Morgoth for good. It is recounted that the Teleri eventually forgave the Noldor for the Kinslaying, and the two kindreds were at peace again.",
			"Nelyar", "Calaquendi", "O-A High Negative", "Blue_Eyes", "Brown_Blonde_Grey_Hair", "fair_skin", false);
		AddElfEthnicity("Sindar",
			"The Sindar or \"Grey-Elves\" are Eldar and were originally part of the great kindred called the Teleri. Unlike the Noldor, Vanyar and the bulk of the Teleri, the Sindar chose not to cross over the sea to Aman; instead they stayed in Middle-Earth. They, like the Silvan Elves, are part of the Moriquendi, the \"Dark Elves\" who never saw the light of Valinor.\r\n\r\nThe Sindar are the most open and cooperative of Middle-Earth's Elves. They are great teachers and borrowers and have an interest in the works of all races. Grey-Elves are a settled people and enjoy the company of others. \r\n\r\nMany of the Sindar feel a kinship to the sea. They build superb ships and are renowned sailors.",
			"Nelyar", "Moriquendi", "Majority O Minor A", "Blue_Eyes", "Brown_Blonde_Grey_Hair", "fair_skin");
		AddElfEthnicity("Silvan",
			"When the Eldar departed from the original Elven homeland during the Elder Days, a number of their brethren remained behind. They decided not to seek the light of the Aman and were labeled as the Avari (meaning \"Unwilling\" or \"Refusers\" in Quenya). These kindreds were left to fend for themselves during the days when Morgoth's Shadow swept over the East. In these dark times they were forced into the secluded safety of the forests of eastern Middle-Earth, where they wandered and hid from the wild Men who dominated most of the lands. They became known as the Silvan or Wood-Elves.\r\n\r\nThe culture of the Silvan Elves is best characterised as unstructured and rustic by Elven standards, but rich and relatively advanced when compared to the ways of Men. They have always been independent, but as of late many have settled in kingdoms ruled by other Elves such as the Noldor or Sindar. Still, all Silvan folk enjoy a good journey or adventure and most look at life much as a game to be played.\r\n\r\nMusic and trickery are their favourite pastimes. The Silvan Elves are also mastered of the wood and know much of wood-craft and wood-lore.",
			"Nelyar", "Moriquendi", "Overwhelmingly O", "Blue_Green_Eyes", "Black_Brown_Blonde_Grey_Hair",
			"Fair_Olive_Skin");

		var elfSkillProg = new FutureProg
		{
			FunctionName = "ElfSkillStartingValue",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine the opening value for a skill for elves at character creation",
			ReturnType = (int)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
		};
		elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = elfSkillProg, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)FutureProgVariableTypes.Toon
		});
		elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = elfSkillProg, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)FutureProgVariableTypes.Trait
		});
		elfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = elfSkillProg, ParameterIndex = 2, ParameterName = "boosts",
			ParameterType = (int)FutureProgVariableTypes.Number
		});
		_context.FutureProgs.Add(elfSkillProg);
		_context.SaveChanges();

		var quenyaCalendar = _context.Calendars.Find(1L);
		var sindarCalendar = _context.Calendars.Find(2L) ?? quenyaCalendar;

		void AddElfCulture(string name, bool useQuenyaCalendar, string description)
		{
			var prog = new FutureProg
			{
				FunctionName = $"CanSelect{name.CollapseString()}Culture",
				Category = "Chargen",
				Subcategory = "Culture",
				FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
				ReturnType = (int)FutureProgVariableTypes.Boolean,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Elf"")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);
			AddCulture(name, "Eldarin", description, _alwaysTrueProg, elfSkillProg,
				useQuenyaCalendar ? quenyaCalendar : sindarCalendar);
		}

		AddElfCulture("Rivendell", true,
			@"Rivendell, also known as Imladris in Sindarin, was an Elven town and the house of Elrond located in Middle-earth. It is described as ""The Last Homely House East of the Sea"" in reference to Valinor, which was west of the Great Sea in Aman.

The peaceful, sheltered town of Rivendell was located at the edge of a narrow gorge of the river Bruinen, but well hidden in the moorlands and foothills of the Misty Mountains. One of the main approaches to Rivendell came from the nearby Ford of Bruinen.

In Imladris, there was a large hall with a dais and several tables for feasting. Another hall, the Hall of Fire, had a fire in it year-round with carven pillars on either side of the hearth; it was used for singing and storytelling on high days, but stood empty otherwise, and people would come there alone to think and ponder. The eastern side of the house had a porch at which Frodo Baggins found his friends once he had awakened, and was where the Council of Elrond was held.

Rivendell was protected from attack (mainly by the River Bruinen, Elrond, and Elven magic), but Elrond himself said that Rivendell was a place of peace and learning, not a stronghold of battle.");
		AddElfCulture("Lothlórien", true,
			@"Lothlórien, also known as Lórien, was a forest and Elven realm near the lower Misty Mountains. It was first settled by Nandorin Elves, but they were later joined by a small number of Ñoldor and Sindar under Celeborn of Doriath and Galadriel, daughter of Finarfin. It was located on the River Celebrant, southeast of Khazad-dum, and was the only place in Middle-earth where the golden Mallorn trees grew.

Galadriel's magic, later revealed as the power of her ring Nenya, enriched the land and made it a magic forest into which evil could not enter without difficulty. The only way that Lothlórien could have been conquered by the armies of Mordor is if Sauron had come there himself.

After Galadriel left for Valinor, the Elves of Lórien were ruled by their lord Celeborn alone, and the realm was expanded to include a part of southern Mirkwood, but it appears to have quickly been de-populated during the Fourth Age. In ""The Tale of Aragorn and Arwen,"" the timeless Elven kingdom is depicted as being wholly abandoned by the time of King Elessar's passing. Even after the assaults on Lórien by Sauron's forces during the War of the Ring, there must have been several thousand Silvan Elves remaining in the land. Celeborn himself went to dwell in Rivendell, whilst many of the other Elves likely moved to Thranduil's Woodland Realm.

Galadriel bore Nenya on a ship from the Grey Havens into the West, accompanied by the other two Elven Rings and their bearers. With the Ring gone, the magic and beauty of Lórien also faded, along with the extraordinary Mallorn trees that had lived for centuries, and it was gradually depopulated. By the time Arwen came there to die in FO 121, Lothlórien was deserted. She was buried on the hill of Cerin Amroth, where she and Aragorn had been betrothed.");
		AddElfCulture("Mithlond", true,
			@"The Grey Havens, known also as Mithlond, was an Elvish port city on the Gulf of Lune in the Elven realm of Lindon in Middle-earth. 

Because of its cultural and spiritual importance to the Elves, the Grey Havens in time became the primary Elven settlement west of the Misty Mountains prior to the establishment of Eregion and, later, Rivendell. Even after the death of Gil-galad and as the Elves dwindled in numbers by the year, the Grey Havens remained a major Elven settlement and the main departure point of Elven ships to Aman.

Despite being a major port, by the late Third Age the Grey Havens had sparse population, like Rivendell and northeastern Mirkwood.");
		AddElfCulture("Forlindon", true,
			@"Forlindon was the northern part of Lindon, which was separated from Harlindon by the Gulf of Lune. The Elven-haven and harbour of Forlond lay on the southern coast of the region, and was ruled by Gil-galad. Like the rest of Lindon it was a remnant of Beleriand that sank under the Great Sea after the War of Wrath, at the end of the First Age.

Lindon was a region of western Middle-earth. Initially populated by Laiquendi, in the following Ages it became an important Elvish realm, known for its harbors and Elven ships that would embark unto the West");
		AddElfCulture("Hardlindon", false,
			@"Harlindon was a green and fair land on the northwestern shores of Middle-earth. It was located west of the Blue Mountains and south of the Gulf of Lune which divided Lindon into the northern Forlindon and the southern Harlindon. At the head of the Gulf lay the seaport Grey Havens. There were woods at the foot of the Blue Mountains and a haven in a small inlet on the southern shores of the bay of Harlond.

Lindon was a region of western Middle-earth. Initially populated by Laiquendi, in the following Ages it became an important Elvish realm, known for its harbors and Elven ships that would embark unto the West");
		AddElfCulture("Woodland Realm", false,
			@"The Woodland Realm was a kingdom of Silvan Elves located deep in Mirkwood, the great forest of Rhovanion, beginning in the Second Age. Following the War of the Last Alliance, Thranduil of the Sindar ruled over the Silvan Elves. Those of the Woodland Realm were known to be less wise and more dangerous than other Elves, but by the late Third Age were the only remaining Elven realm with a king.");
		_context.SaveChanges();

		#endregion Elves

		#region Men

		var isHumanProg = new FutureProg
		{
			FunctionName = "IsRaceHuman",
			FunctionComment = "Determines whether someone is the Human race",
			FunctionText = $"return @ch.Race == ToRace({_context.Races.First(x => x.Name == "Human").Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isHumanProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isHumanProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isHumanProg);
		_context.SaveChanges();
		_raceProgs["Human"] = isHumanProg;

		void AddHumanEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
			string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(_humanRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? _alwaysTrueProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Weighted Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddHumanEthnicity("Eriadoran", @"Eriadoran men are a hardy and independent people who value self-sufficiency and personal freedom. They are skilled farmers, herders, and hunters, and are known for their practicality and resourcefulness. Eriadoran culture is deeply rooted in tradition, and family ties are highly valued. Eriadoran men are also known for their love of song, poetry, and storytelling, and these art forms play a central role in their cultural identity.

Eriadoran men are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, Eriadoran men are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.", "Middle Man", "Eriadoran", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Arnorian", @"Arnorian men are a proud and noble people with a rich cultural heritage. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. Arnorian culture is deeply rooted in tradition, and the history and legends of their forefathers are an important part of their identity.

Arnorian men are known for their bravery, honor, and loyalty. They are skilled warriors, and have a long tradition of military service. They are also deeply religious, and their faith plays a central role in their lives. Arnorian men are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Middle Man", "Arnorian", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Gondorian", @"Gondorian men are a proud and honorable people with a rich cultural heritage. They are descended from the ancient kingdom of Gondor, which was founded by the brothers Isildur and Anarion during the Second Age. Gondorian culture is deeply rooted in tradition, and the history and legends of their forefathers are an important part of their identity.

Gondorian men are known for their bravery, honor, and loyalty. They are skilled warriors, and have a long tradition of military service. They are also deeply religious, and their faith plays a central role in their lives. Gondorian men are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage. Gondorian men are proud of their civilization and its achievements, and are deeply committed to its defense and preservation.", "Middle Man", "Gondorian", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Rohirrim", @"The Rohirrim are a proud and noble people with a strong sense of honor and loyalty. They are skilled horsemen and warriors, and have a deep connection to the land and their herds. The Rohirrim value independence and freedom, and are fiercely protective of their families and homes. They are known for their kindness and generosity towards their friends and allies, and are deeply loyal to their kin. The Rohirrim are also skilled craftsmen, renowned for their horsemanship and the quality of their weapons and armor. Despite their love of peace, the Rohirrim are not afraid to defend their way of life by force if necessary.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Dunlending", @"The Dunlending men are a fierce and independent people who inhabit the wild, hilly regions of Eriador. They are a hardy and practical people, accustomed to living in a harsh and unforgiving land. The Dunlendings are known for their strength and endurance, and are skilled hunters, farmers, and herders. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their rough exterior, the Dunlendings are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Dunlendings are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their reputation as a rough and barbaric people, the Dunlendings are known for their generosity and hospitality towards their friends and allies.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Dorwinrim", @"The Dorwinrim are a proud and noble people who inhabit the western region of Middle-earth known as Dorwinion. They are a wealthy and cultured people, with a long tradition of trade and commerce. The Dorwinrim are known for their love of luxury and refinement, and are renowned for their fine wines and exotic spices. They are also skilled craftsmen, and are known for their intricate metalwork and jewelery.

Despite their love of luxury and indulgence, the Dorwinrim are a fiercely independent people who value honor and integrity above all else. They are skilled warriors, and have a long tradition of military service. The Dorwinrim are deeply religious, and their faith plays a central role in their lives. They are also deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. The Dorwinrim are known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Olive_Skin");
		AddHumanEthnicity("Rhovanian", @"The Rhovanion men are a hardy and independent people who inhabit the wild, wooded regions of Middle-earth known as Rhovanion. They are skilled hunters, farmers, and herders, and are known for their practicality and resourcefulness. The Rhovanion men are also skilled craftsmen, and are renowned for their woodworking and leatherworking.

Despite their rough exterior, the Rhovanion men are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Rhovanion men are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their reputation as a rough and barbaric people, the Rhovanion men are known for their generosity and hospitality towards their friends and allies.", "Middle Man", "Northman", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Skin");
		AddHumanEthnicity("Easterling", @"The Easterling Men are a diverse and complex people who inhabit the eastern regions of Middle-earth. They are a hardy and resourceful people, accustomed to living in a harsh and unforgiving land. The Easterling Men are skilled warriors, and have a long tradition of military service. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their reputation as a barbaric and warlike people, the Easterling Men are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Easterling Men are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary.", "Fallen Man", "Easterling", "Majority O Minor A", "Brown_Green_Blue_Eyes",
			"Brown_Blonde_Red_Grey_Hair", "Fair_Skin");
		AddHumanEthnicity("Variag", @"The Variags are a civilized and cultured people who inhabit the region of Khand in eastern Middle-earth. They are a wealthy and sophisticated people, with a long tradition of trade and commerce. The Variags are known for their love of luxury and refinement, and are renowned for their fine wines and exotic spices. They are also skilled craftsmen, and are known for their intricate metalwork and jewelery.

Despite their love of luxury and indulgence, the Variags are a fiercely independent people who value honor and integrity above all else. They are skilled warriors, and have a long tradition of military service. The Variags are deeply religious, and their faith plays a central role in their lives. They are also deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. The Variags are known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Fallen Man", "Southron", "B Dominant", "Brown_Eyes", "Black_Brown_Grey_Hair",
			"Swarthy_Skin", false);
		AddHumanEthnicity("Umbaric", @"The men of Umbar are a proud and formidable people who inhabit the southern coastal region of Middle-earth known as Umbar. They are a hardy and resourceful people, accustomed to living in a harsh and unforgiving land. The men of Umbar are skilled sailors and traders, and have a long tradition of maritime commerce. They are also skilled warriors, and have a long tradition of military service.

Despite their reputation as a rough and barbaric people, the men of Umbar are a proud and noble people with a rich cultural heritage. They are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. The men of Umbar are deeply devoted to their families and communities, and place a strong emphasis on the bonds of kinship. They are also known for their love of music, poetry, and the arts, and these art forms are an important part of their cultural heritage.", "Fallen Man", "Southron", "B Dominant", "Brown_Eyes", "Black_Brown_Grey_Hair",
			"Swarthy_Skin", false);
		AddHumanEthnicity("Haradrim", @"The Haradrim are a mysterious and reclusive people who inhabit the southern regions of Middle-earth. They are a dark-skinned people, with a culture and way of life that is shrouded in mystery. The Haradrim are skilled warriors, and have a long tradition of military service. They are also skilled craftsmen, and are renowned for their metalworking and leatherworking.

Despite their reputation as a fierce and warlike people, the Haradrim are a proud and noble people with a rich cultural heritage. They have a deep connection to the land and the spirits that inhabit it, and their spiritual beliefs play a central role in their lives. The Haradrim are fiercely independent, and prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. The Haradrim are known for their strong and fierce personalities, and are often feared and misunderstood by the other peoples of Middle-earth.", "Fallen Man", "Southron", "A Dominant", "Brown_Eyes", "Black_Grey_Hair",
			"Dark_Skin", false);
		AddHumanEthnicity("Gondorian Dunedain", @"The Gondorian Dunedain are a proud and noble people who inhabit the lands of Gondor in Middle-earth. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. The Gondorian Dunedain are a tall and fair-skinned people, with bright blue eyes and golden hair. They are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art.

The Gondorian Dunedain are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, the Gondorian Dunedain are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.

The Gondorian Dunedain are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Elendil, the House of Isildur, and the House of Anarion. The House of Elendil is the most powerful and influential of the clans, and is closely associated with the royal family of Gondor. The House of Isildur is known for its military prowess, and has a long tradition of service to the kingdom. The House of Anarion is known for its artistic and scholarly achievements, and has produced many of Gondor's greatest poets, musicians, and scholars. All of these clans are deeply committed to the defense and preservation of Gondor, and are willing to lay down their lives for their kingdom.", "Edain", "Dunedain", "Overwhelmingly O", "Blue_Green_Eyes",
			"Black_Brown_Grey_Hair", "Fair_Skin");
		AddHumanEthnicity("Arnorian Dunedain", @"The Arnorian Dunedain are a proud and noble people who inhabit the lands of Arnor in Middle-earth. They are descendants of the ancient kingdom of Arnor, which was founded by Elendil, one of the greatest kings of the First Age. The Arnorian Dunedain are a tall and fair-skinned people, with bright blue eyes and golden hair. They are known for their beauty, grace, and elegance, as well as their love of music, poetry, and art.

The Arnorian Dunedain are fiercely independent, and they prize their freedom above all else. They are fiercely protective of their families and homes, and are willing to defend them with force if necessary. Despite their tough exterior, the Arnorian Dunedain are known for their kindness and generosity towards their friends and allies. They are deeply loyal to their friends and kin, and will go to great lengths to protect them.

The Arnorian Dunedain are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Elendil, the House of Isildur, and the House of Anarion. The House of Elendil is the most powerful and influential of the clans, and is closely associated with the royal family of Arnor. The House of Isildur is known for its military prowess, and has a long tradition of service to the kingdom. The House of Anarion is known for its artistic and scholarly achievements, and has produced many of Arnor's greatest poets, musicians, and scholars. All of these clans are deeply committed to the defense and preservation of Arnor, and are willing to lay down their lives for their kingdom.", "Edain", "Arnorian", "Overwhelmingly O", "Blue_Green_Eyes",
			"Black_Brown_Grey_Hair", "Fair_Skin");
		AddHumanEthnicity("Black Numenorean", @"The Black Numenoreans, initially named the King's Men, were a fallen group of Numenoreans descended from those who were loyal to the Numenorean Sceptre but in opposition to the Valar and relations with the Elves. After Sauron was brought as a captive to Numenor in fair form, these Numenoreans listened to his words, and were soon corrupted by him. They worshipped the Darkness and its lords (Melkor and later Sauron) and oppressed the Men left in Middle-earth. Ever since, they were the servants of the Enemy and bitter adversaries of the Men of Gondor.

Black Numenoreans are very similar in physical and cultural character to the Dunedain but tend towards darker skin colours due to more intermarriage with Southern peoples. Black Numenoreans usually occupy the upper echelons of whatever society they find themselves in; the model of Black Numenorean ruling class with subject peoples of other cultures or even other races such as orks are common.", "Edain", "Fallen", "Overwhelmingly O", "Blue_Green_Eyes",
			"Black_Brown_Grey_Hair", "Swarthy_Skin");

		void AddHumanCulture(string name, string nameCulture, string description, string canSelectProgText)
		{
			var prog = new FutureProg
			{
				FunctionName = $"CanSelect{name.CollapseString()}Culture",
				Category = "Chargen",
				Subcategory = "Culture",
				FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
				ReturnType = (int)FutureProgVariableTypes.Boolean,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = canSelectProgText
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);
			_context.SaveChanges();
			AddCulture(name, nameCulture, description, prog);
		}

		AddHumanCulture("Bree Folk", "Eriadoran", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race == ToRace(""Hobbit""))
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
if (@ch.Ethnicity.EthnicGroup == ""Middle Man"" or (@ch.Ethnicity.EthnicGroup == ""Edain"" and @ch.Ethnicity.EthnicSubGroup != ""Fallen""))
  return true
end if
return false");
		AddHumanCulture("Arnorian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
if (@ch.Ethnicity.Name != ""Arnorian"" and @ch.Ethnicity.Name != ""Arnorian Dunedain"")
  return false
end if
return true");
		AddHumanCulture("Gondorian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Gondorian"")
    return true
  case (""Gondorian Dunedain"")
    return true
  case (""Haradrim"")
    return true
  case (""Rohirrim"")
    return true
end switch
return true");
		AddHumanCulture("Rohirrim", "Rohirrim", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Gondorian"")
    return true
  case (""Gondorian Dunedain"")
    return true
  case (""Dunlending"")
    return true
  case (""Rohirrim"")
    return true
end switch
return true");
		AddHumanCulture("Dunlending", "Dunlending", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Eriadoran"")
    return true
  case (""Dunlending"")
    return true
  case (""Rohirrim"")
    return true
end switch
return true");
		AddHumanCulture("Dorwinian", "Mannish Sindarin", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
    return true
  case (""Rhovanion"")
    return true
  case (""Easterling"")
    return true
end switch
return true");
		AddHumanCulture("Dale Folk", "Dalish", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
    return true
  case (""Rhovanion"")
    return true
end switch
return true");
		AddHumanCulture("Easterling", "Easterling", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Dorwinrim"")
    return true
  case (""Easterling"")
    return true
end switch
return true");
		AddHumanCulture("Variag", "Variag", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Variag"")
    return true
end switch
return true");
		AddHumanCulture("Haradrim", "Haradrim", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Variag"")
    return true
  case (""Haradrim"")
    return true
  case (""Gondorian"")
    return true
  case (""Black Numenorean"")
    return true
  case (""Umbaric"")
    return true
end switch
return true");
		AddHumanCulture("Corsair", "Corsair", @"", @"if (@ch.Special)
  return true
end if
if (@ch.Race != ToRace(""Human""))
  return false
end if
switch (@ch.Ethnicity.Name)
  case (""Haradrim"")
    return true
  case (""Gondorian"")
    return true
  case (""Black Numenorean"")
    return true
  case (""Umbaric"")
    return true
end switch
return true");

		#endregion Men

		#region Hobbits

		var canSelectHobbitProg = new FutureProg
		{
			FunctionName = "CanSelectHobbitRace",
			Category = "Character",
			Subcategory = "Race",
			FunctionComment = "Determines if the character select the hobbit race in chargen.",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = @"// You might consider checking RPP
return true"
		};
		canSelectHobbitProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectHobbitProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(canSelectHobbitProg);
		var hobbitAttributeProg = new FutureProg
		{
			FunctionName = "HobbitBonusAttributes",
			Category = "Character",
			Subcategory = "Attributes",
			FunctionComment =
				"This prog is called for each attribute for hobbits at chargen time and the resulting value is applied as a modifier to that attribute.",
			ReturnType = (long)FutureProgVariableTypes.Number,
			StaticType = 0,
			FunctionText = @"// You might consider a switch on attributes like so:
//
// switch (@trait.Name)
//   case (""Dexterity"")
//     return 2
// end switch
return 0"
		};
		hobbitAttributeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = hobbitAttributeProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(hobbitAttributeProg);
		_context.SaveChanges();

		var (hobbitBlood, hobbitSweat, _, _) = CreateBloodAndSweat("Hobbit");

		var hobbitRace = new Race
		{
			Name = "Hobbit",
			Description = @"The hobbits are a small, curious, and unassuming people who inhabit the land of the Shire in Middle-earth. They are a peaceful and simple folk, with a strong love of home, hearth, and good food. Hobbits are known for their kind and gentle nature, as well as their love of leisure and comfort.

Despite their small size and unassuming appearance, hobbits are tougher and more resilient than they appear. They are skilled farmers, gardeners, and craftsmen, and are able to survive and thrive in even the most difficult of circumstances. Hobbits are also fiercely independent, and are fiercely protective of their families and homes. They are willing to defend themselves and their way of life with determination and resourcefulness.

Hobbits are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the Bagginses, the Tooks, and the Brandybucks. The Bagginses are a wealthy and influential clan, known for their love of comfort and their orderly and well-planned lives. The Tooks are a more adventurous and restless clan, known for their love of travel and exploration. The Brandybucks are a friendly and hospitable clan, known for their love of music and celebration. All of these clans are deeply devoted to the Shire and its way of life, and are fiercely protective of their homes and families.",
			AllowedGenders = _humanRace.AllowedGenders,
			ParentRace = humanoid,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 2.0,
			CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
			DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NaturalArmourQuality = 3,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SweatRateInLitresPerMinute = 0.5,
			SizeStanding = 5,
			SizeProne = 5,
			SizeSitting = 5,
			CommunicationStrategyType = "humanoid",
			DefaultHandedness = 1,
			HandednessOptions = "1 3",
			MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
			MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = 0,
			TemperatureRangeCeiling = 40,
			BodypartSizeModifier = 0,
			BodypartHealthMultiplier = 1,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			ChildAge = 3,
			YouthAge = 10,
			YoungAdultAge = 16,
			AdultAge = 30,
			ElderAge = 70,
			VenerableAge = 100,
			AttributeBonusProg = hobbitAttributeProg,
			AvailabilityProg = canSelectHobbitProg,
			BaseBody = body,
			BloodLiquid = hobbitBlood,
			BloodModel = humanoid.BloodModel,
			NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
			NaturalArmourType = humanoid.NaturalArmourType,
			RaceButcheryProfile = null,
			SweatLiquid = hobbitSweat
		};
		_context.Races.Add(hobbitRace);
		var hobbitMaleHWModel = new HeightWeightModel
		{
			Name = "Hobbit Male",
			MeanHeight = 107,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		};
		_context.Add(hobbitMaleHWModel);
		var hobbitFemaleHWModel = new HeightWeightModel
		{
			Name = "Hobbit Female",
			MeanHeight = 101,
			MeanBmi = 25.6,
			StddevHeight = 5,
			StddevBmi = 4.9,
			Bmimultiplier = 0.1
		};
		_context.Add(hobbitFemaleHWModel);
		hobbitRace.DefaultHeightWeightModelMale = hobbitMaleHWModel;
		hobbitRace.DefaultHeightWeightModelFemale = hobbitFemaleHWModel;
		hobbitRace.DefaultHeightWeightModelNonBinary = hobbitFemaleHWModel;
		hobbitRace.DefaultHeightWeightModelNeuter = hobbitMaleHWModel;
		_races.Add("Hobbit", hobbitRace);
		_context.SaveChanges();

		var isHobbitProg = new FutureProg
		{
			FunctionName = "IsRaceHobbit",
			FunctionComment = "Determines whether someone is the Hobbit race",
			FunctionText = $"return @ch.Race == ToRace({hobbitRace.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isHobbitProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isHobbitProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isHobbitProg);
		_context.SaveChanges();
		_raceProgs["Hobbit"] = isHobbitProg;

		var isHobbitBabyProg = CreateVariantAgeProg(isBabyProg, hobbitRace);
		var isHobbitToddlerProg = CreateVariantAgeProg(isToddlerProg, hobbitRace);
		var isHobbitBoyProg = CreateVariantAgeProg(isBoyProg, hobbitRace);
		var isHobbitGirlProg = CreateVariantAgeProg(isGirlProg, hobbitRace);
		var isHobbitChildProg = CreateVariantAgeProg(isChildProg, hobbitRace);
		var isHobbitYouthProg = CreateVariantAgeProg(isYouthProg, hobbitRace);
		var isHobbitYoungManProg = CreateVariantAgeProg(isYoungManProg, hobbitRace);
		var isHobbitYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, hobbitRace);
		var isHobbitAdultManProg = CreateVariantAgeProg(isAdultManProg, hobbitRace);
		var isHobbitAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, hobbitRace);
		var isHobbitAdultPersonProg = CreateVariantAgeProg(isAdultProg, hobbitRace);
		var isHobbitOldManProg = CreateVariantAgeProg(isOldManProg, hobbitRace);
		var isHobbitOldWomanProg = CreateVariantAgeProg(isOldWomanProg, hobbitRace);
		var isHobbitOldPersonProg = CreateVariantAgeProg(isOldPersonProg, hobbitRace);
		var hobbitPersonValues = new List<(CharacteristicValue Value, double Weight)>();

		void AddHobbitPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++,
				Definition = personDef,
				Name = name,
				Value = basic,
				AdditionalValue = "",
				Default = false,
				Pluralisation = 0,
				FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			hobbitPersonValues.Add((pw, weight));
		}

		AddHobbitPersonWord("hobbit baby", "hobbit baby", isHobbitBabyProg, 1.0);
		AddHobbitPersonWord("hobbit infant", "hobbit baby", isHobbitBabyProg, 1.0);
		AddHobbitPersonWord("hobbit tot", "hobbit baby", isHobbitBabyProg, 1.0);
		AddHobbitPersonWord("hobbit toddler", "hobbit toddler", isHobbitToddlerProg, 5.0);
		AddHobbitPersonWord("hobbit", "hobbit", isHobbitAdultPersonProg, 5.0);
		AddHobbitPersonWord("hobbit person", "hobbit", isHobbitAdultPersonProg, 5.0);
		AddHobbitPersonWord("hobbit individual", "hobbit", isHobbitAdultPersonProg, 1.0);
		AddHobbitPersonWord("male hobbit", "hobbit", isHobbitAdultManProg, 1.0);
		AddHobbitPersonWord("hobbit man", "hobbit", isHobbitAdultManProg, 5.0);
		AddHobbitPersonWord("female hobbit", "hobbit", isHobbitAdultWomanProg, 1.0);
		AddHobbitPersonWord("hobbit woman", "hobbit", isHobbitAdultWomanProg, 5.0);
		AddHobbitPersonWord("hobbit woman", "hobbit", isHobbitAdultWomanProg, 1.0);
		AddHobbitPersonWord("hobbit lady", "hobbit", isHobbitAdultWomanProg, 3.0);
		AddHobbitPersonWord("hobbit lad", "hobbit boy", isHobbitBoyProg, 1.0);
		AddHobbitPersonWord("hobbit boy", "hobbit boy", isHobbitBoyProg, 3.0);
		AddHobbitPersonWord("hobbit child", "hobbit child", isHobbitChildProg, 3.0);
		AddHobbitPersonWord("hobbit youth", "hobbit youth", isHobbitYouthProg, 3.0);
		AddHobbitPersonWord("hobbit adolescent", "hobbit youth", isHobbitYouthProg, 1.0);
		AddHobbitPersonWord("hobbit youngster", "hobbit youth", isHobbitYouthProg, 1.0);
		AddHobbitPersonWord("hobbit juvenile", "hobbit youth", isHobbitYouthProg, 1.0);
		AddHobbitPersonWord("hobbit kid", "hobbit child", isHobbitChildProg, 1.0);
		AddHobbitPersonWord("young hobbit boy", "hobbit boy", isHobbitBoyProg, 1.0);
		AddHobbitPersonWord("young hobbit", "hobbit", isHobbitYoungManProg, 3.0);
		AddHobbitPersonWord("hobbit youngster", "hobbit", isHobbitYoungManProg, 1.0);
		AddHobbitPersonWord("hobbit lass", "hobbit girl", isHobbitYoungWomanProg, 1.0);
		AddHobbitPersonWord("hobbit girl", "hobbit girl", isHobbitGirlProg, 3.0);
		AddHobbitPersonWord("hobbit girl", "hobbit girl", isHobbitGirlProg, 3.0);
		AddHobbitPersonWord("young hobbit girl", "hobbit girl", isHobbitGirlProg, 1.0);
		AddHobbitPersonWord("young hobbit woman", "hobbit girl", isHobbitYoungWomanProg, 1.0);
		AddHobbitPersonWord("young hobbit", "hobbit girl", isHobbitYoungWomanProg, 1.0);
		AddHobbitPersonWord("hobbit maiden", "hobbit hobbit", isHobbitYoungWomanProg, 1.0);
		AddHobbitPersonWord("old hobbit", "old hobbit", isHobbitOldPersonProg, 1.0);
		AddHobbitPersonWord("old hobbit woman", "old hobbit", isHobbitOldWomanProg, 1.0);
		AddHobbitPersonWord("elderly hobbit woman", "old hobbit", isHobbitOldWomanProg, 1.0);
		AddHobbitPersonWord("old hobbit man", "old hobbit", isHobbitOldManProg, 1.0);
		AddHobbitPersonWord("elderly hobbit man", "old hobbit", isHobbitOldManProg, 1.0);
		AddHobbitPersonWord("elderly hobbit", "old hobbit", isHobbitOldPersonProg, 1.0);
		_context.SaveChanges();

		var hobbitPersons = new CharacteristicProfile
		{
			Name = "Hobbit Person Words",
			Definition = new XElement("Values",
				from item in hobbitPersonValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words applicable to the hobbit race",
			TargetDefinition = personDef
		};
		_context.CharacteristicProfiles.Add(hobbitPersons);
		_profiles[hobbitPersons.Name] = hobbitPersons;
		_context.SaveChanges();

		void AddHobbitEthnicity(string name, string blurb, string facialHair, string ethnicGroup, string ethnicSubGroup,
			string bloodGroup, string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(hobbitRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? canSelectHobbitProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", facialHair);
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Hobbit Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddHobbitEthnicity("Harfoot",
			@"Harfoots were the most common type of hobbit, and in their earliest known history they lived in the lower foothills of the Misty Mountains, in the Vales of Anduin, in an area roughly bounded by the Gladden River in the south and the small forested region where later was the Eagles Eyrie near the High Pass to the north.

They were browner of skin than other hobbits, had no beards, and did not wear any footwear. They lived in holes they called smials, a habit which they long preserved, and were on friendly terms with the Dwarves, who traveled through the High Pass.

The Harfoots were the first to migrate westward into Arnor, and there the Dúnedain named them Periannath or halflings, as recorded in Arnorian records around TA 1050. They tended to settle down for long times, and founded numerous villages as far as Weathertop.

By the 1300s of the Third Age, they had reached Bree, which was the westernmost home of any hobbits for a long while.

The Harfoots were joined between TA 1150 and TA 1300 by the Fallohides and some Stoors. The Harfoots took Fallohides, a bolder breed, as their leaders. The Shire was colonized long after this, in TA 1601, mostly by Harfoots.",
			"No_Facial_Hair", "", "", "B Dominant", "Brown_Green_Blue_Eyes", "Black_Brown_Blonde_Grey_Hair",
			"Fair_Swarthy_Skin");
		AddHobbitEthnicity("Stoor",
			@"The Stoors originally dwelt in the southern vales of the river Anduin. During the hobbits' wandering days, after the Harfoots had migrated westward in TA 1050, and the Fallohides later followed them, the Stoors long remained back in the vale of Anduin, but between TA 1150 and 1300 they too migrated west into Eriador.

They were heavier and broader in build than the other hobbits, and had large hands and feet. Among the hobbits, the Stoors most resembled Men and were most friendly to them. Stoors were the only hobbits who normally grew facial hair.

A habit which set them apart from the Harfoots who lived in the mountain foothills, and the Fallohides who lived in forests far to the north, was that Stoors preferred flat lands and riversides. Only Stoors used boats, fished, and could swim. They also wore boots in muddy weather.

Stoorish characteristics and appearances remained amongst the hobbits of the Eastfarthing, Buckland (such as the Brandybucks) and the Bree-hobbits.",
			"All Facial Hair Styles", "", "", "B Dominant", "Brown_Green_Blue_Eyes", "Brown_Blonde_Grey_Hair",
			"Fair_Swarthy_Skin");
		AddHobbitEthnicity("Fallohide",
			@"The Fallohides were the least common of hobbits, and in their earliest known history they lived in the forested region where later was the Eagles Eyrie near the High Pass to the north, in the Vale of Anduin. To their south lived the far more numerous Harfoots, and far south in the Gladden Fields lived the Stoors.

The Fallohides were fair of skin and hair, and none of them ever grew a beard. They were great lovers of the trees and forests, and skilled hunters. Many of them were friends with the Elves, and because of this they were more learned than the other hobbits. They were the first to later learn Westron, and the only ones to preserve some of their old history. They learned Westron from the Men of Arnor, and it was they who first learned writing.

After the Harfoots had migrated westward in the years following TA 1050, the Fallohides followed them around TA 1150. Unlike the Harfoots they crossed far north of Rivendell, and from there later met up with the Harfoots. The Fallohides were more bold and adventurous than the Harfoots, and many of them became leaders of the Harfoot villages. It was probably under Fallohide rule that the Harfoots migrated westward beyond Weathertop and reached Bree. In TA 1601 two Fallohide brothers, Marcho and Blanco, by permission of Argeleb II, the King in Fornost crossed the Brandywine River and colonized the Shire.

After this, the Fallohides mixed more and more with the Harfoots and later the Stoors, until the three Hobbit breeds became one. The influential Took clan had distinct Fallohide traces both in appearance and character, as did the Oldbuck and later Brandybuck clan. Both Bilbo Baggins and Frodo Baggins were part Fallohide, due to their Took and Brandybuck mothers respectively. Other famous Fallohides included Bandobras Bullroarer Took, who slew an Orc leader, and Peregrin Took.",
			"No_Facial_Hair", "", "", "B Dominant", "Blue_Green_Eyes", "Blonde_Red_Grey_Hair", "Fair_Skin");

		var hobbitSkillProg = new FutureProg
		{
			FunctionName = "HobbitSkillStartingValue",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine the opening value for a skill for hobbits at character creation",
			ReturnType = (int)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
		};
		hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = hobbitSkillProg, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)FutureProgVariableTypes.Toon
		});
		hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = hobbitSkillProg, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)FutureProgVariableTypes.Trait
		});
		hobbitSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = hobbitSkillProg, ParameterIndex = 2, ParameterName = "boosts",
			ParameterType = (int)FutureProgVariableTypes.Number
		});
		_context.FutureProgs.Add(hobbitSkillProg);
		_context.SaveChanges();

		var shireCalendar = _context.Calendars.Find(5L);

		void AddHobbitCulture(string name, string description)
		{
			var prog = new FutureProg
			{
				FunctionName = $"CanSelect{name.CollapseString()}Culture",
				Category = "Chargen",
				Subcategory = "Culture",
				FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
				ReturnType = (int)FutureProgVariableTypes.Boolean,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Hobbit"")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);
			AddCulture(name, "Hobbit", description, prog, hobbitSkillProg, shireCalendar);
		}

		AddHobbitCulture("Shire Hobbit",
			@"The Shire was the homeland of the majority of the hobbits in Middle-earth. It was located in the northwestern portion of Middle-earth, in the northern region of Eriador, within the borders of the Kingdom of Arnor.

The Shire was originally divided in four Farthings (Northfarthing, Southfarthing, Eastfarthing, and Westfarthing), but Buckland and later the Westmarch were added to it in the Fourth Age. Within the Farthings there were some smaller, unofficial divisions such as family lands; nearly all the Tooks lived in the Tookland around Tuckborough, for instance. 

In many cases a hobbit's last name indicates where their family came from: Samwise Gamgee's last name derived from Gamwich, where the family originated. Outside the Farthings, Buckland itself was named for the Oldbucks (later Brandybucks). See further Regions of the Shire.");
		AddHobbitCulture("Bree Hobbit",
			@"Bree was an ancient settlement of men in Eriador by the time of the Third Age of Middle-earth, but after the collapse of the North-kingdom, Bree continued to thrive without any central authority or government for many centuries. Bree had become the most westerly settlement of men in all of Middle-earth by the time of the War of the Ring, and became one of only three or four inhabited settlements in all of Eriador. 

At the time of the War of the Ring, Bree-land was the only part of Middle-earth where Men and hobbits dwelt together. Being located on the most important crossroads in the north, on the crossing of the Great East Road and the Greenway, people would pass through Bree.");
		AddHobbitCulture("Rhovanion Hobbit",
			@"Rhovanion, which is the region of Middle Earth East of Eriador and North of Gondor, is the original homeland of all hobbits, who dwelt along the river Anduin. The hobbits left Rhovanion and wandered West, eventually settling in the Shire. Some hobbits, however, eventually left and returned to their homeland in small numbers.

These Hobbits tend to be more worldly than their kin, and have close ties to the Elves of Lothlorien.");

		_context.SaveChanges();

		#endregion Hobbits

		#region Dwarves

		var canSelectDwarfProg = new FutureProg
		{
			FunctionName = "CanSelectDwarfRace",
			Category = "Character",
			Subcategory = "Race",
			FunctionComment = "Determines if the character select the dwarf race in chargen.",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = @"// You might consider checking RPP
return true"
		};
		canSelectDwarfProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectDwarfProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(canSelectDwarfProg);
		var dwarfAttributeProg = new FutureProg
		{
			FunctionName = "DwarfBonusAttributes",
			Category = "Character",
			Subcategory = "Attributes",
			FunctionComment =
				"This prog is called for each attribute for dwarves at chargen time and the resulting value is applied as a modifier to that attribute.",
			ReturnType = (long)FutureProgVariableTypes.Number,
			StaticType = 0,
			FunctionText = @"// You might consider a switch on attributes like so:
//
// switch (@trait.Name)
//   case (""Strength"")
//     return 2
// end switch
return 0"
		};
		dwarfAttributeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = dwarfAttributeProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(dwarfAttributeProg);
		_context.SaveChanges();

		var (dwarfBlood, dwarfSweat, _, _) = CreateBloodAndSweat("Dwarf");

		var dwarfRace = new Race
		{
			Name = "Dwarf",
			Description = @"The Dwarves are a proud and ancient race of skilled craftsmen who inhabit the mountains of Middle-earth. They are a short and stocky people, with thick beards and a love of gold and precious gems. Dwarves are known for their skill in metalworking, stonecutting, and engineering, and are renowned for their sturdy and well-made craftsmanship.

Despite their love of gold and material wealth, Dwarves are a fiercely independent and honorable people. They have a strong sense of duty and responsibility, and are deeply loyal to their families and clans. Dwarves are also fiercely protective of their homes and possessions, and are willing to defend them with great determination and ferocity.

Dwarves are divided into several different clans, each with its own distinct culture and way of life. The most well-known of these clans are the House of Durin, the House of Thrain, and the House of Bofur. The House of Durin is the most powerful and influential of the clans, and is closely associated with the royal family of the Dwarves. The House of Thrain is known for its military prowess, and has a long tradition of service to the kingdom. The House of Bofur is known for its artistic and scholarly achievements, and has produced many of the Dwarves' greatest poets, musicians, and scholars. All of these clans are deeply devoted to the defense and preservation of the Dwarven kingdom, and are willing to lay down their lives for their people.",
			AllowedGenders = _humanRace.AllowedGenders,
			ParentRace = humanoid,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.75,
			CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
			DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NaturalArmourQuality = 3,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SweatRateInLitresPerMinute = 0.5,
			SizeStanding = 5,
			SizeProne = 5,
			SizeSitting = 5,
			CommunicationStrategyType = "humanoid",
			DefaultHandedness = 1,
			HandednessOptions = "1 3",
			MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
			MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = -20,
			TemperatureRangeCeiling = 40,
			BodypartSizeModifier = 0,
			BodypartHealthMultiplier = 1.25,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			ChildAge = 5,
			YouthAge = 20,
			YoungAdultAge = 45,
			AdultAge = 75,
			ElderAge = 200,
			VenerableAge = 300,
			AttributeBonusProg = dwarfAttributeProg,
			AvailabilityProg = canSelectDwarfProg,
			BaseBody = body,
			BloodLiquid = dwarfBlood,
			BloodModel = humanoid.BloodModel,
			NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
			NaturalArmourType = humanoid.NaturalArmourType,
			RaceButcheryProfile = null,
			SweatLiquid = dwarfSweat
		};
		_context.Races.Add(dwarfRace);
		var dwarfMaleHWModel = new HeightWeightModel
		{
			Name = "Dwarf Male",
			MeanHeight = 142,
			MeanBmi = 25.6,
			StddevHeight = 7.6,
			StddevBmi = 3.7,
			Bmimultiplier = 0.1
		};
		_context.Add(dwarfMaleHWModel);
		var dwarfFemaleHWModel = new HeightWeightModel
		{
			Name = "Dwarf Female",
			MeanHeight = 137,
			MeanBmi = 25.6,
			StddevHeight = 5,
			StddevBmi = 4.9,
			Bmimultiplier = 0.1
		};
		_context.Add(dwarfFemaleHWModel);
		dwarfRace.DefaultHeightWeightModelMale = dwarfMaleHWModel;
		dwarfRace.DefaultHeightWeightModelFemale = dwarfFemaleHWModel;
		dwarfRace.DefaultHeightWeightModelNonBinary = dwarfFemaleHWModel;
		dwarfRace.DefaultHeightWeightModelNeuter = dwarfMaleHWModel;
		_races.Add("Dwarf", dwarfRace);
		_context.SaveChanges();

		var isDwarfProg = new FutureProg
		{
			FunctionName = "IsDwarfRace",
			FunctionComment = "Determines whether someone is the Dwarf race",
			FunctionText = $"return @ch.Race == ToRace({dwarfRace.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isDwarfProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isDwarfProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isDwarfProg);
		_context.SaveChanges();
		_raceProgs["Dwarf"] = isDwarfProg;

		var isDwarfBabyProg = CreateVariantAgeProg(isBabyProg, dwarfRace);
		var isDwarfToddlerProg = CreateVariantAgeProg(isToddlerProg, dwarfRace);
		var isDwarfBoyProg = CreateVariantAgeProg(isBoyProg, dwarfRace);
		var isDwarfGirlProg = CreateVariantAgeProg(isGirlProg, dwarfRace);
		var isDwarfChildProg = CreateVariantAgeProg(isChildProg, dwarfRace);
		var isDwarfYouthProg = CreateVariantAgeProg(isYouthProg, dwarfRace);
		var isDwarfYoungManProg = CreateVariantAgeProg(isYoungManProg, dwarfRace);
		var isDwarfYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, dwarfRace);
		var isDwarfAdultManProg = CreateVariantAgeProg(isAdultManProg, dwarfRace);
		var isDwarfAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, dwarfRace);
		var isDwarfAdultPersonProg = CreateVariantAgeProg(isAdultProg, dwarfRace);
		var isDwarfOldManProg = CreateVariantAgeProg(isOldManProg, dwarfRace);
		var isDwarfOldWomanProg = CreateVariantAgeProg(isOldWomanProg, dwarfRace);
		var isDwarfOldPersonProg = CreateVariantAgeProg(isOldPersonProg, dwarfRace);
		var dwarfPersonValues = new List<(CharacteristicValue Value, double Weight)>();

		void AddDwarfPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++,
				Definition = personDef,
				Name = name,
				Value = basic,
				AdditionalValue = "",
				Default = false,
				Pluralisation = 0,
				FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			dwarfPersonValues.Add((pw, weight));
		}

		AddDwarfPersonWord("dwarven baby", "dwarven baby", isDwarfBabyProg, 1.0);
		AddDwarfPersonWord("dwarven infant", "dwarven baby", isDwarfBabyProg, 1.0);
		AddDwarfPersonWord("dwarven tot", "dwarven baby", isDwarfBabyProg, 1.0);
		AddDwarfPersonWord("dwarven toddler", "dwarven toddler", isDwarfToddlerProg, 5.0);
		AddDwarfPersonWord("dwarf", "dwarf", isDwarfAdultPersonProg, 5.0);
		AddDwarfPersonWord("dwarven person", "dwarf", isDwarfAdultPersonProg, 5.0);
		AddDwarfPersonWord("dwarf person", "dwarf", isDwarfAdultPersonProg, 1.0);
		AddDwarfPersonWord("dwarven individual", "dwarf", isDwarfAdultPersonProg, 1.0);
		AddDwarfPersonWord("male dwarf", "dwarf", isDwarfAdultManProg, 1.0);
		AddDwarfPersonWord("dwarven man", "dwarf", isDwarfAdultManProg, 5.0);
		AddDwarfPersonWord("dwarf man", "dwarf", isDwarfAdultManProg, 1.0);
		AddDwarfPersonWord("female dwarf", "dwarf", isDwarfAdultWomanProg, 1.0);
		AddDwarfPersonWord("dwarven woman", "dwarf", isDwarfAdultWomanProg, 5.0);
		AddDwarfPersonWord("dwarf woman", "dwarf", isDwarfAdultWomanProg, 1.0);
		AddDwarfPersonWord("dwarven lady", "dwarf", isDwarfAdultWomanProg, 3.0);
		AddDwarfPersonWord("dwarf lady", "dwarf", isDwarfAdultWomanProg, 1.0);
		AddDwarfPersonWord("dwarven lad", "dwarven boy", isDwarfBoyProg, 1.0);
		AddDwarfPersonWord("dwarf boy", "dwarven boy", isDwarfBoyProg, 3.0);
		AddDwarfPersonWord("dwarven boy", "dwarven boy", isDwarfBoyProg, 3.0);
		AddDwarfPersonWord("dwarven child", "dwarven child", isDwarfChildProg, 3.0);
		AddDwarfPersonWord("dwarven youth", "dwarven youth", isDwarfYouthProg, 3.0);
		AddDwarfPersonWord("dwarven adolescent", "dwarven youth", isDwarfYouthProg, 1.0);
		AddDwarfPersonWord("dwarven youngster", "dwarven youth", isDwarfYouthProg, 1.0);
		AddDwarfPersonWord("dwarven juvenile", "dwarven youth", isDwarfYouthProg, 1.0);
		AddDwarfPersonWord("dwarven kid", "dwarven child", isDwarfChildProg, 1.0);
		AddDwarfPersonWord("young dwarven boy", "dwarven boy", isDwarfBoyProg, 1.0);
		AddDwarfPersonWord("young dwarf", "dwarf", isDwarfYoungManProg, 3.0);
		AddDwarfPersonWord("dwarven youngster", "dwarf", isDwarfYoungManProg, 1.0);
		AddDwarfPersonWord("dwarven lass", "dwarven girl", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("dwarf lass", "dwarven girl", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("dwarven girl", "dwarven girl", isDwarfGirlProg, 3.0);
		AddDwarfPersonWord("dwarf girl", "dwarven girl", isDwarfGirlProg, 3.0);
		AddDwarfPersonWord("young dwarven girl", "dwarven girl", isDwarfGirlProg, 1.0);
		AddDwarfPersonWord("young dwarf girl", "dwarven girl", isDwarfGirlProg, 1.0);
		AddDwarfPersonWord("young dwarven woman", "dwarven girl", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("young dwarf", "dwarven girl", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("dwarf maiden", "dwarven dwarf", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("dwarven maiden", "dwarven dwarf", isDwarfYoungWomanProg, 1.0);
		AddDwarfPersonWord("old dwarf", "old dwarf", isDwarfOldPersonProg, 1.0);
		AddDwarfPersonWord("old dwarven woman", "old dwarf", isDwarfOldWomanProg, 1.0);
		AddDwarfPersonWord("elderly dwarven woman", "old dwarf", isDwarfOldWomanProg, 1.0);
		AddDwarfPersonWord("old dwarven man", "old dwarf", isDwarfOldManProg, 1.0);
		AddDwarfPersonWord("elderly dwarven man", "old dwarf", isDwarfOldManProg, 1.0);
		AddDwarfPersonWord("elderly dwarf", "old dwarf", isDwarfOldPersonProg, 1.0);
		_context.SaveChanges();

		var dwarfPersons = new CharacteristicProfile
		{
			Name = "Dwarf Person Words",
			Definition = new XElement("Values",
				from item in dwarfPersonValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words applicable to the Dwarf race",
			TargetDefinition = personDef
		};
		_context.CharacteristicProfiles.Add(dwarfPersons);
		_profiles[dwarfPersons.Name] = dwarfPersons;
		_context.SaveChanges();

		void AddDwarfEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
			string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(dwarfRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? canSelectDwarfProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Dwarf Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddDwarfEthnicity("Longbeard",
			@"Durin's Folk were the Longbeards (Sigin-tarâg in Khuzdul), one of the seven kindreds of Dwarves whose leaders were from the House of Durin. Their first king was named Durin, who was one of the seven Fathers of the Dwarves.",
			"Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
		AddDwarfEthnicity("Firebeard",
			@"The Firebeards were one of the seven houses of the Dwarves. They were originally paired with the Broadbeams. The ancestor of the Firebeards was among the oldest (together with the ancestors of the Broadbeams and Longbeards) of the Seven Ancestors of the Dwarves.

The Firebeards (with the Broadbeams) awoke in the Blue Mountains and lived there throughout the history of their people. These two houses built the great Dwarven cities of Nogrod and Belegost in the Blue Mountains, and dwelt in them before their ruining in the War of Wrath.",
			"Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Red_Grey_Hair", "Fair_Swarthy_Skin");
		AddDwarfEthnicity("Broadbeam",
			@"The Broadbeams were one of the seven houses of the Dwarves. They were originally paired with the Firebeards. The ancestor of the Broadbeams was among the oldest of the Seven Fathers of the Dwarves (together with the ancestors of the Firebeards and Longbeards).

The Broadbeams (with the Firebeards) awoke in the Blue Mountains and lived there throughout the history of their people. These two houses built the great Dwarven cities of Nogrod and Belegost in the Blue Mountains, and dwelt in them before their ruining in the War of Wrath. It is not clear whether they shared the two cities or whether each house dwelt in its own. In an earlier version of the legendarium the two cities are clearly inhabited by separate houses; however, Belegost is said to be the home of the Longbeards.",
			"Western", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
		AddDwarfEthnicity("Ironfist",
			@"The Ironfists were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Stiffbeards.

The locations of the four Dwarven clans, including the Ironfists, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Ironfists and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
			"Eastern", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Fair_Swarthy_Skin");
		AddDwarfEthnicity("Stiffbeard",
			@"The Stiffbeards were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Ironfists.

The locations of the four Dwarven clans, including the Stiffbeards, who lived in the East, are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Stiffbeards and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
			"Eastern", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Dark_Skin");
		AddDwarfEthnicity("Blacklock",
			@"The Blacklocks were one of the seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Stonefoots.

The locations of the four Dwarven clans, including the Blacklocks, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Blacklocks and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
			"Eastern", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin");
		AddDwarfEthnicity("Stonefeet",
			@"The Stonefoots were one of seven houses of the Dwarves that dwelt in Rhûn. They were originally paired with the Blacklocks.

The locations of the four Dwarven clans, including the Stonefoots, who lived in the East are unknown. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

Late in the Third Age, when war and terror grew in the East itself, considerable numbers of Stonefoots and dwarves of the Eastern clans left their ancient homelands. They sought refuge in Middle-earth's western lands.",
			"Eastern", "", "B Dominant", "Brown_Green_Eyes", "Brown_Grey_Hair", "Swarthy_Skin");

		var dwarfSkillProg = new FutureProg
		{
			FunctionName = "DwarfSkillStartingValue",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine the opening value for a skill for dwarves at character creation",
			ReturnType = (int)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
		};
		dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = dwarfSkillProg, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)FutureProgVariableTypes.Toon
		});
		dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = dwarfSkillProg, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)FutureProgVariableTypes.Trait
		});
		dwarfSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = dwarfSkillProg, ParameterIndex = 2, ParameterName = "boosts",
			ParameterType = (int)FutureProgVariableTypes.Number
		});
		_context.FutureProgs.Add(dwarfSkillProg);
		_context.SaveChanges();

		var sindarinCalendar = _context.Calendars.Find(2L);

		void AddDwarfCulture(string name, string description)
		{
			var prog = new FutureProg
			{
				FunctionName = $"CanSelect{name.CollapseString()}Culture",
				Category = "Chargen",
				Subcategory = "Culture",
				FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
				ReturnType = (int)FutureProgVariableTypes.Boolean,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Dwarf"")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);
			AddCulture(name, "Dwarven", description, prog, dwarfSkillProg, sindarinCalendar);
		}

		AddDwarfCulture("Khazad-dum",
			@"Your character grew up in Khazad-dum. Khazad-dum, latterly known as Moria (The Black Chasm, The Black Pit), was the grandest and most famous of the mansions of the Dwarves. There, for many thousands of years, a thriving Dwarvish community created the greatest city ever known.

It lay in the central parts of the Misty Mountains, tunnelled and carved through the living rock of the mountains themselves. By the Second Age a traveller could pass through it from the west of the range to the east.");
		AddDwarfCulture("Nogrod",
			@"Your character grew up in the remnant tribes of Nogrod. Nogrod was one of two Dwarven cities in the Ered Luin, the other being Belegost, that prospered during the First Age. It was home to the Dwarves of Nogrod. 

At the end of the First Age, Nogrod was ruined in the War of Wrath, and around the fortieth year of the Second Age the Dwarves of the Blue Mountains began to migrate to Khazad-dum, abandoning Nogrod and Belegost. However, there always remained some Dwarves on the eastern side of the Blue Mountains in days afterward.");
		AddDwarfCulture("Belegost",
			@"Your character grew up in the remnant tribes of Belegost. Belegost was one of two great Dwarven cities in the Ered Luin, the other being Nogrod. It was home to the Dwarves of Belegost. Belegost lay in the north central part of the Ered Luin, north of Nogrod and northeast of Mount Dolmed, guarding one of the only passes through the mountain range.

At the end of the First Age, Belegost was ruined in the War of Wrath, and around the fortieth year of the Second Age the Dwarves of the Blue Mountains began to migrate to Khazad-dum, abandoning Nogrod and Belegost. However, there always remained some Dwarves on the eastern side of the Blue Mountains in days afterward.");
		AddDwarfCulture("Grey Mountains",
			@"The Dwarves of the Grey Mountains were the Dwarves of Durin's Folk who lived in the Grey Mountains in northern Middle-earth.

For many years, Durin's folk prospered in the Ered Mithrin but, the dragons of the Forodwaith eventually multiplied and became strong, and in T.A. 2570 the Dragons made war on the dwarves, sacking and plundering their halls. The Dwarves held out for around twenty years, but finally in 2589 the Dragons attacked the halls of King Dáin I. King Dáin, and his second son Frór, were killed by a Cold-drake outside his door to his halls.

Following the death of their king, most of Durin's Folk abandoned the Grey Mountains. In 2590, King Thrór and his uncle Borin returned to the Erebor with the Arkenstone to re-establish the Kingdom under the Mountain. However, Thrór's younger brother Grór led others to the Iron Hills.");
		AddDwarfCulture("Erebor",
			@"The Kingdom under the Mountain was the name given to the Dwarf realm of Erebor. It was founded in T.A. 1999 when Thráin I came to the Lonely Mountain and discovered the Arkenstone. The kingdom lasted until 2770, when Smaug the Dragon invaded and either killed the Dwarves or forced them to leave. When Smaug was slain in 2941, Dáin Ironfoot became King Dáin II and the kingdom was restored.

Under Dáin's rule the Dwarves of the Lonely Mountain became very rich and prosperous. They rebuilt the town of Dale, their trade greatly increased with their kinsman in the Iron Hills once again and with Men; and the Lonely Mountain was restored to its original greatness.");
		AddDwarfCulture("Iron Hills",
			@"The Dwarves of the Iron Hills were Dwarves belonging to the house of the Longbeards, otherwise known as Durin's Folk, who lived in the Iron Hills. They became well-known for making a metal mesh that could be used for making flexible items like leg-coverings.

In the Third Age, many Longboard Dwarves lived in the Grey Mountains, but they were greatly troubled by Dragons in that region. After King Dáin I was slain by one of these dragons, his surviving sons led an exodus into the east. Dáin's elder son Thrór recreated the Kingdom under the Mountain at Erebor, while his younger brother Grór led a part of the people further into the east to join their kindred living in the Iron Hills.

Grór settled in the Iron Hills in the year T.A. 2590 and became Lord of the Iron Hills. During his reign, the realm became the strongest in the North, being the only realm standing between Sauron and his plans to destroy Rivendell and taking back the lands of Angmar. Also, following the Sack of Erebor many of Durin's folk fleeing from Smaug and those wandering in exile, except for Thrór and his small company of family and followers, came to the Iron Hills, bolstering their numbers.");
		AddDwarfCulture("Aglarond",
			@"The Glittering Caves of Aglarond were a cave system in the White Mountains behind Helm's Deep. Gimli son of Glóin led a large group of Dwarves of Erebor there after the War of the Ring and became the Lord of the Glittering Caves. 

His Dwarves performed great services for the Rohirrim and the Men of Gondor, of which the most famous was the making of new gates for Minas Tirith, forged out of mithril and steel. 

The dwarves of Aglarond restored the Hornburg following the War of the Ring, and it became a fortress they shared with the Rohirrim. 

The Dwarves of the Glittering Caves carefully tended the stone walls and opened new ways and chambers and hung lamps that filled the caverns with light.");
		AddDwarfCulture("Rhûn",
			@"Rhûn was inhabited by four of the Dwarf clans: the Ironfists, Stiffbeards, Blacklocks and Stonefoots. The distance between their mansions in the East and the Misty Mountains, specifically Gundabad, was said to be as great or greater than that of Gundabad's distance from the Blue Mountains in the West.

In the Third Age, Dwarves of those kingdoms journeyed out of Rhûn to join all Middle-earth's other Dwarf clans in the War of the Dwarves and Orcs, which was fought in and under the Misty Mountains. After this war, the survivors returned home. 

Late in the Third Age, when war and terror grew in Rhûn itself, considerable numbers of its Dwarves left their ancient homelands. They sought refuge in Middle-earth's western lands.");
		AddDwarfCulture("Dunland",
			@"The Exiled Realm in Dunland was established by Dwarves fleeing from Erebor after it was sacked by Smaug. This is where Thrór departed when he and his companion Nár journeyed to Moria in TA 2790. 

After the Battle of Azanulbizar, provoked by the Orcs' brutal slaying of Thrór, Thráin II and Thorin led the remnants of their followers back to Dunland but soon left (to eventually settle in the Ered Luin).");

		_context.SaveChanges();

		#endregion Dwarves

		#region Orcs

		var canSelectOrcProg = new FutureProg
		{
			FunctionName = "CanSelectOrcRace",
			Category = "Character",
			Subcategory = "Race",
			FunctionComment = "Determines if the character select the orc race in chargen.",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = @"// You might consider checking RPP
return true"
		};
		canSelectOrcProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectOrcProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(canSelectOrcProg);
		var orcAttributeProg = new FutureProg
		{
			FunctionName = "OrcBonusAttributes",
			Category = "Character",
			Subcategory = "Attributes",
			FunctionComment =
				"This prog is called for each attribute for orcs at chargen time and the resulting value is applied as a modifier to that attribute.",
			ReturnType = (long)FutureProgVariableTypes.Number,
			StaticType = 0,
			FunctionText = @"// You might consider a switch on attributes like so:
//
// switch (@trait.Name)
//   case (""Strength"")
//     return 2
// end switch
return 0"
		};
		orcAttributeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = orcAttributeProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(orcAttributeProg);
		_context.SaveChanges();

		var (orcBlood, orcSweat, _, _) = CreateBloodAndSweat("Orc");

		var orcRace = new Race
		{
			Name = "Orc",
			Description = @"The Orcs are a cruel and brutal race of creatures who inhabit the dark and shadowy regions of Middle-earth. They are a fecund and fearsome people, with a love of violence and destruction. Orcs are known for their strength and ferocity in battle, and are feared and hated by the other peoples of Middle-earth.

Despite their reputation as mindless beasts, Orcs are a highly organized and hierarchical society. They have a strict system of laws and customs, and are highly disciplined and obedient to their superiors. Orcs value strength and power above all else, and will do whatever it takes to gain and maintain it.

Orcish cultural practices are centered around warfare and domination. Orcs are constantly at war with one another and with the other peoples of Middle-earth. They believe that the strongest and most ruthless will rise to the top, and will do whatever it takes to prove themselves worthy. Orcish society is highly patriarchal, and women are often treated as little more than property. Orcs also have a strong tradition of slavery, and will enslave any creatures that they conquer in battle. Orcs are also known for their love of torture and cruelty, and will often inflict great suffering on their enemies for their own enjoyment.",
			AllowedGenders = _humanRace.AllowedGenders,
			ParentRace = humanoid,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.75,
			CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
			DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NaturalArmourQuality = 3,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SweatRateInLitresPerMinute = 0.5,
			SizeStanding = 5,
			SizeProne = 5,
			SizeSitting = 5,
			CommunicationStrategyType = "humanoid",
			DefaultHandedness = 1,
			HandednessOptions = "1 3",
			MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
			MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = -10,
			TemperatureRangeCeiling = 50,
			BodypartSizeModifier = 0,
			BodypartHealthMultiplier = 1,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			ChildAge = 3,
			YouthAge = 9,
			YoungAdultAge = 14,
			AdultAge = 22,
			ElderAge = 40,
			VenerableAge = 60,
			AttributeBonusProg = orcAttributeProg,
			AvailabilityProg = canSelectOrcProg,
			BaseBody = body,
			BloodLiquid = orcBlood,
			BloodModel = humanoid.BloodModel,
			NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
			NaturalArmourType = humanoid.NaturalArmourType,
			RaceButcheryProfile = null,
			SweatLiquid = orcSweat
		};
		_context.Races.Add(orcRace);
		var orcMaleHWModel = new HeightWeightModel
		{
			Name = "Orc Male",
			MeanHeight = 147,
			MeanBmi = 23,
			StddevHeight = 7.6,
			StddevBmi = 3,
			Bmimultiplier = 0.1
		};
		_context.Add(orcMaleHWModel);
		var orcFemaleHWModel = new HeightWeightModel
		{
			Name = "Orc Female",
			MeanHeight = 142,
			MeanBmi = 23.5,
			StddevHeight = 5,
			StddevBmi = 3,
			Bmimultiplier = 0.1
		};
		_context.Add(orcFemaleHWModel);
		orcRace.DefaultHeightWeightModelMale = orcMaleHWModel;
		orcRace.DefaultHeightWeightModelFemale = orcFemaleHWModel;
		orcRace.DefaultHeightWeightModelNonBinary = orcFemaleHWModel;
		orcRace.DefaultHeightWeightModelNeuter = orcMaleHWModel;
		_races.Add("Orc", orcRace);
		_context.SaveChanges();

		var isOrcProg = new FutureProg
		{
			FunctionName = "IsOrcRace",
			FunctionComment = "Determines whether someone is the Orc race",
			FunctionText = $"return @ch.Race == ToRace({orcRace.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isOrcProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isOrcProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isOrcProg);
		_context.SaveChanges();
		_raceProgs["Orc"] = isOrcProg;

		var isOrcBabyProg = CreateVariantAgeProg(isBabyProg, orcRace);
		var isOrcToddlerProg = CreateVariantAgeProg(isToddlerProg, orcRace);
		var isOrcBoyProg = CreateVariantAgeProg(isBoyProg, orcRace);
		var isOrcGirlProg = CreateVariantAgeProg(isGirlProg, orcRace);
		var isOrcChildProg = CreateVariantAgeProg(isChildProg, orcRace);
		var isOrcYouthProg = CreateVariantAgeProg(isYouthProg, orcRace);
		var isOrcYoungManProg = CreateVariantAgeProg(isYoungManProg, orcRace);
		var isOrcYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, orcRace);
		var isOrcAdultManProg = CreateVariantAgeProg(isAdultManProg, orcRace);
		var isOrcAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, orcRace);
		var isOrcAdultPersonProg = CreateVariantAgeProg(isAdultProg, orcRace);
		var isOrcOldManProg = CreateVariantAgeProg(isOldManProg, orcRace);
		var isOrcOldWomanProg = CreateVariantAgeProg(isOldWomanProg, orcRace);
		var isOrcOldPersonProg = CreateVariantAgeProg(isOldPersonProg, orcRace);
		var orcPersonValues = new List<(CharacteristicValue Value, double Weight)>();

		void AddOrcPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++,
				Definition = personDef,
				Name = name,
				Value = basic,
				AdditionalValue = "",
				Default = false,
				Pluralisation = 0,
				FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			orcPersonValues.Add((pw, weight));
		}

		AddOrcPersonWord("orc baby", "orc baby", isOrcBabyProg, 1.0);
		AddOrcPersonWord("orc infant", "orc baby", isOrcBabyProg, 1.0);
		AddOrcPersonWord("orc tot", "orc baby", isOrcBabyProg, 1.0);
		AddOrcPersonWord("orc toddler", "orc toddler", isOrcToddlerProg, 5.0);
		AddOrcPersonWord("orc", "orc", isOrcAdultPersonProg, 5.0);
		AddOrcPersonWord("orkish person", "orc", isOrcAdultPersonProg, 5.0);
		AddOrcPersonWord("orc person", "orc", isOrcAdultPersonProg, 1.0);
		AddOrcPersonWord("orkish individual", "orc", isOrcAdultPersonProg, 1.0);
		AddOrcPersonWord("male orc", "orc", isOrcAdultManProg, 1.0);
		AddOrcPersonWord("orkish man", "orc", isOrcAdultManProg, 5.0);
		AddOrcPersonWord("orc man", "orc", isOrcAdultManProg, 1.0);
		AddOrcPersonWord("female orc", "orc", isOrcAdultWomanProg, 1.0);
		AddOrcPersonWord("orkish woman", "orc", isOrcAdultWomanProg, 5.0);
		AddOrcPersonWord("orc woman", "orc", isOrcAdultWomanProg, 1.0);
		AddOrcPersonWord("orkish lady", "orc", isOrcAdultWomanProg, 3.0);
		AddOrcPersonWord("orc lady", "orc", isOrcAdultWomanProg, 1.0);
		AddOrcPersonWord("orc lad", "orc boy", isOrcBoyProg, 1.0);
		AddOrcPersonWord("orc boy", "orc boy", isOrcBoyProg, 3.0);
		AddOrcPersonWord("orc child", "orc child", isOrcChildProg, 3.0);
		AddOrcPersonWord("orkish youth", "orc youth", isOrcYouthProg, 3.0);
		AddOrcPersonWord("orkish adolescent", "orc youth", isOrcYouthProg, 1.0);
		AddOrcPersonWord("orkish youngster", "orc youth", isOrcYouthProg, 1.0);
		AddOrcPersonWord("orkish juvenile", "orc youth", isOrcYouthProg, 1.0);
		AddOrcPersonWord("orkish kid", "orc child", isOrcChildProg, 1.0);
		AddOrcPersonWord("young orkish boy", "orc boy", isOrcBoyProg, 1.0);
		AddOrcPersonWord("young orc", "orc", isOrcYoungManProg, 3.0);
		AddOrcPersonWord("orkish youngster", "orc", isOrcYoungManProg, 1.0);
		AddOrcPersonWord("orkish lass", "orc girl", isOrcYoungWomanProg, 1.0);
		AddOrcPersonWord("orc lass", "orc girl", isOrcYoungWomanProg, 1.0);
		AddOrcPersonWord("orkish girl", "orc girl", isOrcGirlProg, 3.0);
		AddOrcPersonWord("orc girl", "orc girl", isOrcGirlProg, 3.0);
		AddOrcPersonWord("young orkish girl", "orc girl", isOrcGirlProg, 1.0);
		AddOrcPersonWord("young orc girl", "orc girl", isOrcGirlProg, 1.0);
		AddOrcPersonWord("young orkish woman", "orc girl", isOrcYoungWomanProg, 1.0);
		AddOrcPersonWord("young orc", "orc girl", isOrcYoungWomanProg, 1.0);
		AddOrcPersonWord("old orc", "old orc", isOrcOldPersonProg, 1.0);
		AddOrcPersonWord("old orkish woman", "old orc", isOrcOldWomanProg, 1.0);
		AddOrcPersonWord("elderly orkish woman", "old orc", isOrcOldWomanProg, 1.0);
		AddOrcPersonWord("old orkish man", "old orc", isOrcOldManProg, 1.0);
		AddOrcPersonWord("elderly orkish man", "old orc", isOrcOldManProg, 1.0);
		AddOrcPersonWord("elderly orc", "old orc", isOrcOldPersonProg, 1.0);
		_context.SaveChanges();

		var orcPersons = new CharacteristicProfile
		{
			Name = "Orc Person Words",
			Definition = new XElement("Values",
				from item in orcPersonValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words applicable to the Orc race",
			TargetDefinition = personDef
		};
		_context.CharacteristicProfiles.Add(orcPersons);
		_profiles[orcPersons.Name] = orcPersons;
		_context.SaveChanges();

		void AddOrcEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
			string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(orcRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? canSelectOrcProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Orc Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddOrcEthnicity("Uruk",
			@"Orcs were the primary foot soldiers of the Dark Lords' armies and sometimes the weakest (but most numerous) of their servants. They were created by the first Dark Lord, Morgoth, before the First Age and served him and later his successor in their quest to dominate Middle-earth. Before Oromë first found the Elves at Cuiviénen, Melkor kidnapped some of them and cruelly tortured them, twisting them into the first Orcs.

Orcs were cruel, sadistic, black-hearted, vicious, and hateful of everybody and everything, particularly the orderly and prosperous. Physically, they were short in stature and humanoid in shape. They were generally squat, broad, flat-nosed, sallow-skinned, bow-legged, with wide mouths and slant eyes, long arms, dark skin, and fangs. They were roughly humanoid in shape with pointed ears, sharpened teeth and grimy skin. Their appearance was considered revolting by most of the other races.",
			"", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin");
		AddOrcEthnicity("Uruk-Hai",
			@"Uruk-hai were brutal warriors of Middle-earth, and the strongest Orcs, who dwelt in Isengard.

The Uruks first appeared out of Mordor in TA 2475, when they assaulted and managed to conquer Ithilien and destroyed the city of Osgiliath. The Uruks in the service of Barad-dûr used the symbol of the red Eye of Sauron, which was also painted on their shields.

Uruk-hai were later bred by the Wizard Saruman the White late in the Third Age, by his dark arts in the pits of Isengard. In the War of the Ring, the Uruk-hai made up a large part of Saruman's army, together with the Dunlendings, man-enemies of Rohan.",
			"", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair", "Swarthy_Skin", false);

		var orcSkillProg = new FutureProg
		{
			FunctionName = "OrcSkillStartingValue",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine the opening value for a skill for orcs at character creation",
			ReturnType = (int)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
		};
		orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = orcSkillProg, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)FutureProgVariableTypes.Toon
		});
		orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = orcSkillProg, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)FutureProgVariableTypes.Trait
		});
		orcSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = orcSkillProg, ParameterIndex = 2, ParameterName = "boosts",
			ParameterType = (int)FutureProgVariableTypes.Number
		});
		_context.FutureProgs.Add(orcSkillProg);
		_context.SaveChanges();

		void AddOrcCulture(string name, string description)
		{
			var prog = new FutureProg
			{
				FunctionName = $"CanSelect{name.CollapseString()}Culture",
				Category = "Chargen",
				Subcategory = "Culture",
				FunctionComment = $"Used to determine whether you can pick the {name} culture in Chargen.",
				ReturnType = (int)FutureProgVariableTypes.Boolean,
				AcceptsAnyParameters = false,
				Public = false,
				StaticType = 0,
				FunctionText = @"return @ch.Special or @ch.Race == ToRace(""Orc"") or @ch.Race == ToRace(""Troll"")"
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)FutureProgVariableTypes.Toon
			});
			_context.FutureProgs.Add(prog);
			AddCulture(name, "Orc", description, prog, orcSkillProg, sindarinCalendar);
		}

		AddOrcCulture("Misty Mountains Orc",
			@"Mountain Orcs, also known as Orcs of the Mountain, were a sub-race of Orcs that lived in the Misty Mountains.

In the year TA 2790, two tribes of Mountain Orcs; the Gundabad and Guldur Orcs managed to reclaim the ancient dwarf kingdom of Moria.");
		AddOrcCulture("Grey Mountains Orc",
			@"The Grey Mountains, or the Ered Mithrin, was a large mountain range in Middle-earth located to the north of Rhovanion. The Grey Mountains were originally part of the Iron Mountains, the ancient mountain range in the north of Middle-earth, making them one of the oldest mountain ranges in Arda. After the War of Wrath, the Iron Mountains were broken, leaving the Grey Mountains as a separate range and a northern spur of the newer Misty Mountains.

The Dwarves of Durin's Folk considered the Ered Mithrin as part of their land as far back as the reign of Durin I. Because of constant attack by both Orcs of Morgoth and possibly Dragons, they were not heavily explored or settled until the Third Age. By the end of the Third Age all Dwarven strongholds had been abandoned or raided by dragons, and the Grey Mountains served only to divide Forodwaith from Wilderland. It subsequently became home to many Orc tribes.");
		AddOrcCulture("Isengard Orc",
			@"Isengard Orcs also known as Isengard Goblins or Orcs of Isengard and sometimes as Saruman's Orcs were a Goblin tribe originally from the Misty Mountains in the service of the traitorous wizard Saruman");
		AddOrcCulture("Mirkwood Orc",
			@"Many of the Orc tribes of Mirkwood serve under the control of Dol Guldur, serving the Necromancer's wishes. These Orcs are friends of spiders and other foul things, and hate the Elves who hunt them above all else.");
		AddOrcCulture("Mordorian Orc",
			@"Mordorian Orcs live in the mountains and plains of Mordor, as well as several of the surrounding mountain ranges. They are closer to the Dark Lord than their more distant brethren and customarily serve his wishes. This tribe is also most likely to use Black Speech.");

		_context.SaveChanges();

		#endregion Orcs

		#region Trolls

		var canSelectTrollProg = new FutureProg
		{
			FunctionName = "CanSelectTrollRace",
			Category = "Character",
			Subcategory = "Race",
			FunctionComment = "Determines if the character select the troll race in chargen.",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			StaticType = 0,
			FunctionText = @"// You might consider checking RPP
return true"
		};
		canSelectTrollProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = canSelectTrollProg,
			ParameterIndex = 0,
			ParameterType = 8200,
			ParameterName = "ch"
		});
		_context.FutureProgs.Add(canSelectTrollProg);
		var trollAttributeProg = new FutureProg
		{
			FunctionName = "TrollBonusAttributes",
			Category = "Character",
			Subcategory = "Attributes",
			FunctionComment =
				"This prog is called for each attribute for trolls at chargen time and the resulting value is applied as a modifier to that attribute.",
			ReturnType = (long)FutureProgVariableTypes.Number,
			StaticType = 0,
			FunctionText = @"// You might consider a switch on attributes like so:
//
// switch (@trait.Name)
//   case (""Strength"")
//     return 2
// end switch
return 0"
		};
		trollAttributeProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = trollAttributeProg,
			ParameterName = "trait",
			ParameterIndex = 0,
			ParameterType = 16384
		});
		_context.FutureProgs.Add(trollAttributeProg);
		_context.SaveChanges();

		var (trollBlood, trollSweat, _, _) = CreateBloodAndSweat("Troll");

		var trollRace = new Race
		{
			Name = "Troll",
			Description = @"Trolls are a large and brutish race of creatures who inhabit the dark and shadowy regions of Middle-earth. They are a primitive and barbaric people, with a love of violence and destruction. Trolls are known for their immense strength and ferocity in battle, and are feared and hated by the other peoples of Middle-earth. In the Black Speech of Sauron, the word for Troll is Olog.

Despite their reputation as mindless beasts, Trolls are highly intelligent and cunning. They are skilled at adapting to their surroundings and are capable of using simple tools and weapons. Trolls are also highly social, and will often live and hunt in packs. They are fiercely territorial, and will fiercely defend their homes and families from any perceived threat.

Trolls are primarily scavengers and predators, and will eat anything they can catch. They are especially fond of the flesh of Elves and Men, and will often raid their settlements in search of food. Trolls are also known for their love of torture and cruelty, and will often inflict great suffering on their enemies for their own enjoyment. They are feared and hated by the other peoples of Middle-earth, and are often hunted down and destroyed on sight.",
			AllowedGenders = _humanRace.AllowedGenders,
			ParentRace = humanoid,
			AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == 1) * 12,
			IndividualAttributeCap = 20,
			DiceExpression = "3d6+1",
			IlluminationPerceptionMultiplier = 1.75,
			CorpseModelId = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse").Id,
			DefaultHealthStrategyId = humanoid.DefaultHealthStrategyId,
			CanUseWeapons = true,
			CanAttack = true,
			CanDefend = true,
			NaturalArmourQuality = 9,
			NeedsToBreathe = true,
			BreathingModel = "simple",
			SweatRateInLitresPerMinute = 3.0,
			SizeStanding = 7,
			SizeProne = 7,
			SizeSitting = 7,
			CommunicationStrategyType = "humanoid",
			DefaultHandedness = 1,
			HandednessOptions = "1 3",
			MaximumDragWeightExpression = humanoid.MaximumDragWeightExpression,
			MaximumLiftWeightExpression = humanoid.MaximumLiftWeightExpression,
			RaceUsesStamina = true,
			CanEatCorpses = false,
			BiteWeight = 1000,
			EatCorpseEmoteText = "",
			CanEatMaterialsOptIn = false,
			TemperatureRangeFloor = -10,
			TemperatureRangeCeiling = 50,
			BodypartSizeModifier = 1,
			BodypartHealthMultiplier = 2.0,
			BreathingVolumeExpression = "7",
			HoldBreathLengthExpression = humanoid.HoldBreathLengthExpression,
			CanClimb = true,
			CanSwim = true,
			MinimumSleepingPosition = 4,
			ChildAge = 3,
			YouthAge = 9,
			YoungAdultAge = 14,
			AdultAge = 22,
			ElderAge = 40,
			VenerableAge = 60,
			AttributeBonusProg = trollAttributeProg,
			AvailabilityProg = canSelectTrollProg,
			BaseBody = body,
			BloodLiquid = trollBlood,
			BloodModel = humanoid.BloodModel,
			NaturalArmourMaterial = humanoid.NaturalArmourMaterial,
			NaturalArmourType = humanoid.NaturalArmourType,
			RaceButcheryProfile = null,
			SweatLiquid = trollSweat
		};
		_context.Races.Add(trollRace);
		var trollMaleHWModel = new HeightWeightModel
		{
			Name = "Troll Male",
			MeanHeight = 350,
			MeanBmi = 23,
			StddevHeight = 35,
			StddevBmi = 3,
			Bmimultiplier = 0.1
		};
		_context.Add(trollMaleHWModel);
		var trollFemaleHWModel = new HeightWeightModel
		{
			Name = "Troll Female",
			MeanHeight = 350,
			MeanBmi = 23.5,
			StddevHeight = 35,
			StddevBmi = 3,
			Bmimultiplier = 0.1
		};
		_context.Add(trollFemaleHWModel);
		trollRace.DefaultHeightWeightModelMale = trollFemaleHWModel;
		trollRace.DefaultHeightWeightModelFemale = trollFemaleHWModel;
		trollRace.DefaultHeightWeightModelNonBinary = trollFemaleHWModel;
		trollRace.DefaultHeightWeightModelNeuter = trollFemaleHWModel;
		_races.Add("Troll", trollRace);
		_context.SaveChanges();

		var isTrollProg = new FutureProg
		{
			FunctionName = "IsTrollRace",
			FunctionComment = "Determines whether someone is the Troll race",
			FunctionText = $"return @ch.Race == ToRace({trollRace.Id})",
			ReturnType = (long)FutureProgVariableTypes.Boolean,
			Category = "Character",
			Subcategory = "Race",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
		isTrollProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = isTrollProg,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)FutureProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(isTrollProg);
		_context.SaveChanges();
		_raceProgs["Troll"] = isTrollProg;

		var isTrollBabyProg = CreateVariantAgeProg(isBabyProg, trollRace);
		var isTrollToddlerProg = CreateVariantAgeProg(isToddlerProg, trollRace);
		var isTrollBoyProg = CreateVariantAgeProg(isBoyProg, trollRace);
		var isTrollGirlProg = CreateVariantAgeProg(isGirlProg, trollRace);
		var isTrollChildProg = CreateVariantAgeProg(isChildProg, trollRace);
		var isTrollYouthProg = CreateVariantAgeProg(isYouthProg, trollRace);
		var isTrollYoungManProg = CreateVariantAgeProg(isYoungManProg, trollRace);
		var isTrollYoungWomanProg = CreateVariantAgeProg(isYoungWomanProg, trollRace);
		var isTrollAdultManProg = CreateVariantAgeProg(isAdultManProg, trollRace);
		var isTrollAdultWomanProg = CreateVariantAgeProg(isAdultWomanProg, trollRace);
		var isTrollAdultPersonProg = CreateVariantAgeProg(isAdultProg, trollRace);
		var isTrollOldManProg = CreateVariantAgeProg(isOldManProg, trollRace);
		var isTrollOldWomanProg = CreateVariantAgeProg(isOldWomanProg, trollRace);
		var isTrollOldPersonProg = CreateVariantAgeProg(isOldPersonProg, trollRace);
		var trollPersonValues = new List<(CharacteristicValue Value, double Weight)>();

		void AddTrollPersonWord(string name, string basic, FutureProg prog, double weight)
		{
			var pw = new CharacteristicValue
			{
				Id = nextId++,
				Definition = personDef,
				Name = name,
				Value = basic,
				AdditionalValue = "",
				Default = false,
				Pluralisation = 0,
				FutureProg = prog
			};
			_context.CharacteristicValues.Add(pw);
			trollPersonValues.Add((pw, weight));
		}

		AddTrollPersonWord("troll baby", "troll baby", isTrollBabyProg, 1.0);
		AddTrollPersonWord("troll infant", "troll baby", isTrollBabyProg, 1.0);
		AddTrollPersonWord("troll toddler", "troll toddler", isTrollToddlerProg, 5.0);
		AddTrollPersonWord("troll", "troll", isTrollAdultPersonProg, 5.0);
		AddTrollPersonWord("male troll", "troll", isTrollAdultManProg, 1.0);
		AddTrollPersonWord("female troll", "troll", isTrollAdultWomanProg, 1.0);
		AddTrollPersonWord("troll child", "troll child", isTrollChildProg, 3.0);
		AddTrollPersonWord("troll youth", "troll youth", isTrollYouthProg, 3.0);
		AddTrollPersonWord("young troll", "troll youth", isTrollYouthProg, 3.0);
		AddTrollPersonWord("troll adolescent", "troll youth", isTrollYouthProg, 1.0);
		AddTrollPersonWord("old troll", "old troll", isTrollOldPersonProg, 1.0);
		AddTrollPersonWord("old female troll", "old troll", isTrollOldWomanProg, 1.0);
		AddTrollPersonWord("old male troll", "old troll", isTrollOldManProg, 1.0);
		_context.SaveChanges();

		var trollPersons = new CharacteristicProfile
		{
			Name = "Troll Person Words",
			Definition = new XElement("Values",
				from item in trollPersonValues
				select new XElement("Value", new XAttribute("weight", item.Weight), item.Value.Id)
			).ToString(),
			Type = "weighted",
			Description = "All person words applicable to the Troll race",
			TargetDefinition = personDef
		};
		_context.CharacteristicProfiles.Add(trollPersons);
		_profiles[trollPersons.Name] = trollPersons;
		_context.SaveChanges();

		void AddTrollEthnicity(string name, string blurb, string ethnicGroup, string ethnicSubGroup, string bloodGroup,
			string eyeColour, string hairColour, string skinColour, bool selectable = true)
		{
			AddEthnicity(trollRace, name, ethnicGroup, bloodGroup, 0.0, 0.0, ethnicSubGroup,
				selectable ? canSelectTrollProg : _alwaysFalseProg, blurb);
			AddEthnicityVariable(name, "Distinctive Feature", "All Distinctive Features");
			AddEthnicityVariable(name, "Eye Colour", eyeColour);
			AddEthnicityVariable(name, "Eye Shape", "All Eye Shapes");
			AddEthnicityVariable(name, "Ears", "All Ears");
			AddEthnicityVariable(name, "Facial Hair Colour", hairColour);
			AddEthnicityVariable(name, "Facial Hair Style", "All Facial Hair Styles");
			AddEthnicityVariable(name, "Hair Colour", hairColour);
			AddEthnicityVariable(name, "Hair Style", "All Hair Styles");
			AddEthnicityVariable(name, "Humanoid Frame", "All Frames");
			AddEthnicityVariable(name, "Nose", "All Noses");
			AddEthnicityVariable(name, "Person Word", "Troll Person Words");
			AddEthnicityVariable(name, "Skin Colour", skinColour);
			_context.SaveChanges();
		}

		AddTrollEthnicity("Cave Troll", @"Of the Wild Trolls, Cave Trolls are the largest and most powerful breed. They are extremely solitary and cannibalistic. They are almost blind, but have a superb sense of hearing and smell. Their scale hides are pale, like those of most cave-dwelling creatures.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
			"Fair_Skin");
		AddTrollEthnicity("Forest Troll", @"Forest Trolls are the least brutal of the Wild Trolls. They are more graceful (relatively) and live in loosely organised bands who hunt together. They are rarely cannibalistic, preferring woodland game (or Man or Dwarf if they can manage). They are experts with slings, snares and skinning knives.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
			"Swarthy_Skin");
		AddTrollEthnicity("Hill Troll", @"Hill Trolls live in small groups, but are quite quarrelsome and greedy. They prefer clubs and thrown stones in a fight and are very territorial. They guard their stolen treasures jealously, even those of little use to them such as books.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
			"Swarthy_Skin");
		AddTrollEthnicity("Snow Troll", @"Snow Trolls are rare creatures with grey-white hides and icy blue eyes. When exposed to sunlight they turn into pillars of icy slag. They go for long periods without food, but are virtually unstoppable when they at last sight prey.", "Wild", "", "B Dominant", "Blue_Eyes", "Black_Brown_Grey_Hair",
			"Fair_Skin");
		AddTrollEthnicity("Stone Troll", @"Stone Trolls spend a great deal of time hoarding piles of food and treasure, stealing it from each other, and boasting of their riches. Their fratricidal tendencies are extreme.", "Wild", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
			"Olive_Skin");
		AddTrollEthnicity("Olog-Hai", @"The Olog-hai have been bred by Sauron from lesser Troll stock and have until late been a rare breed. Cunning and organised; yet as big and strong as their lesser brethren. The Olog-hai are superb warriors; they know no fear and thirst for blood and victory.

Olog-hai are also called Black Trolls by some, for they have black scaly hides and black blood. Most carry blank shields and warhammers, although they are adept at using almost any weapon. They differ from older Troll varieties in other ways as well, for example, they do not turn to stone in daylight.", "Greater", "", "B Dominant", "Brown_Green_Eyes", "Black_Brown_Grey_Hair",
			"Dark_Skin", false);

		var trollSkillProg = new FutureProg
		{
			FunctionName = "TrollSkillStartingValue",
			Category = "Chargen",
			Subcategory = "Skills",
			FunctionComment = "Used to determine the opening value for a skill for trolls at character creation",
			ReturnType = (int)FutureProgVariableTypes.Number,
			AcceptsAnyParameters = false,
			Public = false,
			StaticType = 0,
			FunctionText = @"if (@trait.group == ""Language"")
   return 150 + (@boosts * 50)
 end if
 return 35 + (@boosts * 15)"
		};
		trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = trollSkillProg, ParameterIndex = 0, ParameterName = "ch",
			ParameterType = (int)FutureProgVariableTypes.Toon
		});
		trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = trollSkillProg, ParameterIndex = 1, ParameterName = "trait",
			ParameterType = (int)FutureProgVariableTypes.Trait
		});
		trollSkillProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = trollSkillProg, ParameterIndex = 2, ParameterName = "boosts",
			ParameterType = (int)FutureProgVariableTypes.Number
		});
		_context.FutureProgs.Add(trollSkillProg);
		_context.SaveChanges();

		#endregion Trolls
	}
}