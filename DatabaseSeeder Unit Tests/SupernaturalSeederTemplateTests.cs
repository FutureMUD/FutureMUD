#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SupernaturalSeederTemplateTests
{
	private static bool PlanarElementContains(XElement? element, long planeId)
	{
		return element?
			.Elements("Plane")
			.Any(x => long.Parse(x.Attribute("id")?.Value ?? "0") == planeId) == true;
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static BodyProto CreateBody(long id, string name, BodyProto? countsAs = null)
	{
		return new BodyProto
		{
			Id = id,
			Name = name,
			CountsAs = countsAs,
			CountsAsId = countsAs?.Id,
			ConsiderString = string.Empty,
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
	}

	private static BodypartProto CreateBodypart(long id, BodyProto body, string alias, string description, int order)
	{
		return new BodypartProto
		{
			Id = id,
			Body = body,
			BodyId = body.Id,
			Name = alias,
			Description = description,
			BodypartType = (int)BodypartTypeEnum.Wear,
			DisplayOrder = order,
			MaxLife = 100,
			SeveredThreshold = 100,
			PainModifier = 1.0,
			BleedModifier = 1.0,
			RelativeHitChance = 100,
			MaxSingleSize = (int)SizeCategory.Normal,
			IsCore = true
		};
	}

	[TestMethod]
	public void SeederQuestions_ReusesSharedNonHumanQuestionAnswers()
	{
		Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)new SupernaturalSeeder()).Questions
			.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual("nonhuman-health-model", questions["model"].SharedAnswerKey);
		Assert.AreEqual("damage-randomness", questions["random"].SharedAnswerKey);
		Assert.AreEqual("combat-message-style", questions["messagestyle"].SharedAnswerKey);
		Assert.IsTrue(questions.Values.All(x => x.AutoReuseLastAnswer));
	}

	[TestMethod]
	public void BodyAdditionsForTesting_HumanoidTailClonesFromQuadrupedBaseOwner()
	{
		using FuturemudDatabaseContext context = BuildContext();
		BodyProto quadrupedBase = CreateBody(1, "Quadruped Base");
		BodyProto toedQuadruped = CreateBody(2, "Toed Quadruped", quadrupedBase);
		BodyProto hornedFiend = CreateBody(3, "Supernatural Horned Fiend");
		context.BodyProtos.AddRange(quadrupedBase, toedQuadruped, hornedFiend);

		BodypartProto quadrupedBack = CreateBodypart(1, quadrupedBase, "lback", "lower back", 1);
		BodypartProto upperTail = CreateBodypart(2, quadrupedBase, "utail", "upper tail", 2);
		BodypartProto middleTail = CreateBodypart(3, quadrupedBase, "mtail", "middle tail", 3);
		BodypartProto lowerTail = CreateBodypart(4, quadrupedBase, "ltail", "lower tail", 4);
		BodypartProto fiendBack = CreateBodypart(5, hornedFiend, "lback", "lower back", 1);
		context.BodypartProtos.AddRange(quadrupedBack, upperTail, middleTail, lowerTail, fiendBack);
		context.BodypartProtoBodypartProtoUpstream.AddRange(
			new BodypartProtoBodypartProtoUpstream
			{
				Child = upperTail.Id,
				Parent = quadrupedBack.Id,
				ChildNavigation = upperTail,
				ParentNavigation = quadrupedBack
			},
			new BodypartProtoBodypartProtoUpstream
			{
				Child = middleTail.Id,
				Parent = upperTail.Id,
				ChildNavigation = middleTail,
				ParentNavigation = upperTail
			},
			new BodypartProtoBodypartProtoUpstream
			{
				Child = lowerTail.Id,
				Parent = middleTail.Id,
				ChildNavigation = lowerTail,
				ParentNavigation = middleTail
			});
		context.SaveChanges();

		var seeder = new SupernaturalSeeder();
		typeof(SupernaturalSeeder).GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, context);
		typeof(SupernaturalSeeder).GetField("_quadrupedBody", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, quadrupedBase);
		typeof(SupernaturalSeeder).GetField("_toedQuadrupedBody", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(seeder, toedQuadruped);

		typeof(SupernaturalSeeder).GetMethod("EnsureHumanoidTail", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(seeder, new object[] { hornedFiend });

		string[] clonedAliases = context.BodypartProtos
			.Where(x => x.BodyId == hornedFiend.Id)
			.Select(x => x.Name)
			.AsEnumerable()
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.IsSubsetOf(new[] { "utail", "mtail", "ltail" }, clonedAliases);
		Assert.IsTrue(context.BodypartProtoBodypartProtoUpstream.Any(x =>
			x.ChildNavigation.Name == "utail" &&
			x.ParentNavigation.Id == fiendBack.Id));
		Assert.IsTrue(context.Limbs.Any(x =>
			x.RootBodyId == hornedFiend.Id &&
			x.Name == "Tail" &&
			x.RootBodypart.Name == "utail"));
	}

	[TestMethod]
	public void PlanarProfileXml_IncorporealProfile_IsVisibleButNotPhysicalOnPrime()
	{
		XElement root = SupernaturalSeeder.BuildPlanarProfileXmlForTesting(1, 2,
			SupernaturalSeeder.SupernaturalPlanarProfile.Incorporeal);

		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 2));
		Assert.IsTrue(PlanarElementContains(root.Element("VisibleTo"), 1));
		Assert.IsTrue(bool.Parse(root.Element("Flags")!.Attribute("suspendsPhysicalContact")!.Value));

		XElement physical = root.Element("Interactions")!
			.Elements("Interaction")
			.Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
		Assert.IsFalse(PlanarElementContains(physical, 1));
		Assert.IsFalse(PlanarElementContains(physical, 2));
	}

	[TestMethod]
	public void PlanarProfileXml_DualNaturedProfile_IsPresentAndPhysicalOnBothPlanes()
	{
		XElement root = SupernaturalSeeder.BuildPlanarProfileXmlForTesting(1, 2,
			SupernaturalSeeder.SupernaturalPlanarProfile.DualNatured);

		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 1));
		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 2));
		XElement physical = root.Element("Interactions")!
			.Elements("Interaction")
			.Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
		Assert.IsTrue(PlanarElementContains(physical, 1));
		Assert.IsTrue(PlanarElementContains(physical, 2));
	}

	[TestMethod]
	public void AdditionalBodyFormMeritXml_DefaultsToSafeBuilderControlledForms()
	{
		SupernaturalSeeder.SupernaturalFormTemplate template =
			SupernaturalSeeder.FormTemplatesForTesting.Single(x => x.MeritName == "Supernatural Werewolf Hybrid Form");

		XElement root = SupernaturalSeeder.BuildAdditionalBodyFormMeritDefinitionForTesting(
			template,
			42,
			1,
			2);

		Assert.AreEqual("42", root.Element("Race")!.Value);
		Assert.AreEqual("hybrid", root.Element("Alias")!.Value);
		Assert.AreEqual(((int)BodySwitchTraumaMode.Automatic).ToString(), root.Element("TraumaMode")!.Value);
		Assert.IsTrue(bool.Parse(root.Element("AllowVoluntarySwitch")!.Value));
		Assert.IsFalse(bool.Parse(root.Element("AutoTransformWhenApplicable")!.Value));
		Assert.AreEqual("2", root.Element("ChargenAvailableProg")!.Value);
	}
}
