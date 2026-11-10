#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DiscordChargenOptionsTests
{
	[TestMethod]
	public void RaceChoices_UseOwnedDraftContextWithoutChangingIt()
	{
		var draft = new Mock<IChargen>();
		var race = new Mock<IRace>();
		race.SetupGet(x => x.Name).Returns("Human");
		race.Setup(x => x.ChargenAvailable(draft.Object)).Returns(true);
		var races = BuildRepository(new[] { race.Object });
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Races).Returns(races.Object);
		var storyboard = new RacePickerScreenStoryboard(world.Object, new MudSharp.Models.ChargenScreenStoryboard
		{
			Id = 1,
			StageDefinition = "<Definition><Blurb>Pick a race.</Blurb><ShowUnselectableRacesAsBlanks>false</ShowUnselectableRacesAsBlanks></Definition>"
		});

		var choices = storyboard.GetDiscordOptions(draft.Object).ToList();

		CollectionAssert.AreEqual(new[] { "Human" }, choices);
		race.Verify(x => x.ChargenAvailable(draft.Object), Times.Once);
		draft.VerifySet(x => x.SelectedRace = It.IsAny<IRace>(), Times.Never);
	}

	private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> values)
		where T : class, IFrameworkItem
	{
		var repository = new Mock<IUneditableAll<T>>();
		repository.Setup(x => x.GetEnumerator()).Returns(() => values.GetEnumerator());
		return repository;
	}
}
