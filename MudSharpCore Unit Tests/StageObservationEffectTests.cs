#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.Form.Audio;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Performances;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StageObservationEffectTests
{
	private static Mock<IRoom> Room(long id)
	{
		var room = new Mock<IRoom>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		return room;
	}

	[TestMethod]
	public void Handles_AllowIndependentOwnersAndPersistAcrossSerialization()
	{
		var source = Room(1);
		var audience = Room(2);
		var effect = new StageObservationEffect(source.Object, audience.Object, 10, "Curtain", "run:one");
		Assert.IsTrue(effect.AddHandle(new StageObservationEffect.Handle(11, "Staff", null)));
		Assert.IsFalse(effect.AddHandle(new StageObservationEffect.Handle(10, "duplicate", null)));

		var copy = new StageObservationEffect(effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()), source.Object);
		Assert.AreEqual(2, copy.Handles.Count);
		Assert.AreEqual(2L, copy.AudienceRoomId);
		Assert.AreEqual("run:one", copy.Handles.Single(x => x.Id == 10).RunKey);
		Assert.IsNull(copy.Handles.Single(x => x.Id == 11).RunKey);

		copy.RemoveRunHandles(new HashSet<string>());
		Assert.AreEqual(1, copy.Handles.Count);
		Assert.AreEqual(11L, copy.Handles[0].Id);
		Assert.IsTrue(copy.RemoveHandle(11));
		Assert.AreEqual(0, copy.Handles.Count);
	}

	[TestMethod]
	[DataRow(false, false, null)]
	[DataRow(false, true, "A voice speaks words.")]
	[DataRow(true, false, "The singer sings something that you cannot hear.")]
	[DataRow(true, true, "The singer sings, raising one hand, words.")]
	public void SpeechIncludesOnlyWhatTheSpectatorCanPerceive(bool sees, bool hears, string? expected)
	{
		var source = Mock.Of<ICharacter>();
		var viewer = new Mock<ICharacter>();
		viewer.Setup(x => x.CanSee(source, PerceiveIgnoreFlags.None)).Returns(sees);
		viewer.Setup(x => x.CanHear(source)).Returns(hears);
		var language = new Mock<ILanguage> { DefaultValue = DefaultValue.Mock };
		var words = new Mock<SpokenLanguageInfo>(language.Object, Mock.Of<IAccent>(), AudioVolume.Decent,
			"words", Outcome.Pass, source, null!, null!, null!);
		words.SetupGet(x => x.Form).Returns(LanguageForm.Spoken);
		words.Setup(x => x.ParseFor(viewer.Object)).Returns(" words.");
		var preamble = new Mock<IEmote>();
		preamble.Setup(x => x.ParseFor(viewer.Object, PerceiveIgnoreFlags.None)).Returns("The singer sings");
		var output = new Mock<IRecordableLanguageOutput>();
		output.SetupGet(x => x.PreLanguageEmote).Returns(preamble.Object);
		output.SetupGet(x => x.LanguageInfo).Returns(words.Object);
		output.SetupGet(x => x.DefaultSource).Returns(source);
		output.Setup(x => x.ShouldSee(viewer.Object)).Returns(true);
		output.Setup(x => x.ParseFor(viewer.Object)).Returns("The singer sings, raising one hand, words.");
		Assert.AreEqual(expected, StageObservationEffect.RenderFor(output.Object, viewer.Object, source));
	}

	[TestMethod]
	public void BlindSpectatorsCannotSeeAnOrdinaryUnflaggedAction()
	{
		var viewer = new Mock<ICharacter>();
		var source = Mock.Of<ICharacter>();
		viewer.Setup(x => x.CanHear(source)).Returns(true);
		var action = new Mock<IEmoteOutput>();
		action.SetupGet(x => x.DefaultSource).Returns(source);
		action.Setup(x => x.ShouldSee(viewer.Object)).Returns(true);
		action.Setup(x => x.ParseFor(viewer.Object)).Returns("Someone waves.");
		Assert.IsNull(StageObservationEffect.RenderFor(action.Object, viewer.Object, source));
		viewer.Setup(x => x.CanSee(source, PerceiveIgnoreFlags.None)).Returns(true);
		Assert.AreEqual("Someone waves.", StageObservationEffect.RenderFor(action.Object, viewer.Object, source));
	}

	[TestMethod]
	public void SilentInstrumentHasNoHearingOnlyRendition()
	{
		var performer = Mock.Of<ICharacter>();
		var listener = new Mock<ICharacter>();
		listener.Setup(x => x.CanHear(performer)).Returns(true);
		var action = new Mock<IEmote>();
		action.SetupGet(x => x.Source).Returns(performer);
		action.SetupGet(x => x.Valid).Returns(true);
		var instrument = new InstrumentPerformanceOutput(action.Object, action.Object, AudioVolume.Silent);
		Assert.IsNull(StageObservationEffect.RenderFor(instrument, listener.Object, performer));
	}

	[TestMethod]
	public void InstrumentEmission_SuppressesDistantAudioOnlyForActualStageRecipients()
	{
		var listener = new Mock<ICharacter>().Object;
		var other = new Mock<ICharacter>().Object;
		using (StageEmissionContext.BeginEmission())
		{
			StageEmissionContext.StageDelivered(listener);
			Assert.IsTrue(StageEmissionContext.AlreadyHeard(listener));
			Assert.IsFalse(StageEmissionContext.AlreadyHeard(other));
			using (StageEmissionContext.BeginEmission())
				Assert.IsFalse(StageEmissionContext.AlreadyHeard(listener));
			Assert.IsTrue(StageEmissionContext.AlreadyHeard(listener));
		}
		Assert.IsFalse(StageEmissionContext.AlreadyHeard(listener));
	}
}
