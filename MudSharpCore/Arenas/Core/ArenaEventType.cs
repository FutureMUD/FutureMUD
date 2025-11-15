#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Arenas;

public sealed class ArenaEventType : SaveableItem, IArenaEventType
{
	private readonly List<IArenaEventTypeSide> _sides = new();

	public ArenaEventType(MudSharp.Models.ArenaEventType model, CombatArena arena,
		Func<long, ICombatantClass?> classLookup, IArenaEliminationStrategy? eliminationStrategy = null)
	{
		Gameworld = arena.Gameworld;
		Arena = arena;
		_id = model.Id;
		_name = model.Name;
		BringYourOwn = model.BringYourOwn;
		RegistrationDuration = TimeSpan.FromSeconds(model.RegistrationDurationSeconds);
		PreparationDuration = TimeSpan.FromSeconds(model.PreparationDurationSeconds);
		TimeLimit = model.TimeLimitSeconds.HasValue
			? TimeSpan.FromSeconds(model.TimeLimitSeconds.Value)
			: null;
		BettingModel = (BettingModel)model.BettingModel;
		AppearanceFee = model.AppearanceFee;
		VictoryFee = model.VictoryFee;
		IntroProg = model.IntroProgId.HasValue ? Gameworld.FutureProgs.Get(model.IntroProgId.Value) : null;
		ScoringProg = model.ScoringProgId.HasValue ? Gameworld.FutureProgs.Get(model.ScoringProgId.Value) : null;
		ResolutionOverrideProg = model.ResolutionOverrideProgId.HasValue
			? Gameworld.FutureProgs.Get(model.ResolutionOverrideProgId.Value)
			: null;
		EliminationStrategy = eliminationStrategy;

		foreach (var side in model.ArenaEventTypeSides)
		{
			_sides.Add(new ArenaEventTypeSide(side, this, classLookup));
		}
	}

	public CombatArena Arena { get; }
	ICombatArena IArenaEventType.Arena => Arena;

	public IEnumerable<IArenaEventTypeSide> Sides => _sides;
	public bool BringYourOwn { get; }
	public TimeSpan RegistrationDuration { get; }
	public TimeSpan PreparationDuration { get; }
	public TimeSpan? TimeLimit { get; }
	public BettingModel BettingModel { get; }
	public decimal AppearanceFee { get; }
	public decimal VictoryFee { get; }
	public IFutureProg? IntroProg { get; }
	public IFutureProg? ScoringProg { get; }
	public IFutureProg? ResolutionOverrideProg { get; }
	public IArenaEliminationStrategy? EliminationStrategy { get; private set; }

	public IArenaEvent CreateInstance(DateTime scheduledTime, IEnumerable<IArenaReservation>? reservations = null)
	{
		return Arena.CreateEvent(this, scheduledTime, reservations);
	}

	public IArenaEventType Clone(string newName, ICharacter originator)
	{
		_ = originator;

		if (string.IsNullOrWhiteSpace(newName))
		{
			throw new ArgumentException("Clone name must be provided.", nameof(newName));
		}

		using (new FMDB())
		{
			var dbType = new MudSharp.Models.ArenaEventType
			{
				ArenaId = Arena.Id,
				Name = newName,
				BringYourOwn = BringYourOwn,
				RegistrationDurationSeconds = (int)RegistrationDuration.TotalSeconds,
				PreparationDurationSeconds = (int)PreparationDuration.TotalSeconds,
				TimeLimitSeconds = TimeLimit.HasValue ? (int)TimeLimit.Value.TotalSeconds : null,
				BettingModel = (int)BettingModel,
				AppearanceFee = AppearanceFee,
				VictoryFee = VictoryFee,
				IntroProgId = IntroProg?.Id,
				ScoringProgId = ScoringProg?.Id,
				ResolutionOverrideProgId = ResolutionOverrideProg?.Id
			};
			foreach (var side in _sides.OfType<ArenaEventTypeSide>())
			{
				var dbSide = new MudSharp.Models.ArenaEventTypeSide
				{
					Index = side.Index,
					Capacity = side.Capacity,
					Policy = (int)side.Policy,
					AllowNpcSignup = side.AllowNpcSignup,
					AutoFillNpc = side.AutoFillNpc,
					OutfitProgId = side.OutfitProg?.Id,
					NpcLoaderProgId = side.NpcLoaderProg?.Id
				};
				foreach (var cls in side.EligibleClasses)
				{
					dbSide.ArenaEventTypeSideAllowedClasses.Add(new ArenaEventTypeSideAllowedClass
					{
						ArenaCombatantClassId = cls.Id
					});
				}

				dbType.ArenaEventTypeSides.Add(dbSide);
			}

			FMDB.Context.ArenaEventTypes.Add(dbType);
			FMDB.Context.SaveChanges();
			var newType = new ArenaEventType(dbType, Arena, Arena.GetCombatantClass, EliminationStrategy);
			Arena.AddEventType(newType);
			return newType;
		}
	}

	public override string FrameworkItemType => "ArenaEventType";

	public override void Save()
	{
		Changed = false;
	}

}

internal sealed class ArenaEventTypeSide : IArenaEventTypeSide
{
	public ArenaEventTypeSide(MudSharp.Models.ArenaEventTypeSide model, ArenaEventType parent,
		Func<long, ICombatantClass?> classLookup)
	{
		EventType = parent;
		Index = model.Index;
		Capacity = model.Capacity;
		Policy = (ArenaSidePolicy)model.Policy;
		AllowNpcSignup = model.AllowNpcSignup;
		AutoFillNpc = model.AutoFillNpc;
		OutfitProg = model.OutfitProgId.HasValue ? parent.Gameworld.FutureProgs.Get(model.OutfitProgId.Value) : null;
		NpcLoaderProg = model.NpcLoaderProgId.HasValue
			? parent.Gameworld.FutureProgs.Get(model.NpcLoaderProgId.Value)
			: null;

		EligibleClasses = model.ArenaEventTypeSideAllowedClasses
			.Select(x => classLookup(x.ArenaCombatantClassId))
			.OfType<ICombatantClass>()
			.ToList();
	}

	public IArenaEventType EventType { get; }
	public int Index { get; }
	public int Capacity { get; }
	public ArenaSidePolicy Policy { get; }
	public IEnumerable<ICombatantClass> EligibleClasses { get; }
	public IFutureProg? OutfitProg { get; }
	public bool AllowNpcSignup { get; }
	public bool AutoFillNpc { get; }
	public IFutureProg? NpcLoaderProg { get; }

}
