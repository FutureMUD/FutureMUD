#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
[DoNotParallelize]
public class ChargenReorderTests
{
	[TestMethod]
	public void Reorder_ExistingPredecessorOrSelf_PreservesOrderAndProgression()
	{
		using var f = new Fixture();
		f.Command("NotePicker WelcomeScreen");
		f.Command("NotePicker NotePicker");
		f.AssertSequence(ChargenStage.SelectNotes, ChargenStage.SelectDescription);
		Assert.IsFalse(f.Storyboard.StageScreenMap.Values.Any(x => x.Changed));
		f.Reload();
		f.AssertSequence(ChargenStage.SelectNotes, ChargenStage.SelectDescription);
	}

	[TestMethod]
	public void Reorder_MovesInBothDirections_PersistsOrderAndProgressionWithoutLinkingTheMenu()
	{
		using var f = new Fixture();
		f.Command("DescriptionPicker WelcomeScreen");
		f.AssertSequence(ChargenStage.SelectDescription, ChargenStage.SelectNotes);
		f.Reload();
		f.AssertSequence(ChargenStage.SelectDescription, ChargenStage.SelectNotes);
		f.Command("DescriptionPicker NotePicker");
		f.Reload();
		f.AssertSequence(ChargenStage.SelectNotes, ChargenStage.SelectDescription);
	}

	[TestMethod]
	public void Reorder_DependencyWouldFollowItsConsumer_RejectsWithoutChangingEitherOrdering()
	{
		using var f = new Fixture();
		f.Storyboard.AddDependency(ChargenStage.SelectDescription, ChargenStage.SelectNotes);
		f.Reload();
		f.Command("DescriptionPicker WelcomeScreen");
		f.AssertSequence(ChargenStage.SelectNotes, ChargenStage.SelectDescription);
		Assert.IsFalse(f.Storyboard.StageScreenMap.Values.Any(x => x.Changed));
	}

	private sealed class Fixture : IDisposable
	{
		private readonly InMemoryDatabaseTestScope _database = new();
		private readonly Mock<IFuturemud> _world = new();
		private readonly Mock<ICharacter> _actor = new();
		public ChargenStoryboard Storyboard { get; private set; }

		public Fixture()
		{
			var definitions = new (ChargenStage Stage, string Type, string Definition)[]
			{
				(ChargenStage.Welcome, "WelcomeScreen", "<Definition />"),
				(ChargenStage.SelectNotes, "NotePicker", "<Definition />"),
				(ChargenStage.SelectDescription, "DescriptionPicker", "<Definition><AllowCustomDescription>true</AllowCustomDescription><AllowEntityDescriptionPatterns>true</AllowEntityDescriptionPatterns><FullDescBlurb>Describe yourself</FullDescBlurb><SDescBlurb>Short description</SDescBlurb></Definition>"),
				(ChargenStage.Submit, "Submit", "<Definition />"),
				(ChargenStage.Menu, "Menu", "<Definition />")
			};
			for (var i = 0; i < definitions.Length; i++)
			{
				var entry = definitions[i];
				_database.Context.ChargenScreenStoryboards.Add(new MudSharp.Models.ChargenScreenStoryboard
				{
					Id = i + 1, ChargenStage = (int)entry.Stage, ChargenType = entry.Type, Order = i * 10,
					StageDefinition = entry.Definition,
					NextStage = (int)(i < 3 ? definitions[i + 1].Stage : ChargenStage.None)
				});
			}

			_database.Context.SaveChanges();
			_world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Storyboard = new ChargenStoryboard(_world.Object);
			_world.SetupGet(x => x.ChargenStoryboard).Returns(() => Storyboard);
			_actor.SetupGet(x => x.Gameworld).Returns(_world.Object);
			_actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		}

		public void Command(string input) => typeof(ChargenModule)
			.GetMethod("ChargenReorder", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [_actor.Object, new StringStack(input)]);

		public void Reload()
		{
			foreach (var screen in Storyboard.StageScreenMap.Values.Where(x => x.Changed)) screen.Save();
			_database.Context.SaveChanges();
			_database.Context.ChangeTracker.Clear();
			Storyboard = new ChargenStoryboard(_world.Object);
		}

		public void AssertSequence(ChargenStage first, ChargenStage second)
		{
			var expected = new[] { ChargenStage.Welcome, first, second, ChargenStage.Submit, ChargenStage.Menu };
			CollectionAssert.AreEqual(expected, Storyboard.DefaultOrder.ToArray());
			Assert.AreEqual(ChargenStage.Welcome, Storyboard.FirstStage);
			var visited = new List<ChargenStage>();
			for (var stage = Storyboard.FirstStage; stage != ChargenStage.None; stage = Storyboard.DefaultNextStage[stage])
			{
				Assert.IsFalse(visited.Contains(stage), "Progression contains a cycle.");
				visited.Add(stage);
			}
			CollectionAssert.AreEqual(expected.Take(4).ToArray(), visited.ToArray());
			Assert.AreEqual(ChargenStage.None, Storyboard.DefaultNextStage[ChargenStage.Menu]);
		}

		public void Dispose() => _database.Dispose();
	}
}
