#nullable enable

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.Impacts;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProjectMasteryTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SwordsFocus_LeavingAndReloadingPreservesTrainedSkillUntilRefocused(bool secondary)
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.MagicCasting).Returns((IMagicCastingService?)null);
		var improver = new Mock<IImprovementModel>();
		world.Setup(x => x.ImprovementModels.Get(1)).Returns(improver.Object);
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition
		{
			Id = 12, Name = "Swords", Type = (int)TraitType.Skill, ImproverId = 1
		}, world.Object) { Cap = new TraitExpression("70", world.Object) };
		world.Setup(x => x.Traits.Get(12)).Returns(definition);
		var impact = new TraitCapImpact(new MudSharp.Models.ProjectLabourImpact
		{
			MinimumHoursForImpactToKickIn = 1,
			Definition = "<Impact><Trait>12</Trait><TraitChange>10</TraitChange></Impact>"
		}, world.Object);
		var focus = Mock.Of<IProjectLabourRequirement>(x => x.LabourImpacts == new[] { impact });
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(11);
		actor.SetupProperty(x => x.CurrentProject);
		actor.SetupProperty(x => x.CurrentProjectHours);
		var body = TestObjectFactory.CreateUninitialized<RuntimeBody>();
		body.Actor = actor.Object;
		actor.Setup(x => x.TraitMaxValue(It.IsAny<ITrait>())).Returns((ITrait trait) => body.TraitMaxValue(trait));
		var owner = secondary ? Mock.Of<ICharacter>(x => x.Id == 7) : actor.Object;
		var skill = new Skill(definition, 70, owner);
		actor.Setup(x => x.TraitValue(definition, TraitBonusContext.None)).Returns(() => skill.ValueFor(actor.Object));
		actor.Setup(x => x.TraitRawValue(definition)).Returns(() => skill.ValueFor(actor.Object));
		var display = new CurrentMaxDecorator(new MudSharp.Models.TraitDecorator(), false);
		improver.Setup(x => x.GetImprovement(actor.Object, It.IsAny<ITrait>(), Difficulty.Normal, Outcome.Pass,
			TraitUseType.Practical)).Returns(8);
		bool Practise() => skill.TraitUsed(actor.Object, Outcome.Pass, Difficulty.Normal, TraitUseType.Practical, []);

		var project = Mock.Of<IActiveProject>();
		actor.Object.CurrentProject = (project, focus);
		Assert.IsFalse(Practise());
		actor.Object.CurrentProjectHours = 1;
		Assert.AreEqual(70.0, skill.ValueFor(actor.Object));
		Assert.IsTrue(Practise());
		Assert.AreEqual(78.0, skill.ValueFor(actor.Object));
		Assert.AreEqual("78 / 80", display.Decorate(skill, actor.Object));
		Assert.AreEqual(secondary ? 70.0 : 78.0, skill.Value);

		actor.Object.CurrentProject = (null, null);
		Assert.AreEqual(70.0, skill.Value);
		Assert.AreEqual("70 / 70", display.Decorate(skill, actor.Object));
		Assert.IsFalse(Practise());
		Assert.AreEqual(78.0, skill.RawValue);
		skill.Save();
		database.Context.SaveChanges();
		database.Context.ChangeTracker.Clear();
		var row = database.Context.CharacterTraits.AsNoTracking().Single();
		Assert.AreEqual(78.0, row.Value);
		skill = (Skill)definition.LoadTrait(new MudSharp.Models.Trait { Value = row.Value }, owner);
		Assert.AreEqual(70.0, skill.Value);

		actor.Object.CurrentProject = (project, focus);
		Assert.AreEqual(78.0, skill.ValueFor(actor.Object));
		var casting = new Mock<IMagicCastingService>();
		casting.Setup(x => x.RawSkillImprovementCap(owner, definition.Id)).Returns(79.0);
		world.SetupGet(x => x.MagicCasting).Returns(casting.Object);
		Assert.IsTrue(Practise());
		Assert.AreEqual(79.0, skill.RawValue);
		Assert.IsFalse(Practise());
		world.SetupGet(x => x.MagicCasting).Returns((IMagicCastingService?)null);
		Assert.IsTrue(Practise());
		Assert.AreEqual(80.0, skill.ValueFor(actor.Object));
		Assert.IsFalse(Practise());
		Assert.AreEqual(80.0, skill.RawValue);
	}
}
