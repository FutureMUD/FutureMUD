#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MediaErasureTests
{
	// 0: physical digital command; 1: implant internal storage; 2: implant selected drive.
	[DataTestMethod]
	[DataRow(0, "notes.txt")]
	[DataRow(0, "NOTES.TXT")]
	[DataRow(1, "notes.txt")]
	[DataRow(1, "NOTES.TXT")]
	[DataRow(2, "notes.txt")]
	[DataRow(2, "NOTES.TXT")]
	public void Erase_ExactTextFile_IsPreservedWithoutEventsOrDirtyState(int path, string name)
	{
		var fixture = new RecorderFixture(path);
		fixture.Files.WriteFile("notes.txt", "valuable text");
		Assert.IsTrue(fixture.Files.WriteMediaFile("notes.txt.av", 1L, 100L, false, out _));
		fixture.AssertRejectedWithoutChanges(name);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Erase_LoadedTextAndMediaCaseCollision_PreservesBoth(int path)
	{
		var fixture = new RecorderFixture(path);
		fixture.Files.LoadFiles([new ComputerMutableTextFile { FileName = "clip", TextContents = "text" }]);
		fixture.Files.LoadMediaFiles([new ComputerMutableMediaFile
			{ FileName = "CLIP", MediaRecordingId = 1L, SizeInBytes = 100L }]);
		fixture.AssertRejectedWithoutChanges("CLIP");
	}

	[DataTestMethod]
	[DataRow(0, "clip")]
	[DataRow(1, "clip")]
	[DataRow(2, "clip")]
	[DataRow(0, "1")]
	[DataRow(1, "1")]
	[DataRow(2, "1")]
	[DataRow(0, "missing")]
	[DataRow(1, "missing")]
	[DataRow(2, "missing")]
	[DataRow(0, "")]
	[DataRow(1, "")]
	[DataRow(2, "")]
	public void Erase_NonExactAmbiguousNumericMissingOrEmptyName_PreservesFiles(int path, string name)
	{
		var fixture = new RecorderFixture(path);
		Assert.IsTrue(fixture.Files.WriteMediaFile("clip-one.av", 1L, 100L, false, out _));
		Assert.IsTrue(fixture.Files.WriteMediaFile("clip-two.av", 2L, 200L, false, out _));
		fixture.AssertRejectedWithoutChanges(name);
	}

	[DataTestMethod]
	[DataRow(0, "audio.wav", "AUDIO.WAV", 1L)]
	[DataRow(1, "audio.wav", "AUDIO.WAV", 1L)]
	[DataRow(2, "audio.wav", "AUDIO.WAV", 1L)]
	[DataRow(0, "video.av", "video.av", 2L)]
	[DataRow(1, "video.av", "video.av", 2L)]
	[DataRow(2, "video.av", "video.av", 2L)]
	[DataRow(0, "clip", "clip", 3L)]
	[DataRow(1, "clip", "clip", 3L)]
	[DataRow(2, "clip", "clip", 3L)]
	[DataRow(0, "recording one.av", "\"recording one.av\"", 4L)]
	[DataRow(1, "recording one.av", "\"recording one.av\"", 4L)]
	[DataRow(2, "recording one.av", "\"recording one.av\"", 4L)]
	public void Erase_ExactMedia_DeletesOnlySelectedReferenceWithOriginalMessageAndEvents(
		int path, string fileName, string argument, long recordingId)
	{
		var fixture = new RecorderFixture(path);
		fixture.Files.WriteFile("notes.txt", "valuable text");
		Assert.IsTrue(fixture.Files.WriteMediaFile(fileName, recordingId, 100L, false, out _));
		Assert.IsTrue(fixture.Files.WriteMediaFile(fileName + "2", 5L, 200L, false, out _));
		fixture.ResetObservations();
		var usedBytes = fixture.Files.UsedBytes;

		fixture.Erase(argument);

		Assert.IsFalse(fixture.Files.FileExists(fileName));
		Assert.AreEqual("valuable text", fixture.Files.ReadFile("notes.txt"));
		Assert.IsTrue(fixture.Files.FileExists(fileName + "2"));
		Assert.AreEqual(usedBytes - 100L, fixture.Files.UsedBytes);
		Assert.IsTrue(fixture.FileOwner.Changed);
		Assert.AreEqual(1, fixture.Changes.Count);
		var change = fixture.Changes.Single();
		Assert.AreEqual(ComputerFileSystemChangeType.Deleted, change.ChangeType);
		Assert.AreEqual(ComputerFileKind.Media, change.Kind);
		Assert.AreEqual(recordingId, change.MediaRecordingId);
		Assert.AreEqual(new StringStack(argument).SafeRemainingArgument, change.FileName);
		fixture.Recordings.Verify(x => x.DeleteReference(fixture.FileOwner.Id, change.FileName,
			out It.Ref<string>.IsAny), Times.Once);
		Assert.AreEqual(1, fixture.Messages.Count);
		Assert.IsTrue(path == 0 ? fixture.Messages[0].StartsWith("You erase ") :
			fixture.Messages[0] == "Recording erased.");
		fixture.AssertInactiveInternalStoragePreserved();
	}

	[TestMethod]
	public void GeneralDeleteFile_AuthorisedTextDeletion_RemainsAvailable()
	{
		var fixture = new RecorderFixture(0);
		fixture.Files.WriteFile("notes.txt", "valuable text");
		Assert.IsTrue(fixture.Files.WriteMediaFile("notes.txt.av", 1L, 100L, false, out _));
		fixture.ResetObservations();

		Assert.IsTrue(fixture.Files.DeleteFile("NOTES.TXT"));

		Assert.IsFalse(fixture.Files.FileExists("notes.txt"));
		Assert.IsTrue(fixture.Files.FileExists("notes.txt.av"));
		Assert.IsTrue(fixture.FileOwner.Changed);
		Assert.AreEqual(ComputerFileKind.Text, fixture.Changes.Single().Kind);
		fixture.Recordings.Verify(x => x.DeleteReference(It.IsAny<long>(), It.IsAny<string>(),
			out It.Ref<string>.IsAny), Times.Never);
	}

	private sealed class RecorderFixture
	{
		private readonly int _path;
		private readonly Mock<ICharacter> _actor = new();
		private readonly Mock<IGameItem> _item = new();
		private readonly DigitalMediaRecorderGameItemComponent _recorder;
		private readonly ComputerMutableFileSystem? _inactiveInternalStorage;
		public Mock<IMediaRecordingService> Recordings { get; } = new();
		public List<ComputerFileSystemChange> Changes { get; } = [];
		public List<string> Messages { get; } = [];
		public ComputerMutableFileSystem Files { get; }
		public GameItemComponent FileOwner { get; }

		public RecorderFixture(int path)
		{
			_path = path;
			var gameworld = new Mock<IFuturemud>();
			gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			gameworld.SetupGet(x => x.MediaRecordingService).Returns(Recordings.Object);
			var bodyPrototype = new Mock<IBodyPrototype>();
			bodyPrototype.SetupGet(x => x.AllBodypartsBonesAndOrgans).Returns(Array.Empty<IBodypart>());
			var bodies = new Mock<IUneditableAll<IBodyPrototype>>();
			bodies.Setup(x => x.Get(It.IsAny<long>())).Returns(bodyPrototype.Object);
			gameworld.SetupGet(x => x.BodyPrototypes).Returns(bodies.Object);
			_item.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			_item.Setup(x => x.HealthStrategy.CurrentHealthPercentage(_item.Object)).Returns(1.0);
			_item.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("a recorder");
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), true, false))
				.Callback<string, bool, bool>((message, _, _) => Messages.Add(message));
			_actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			if (path == 0)
			{
				_recorder = new DigitalMediaRecorderGameItemComponent(
					LoadPrototype<DigitalMediaRecorderGameItemComponentProto>(gameworld.Object), _item.Object);
				FileOwner = _recorder;
			}
			else
			{
				var implant = new ImplantAVRecorderGameItemComponent(
					LoadPrototype<ImplantAVRecorderGameItemComponentProto>(gameworld.Object), _item.Object);
				_recorder = implant;
				FileOwner = implant;
				var body = new Mock<IBody>();
				body.SetupGet(x => x.Actor).Returns(_actor.Object);
				var bus = new Mock<IImplantNeuralLink>();
				bus.SetupGet(x => x.DNIConnected).Returns(true);
				bus.Setup(x => x.IsLinkedTo(It.IsAny<IImplant>())).Returns(true);
				var implants = new List<IImplant> { implant, bus.Object };
				body.SetupGet(x => x.Implants).Returns(implants);
				Support(implant).Install(body.Object);
				typeof(PoweredMachineBaseGameItemComponent).GetField("_onAndPowered",
					BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(implant, true);
				if (path == 2)
				{
					_inactiveInternalStorage = (ComputerMutableFileSystem)implant.FileSystem!;
					_inactiveInternalStorage.WriteFile("internal.txt", "internal text");
					Assert.IsTrue(_inactiveInternalStorage.WriteMediaFile("internal.av", 10L, 100L, false, out _));
					var storage = new ImplantComputerStorageGameItemComponent(
						LoadPrototype<ImplantComputerStorageGameItemComponentProto>(gameworld.Object), _item.Object);
					storage.InitialiseWithoutPersistence(88L);
					Support(storage).Install(body.Object);
					typeof(ImplantComputerStorageGameItemComponent).GetField("_powered",
						BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(storage, true);
					implants.Add(storage);
					typeof(ImplantAVRecorderGameItemComponent).GetField("_selectedStorageId",
						BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(implant, storage.Id);
					FileOwner = storage;
				}
			}
			_recorder.InitialiseWithoutPersistence(77L);
			Files = (ComputerMutableFileSystem)_recorder.RecordingFileSystem;
			Files.FileChanged += (_, change) => Changes.Add(change);
		}

		public void Erase(string argument)
		{
			if (_path == 0)
			{
				typeof(MudSharp.Character.Character).Assembly.GetType("MudSharp.Commands.Modules.ElectronicsModule")!
					.GetMethod("MediaErase", BindingFlags.Static | BindingFlags.NonPublic)!
					.Invoke(null, [_actor.Object, _item.Object, null, _recorder, new StringStack(argument)]);
				return;
			}
			((ImplantAVRecorderGameItemComponent)_recorder).IssueCommand("erase", new StringStack(argument));
		}

		public void ResetObservations()
		{
			FileOwner.Changed = false;
			_recorder.Changed = false;
			Changes.Clear();
			Messages.Clear();
			Recordings.Invocations.Clear();
		}

		public void AssertRejectedWithoutChanges(string argument)
		{
			ResetObservations();
			var files = Files.Files.ToArray();
			var usedBytes = Files.UsedBytes;
			Erase(argument);
			CollectionAssert.AreEqual(files, Files.Files.ToArray());
			Assert.AreEqual(usedBytes, Files.UsedBytes);
			Assert.IsFalse(FileOwner.Changed);
			Assert.IsFalse(_recorder.Changed);
			Assert.AreEqual(0, Changes.Count);
			Recordings.Verify(x => x.DeleteReference(It.IsAny<long>(), It.IsAny<string>(),
				out It.Ref<string>.IsAny), Times.Never);
			Assert.AreEqual(_path == 0 ? argument.Length == 0 ? "Which recording do you want to erase?" :
				"That recorder has no media file with that name." : "There is no such recording.", Messages.Single());
			AssertInactiveInternalStoragePreserved();
		}

		public void AssertInactiveInternalStoragePreserved()
		{
			if (_inactiveInternalStorage is null) return;
			Assert.AreEqual("internal text", _inactiveInternalStorage.ReadFile("internal.txt"));
			Assert.AreEqual(10L, _inactiveInternalStorage.GetFile("internal.av")!.MediaRecordingId);
			Assert.AreEqual(2, _inactiveInternalStorage.Files.Count());
			Assert.IsFalse(_recorder.Changed);
		}

		private static ImplantMachineRuntime Support(GameItemComponent component) =>
			(ImplantMachineRuntime)component.GetType().GetField("_implant",
				BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(component)!;

		private static T LoadPrototype<T>(IFuturemud gameworld) where T : GameItemComponentProto =>
			(T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic, null,
				[new MudSharp.Models.GameItemComponentProto
				{
					Id = 1L, Name = typeof(T).Name, Description = "test recorder", Definition = "<Definition />",
					EditableItem = new MudSharp.Models.EditableItem { BuilderDate = DateTime.UtcNow }
				}, gameworld], null)!;
	}
}
