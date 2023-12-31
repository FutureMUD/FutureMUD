using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Menus;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.Character;

public partial class Character
{
	#region IMortal Members

	public ICorpse Corpse { get; set; }

	public event PerceivableEvent OnDeath;

	public virtual IGameItem Die()
	{
		// Reasons why character can't die
		if (IsAdministrator())
		{
			return null;
		}

		var deathBoard = Gameworld.Boards.Get(Gameworld.GetStaticLong("DeathsBoardId"));
		if (deathBoard != null)
		{
			var deathProg = Gameworld.FutureProgs.Get(Gameworld.GetStaticLong("PostToDeathsProg"));
			if ((bool?)deathProg?.Execute(this) != false)
			{
				var calendar = Location.Calendars.First();
				var clock = Location.Clocks.First();
				deathBoard.MakeNewPost(default(IAccount),
					$"{Id} - {PersonalName.GetName(NameStyle.FullWithNickname)} - {calendar.DisplayDate(Location.Date(calendar), CalendarDisplayMode.Short)} {clock.DisplayTime(Location.Time(clock), TimeDisplayTypes.Short)}",
					$"Character #{Id} ({PersonalName.GetName(NameStyle.FullWithNickname)}) died at Location #{Location.Id} ({Location.CurrentOverlay.CellName})\n\nBlood: {Body.CurrentBloodVolumeLitres / Body.TotalBloodVolumeLitres:P2}\nHad wounds from the following people:\n{Body.Wounds.Select(x => x.ActorOrigin).Where(x => x != this && x != null).Distinct().Select(x => $"  #{x.Id} ({x.PersonalName.GetName(NameStyle.FullWithNickname)})").ListToString(conjunction: "", twoItemJoiner: "\n", separator: "\n")}\n\nActive Drugs:\n{Body.ActiveDrugDosages.Select(x => $"  {x.Drug.Name} - {x.Grams:N3}g - {x.OriginalVector.Describe()}").ListToString(conjunction: "", twoItemJoiner: "\n", separator: "\n")}"
				);
			}
		}

		if (IsPlayerCharacter)
		{
			Gameworld.DiscordConnection.NotifyDeath(this);
			Gameworld.GameStatistics.UpdatePlayerDeath();
		}
		else
		{
			Gameworld.GameStatistics.UpdateNonPlayerDeath();
		}

		OnDeath?.Invoke(this);
		HandleEvent(Events.EventType.CharacterDies, this);
		foreach (var witness in Location.EventHandlers)
		{
			if (witness == this)
			{
				continue;
			}

			witness.HandleEvent(Events.EventType.CharacterDiesWitness, this, witness);
		}

		foreach (var ch in (Combat?.Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == this) ??
		                    Enumerable.Empty<ICharacter>())
		         .ToList())
		{
			ch.HandleEvent(Events.EventType.TargetSlain, ch, this);
		}

		State = CharacterState.Dead;
		_status = CharacterStatus.Deceased;

		if (!IsGuest && IsPlayerCharacter)
		{
			EmailHelper.Instance.SendEmail(EmailTemplateTypes.CharacterDeath, Account.EmailAddress, Account.Name,
				PersonalName.GetName(NameStyle.FullName), Gender.Subjective(),
				Location.Date(Birthday.Calendar).YearsDifference(Birthday).ToString("N0", this));
		}

