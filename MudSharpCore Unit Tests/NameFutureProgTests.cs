#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Name;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NameFutureProgTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void NameTypes_ExposeDocumentedDotReferences()
	{
		var nameCulture = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.NameCulture];
		Assert.AreEqual(ProgVariableTypes.RandomNameProfile | ProgVariableTypes.Collection,
			nameCulture.PropertyTypeMap["randomnameprofiles"]);
		AssertHelp(nameCulture, "randomnameprofiles");

		var randomNameProfile = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.RandomNameProfile];
		Assert.AreEqual(ProgVariableTypes.NameCulture, randomNameProfile.PropertyTypeMap["culture"]);
		Assert.AreEqual(ProgVariableTypes.PersonalName, randomNameProfile.PropertyTypeMap["randomname"]);
		AssertHelp(randomNameProfile, "randomname");

		var personalName = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.PersonalName];
		Assert.AreEqual(ProgVariableTypes.NameCulture, personalName.PropertyTypeMap["culture"]);
		Assert.AreEqual(ProgVariableTypes.Text, personalName.PropertyTypeMap["surname"]);
		Assert.AreEqual(ProgVariableTypes.Text | ProgVariableTypes.Collection, personalName.PropertyTypeMap["elements"]);
		AssertHelp(personalName, "elements");

		var character = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Character];
		Assert.AreEqual(ProgVariableTypes.Text, character.PropertyTypeMap["name"]);
		Assert.AreEqual(ProgVariableTypes.PersonalName, character.PropertyTypeMap["personalname"]);
		Assert.AreEqual(ProgVariableTypes.PersonalName, character.PropertyTypeMap["currentname"]);
		AssertHelp(character, "personalname");

		var chargen = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Chargen];
		Assert.AreEqual(ProgVariableTypes.PersonalName, chargen.PropertyTypeMap["personalname"]);
		AssertHelp(chargen, "personalname");

		var toon = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Toon];
		Assert.AreEqual(ProgVariableTypes.PersonalName, toon.PropertyTypeMap["personalname"]);
		AssertHelp(toon, "personalname");

		var culture = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Culture];
		Assert.AreEqual(ProgVariableTypes.Text | ProgVariableTypes.Collection, culture.PropertyTypeMap["namecultures"]);
		Assert.AreEqual(ProgVariableTypes.NameCulture | ProgVariableTypes.Collection,
			culture.PropertyTypeMap["namecultureobjects"]);
		AssertHelp(culture, "namecultureobjects");

		var ethnicity = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.Ethnicity];
		Assert.AreEqual(ProgVariableTypes.NameCulture, ethnicity.PropertyTypeMap["malenameculture"]);
		AssertHelp(ethnicity, "malenameculture");
	}

	[TestMethod]
	public void PersonalName_ExposesFormattedAndElementProperties()
	{
		var culture = CreateNameCultureMock(11L, "Common");
		var name = new PersonalName(culture.Object, new Dictionary<NameUsage, List<string>>
		{
			[NameUsage.BirthName] = ["alex"],
			[NameUsage.Surname] = ["smith"],
			[NameUsage.Nickname] = ["ace"]
		}, true);

		Assert.AreEqual(ProgVariableTypes.PersonalName, name.Type);
		Assert.AreSame(culture.Object, name.GetProperty("culture"));
		Assert.AreEqual("Alex Smith", name.GetProperty("fullname").GetObject);
		Assert.AreEqual("Alex \"Ace\" Smith", name.GetProperty("withnickname").GetObject);
		Assert.AreEqual("Alex", name.GetProperty("diminutive").GetObject);
		Assert.AreEqual("Smith", name.GetProperty("surnameelement").GetObject);
		Assert.AreEqual(3, ((IList<IProgVariable>)name.GetProperty("elements").GetObject).Count);

		var sameNameInAnotherOrder = new PersonalName(culture.Object, new Dictionary<NameUsage, List<string>>
		{
			[NameUsage.Surname] = ["SMITH"],
			[NameUsage.Nickname] = ["Ace"],
			[NameUsage.BirthName] = ["Alex"]
		}, true);
		Assert.AreEqual(name, sameNameInAnotherOrder);
		Assert.AreEqual(name.GetHashCode(), sameNameInAnotherOrder.GetHashCode());
	}

	[TestMethod]
	public void NameFunctions_RegisterDocumentedExpectedOverloads()
	{
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		AssertFunction(functions, "tonameculture", [ProgVariableTypes.Number], ProgVariableTypes.NameCulture);
		AssertFunction(functions, "tonameculture", [ProgVariableTypes.Text], ProgVariableTypes.NameCulture);
		AssertFunction(functions, "torandomnameprofile", [ProgVariableTypes.Number], ProgVariableTypes.RandomNameProfile);
		AssertFunction(functions, "torandomnameprofile", [ProgVariableTypes.NameCulture, ProgVariableTypes.Text],
			ProgVariableTypes.RandomNameProfile);
		AssertFunction(functions, "getpersonalname", [ProgVariableTypes.NameCulture, ProgVariableTypes.Text],
			ProgVariableTypes.PersonalName);
		AssertFunction(functions, "randompersonalname", [ProgVariableTypes.RandomNameProfile],
			ProgVariableTypes.PersonalName);
	}

	[TestMethod]
	public void NameFunctions_LookUpAndCreatePersonalNames()
	{
		var culture = CreateNameCultureMock(11L, "Common");
		var name = new PersonalName(culture.Object, new Dictionary<NameUsage, List<string>>
		{
			[NameUsage.BirthName] = ["alex"],
			[NameUsage.Surname] = ["smith"],
			[NameUsage.Nickname] = ["ace"]
		}, true);
		culture.Setup(x => x.GetPersonalName("alex smith", true)).Returns(name);

		var profile = CreateRandomNameProfileMock(12L, "Common People", culture.Object, name);
		var cultures = new All<INameCulture>();
		cultures.Add(culture.Object);
		var profiles = new All<IRandomNameProfile>();
		profiles.Add(profile.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.NameCultures).Returns(cultures);
		gameworld.SetupGet(x => x.RandomNameProfiles).Returns(profiles);

		var lookupProg = CompileProg(gameworld.Object, "NameCultureLookup", ProgVariableTypes.Text,
			[], "return ToNameCulture(11).Name");
		Assert.AreEqual("Common", lookupProg.ExecuteString());

		var parsedNameProg = CompileProg(gameworld.Object, "ParsedPersonalName", ProgVariableTypes.Text,
			[Tuple.Create(ProgVariableTypes.NameCulture, "culture")],
			"return GetPersonalName(@culture, \"alex smith\").FullWithNickname");
		Assert.AreEqual("Alex \"Ace\" Smith", parsedNameProg.ExecuteString(culture.Object));

		var randomNameProg = CompileProg(gameworld.Object, "RandomPersonalName", ProgVariableTypes.Text,
			[Tuple.Create(ProgVariableTypes.RandomNameProfile, "profile")],
			"return RandomPersonalName(@profile).FullName");
		Assert.AreEqual("Alex Smith", randomNameProg.ExecuteString(profile.Object));

		culture.Setup(x => x.GetPersonalName("alex smith", true)).Returns(() => new PersonalName(culture.Object,
			new Dictionary<NameUsage, List<string>>
			{
				[NameUsage.BirthName] = ["alex"],
				[NameUsage.Surname] = ["smith"]
			}, true));
		var equalityProg = CompileProg(gameworld.Object, "EqualPersonalNames", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.NameCulture, "culture")],
			"return GetPersonalName(@culture, \"alex smith\") == GetPersonalName(@culture, \"alex smith\")");
		Assert.IsTrue(equalityProg.ExecuteBool(culture.Object));
	}

	private static void AssertFunction(IEnumerable<FunctionCompilerInformation> functions, string name,
		IEnumerable<ProgVariableTypes> parameters, ProgVariableTypes returnType)
	{
		var function = functions.SingleOrDefault(x => x.FunctionName.EqualTo(name) &&
		                                             x.Parameters.SequenceEqual(parameters));
		Assert.IsNotNull(function, $"Missing {name}({string.Join(", ", parameters.Select(x => x.Describe()))}).");
		Assert.AreEqual(returnType, function.ReturnType);
		Assert.AreEqual("Names", function.Category);
		Assert.IsFalse(string.IsNullOrWhiteSpace(function.FunctionHelp));
		Assert.IsTrue(function.ParameterNames.All(x => !string.IsNullOrWhiteSpace(x)));
		Assert.IsTrue(function.ParameterHelp.All(x => !string.IsNullOrWhiteSpace(x)));
	}

	private static void AssertHelp(FutureProgVariableCompileInfo compileInfo, string property)
	{
		Assert.IsTrue(compileInfo.PropertyHelpInfo.TryGetValue(property, out var help));
		Assert.IsFalse(string.IsNullOrWhiteSpace(help));
	}

	private static FutureProg CompileProg(IFuturemud gameworld, string name, ProgVariableTypes returnType,
		IEnumerable<Tuple<ProgVariableTypes, string>> parameters, string functionText)
	{
		var prog = new FutureProg(gameworld, name, returnType, parameters, functionText);
		prog.Compile();
		Assert.IsTrue(string.IsNullOrWhiteSpace(prog.CompileError), prog.CompileError);
		return prog;
	}

	private static Mock<INameCulture> CreateNameCultureMock(long id, string name)
	{
		var culture = new Mock<INameCulture>();
		culture.SetupGet(x => x.Id).Returns(id);
		culture.SetupGet(x => x.Name).Returns(name);
		culture.SetupGet(x => x.FrameworkItemType).Returns("NameCulture");
		culture.SetupGet(x => x.Gameworld).Returns(FutureProgTestBootstrap.Gameworld);
		culture.SetupGet(x => x.Type).Returns(ProgVariableTypes.NameCulture);
		culture.SetupGet(x => x.GetObject).Returns(() => culture.Object);
		culture.Setup(x => x.NamePattern(It.IsAny<NameStyle>()))
		       .Returns((NameStyle style) => style switch
		       {
			       NameStyle.GivenOnly => Tuple.Create("{0}", new List<NameUsage> { NameUsage.BirthName }),
			       NameStyle.SimpleFull => Tuple.Create("{0} {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname }),
			       NameStyle.FullName => Tuple.Create("{0} {1}", new List<NameUsage> { NameUsage.BirthName, NameUsage.Surname }),
			       NameStyle.Affectionate => Tuple.Create("{0}", new List<NameUsage> { NameUsage.Nickname }),
			       NameStyle.SurnameOnly => Tuple.Create("{0}", new List<NameUsage> { NameUsage.Surname }),
			       NameStyle.FullWithNickname => Tuple.Create("{0} \"{2}\" {1}", new List<NameUsage>
			       {
				       NameUsage.BirthName, NameUsage.Surname, NameUsage.Nickname
			       }),
			       _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
		       });
		culture.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((string property) => property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(id),
			"name" => new TextVariable(name),
			_ => throw new ArgumentException($"Unexpected name culture property {property}")
		});
		return culture;
	}

	private static Mock<IRandomNameProfile> CreateRandomNameProfileMock(long id, string name,
		INameCulture culture, IPersonalName randomName)
	{
		var profile = new Mock<IRandomNameProfile>();
		profile.SetupGet(x => x.Id).Returns(id);
		profile.SetupGet(x => x.Name).Returns(name);
		profile.SetupGet(x => x.FrameworkItemType).Returns("RandomNameProfile");
		profile.SetupGet(x => x.Culture).Returns(culture);
		profile.SetupGet(x => x.IsReady).Returns(true);
		profile.SetupGet(x => x.Type).Returns(ProgVariableTypes.RandomNameProfile);
		profile.SetupGet(x => x.GetObject).Returns(() => profile.Object);
		profile.Setup(x => x.GetRandomPersonalName(true)).Returns(randomName);
		profile.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((string property) => property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(id),
			"name" => new TextVariable(name),
			"culture" => culture,
			_ => throw new ArgumentException($"Unexpected random name profile property {property}")
		});
		return profile;
	}
}
