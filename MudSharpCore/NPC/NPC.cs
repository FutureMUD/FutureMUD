using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.NPC.AI;
using MudSharp.NPC.Templates;
using MudSharp.PerceptionEngine.Handlers;

namespace MudSharp.NPC;

public class NPC : Character.Character, INPC
{
	private readonly List<IArtificialIntelligence> _AIs = new();
	public IEnumerable<IArtificialIntelligence> AIs => _AIs;
	private bool _aiChanged = false;

	public bool AIChanged
	{
		get => _aiChanged;
		set
		{
			_aiChanged = value;
			if (value)
			{
				Changed = true;
			}
		}
	}

	private long _npcID;
	private long? _bodyguardingCharacterId;

	public NPC(Models.Npc npc, Models.Character dbchar, IFuturemud gameworld)
		: base(dbchar, gameworld)
	{
		LoadNPCFromDB(npc);
		var controller = new NPCController();
		controller.UpdateControlFocus(this);
		SilentAssumeControl(controller);
		PermissionLevel = PermissionLevel.NPC;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		Register(new NonPlayerOutputHandler());
	}

	public NPC(IFuturemud gameworld, ICharacterTemplate template, INPCTemplate npcTemplate)
		: base(gameworld, template)
	{
		_AIs.AddRange(npcTemplate.ArtificialIntelligences);
		Template = npcTemplate;
		var controller = new NPCController();
		controller.UpdateControlFocus(this);
		SilentAssumeControl(controller);
		PermissionLevel = PermissionLevel.NPC;
		CommandTree = Gameworld.RetrieveAppropriateCommandTree(this);
		Register(new NonPlayerOutputHandler());
	}

	public INPCTemplate Template { get; private set; }

	public override bool IsPlayerCharacter => false;

	public void SetupEventSubscriptions()
	{
		if (AIs.Any(x => x.HandlesEvent(EventType.FiveSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += FiveSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.TenSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.MinuteTick)))
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += MinuteHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.HourTick)))
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HourHeartbeat;
		}
	}

	public void ReleaseEventSubscriptions()
	{
		if (AIs.Any(x => x.HandlesEvent(EventType.FiveSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= FiveSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.TenSecondTick)))
		{
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.MinuteTick)))
		{
			Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat -= MinuteHeartbeat;
		}

		if (AIs.Any(x => x.HandlesEvent(EventType.HourTick)))
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HourHeartbeat;
		}
	}

	public override bool Quit(bool silent = false)
	{
		ReleaseEventSubscriptions();
		if (_bodyguardingCharacterId.HasValue)
		{
			if (!Gameworld.CachedBodyguards.ContainsKey(_bodyguardingCharacterId.Value))
			{
				Gameworld.CachedBodyguards[_bodyguardingCharacterId.Value] = new List<ICharacter>();
			}

			Gameworld.CachedBodyguards[_bodyguardingCharacterId.Value].Add(this);
		}

		return base.Quit(silent);
	}

	#region Overrides of Character

	public override IGameItem Die()
	{
		if (_bodyguardingCharacterId.HasValue)
		{
			_bodyguardingCharacterId = null;
			Changed = true;
		}

		return base.Die();
	}

	#endregion

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (State.HasFlag(CharacterState.Dead) || State.HasFlag(CharacterState.Stasis))
		{
			return false;
		}

		if (EffectsOfType<IPauseAIEffect>().Any())
			// Hooks still fire when AI is paused
		{
			return base.HandleEvent(type, arguments);
		}

		// AI Events are only handled once. AI Events firing does not prevent regular hooks from firing.
		var AIEvents = _AIs.Any(x => x.HandleEvent(type, arguments));
		return base.HandleEvent(type, arguments) || AIEvents;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return base.HandlesEvent(types) || AIs.Any(x => x.HandlesEvent(types));
	}

	protected void LoadNPCFromDB(Models.Npc npc)
	{
		_npcID = npc.Id;
		Template = Gameworld.NpcTemplates.Get(npc.TemplateId, npc.TemplateRevnum);
		_bodyguardingCharacterId = npc.BodyguardCharacterId;
		foreach (var ai in npc.NpcsArtificialIntelligences)
		{
			_AIs.Add(Gameworld.AIs.Get(ai.ArtificialIntelligenceId));
		}
	}

	public override object DatabaseInsert()
	{
		var co = base.DatabaseInsert();
		var dbitem = new Models.Npc();
		FMDB.Context.Npcs.Add(dbitem);
		dbitem.Character = (MudSharp.Models.Character)co;
		dbitem.TemplateId = Template.Id;
		dbitem.TemplateRevnum = Template.RevisionNumber;
		foreach (var item in _AIs)
		{
			dbitem.NpcsArtificialIntelligences.Add(new Models.NpcsArtificialIntelligences
				{ Npc = dbitem, ArtificialIntelligenceId = item.Id });
		}

		return Tuple.Create(co, dbitem);
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		var item = (Tuple<object, Models.Npc>)dbitem;
		base.SetIDFromDatabase(item.Item1);
		_npcID = item.Item2.Id;
	}

	#region Overrides of Character

	/// <summary>Tells the object to perform whatever save action it needs to do</summary>
	public override void Save()
	{
		var dbnpc = FMDB.Context.Npcs.Find(_npcID);
		if (dbnpc != null)
		{
			dbnpc.BodyguardCharacterId = _bodyguardingCharacterId;
			if (AIChanged)
			{
				FMDB.Context.NpcsArtificialIntelligences.RemoveRange(dbnpc.NpcsArtificialIntelligences);
				foreach (var ai in _AIs)
				{
					dbnpc.NpcsArtificialIntelligences.Add(new NpcsArtificialIntelligences
					{
						Npc = dbnpc,
						ArtificialIntelligenceId = ai.Id
					});
				}
				_aiChanged = false;
			}
		}

		base.Save();
	}

	#endregion

	public long? BodyguardingCharacterID
	{
		get => _bodyguardingCharacterId;
		set
		{
			_bodyguardingCharacterId = value;
			Changed = true;
		}
	}

	public void AddAI(IArtificialIntelligence ai)
	{
		_AIs.Add(ai);
		AIChanged = true;
	}

	public void RemoveAI(IArtificialIntelligence ai)
	{
		_AIs.Remove(ai);
		AIChanged = true;
	}
}