#nullable enable

using MudSharp.Character.Name;
using MudSharp.Form.Shape;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private void SeedAdditionalModernNameProfiles()
	{
		NameCulture westernEuropean = _context.NameCultures.First(x => x.Name == "Western European");
		NameCulture chinese = EnsureModernChineseCulture();
		NameCulture indian = EnsureModernGivenFamilyCulture("Modern Indian");

		SeedGivenFamilyProfile("Modern French Male", Gender.Male, westernEuropean,
			[
				("Jean", 10), ("Pierre", 10), ("Louis", 9), ("Henri", 9), ("Luc", 8), ("Mathieu", 8),
				("Nicolas", 7), ("Julien", 7), ("Etienne", 7), ("Antoine", 7), ("Francois", 6), ("Claude", 6),
				("Remy", 5), ("Pascal", 5), ("Thierry", 4), ("Alexandre", 4)
			],
			[
				("Martin", 10), ("Bernard", 9), ("Dubois", 9), ("Moreau", 8), ("Petit", 8), ("Durand", 8),
				("Blanc", 7), ("Robert", 7), ("Richard", 7), ("Simon", 6), ("Lefevre", 6), ("Rousseau", 6),
				("Garnier", 5), ("Fabre", 5), ("Mercier", 4), ("Fontaine", 4)
			]);
		SeedGivenFamilyProfile("Modern French Female", Gender.Female, westernEuropean,
			[
				("Marie", 10), ("Claire", 10), ("Sophie", 9), ("Juliette", 9), ("Elise", 8), ("Celine", 8),
				("Amelie", 7), ("Camille", 7), ("Isabelle", 7), ("Colette", 6), ("Genevieve", 6), ("Delphine", 5),
				("Aurore", 5), ("Margot", 4), ("Lucie", 4), ("Charlotte", 4)
			],
			[
				("Martin", 10), ("Bernard", 9), ("Dubois", 9), ("Moreau", 8), ("Petit", 8), ("Durand", 8),
				("Blanc", 7), ("Robert", 7), ("Richard", 7), ("Simon", 6), ("Lefevre", 6), ("Rousseau", 6),
				("Garnier", 5), ("Fabre", 5), ("Mercier", 4), ("Fontaine", 4)
			]);

		SeedGivenFamilyProfile("Modern German Male", Gender.Male, westernEuropean,
			[
				("Johann", 10), ("Karl", 10), ("Friedrich", 9), ("Lukas", 9), ("Matthias", 8), ("Stefan", 8),
				("Markus", 7), ("Heinrich", 7), ("Leon", 7), ("Max", 6), ("Tobias", 6), ("Andreas", 5),
				("Felix", 5), ("Anton", 4), ("Martin", 4), ("Niklas", 4)
			],
			[
				("Muller", 10), ("Schmidt", 10), ("Schneider", 9), ("Fischer", 9), ("Weber", 8), ("Meyer", 8),
				("Wagner", 7), ("Becker", 7), ("Hoffmann", 7), ("Schulz", 6), ("Braun", 6), ("Zimmermann", 6),
				("Richter", 5), ("Klein", 5), ("Wolf", 4), ("Hartmann", 4)
			]);
		SeedGivenFamilyProfile("Modern German Female", Gender.Female, westernEuropean,
			[
				("Anna", 10), ("Sophie", 10), ("Maria", 9), ("Greta", 9), ("Klara", 8), ("Lena", 8),
				("Johanna", 7), ("Elisabeth", 7), ("Marta", 7), ("Heidi", 6), ("Liesel", 6), ("Ingrid", 5),
				("Sabine", 5), ("Katrin", 4), ("Nina", 4), ("Emilia", 4)
			],
			[
				("Muller", 10), ("Schmidt", 10), ("Schneider", 9), ("Fischer", 9), ("Weber", 8), ("Meyer", 8),
				("Wagner", 7), ("Becker", 7), ("Hoffmann", 7), ("Schulz", 6), ("Braun", 6), ("Zimmermann", 6),
				("Richter", 5), ("Klein", 5), ("Wolf", 4), ("Hartmann", 4)
			]);

		SeedGivenFamilyProfile("Modern Spanish Male", Gender.Male, westernEuropean,
			[
				("Alejandro", 10), ("Carlos", 10), ("Diego", 9), ("Javier", 9), ("Miguel", 8), ("Pablo", 8),
				("Mateo", 7), ("Andres", 7), ("Enrique", 7), ("Jose", 6), ("Rafael", 6), ("Luis", 5),
				("Antonio", 5), ("Tomas", 4), ("Sergio", 4), ("Alvaro", 4)
			],
			[
				("Garcia", 10), ("Fernandez", 10), ("Gonzalez", 9), ("Rodriguez", 9), ("Lopez", 8), ("Martinez", 8),
				("Sanchez", 7), ("Perez", 7), ("Gomez", 7), ("Martin", 6), ("Jimenez", 6), ("Ruiz", 5),
				("Diaz", 5), ("Hernandez", 4), ("Navarro", 4), ("Romero", 4)
			]);
		SeedGivenFamilyProfile("Modern Spanish Female", Gender.Female, westernEuropean,
			[
				("Maria", 10), ("Carmen", 10), ("Isabel", 9), ("Lucia", 9), ("Elena", 8), ("Sofia", 8),
				("Ana", 7), ("Teresa", 7), ("Pilar", 7), ("Beatriz", 6), ("Ines", 6), ("Raquel", 5),
				("Marta", 5), ("Alba", 4), ("Julia", 4), ("Rosa", 4)
			],
			[
				("Garcia", 10), ("Fernandez", 10), ("Gonzalez", 9), ("Rodriguez", 9), ("Lopez", 8), ("Martinez", 8),
				("Sanchez", 7), ("Perez", 7), ("Gomez", 7), ("Martin", 6), ("Jimenez", 6), ("Ruiz", 5),
				("Diaz", 5), ("Hernandez", 4), ("Navarro", 4), ("Romero", 4)
			]);

		SeedGivenFamilyProfile("Modern Portugese Male", Gender.Male, westernEuropean,
			[
				("Joao", 10), ("Miguel", 10), ("Tiago", 9), ("Pedro", 9), ("Diogo", 8), ("Andre", 8),
				("Ricardo", 7), ("Filipe", 7), ("Duarte", 7), ("Goncalo", 6), ("Antonio", 6), ("Manuel", 5),
				("Rui", 5), ("Vasco", 4), ("Nuno", 4), ("Henrique", 4)
			],
			[
				("Silva", 10), ("Santos", 10), ("Ferreira", 9), ("Pereira", 9), ("Costa", 8), ("Rodrigues", 8),
				("Martins", 7), ("Jesus", 7), ("Oliveira", 7), ("Sousa", 6), ("Fernandes", 6), ("Goncalves", 5),
				("Gomes", 5), ("Lopes", 4), ("Marques", 4), ("Carvalho", 4)
			]);
		SeedGivenFamilyProfile("Modern Portugese Female", Gender.Female, westernEuropean,
			[
				("Maria", 10), ("Ines", 10), ("Ana", 9), ("Leonor", 9), ("Joana", 8), ("Sofia", 8),
				("Catarina", 7), ("Beatriz", 7), ("Rita", 7), ("Mariana", 6), ("Teresa", 6), ("Matilde", 5),
				("Margarida", 5), ("Filipa", 4), ("Carolina", 4), ("Helena", 4)
			],
			[
				("Silva", 10), ("Santos", 10), ("Ferreira", 9), ("Pereira", 9), ("Costa", 8), ("Rodrigues", 8),
				("Martins", 7), ("Jesus", 7), ("Oliveira", 7), ("Sousa", 6), ("Fernandes", 6), ("Goncalves", 5),
				("Gomes", 5), ("Lopes", 4), ("Marques", 4), ("Carvalho", 4)
			]);

		SeedGivenFamilyProfile("Modern Chinese Male", Gender.Male, chinese,
			[
				("Wei", 10), ("Jun", 10), ("Jian", 9), ("Ming", 9), ("Hao", 8), ("Tao", 8),
				("Peng", 7), ("Lei", 7), ("Hong", 7), ("Chao", 6), ("Qiang", 6), ("Bin", 5),
				("Yong", 5), ("Xiang", 4), ("Bo", 4), ("Guang", 4)
			],
			[
				("Wang", 10), ("Li", 10), ("Zhang", 9), ("Liu", 9), ("Chen", 8), ("Yang", 8),
				("Zhao", 7), ("Huang", 7), ("Wu", 7), ("Zhou", 6), ("Xu", 6), ("Sun", 5),
				("Ma", 5), ("Zhu", 4), ("Hu", 4), ("Guo", 4)
			]);
		SeedGivenFamilyProfile("Modern Chinese Female", Gender.Female, chinese,
			[
				("Mei", 10), ("Li", 10), ("Jing", 9), ("Fang", 9), ("Yan", 8), ("Ling", 8),
				("Xiuying", 7), ("Lan", 7), ("Na", 7), ("Ying", 6), ("Hui", 6), ("Xia", 5),
				("Qian", 5), ("Juan", 4), ("Lian", 4), ("Xiu", 4)
			],
			[
				("Wang", 10), ("Li", 10), ("Zhang", 9), ("Liu", 9), ("Chen", 8), ("Yang", 8),
				("Zhao", 7), ("Huang", 7), ("Wu", 7), ("Zhou", 6), ("Xu", 6), ("Sun", 5),
				("Ma", 5), ("Zhu", 4), ("Hu", 4), ("Guo", 4)
			]);

		SeedGivenFamilyProfile("Modern Indian Male", Gender.Male, indian,
			[
				("Arjun", 10), ("Rahul", 10), ("Vikram", 9), ("Rajesh", 9), ("Amit", 8), ("Karan", 8),
				("Rohan", 7), ("Dev", 7), ("Anand", 7), ("Manish", 6), ("Sanjay", 6), ("Ajay", 5),
				("Vijay", 5), ("Sameer", 4), ("Suresh", 4), ("Naveen", 4)
			],
			[
				("Patel", 10), ("Singh", 10), ("Kumar", 9), ("Sharma", 9), ("Gupta", 8), ("Verma", 8),
				("Reddy", 7), ("Nair", 7), ("Rao", 7), ("Kapoor", 6), ("Malhotra", 6), ("Mehta", 5),
				("Joshi", 5), ("Shah", 4), ("Iyer", 4), ("Pillai", 4)
			]);
		SeedGivenFamilyProfile("Modern Indian Female", Gender.Female, indian,
			[
				("Priya", 10), ("Ananya", 10), ("Kavita", 9), ("Meera", 9), ("Lakshmi", 8), ("Asha", 8),
				("Pooja", 7), ("Rani", 7), ("Sunita", 7), ("Deepa", 6), ("Anita", 6), ("Radha", 5),
				("Nisha", 5), ("Leela", 4), ("Divya", 4), ("Maya", 4)
			],
			[
				("Patel", 10), ("Singh", 10), ("Kumar", 9), ("Sharma", 9), ("Gupta", 8), ("Verma", 8),
				("Reddy", 7), ("Nair", 7), ("Rao", 7), ("Kapoor", 6), ("Malhotra", 6), ("Mehta", 5),
				("Joshi", 5), ("Shah", 4), ("Iyer", 4), ("Pillai", 4)
			]);

		SeedModernEthnicityProfiles();
	}

	private NameCulture EnsureModernGivenFamilyCulture(string name)
	{
		return AddNameCulture(name, @"^(?<birthname>[\w'-]+)\s+(?<surname>[\w'-]+)$",
			new[]
			{
				("Given Name", 1, 1, "Your given name is the personal name chosen by your parents or family.", NameUsage.BirthName),
				("Family Name", 1, 1, "Your family name identifies the household or family that you belong to.", NameUsage.Surname)
			},
			new[]
			{
				(NameStyle.GivenOnly, "{0}", new[] { NameUsage.BirthName }),
				(NameStyle.SimpleFull, "{0} {1}", new[] { NameUsage.BirthName, NameUsage.Surname }),
				(NameStyle.FullName, "{0} {1}", new[] { NameUsage.BirthName, NameUsage.Surname }),
				(NameStyle.Affectionate, "{0}", new[] { NameUsage.BirthName }),
				(NameStyle.SurnameOnly, "{0}", new[] { NameUsage.Surname }),
				(NameStyle.FullWithNickname, "{0} {1}", new[] { NameUsage.BirthName, NameUsage.Surname })
			});
	}

	private NameCulture EnsureModernChineseCulture()
	{
		return AddNameCulture("Modern Chinese", @"^(?<surname>[\w'-]+)\s+(?<birthname>[\w'-]+)$",
			new[]
			{
				("Family Name", 1, 1, "Your family name comes first and identifies your wider family or clan.", NameUsage.Surname),
				("Given Name", 1, 1, "Your given name follows your family name and is the personal name chosen for you.", NameUsage.BirthName)
			},
			new[]
			{
				(NameStyle.GivenOnly, "{0}", new[] { NameUsage.BirthName }),
				(NameStyle.SimpleFull, "{0} {1}", new[] { NameUsage.Surname, NameUsage.BirthName }),
				(NameStyle.FullName, "{0} {1}", new[] { NameUsage.Surname, NameUsage.BirthName }),
				(NameStyle.Affectionate, "{0}", new[] { NameUsage.BirthName }),
				(NameStyle.SurnameOnly, "{0}", new[] { NameUsage.Surname }),
				(NameStyle.FullWithNickname, "{0} {1}", new[] { NameUsage.Surname, NameUsage.BirthName })
			});
	}

	private void SeedGivenFamilyProfile(
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
}
