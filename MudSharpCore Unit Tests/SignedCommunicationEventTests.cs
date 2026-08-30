#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SignedCommunicationEventTests
{
	[TestMethod]
	public void HandleEvents_DirectSigningPassesLanguageAndVarietyObjects()
	{
		var actor = new Mock<ICharacter>();
		var target = new Mock<IPerceivable>();
		var body = new Mock<IBody>();
		var location = new Mock<ICell>();
		var language = new Mock<ISignedLanguage>();
		var variety = new Mock<ISignedLanguageVariety>();
		location.SetupGet(x => x.EventHandlers).Returns([]);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		body.SetupGet(x => x.Location).Returns(location.Object);
		object[]? actorPayload = null;
		object[]? targetPayload = null;
		actor.Setup(x => x.HandleEvent(EventType.CharacterSignsDirect, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => actorPayload = arguments);
		target.Setup(x => x.HandleEvent(EventType.CharacterSignsDirectTarget, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => targetPayload = arguments);

		SignedCommunicationService.HandleEvents(body.Object, target.Object, "hello", language.Object,
			variety.Object, Outcome.Pass);

		Assert.IsNotNull(actorPayload);
		Assert.AreSame(language.Object, actorPayload[2]);
		Assert.AreSame(variety.Object, actorPayload[3]);
		Assert.IsNotNull(targetPayload);
		Assert.AreSame(language.Object, targetPayload[2]);
		Assert.AreSame(variety.Object, targetPayload[3]);
	}
}
