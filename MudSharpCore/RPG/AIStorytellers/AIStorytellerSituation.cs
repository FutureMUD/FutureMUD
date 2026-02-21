using System;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerSituation : SaveableItem, IAIStorytellerSituation
{
	public AIStorytellerSituation(Models.AIStorytellerSituation dbitem, IAIStoryteller storyteller)
	{
		Gameworld = storyteller.Gameworld;
		_id = dbitem.Id;
		AIStoryteller = storyteller;
		_name = dbitem.Name;
		SituationText = dbitem.SituationText;
		CreatedOn = dbitem.CreatedOn;
		IsResolved = dbitem.IsResolved;
		ScopeCharacterId = dbitem.ScopeCharacterId;
		ScopeRoomId = dbitem.ScopeRoomId;
		if (ScopeCharacterId is not null && ScopeRoomId is not null)
		{
			ScopeRoomId = null;
		}
	}

	public AIStorytellerSituation(IFuturemud gameworld, IAIStoryteller storyteller, string title, string situationText,
		long? scopeCharacterId = null, long? scopeRoomId = null)
	{
		ValidateScope(scopeCharacterId, scopeRoomId);
		Gameworld = gameworld;
		AIStoryteller = storyteller;
		_name = title;
		SituationText = situationText;
		CreatedOn = DateTime.UtcNow;
		IsResolved = false;
		ScopeCharacterId = scopeCharacterId;
		ScopeRoomId = scopeRoomId;
		using (new FMDB())
		{
			var dbitem = new Models.AIStorytellerSituation
			{
				AIStorytellerId = storyteller.Id,
				Name = title,
				SituationText = situationText,
				CreatedOn = CreatedOn,
				IsResolved = IsResolved,
				ScopeCharacterId = ScopeCharacterId,
				ScopeRoomId = ScopeRoomId
			};
			FMDB.Context.AIStorytellerSituations.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IAIStoryteller AIStoryteller { get; private set; }
	public string SituationText { get; private set; }
	public DateTime CreatedOn { get; }
	public bool IsResolved { get; private set; }
	public long? ScopeCharacterId { get; private set; }
	public long? ScopeRoomId { get; private set; }

	public void Resolve()
	{
		IsResolved = true;
		Changed = true;
	}

	public void UpdateSituation(string newTitle, string newSituationText)
	{
		_name = newTitle;
		SituationText = newSituationText;
		Changed = true;
	}

	public void SetScope(long? scopeCharacterId, long? scopeRoomId)
	{
		ValidateScope(scopeCharacterId, scopeRoomId);
		if (ScopeCharacterId == scopeCharacterId && ScopeRoomId == scopeRoomId)
		{
			return;
		}

		ScopeCharacterId = scopeCharacterId;
		ScopeRoomId = scopeRoomId;
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AIStorytellerSituations.Find(Id);
		dbitem.IsResolved = IsResolved;
		dbitem.SituationText = SituationText;
		dbitem.Name = Name;
		dbitem.ScopeCharacterId = ScopeCharacterId;
		dbitem.ScopeRoomId = ScopeRoomId;
		Changed = false;
	}

	private static void ValidateScope(long? scopeCharacterId, long? scopeRoomId)
	{
		if (scopeCharacterId is not null && scopeRoomId is not null)
		{
			throw new ArgumentException("Situations may only be scoped to either a character or a room.");
		}
	}

	public override string FrameworkItemType => "AIStorytellerSituation";
}
