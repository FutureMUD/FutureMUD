#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaRatingsServiceTests
{
	[TestMethod]
	public void GetRating_ReturnsDefault_WhenNoRecordExists()
	{
		using var context = BuildContext();
		var service = CreateService(context);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(42L);
		var combatantClass = new Mock<ICombatantClass>();
		combatantClass.SetupGet(x => x.Id).Returns(7L);

		var rating = service.GetRating(character.Object, combatantClass.Object);

		Assert.AreEqual(1500.0m, rating);
	}

	[TestMethod]
	public void UpdateRatings_CreatesAndUpdatesRecords()
	{
		using var context = BuildContext();
		var service = CreateService(context);
		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(11L);

		var participantA = BuildParticipant(10L, 100L, 0, 1500.0m);
		var participantB = BuildParticipant(20L, 200L, 1, 1500.0m);

		var evt = new Mock<IArenaEvent>();
		evt.SetupGet(x => x.Arena).Returns(arena.Object);
		evt.SetupGet(x => x.Participants)
		   .Returns(new[] { participantA.Participant.Object, participantB.Participant.Object });

		var deltas = new Dictionary<ICharacter, decimal>
		{
			{ participantA.Character.Object, 12.5m },
			{ participantB.Character.Object, -7.5m }
		};

		service.UpdateRatings(evt.Object, deltas);

		var records = context.ArenaRatings.ToList();
		Assert.AreEqual(2, records.Count);
		Assert.AreEqual(1512.50m, records.Single(x => x.CharacterId == 10L).Rating);
		Assert.AreEqual(1492.50m, records.Single(x => x.CharacterId == 20L).Rating);

		var followUp = new Dictionary<ICharacter, decimal>
		{
			{ participantA.Character.Object, -2.5m }
		};

		service.UpdateRatings(evt.Object, followUp);

		var updated = context.ArenaRatings.Single(x => x.CharacterId == 10L);
		Assert.AreEqual(1510.00m, updated.Rating);
	}

	[TestMethod]
	public void ApplyDefaultElo_ComputesWinnerAndLoserDeltas()
	{
		using var context = BuildContext();
		var service = CreateService(context);
		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(5L);

		var winningParticipant = BuildParticipant(30L, 300L, 0, 1500.0m);
		var losingParticipant = BuildParticipant(40L, 400L, 1, 1500.0m);

		var evt = new Mock<IArenaEvent>();
		evt.SetupGet(x => x.Arena).Returns(arena.Object);
		evt.SetupGet(x => x.Participants)
		   .Returns(new[] { winningParticipant.Participant.Object, losingParticipant.Participant.Object });
		evt.Setup(x => x.Outcome)
		   .Returns(ArenaOutcome.Win);
		evt.Setup(x => x.WinningSides)
		   .Returns([0]);

		service.ApplyDefaultElo(evt.Object);

		var ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
		Assert.AreEqual(1516.00m, ratings[30L]);
		Assert.AreEqual(1484.00m, ratings[40L]);
	}

	[TestMethod]
	public void ApplyDefaultElo_UsesCharacterIds_WhenCharactersAreNotLoaded()
	{
		using var context = BuildContext();
		var service = CreateService(context);
		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(9L);

		var winner = BuildParticipant(50L, 500L, 0, 1500.0m, includeCharacter: false);
		var loser = BuildParticipant(60L, 600L, 1, 1500.0m, includeCharacter: false);

		var evt = new Mock<IArenaEvent>();
		evt.SetupGet(x => x.Arena).Returns(arena.Object);
		evt.SetupGet(x => x.Participants)
			.Returns(new[] { winner.Participant.Object, loser.Participant.Object });
		evt.SetupGet(x => x.Outcome).Returns(ArenaOutcome.Win);
		evt.SetupGet(x => x.WinningSides).Returns([0]);

		service.ApplyDefaultElo(evt.Object);

		var ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
		Assert.AreEqual(1516.00m, ratings[50L]);
		Assert.AreEqual(1484.00m, ratings[60L]);
	}

	[TestMethod]
	public void ApplyDefaultElo_RespectsEventTypeStyleAndKFactor()
	{
		using var context = BuildContext();
		var service = CreateService(context);
		var arena = new Mock<ICombatArena>();
		arena.SetupGet(x => x.Id).Returns(12L);
		var eventType = new Mock<IArenaEventType>();
		eventType.SetupGet(x => x.EloStyle).Returns(ArenaEloStyle.PairwiseIndividual);
		eventType.SetupGet(x => x.EloKFactor).Returns(16.0m);

		var winner = BuildParticipant(70L, 700L, 0, 1500.0m);
		var loser = BuildParticipant(80L, 800L, 1, 1500.0m);

		var evt = new Mock<IArenaEvent>();
		evt.SetupGet(x => x.Arena).Returns(arena.Object);
		evt.SetupGet(x => x.EventType).Returns(eventType.Object);
		evt.SetupGet(x => x.Participants).Returns(new[] { winner.Participant.Object, loser.Participant.Object });
		evt.SetupGet(x => x.Outcome).Returns(ArenaOutcome.Win);
		evt.SetupGet(x => x.WinningSides).Returns([0]);

		service.ApplyDefaultElo(evt.Object);

		var ratings = context.ArenaRatings.ToDictionary(x => x.CharacterId, x => x.Rating);
		Assert.AreEqual(1508.00m, ratings[70L]);
		Assert.AreEqual(1492.00m, ratings[80L]);
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		    .UseInMemoryDatabase(Guid.NewGuid().ToString())
		    .Options;
		return new FuturemudDatabaseContext(options);
	}

	private static ArenaRatingsService CreateService(FuturemudDatabaseContext context)
	{
		var gameworld = new Mock<IFuturemud>();
		return new ArenaRatingsService(gameworld.Object, () => context);
	}

	private static (Mock<IArenaParticipant> Participant, Mock<ICharacter> Character, Mock<ICombatantClass> CombatantClass) BuildParticipant(
		long characterId,
		long classId,
		int sideIndex,
		decimal? startingRating = null,
		bool isNpc = false,
		bool includeCharacter = true)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(characterId);
		var combatantClass = new Mock<ICombatantClass>();
		combatantClass.SetupGet(x => x.Id).Returns(classId);
		var participant = new Mock<IArenaParticipant>();
		participant.SetupGet(x => x.CharacterId).Returns(characterId);
		participant.SetupGet(x => x.Character).Returns(includeCharacter ? character.Object : null);
		participant.SetupGet(x => x.CombatantClass).Returns(combatantClass.Object);
		participant.SetupGet(x => x.SideIndex).Returns(sideIndex);
		participant.SetupGet(x => x.IsNpc).Returns(isNpc);
		participant.SetupGet(x => x.StartingRating).Returns(startingRating);
		return (participant, character, combatantClass);
	}

	private sealed class TestProgVariable : IProgVariable
	{
		private readonly object _value;
		private readonly ProgVariableTypes _type;

		public TestProgVariable(object value, ProgVariableTypes type)
		{
			_value = value;
			_type = type;
		}

		public ProgVariableTypes Type => _type;

		public object GetObject => _value;

		public IProgVariable GetProperty(string property)
		{
			throw new NotSupportedException();
		}
	}
}