		// TODO - race or even character specific death echoes?
		OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("RegularDeathEmote"), this, this)));
		PositionState = PositionSprawled.Instance;
		Party?.LeaveParty();
		Combat?.LeaveCombat(this);
		Movement?.CancelForMoverOnly(this);
		var combatTarget = CombatTarget;
		CombatTarget = null;
		combatTarget?.CheckCombatStatus();

		// Set HealthModel to Dead
		IGameItem corpse = null;
		var corpseModel = Race.CorpseModel; // TODO - overriding this
		if (corpseModel?.CreateCorpse == true)
		{
			corpse = CorpseGameItemComponentProto.CreateNewCorpse(this);
			Corpse = corpse.GetItemType<ICorpse>();
			Gameworld.Add(corpse);
			corpse.RoomLayer = RoomLayer;
			Location.Insert(corpse);
		}

		Location.Leave(this);

		if (Controller != null)
		{
			var oldOutputHandler = OutputHandler;
			Register(new NonPlayerOutputHandler());
			OutputHandler.Register(this);
			oldOutputHandler.Send(Gameworld.GetStaticString("DeathMessage"), false, true);
			_nextContext = new LoggedInMenu(Account, Gameworld);
			Controller.SetContext(_nextContext);
			_nextContext = null;
		}

		Gameworld.HeartbeatManager.TenSecondHeartbeat -= NeedsHeartbeat;
		Body.Die();
		using (new FMDB())
		{
			var dbchar = FMDB.Context.Characters.Find(Id);
			SaveMinutes(null);
			dbchar.State = (int)CharacterState.Dead;
			dbchar.Status = (int)CharacterStatus.Deceased;
			dbchar.DeathTime = DateTime.UtcNow;
			dbchar.NeedsModel = "NoNeeds";
			NeedsModel = new NoNeedsModel();
			FMDB.Context.SaveChanges();
		}

		Gameworld.Destroy(this);
		Changed = true;
		return corpse;
	}

	public ICharacter Resurrect(ICell location)
	{
		Location?.Leave(this);
		Location = location;
		location.Enter(this);

		using (new FMDB())
		{
			var dbchar = FMDB.Context.Characters.Find(Id);
			if (dbchar == null)
			{
				throw new ApplicationException("Tried to resurrect a non-existent character ID " + Id);
			}

			dbchar.State = (int)CharacterState.Awake;
			dbchar.Status = (int)CharacterStatus.Active;
			_status = CharacterStatus.Active;
			State = CharacterState.Awake;
			dbchar.NeedsModel = Chargen.NeedsModelProg != null
				? (string)Chargen.NeedsModelProg.Execute(this)
				: "NoNeeds";
			dbchar.DeathTime = null;
			NeedsModel = NeedsModelFactory.LoadNeedsModel(dbchar, this);
			FMDB.Context.SaveChanges();
		}

		LoginDateTime = DateTime.UtcNow;
		LastMinutesUpdate = LoginDateTime;
		StartNeedsHeartbeat();

		Body.Resurrect(location);
		Corpse = null;
		return this;
	}

	public void CheckHealthStatus()
	{
		Body.CheckHealthStatus();
	}

	public event WoundEvent OnWounded
	{
		add => Body.OnWounded += value;
		remove => Body.OnWounded -= value;
	}

	public event WoundEvent OnHeal
	{
		add => Body.OnHeal += value;
		remove => Body.OnHeal -= value;
	}

	public event WoundEvent OnRemoveWound
	{
		add => Body.OnRemoveWound += value;
		remove => Body.OnRemoveWound -= value;
	}

	#endregion

	#region IBreathe Members

	public bool NeedsToBreathe => Body.NeedsToBreathe;
	public bool IsBreathing => Body.IsBreathing;
	public bool CanBreathe => Body.CanBreathe;

	public TimeSpan HeldBreathTime
	{
		get => Body.HeldBreathTime;
		set => Body.HeldBreathTime = value;
	}

	public double HeldBreathPercentage => Body.HeldBreathPercentage;

	public IFluid BreathingFluid => Body.BreathingFluid;

	public IBreathingStrategy BreathingStrategy => Body.BreathingStrategy;

	#endregion

	#region IHaveWounds Members

	public IHealthStrategy HealthStrategy => Body.HealthStrategy;

	public IEnumerable<IWound> Wounds => Body.Wounds;

	public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
	{
		return Body.VisibleWounds(voyeur, examinationType);
	}

	public void EvaluateWounds()
	{
		Body.EvaluateWounds();
	}

	public void ProcessPassiveWound(IWound wound)
	{
		Body.ProcessPassiveWound(wound);
	}

	public void AddWound(IWound wound)
	{
		Body.AddWound(wound);
	}

	public void AddWounds(IEnumerable<IWound> wounds)
	{
		Body.AddWounds(wounds);
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
	{
		return Body.PassiveSufferDamage(damage);
	}

	public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
	{
		return Body.PassiveSufferDamage(damage, proximity, facing);
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		return Body.SufferDamage(damage);
	}

	public WoundSeverity GetSeverityFor(IWound wound)
	{
		return Body.GetSeverityFor(wound);
	}

	public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
	{
		return Body.GetSeverityFloor(severity, usePercentageModel);
	}

	public void StartHealthTick(bool initial = false)
	{
		Body.StartHealthTick(initial);
	}

	public void EndHealthTick()
	{
		Body.EndHealthTick();
	}

	public void CureAllWounds()
	{
		Body.CureAllWounds();
	}

	#endregion
}