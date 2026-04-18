#nullable enable

using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private void SeedMedievalEuropeNameCorrections()
	{
		RepairDutchProfiles();
		RepairFinnoUgricProfiles();
		RepairBasqueProfiles();
		RepairAlbanianProfiles();
	}

	private void RepairDutchProfiles()
	{
		NameCulture culture = _context.NameCultures.First(x => x.Name == "Dutch");
		IEnumerable<(string Name, int Weight)> surnames =
		[
			("de Vries", 10), ("Jansen", 10), ("van den Berg", 9), ("Bakker", 9), ("Janssen", 8), ("Visser", 8),
			("Smit", 8), ("Meijer", 7), ("de Jong", 7), ("van Dijk", 7), ("Mulder", 7), ("de Boer", 7),
			("de Groot", 6), ("Bos", 6), ("Vos", 6), ("Peters", 6), ("Hendriks", 6), ("van Leeuwen", 6),
			("van der Meer", 6), ("van der Linden", 6), ("van der Wal", 6), ("van Beek", 6), ("van den Heuvel", 6),
			("van den Broek", 6), ("van Es", 5), ("van der Veen", 5), ("van Dam", 5), ("van der Laan", 5),
			("van den Bosch", 5), ("van Dongen", 5), ("van der Ven", 5), ("van der Zwan", 5),
			("van der Heijden", 5), ("van Loon", 5), ("van der Ploeg", 5), ("van den Brink", 5),
			("de Graaf", 5), ("de Bruin", 5), ("de Wit", 5), ("de Leeuw", 5), ("de Bruijn", 5),
			("de Koning", 5), ("de Haan", 5), ("de Ruiter", 5), ("de Ridder", 5), ("de Goede", 4),
			("de Wilde", 4), ("de Klerk", 4), ("de Greef", 4), ("de Wolff", 4)
		];

		ResetSimpleNameProfile("Dutch Male", Gender.Male, culture,
			[
				("Jan", 10), ("Pieter", 10), ("Willem", 9), ("Hendrik", 9), ("Cornelis", 8), ("Jacob", 8),
				("Dirk", 7), ("Klaas", 7), ("Gerrit", 7), ("Johannes", 6), ("Nicolaas", 6), ("Adriaan", 5),
				("Frans", 5), ("Lambert", 5), ("Floris", 4), ("Huibert", 4), ("Maarten", 4), ("Simon", 4),
				("Rutger", 3), ("Roelof", 3), ("Aart", 3), ("Bastiaan", 3), ("Joris", 3), ("Koenraad", 3), ("Michiel", 3)
			],
			surnames);
		ResetSimpleNameProfile("Dutch Female", Gender.Female, culture,
			[
				("Maria", 10), ("Anna", 10), ("Johanna", 9), ("Margaretha", 9), ("Elisabeth", 8), ("Catharina", 8),
				("Cornelia", 7), ("Geertruida", 7), ("Adriana", 7), ("Petronella", 6), ("Hendrika", 6), ("Jacoba", 5),
				("Alida", 5), ("Machteld", 5), ("Sara", 4), ("Susanna", 4), ("Willemijntje", 4), ("Antonia", 4),
				("Neeltje", 3), ("Catrina", 3), ("Femke", 3), ("Grietje", 3), ("Lysbeth", 3), ("Aleid", 3), ("Marieke", 3)
			],
			surnames);
	}

	private void RepairFinnoUgricProfiles()
	{
		NameCulture culture = _context.NameCultures.First(x => x.Name == "Finno-Ugric");
		IEnumerable<(string Name, int Weight)> surnames =
		[
			("Korhonen", 10), ("Virtanen", 10), ("Nieminen", 9), ("Makela", 9), ("Koskinen", 8), ("Heikkinen", 8),
			("Salminen", 8), ("Laine", 7), ("Lehtonen", 7), ("Lehtinen", 7), ("Niemi", 7), ("Jarvinen", 7),
			("Hamalainen", 6), ("Rasanen", 6), ("Heiskanen", 6), ("Miettinen", 6), ("Saarinen", 6), ("Paananen", 6),
			("Aalto", 6), ("Salonen", 6), ("Lahtinen", 6), ("Ojala", 5), ("Nurmi", 5), ("Leppanen", 5),
			("Saaristo", 5), ("Tikkanen", 5), ("Rinne", 5), ("Heikkila", 5), ("Alatalo", 5), ("Kallio", 5),
			("Jokinen", 5), ("Salomaa", 5), ("Rantanen", 5), ("Haapala", 5), ("Leinonen", 5), ("Karjalainen", 5),
			("Ahola", 4), ("Sipila", 4), ("Rantala", 4), ("Koskela", 4), ("Makinen", 4), ("Tammela", 4),
			("Soini", 4), ("Korpela", 4), ("Hiltunen", 4), ("Kinnunen", 4), ("Niskanen", 4), ("Vaisanen", 4), ("Lammi", 4)
		];

		ResetSimpleNameProfile("Finno-Ugric Male", Gender.Male, culture,
			[
				("Matti", 10), ("Johannes", 10), ("Juhani", 9), ("Mikael", 9), ("Antti", 8), ("Pekka", 8),
				("Heikki", 7), ("Tuomas", 7), ("Kaarle", 7), ("Lauri", 6), ("Olli", 6), ("Ilmari", 5),
				("Tapio", 5), ("Vaino", 5), ("Jari", 4), ("Eero", 4), ("Paavo", 4), ("Hannu", 4),
				("Reino", 3), ("Kalevi", 3), ("Jukka", 3), ("Arvo", 3), ("Risto", 3), ("Erkki", 3), ("Toivo", 3)
			],
			surnames);
		ResetSimpleNameProfile("Finno-Ugric Female", Gender.Female, culture,
			[
				("Maria", 10), ("Anna", 10), ("Johanna", 9), ("Kaisa", 9), ("Liisa", 8), ("Helena", 8),
				("Aino", 7), ("Elina", 7), ("Katri", 7), ("Sofia", 6), ("Vilhelmiina", 6), ("Outi", 5),
				("Sari", 5), ("Eeva", 5), ("Marja", 4), ("Reeta", 4), ("Saara", 4), ("Ilona", 4),
				("Heta", 3), ("Kirsti", 3), ("Tuulia", 3), ("Leena", 3), ("Marjatta", 3), ("Kyllikki", 3), ("Inkeri", 3)
			],
			surnames);
	}

	private void RepairBasqueProfiles()
	{
		NameCulture culture = _context.NameCultures.First(x => x.Name == "Basque");
		IEnumerable<(string Name, int Weight)> surnames =
		[
			("Aguirre", 10), ("Arana", 9), ("Aramburu", 9), ("Arizmendi", 8), ("Arratia", 8), ("Arrizabalaga", 8),
			("Azkue", 7), ("Echeverria", 7), ("Etxeberria", 7), ("Elizondo", 7), ("Garmendia", 6), ("Garro", 6),
			("Goikoetxea", 6), ("Ibarra", 6), ("Idiakez", 6), ("Iturriaga", 5), ("Iturralde", 5), ("Loyola", 5),
			("Mendia", 5), ("Mendizabal", 5), ("Ochoa", 5), ("Olaizola", 4), ("Onate", 4), ("Salazar", 4),
			("Ugarte", 4), ("Urrutia", 4), ("Zabaleta", 4), ("Zubia", 4), ("Zubizarreta", 4), ("Goyeneche", 4)
		];

		ResetSimpleNameProfile("Basque Male", Gender.Male, culture,
			[
				("Sancho", 10), ("Inigo", 10), ("Lope", 9), ("Garcia", 9), ("Eneko", 8), ("Fortun", 8),
				("Jimeno", 8), ("Otxoa", 7), ("Miguel", 7), ("Peru", 7), ("Martin", 6), ("Diego", 6),
				("Ander", 6), ("Xabier", 6), ("Antso", 6), ("Joanes", 5), ("Mikel", 5), ("Rodrigo", 5),
				("Gonzalo", 5), ("Bernat", 4), ("Domingo", 4), ("Esteban", 4), ("Pedro", 4), ("Juan", 4), ("Beltran", 3)
			],
			surnames);
		ResetSimpleNameProfile("Basque Female", Gender.Female, culture,
			[
				("Toda", 10), ("Oneka", 10), ("Urraca", 9), ("Sancha", 9), ("Elvira", 8), ("Mencia", 8),
				("Maria", 8), ("Catalina", 7), ("Gracia", 7), ("Teresa", 7), ("Leonor", 6), ("Aldonza", 6),
				("Mayor", 6), ("Ines", 6), ("Beatriz", 5), ("Blanca", 5), ("Juana", 5), ("Ximena", 5),
				("Margarita", 4), ("Isabel", 4), ("Dominga", 4), ("Otxanda", 4), ("Madalena", 3), ("Maiora", 3), ("Terexa", 3)
			],
			surnames);
	}

	private void RepairAlbanianProfiles()
	{
		NameCulture culture = _context.NameCultures.First(x => x.Name == "Albanian");
		IEnumerable<(string Name, int Weight)> surnames =
		[
			("Berisha", 10), ("Doda", 10), ("Gashi", 10), ("Hoxha", 10), ("Krasniqi", 9), ("Leka", 9),
			("Ndreu", 8), ("Pashaj", 8), ("Rexhepi", 8), ("Shehu", 8), ("Xhaferi", 7), ("Zheji", 7),
			("Suli", 7), ("Sadiku", 7), ("Shala", 7), ("Halili", 6), ("Islami", 6), ("Mahmutaj", 6),
			("Qorri", 6), ("Topalli", 6), ("Toska", 5), ("Uka", 5), ("Ymeri", 5), ("Zeneli", 5),
			("Dervishi", 5), ("Gjonaj", 4), ("Hyseni", 4), ("Ibrahimi", 4), ("Kastrati", 4), ("Marku", 4)
		];

		ResetPatronymicNameProfile("Albanian Male", Gender.Male, culture,
			[
				("Mark", 10), ("Nikolle", 10), ("Gjergj", 10), ("Leke", 9), ("Petrit", 9), ("Arben", 9),
				("Ilir", 8), ("Dritan", 8), ("Bardh", 8), ("Flamur", 8), ("Altin", 7), ("Shkelzen", 7),
				("Ylli", 7), ("Erion", 7), ("Kujtim", 6), ("Valon", 6), ("Agron", 6), ("Sokol", 6),
				("Bledar", 5), ("Luan", 5), ("Artan", 5), ("Besnik", 5), ("Fatmir", 4), ("Genc", 4),
				("Jetmir", 4), ("Kreshnik", 4), ("Lirim", 4), ("Mentor", 4), ("Valdet", 3), ("Zef", 3)
			],
			[
				("Marki", 10), ("Nikolli", 10), ("Gjergji", 10), ("Leki", 9), ("Petriti", 9), ("Arbeni", 9),
				("Iliri", 8), ("Dritani", 8), ("Bardhi", 8), ("Flamuri", 8), ("Altini", 7), ("Shkelzeni", 7),
				("Ylli", 7), ("Erioni", 7), ("Kujtimi", 6), ("Valoni", 6), ("Agroni", 6), ("Sokoli", 6),
				("Bledari", 5), ("Luani", 5), ("Artani", 5), ("Besniki", 5), ("Fatmiri", 4), ("Genci", 4),
				("Jetmiri", 4), ("Kreshniki", 4), ("Lirimi", 4), ("Mentori", 4), ("Valdeti", 3), ("Zefi", 3)
			],
			surnames);
		ResetPatronymicNameProfile("Albanian Female", Gender.Female, culture,
			[
				("Maria", 10), ("Prenda", 10), ("Marta", 9), ("Lena", 9), ("Drita", 8), ("Elira", 8),
				("Anila", 8), ("Albana", 8), ("Arta", 7), ("Enkeleda", 7), ("Valbona", 7), ("Arbana", 7),
				("Mirela", 6), ("Shqipe", 6), ("Fjolla", 6), ("Jeta", 6), ("Teuta", 5), ("Mirjeta", 5),
				("Valdete", 5), ("Donika", 5), ("Lirije", 4), ("Aferdita", 4), ("Bardha", 4), ("Gentiana", 4),
				("Flutura", 4), ("Merita", 4), ("Shpresa", 3), ("Vjollca", 3), ("Rudina", 3), ("Kaltrina", 3)
			],
			[
				("Marke", 10), ("Nikolle", 10), ("Gjergje", 10), ("Leke", 9), ("Petrite", 9), ("Arbene", 9),
				("Ilire", 8), ("Dritane", 8), ("Bardhe", 8), ("Flamure", 8), ("Altine", 7), ("Shkelzene", 7),
				("Yllie", 7), ("Erione", 7), ("Kujtime", 6), ("Valone", 6), ("Agrone", 6), ("Sokole", 6),
				("Bledare", 5), ("Luane", 5), ("Artane", 5), ("Besnike", 5), ("Fatmire", 4), ("Gence", 4),
				("Jetmire", 4), ("Kreshnike", 4), ("Lirime", 4), ("Mentore", 4), ("Valdete", 3), ("Zefe", 3)
			],
			surnames);
	}

	private void ResetSimpleNameProfile(
		string profileName,
		Gender gender,
		NameCulture culture,
		IEnumerable<(string Name, int Weight)> givenNames,
		IEnumerable<(string Name, int Weight)> surnames)
	{
		RandomNameProfile profile = AddRandomNameProfile(profileName, gender, culture);
		AddRandomNameDice(profile, NameUsage.BirthName, "1");
		AddRandomNameDice(profile, NameUsage.Surname, "1");

		foreach ((string name, int weight) in givenNames)
		{
			AddRandomNameElement(profile, NameUsage.BirthName, name, weight);
		}

		foreach ((string name, int weight) in surnames)
		{
			AddRandomNameElement(profile, NameUsage.Surname, name, weight);
		}

		_context.SaveChanges();
	}

	private void ResetPatronymicNameProfile(
		string profileName,
		Gender gender,
		NameCulture culture,
		IEnumerable<(string Name, int Weight)> givenNames,
		IEnumerable<(string Name, int Weight)> patronyms,
		IEnumerable<(string Name, int Weight)> surnames)
	{
		RandomNameProfile profile = AddRandomNameProfile(profileName, gender, culture);
		AddRandomNameDice(profile, NameUsage.BirthName, "1");
		AddRandomNameDice(profile, NameUsage.Patronym, "1");
		AddRandomNameDice(profile, NameUsage.Surname, "1");

		foreach ((string name, int weight) in givenNames)
		{
			AddRandomNameElement(profile, NameUsage.BirthName, name, weight);
		}

		foreach ((string name, int weight) in patronyms)
		{
			AddRandomNameElement(profile, NameUsage.Patronym, name, weight);
		}

		foreach ((string name, int weight) in surnames)
		{
			AddRandomNameElement(profile, NameUsage.Surname, name, weight);
		}

		_context.SaveChanges();
	}
}
