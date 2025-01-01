using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	private readonly Dictionary<string, NameCulture> _nameCultures = new();
	private CollectionDictionary<(string, NameUsage), string> _addedNames = new();
	private void AddRandomNameElement(RandomNameProfile profile, NameUsage usage, string name, int weight)
	{
		if (_addedNames[(profile.Name, usage)].Contains(name, StringComparer.OrdinalIgnoreCase))
		{
#if DEBUG
			ConsoleUtilities.WriteLineConsole($"Duplicate name element entry: #3{profile.Name}#0 - #2{usage.DescribeEnum()}#0 - #6{name}#0");
#else
#endif
			return;
		}

		_addedNames.Add((profile.Name, usage), name);
		_context.RandomNameProfilesElements.Add(new RandomNameProfilesElements { RandomNameProfile = profile, NameUsage = (int)usage, Name = name, Weighting = weight });
	}

	private void AddRandomNameDice(RandomNameProfile profile, NameUsage usage, string dice)
	{
		_context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
			{ RandomNameProfile = profile, NameUsage = (int)usage, DiceExpression = dice });
	}

	private RandomNameProfile AddRandomNameProfile(string name, Gender gender, NameCulture culture)
	{
		var random = new RandomNameProfile
		{
			Name = name,
			Gender = (short)gender,
			NameCulture = culture
		};
		_context.RandomNameProfiles.Add(random);
		_context.SaveChanges();
		return random;
	}

	private NameCulture AddNameCulture(string name, string definition)
	{
		var culture = new NameCulture
		{
			Name = name,
			Definition = definition
		};
		_context.NameCultures.Add(culture);
		_context.SaveChanges();

		_nameCultures[name] = culture;
		return culture;
	}

	private NameCulture AddNameCulture<T, U>(string name, string regex, T elements, U patterns)
		where T : IEnumerable<(string Name, int Minimum, int Maximum, string Description, NameUsage Usage)>
		where U : IEnumerable<(NameStyle Style, string Pattern, NameUsage[] Parameters)>
	{
		var definition = new XElement("Definition",
			new XElement("NameEntryRegex", new XCData(regex)),
			new XElement("Elements",
				from element in elements
				select new XElement("Element",
					new XAttribute("Name", element.Name),
					new XAttribute("Usage", (int)element.Usage),
					new XAttribute("MinimumCount", element.Minimum),
					new XAttribute("MaximumCount", element.Maximum),
					new XCData(element.Description)
				)
			),
			new XElement("Patterns",
				from pattern in patterns
				select new XElement("Pattern",
					new XAttribute("Style", (int)pattern.Style),
					new XAttribute("Params",
						pattern.Parameters.Select(x => ((int)x).ToString()).ListToCommaSeparatedValues()),
					new XAttribute("Text", pattern.Pattern)
				)
			)
		).ToString();
		var culture = new NameCulture
		{
			Name = name,
			Definition = definition
		};
		_context.NameCultures.Add(culture);
		_context.SaveChanges();

		_nameCultures[name] = culture;
		return culture;
	}

	private void SeedSimple(FuturemudDatabaseContext context)
	{
		var simpleName = new NameCulture
		{
			Name = "Simple",
			Definition =
				@"<NameCulture><Patterns>     <Pattern Style=""0"" Text=""{0}"" Params=""0""/>     <Pattern Style=""1"" Text=""{0}"" Params=""0""/>     <Pattern Style=""2"" Text=""{0}"" Params=""0""/>     <Pattern Style=""3"" Text=""{0}"" Params=""0""/>     <Pattern Style=""4"" Text=""{0}"" Params=""0""/>     <Pattern Style=""5"" Text=""{0}"" Params=""0""/>   </Patterns>   <Elements>     <Element Usage=""0"" MinimumCount=""1"" MaximumCount=""1"" Name=""Name""><![CDATA[You are known by a single name, and this name defines who you are to those who know you. You should select a name that is appropriate to your chosen culture.]]></Element>   </Elements>   <NameEntryRegex><![CDATA[^(?<birthname>[\w '-]+)$]]></NameEntryRegex> </NameCulture>"
		};
		context.NameCultures.Add(simpleName);
		context.SaveChanges();

		var simpleTwoName = new NameCulture
		{
			Name = "Given and Family",
			Definition =
				@"<NameCulture><Patterns>     <Pattern Style=""0"" Text=""{0}"" Params=""0""/>    <Pattern Style=""1"" Text=""{0} {1}"" Params=""0,6""/>    <Pattern Style=""2"" Text=""{0} {1}"" Params=""0,6""/>    <Pattern Style=""3"" Text=""{0}"" Params=""0""/>    <Pattern Style=""4"" Text=""{0}"" Params=""6""/>    <Pattern Style=""5"" Text=""{0} {1}"" Params=""0,6""/>   </Patterns>   <Elements>     <Element Usage=""0"" MinimumCount=""1"" MaximumCount=""1"" Name=""Given Name""><![CDATA[Your given name is a name given to you by your parents.]]></Element>   <Element Usage=""6"" MinimumCount=""1"" MaximumCount=""1"" Name=""Family Name""><![CDATA[Your family name is likely the same name as one or both of your parents' family name.]]></Element> </Elements>   <NameEntryRegex><![CDATA[^(?<birthname>[\w'-]+)\s+(?<surname>[\w '-]+)$]]></NameEntryRegex> </NameCulture>"
		};
		context.NameCultures.Add(simpleTwoName);
		context.SaveChanges();

		var simplePatronym = new NameCulture
		{
			Name = "Given and Patronym",
			Definition =
				@"<NameCulture><Patterns><Pattern Style=""0"" Text=""{0}"" Params=""0""/><Pattern Style=""1"" Text=""{0} {1}"" Params=""0,7""/>    <Pattern Style=""2"" Text=""{0} {1}"" Params=""0,7""/>    <Pattern Style=""3"" Text=""{0}"" Params=""0""/>    <Pattern Style=""4"" Text=""{0}"" Params=""7""/>    <Pattern Style=""5"" Text=""{0} {1}"" Params=""0,7""/>   </Patterns>   <Elements>     <Element Usage=""0"" MinimumCount=""1"" MaximumCount=""1"" Name=""Given Name""><![CDATA[Your given name is a name given to you by your parents.]]></Element>   <Element Usage=""7"" MinimumCount=""1"" MaximumCount=""1"" Name=""Patronym""><![CDATA[Your patronym is used to help identify you with a family. It is usually formed from your father's name (or husband, if married).]]></Element> </Elements>   <NameEntryRegex><![CDATA[^(?<birthname>[\w'-]+)\s+(?<patronym>[\w '-]+)$]]></NameEntryRegex> </NameCulture>"
		};
		context.NameCultures.Add(simplePatronym);
		context.SaveChanges();

		var simpleToponym = new NameCulture
		{
			Name = "Given and Toponym",
			Definition =
				@"<NameCulture><Patterns><Pattern Style=""0"" Text=""{0}"" Params=""0""/><Pattern Style=""1"" Text=""{0} {1}"" Params=""0,13""/>    <Pattern Style=""2"" Text=""{0} {1}"" Params=""0,13""/>    <Pattern Style=""3"" Text=""{0}"" Params=""0""/>    <Pattern Style=""4"" Text=""{0}"" Params=""13""/>    <Pattern Style=""5"" Text=""{0} {1}"" Params=""0,13""/>   </Patterns>   <Elements>     <Element Usage=""0"" MinimumCount=""1"" MaximumCount=""1"" Name=""Given Name""><![CDATA[Your given name is a name given to you by your parents.]]></Element>   <Element Usage=""13"" MinimumCount=""1"" MaximumCount=""1"" Name=""Toponym""><![CDATA[Your toponym is used to show where you have come from. It is typically your place of birth or origin. It is often in the form of 'of <place>', for example, 'of Avon River']]></Element> </Elements>   <NameEntryRegex><![CDATA[^(?<birthname>[\w'-]+)\s+(?<toponym>[\w '-]+)$]]></NameEntryRegex> </NameCulture>"
		};
		context.NameCultures.Add(simpleToponym);
		context.SaveChanges();

		var random = new RandomNameProfile
		{
			Name = "Wild Animal (Male)",
			Gender = (short)Gender.Male,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});
		context.RandomNameProfilesElements.Add(new RandomNameProfilesElements
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			Name = "Beast",
			Weighting = 1
		});
		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Wild Animal (Female)",
			Gender = (short)Gender.Female,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});
		context.RandomNameProfilesElements.Add(new RandomNameProfilesElements
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			Name = "Beast",
			Weighting = 1
		});
		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Wild Animal (Neuter)",
			Gender = (short)Gender.Neuter,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});
		context.RandomNameProfilesElements.Add(new RandomNameProfilesElements
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			Name = "Beast",
			Weighting = 1
		});
		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Dog (Male)",
			Gender = (short)Gender.Male,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});
		AddRandomNameElement(random, NameUsage.BirthName, "Archie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bailey", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bandit", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bear", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Benji", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Boomer", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Brownie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bruiser", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bruno", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Brutus", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Buddy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Buster", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Charlie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Duke", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Fido", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Fluffy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Gizmo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Harley", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Harry", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hunter", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Jack", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Jake", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "King", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lucky", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Max", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Milo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Patches", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rex", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rocky", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Romeo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rufus", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rusty", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Samson", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Scout", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Shadow", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sparky", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Spot", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Teddy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Toby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tucker", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Winston", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Zeus", 100);
		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Dog (Female)",
			Gender = (short)Gender.Female,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});
		AddRandomNameElement(random, NameUsage.BirthName, "Abby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bailey", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Bella", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Brandy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chloe", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Daisy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Dixie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ginger", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Holly", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Honey", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lily", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lola", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lucy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lulu", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Maggie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Mia", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Missy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Molly", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Penny", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Pepper", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Princess", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rosie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Roxy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ruby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sadie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sandy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sasha", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Shelby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sophie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Stella", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Zoe", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Coco", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Gracie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sugar", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lady", 100);
		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Elephant (Male)",
			Gender = (short)Gender.Male,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});

		AddRandomNameElement(random, NameUsage.BirthName, "Abul-Abbas", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Akavoor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Alexander", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Arjuna", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Arthur", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Babar", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Balarama", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Batyr", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Berilia", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Castor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chengalloor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chota", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chunee", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Cornelius", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Drona", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Echo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Elmer", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Fanny", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hanno", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hansken", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hattie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Horton", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ian", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Icebones", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Jerakeen", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Jumbo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kabumpo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kala Nag", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kandula", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kashin", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kesavan", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kolakolli", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lallah", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lizzie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Mary", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Mona", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Motty", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Old Bet", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Packy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Pollux", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Pom", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Poutifour", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Queenie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Raja", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Raja Gaj", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rajje", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rookh", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rosie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ruby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Satao", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Silverhair", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sivasundar", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Solomon", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Suleiman", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Surus", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tabul", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tai", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tantor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Thiruvambadi", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Thunder", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Topsy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tuffi", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tusko", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tyke", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ziggy", 100);

		context.SaveChanges();

		random = new RandomNameProfile
		{
			Name = "Elephant (Female)",
			Gender = (short)Gender.Female,
			NameCulture = simpleName
		};
		context.RandomNameProfiles.Add(random);
		context.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions
		{
			RandomNameProfile = random,
			NameUsage = (int)NameUsage.BirthName,
			DiceExpression = "1"
		});

		AddRandomNameElement(random, NameUsage.BirthName, "Abul-Abbas", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Akavoor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Arjuna", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Balarama", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Batyr", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Castor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Celeste", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chengalloor", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Chunee", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Drona", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Echo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ella", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Fanny", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Flora", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hanno", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hansken", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Hattie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ian", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Isabelle", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Jumbo", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kandula", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kashin", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kesavan", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Kolakolli", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lallah", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lily", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Lizzie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Mary", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Mona", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Motty", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Old Bet", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Packy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Pollux", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Queenie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Raja", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Raja Gaj", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rajje", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rookh", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Rosie", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ruby", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Satao", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Sivasundar", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Suleiman", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Surus", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tai", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Thiruvambadi", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Topsy", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tuffi", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tusko", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Tyke", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Zenobia", 100);
		AddRandomNameElement(random, NameUsage.BirthName, "Ziggy", 100);

		context.SaveChanges();
	}
}