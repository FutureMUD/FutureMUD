#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyTemplateHealthTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TemplateConstruction_PreservesExplicitHealthStrategyOrUsesRaceDefault(bool hasOverride)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var prototype = new Mock<IBodyPrototype> { DefaultValue = DefaultValue.Mock };
		var baseline = new Mock<IHealthStrategy>();
		var selected = new Mock<IHealthStrategy>();
		var race = new Mock<IRace> { DefaultValue = DefaultValue.Mock };
		race.SetupGet(x => x.BaseBody).Returns(prototype.Object);
		race.SetupGet(x => x.DefaultHealthStrategy).Returns(baseline.Object);
		var template = new Mock<ICharacterTemplate> { DefaultValue = DefaultValue.Mock };
		template.SetupGet(x => x.SelectedRace).Returns(race.Object);
		template.SetupGet(x => x.HealthStrategy).Returns(hasOverride ? selected.Object : null);

		var body = new Body(world.Object, actor.Object, template.Object, deferInitialisation: true);

		Assert.AreSame(hasOverride ? selected.Object : baseline.Object, body.HealthStrategy);
		Assert.AreSame(baseline.Object, race.Object.DefaultHealthStrategy);
	}
}
