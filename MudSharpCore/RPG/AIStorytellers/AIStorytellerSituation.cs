using System;
using System.Collections.Generic;
using System.Text;
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
	}

	public AIStorytellerSituation(IFuturemud gameworld, IAIStoryteller storyteller, string title, string situationText)
	{
		Gameworld = gameworld;
		AIStoryteller = storyteller;
		_name = title;
		SituationText = situationText;
		CreatedOn = DateTime.UtcNow;
		IsResolved = false;
		using (new FMDB())
		{
			var dbitem = new Models.AIStorytellerSituation
			{
				AIStorytellerId = storyteller.Id,
				Name = title,
				SituationText = situationText,
				CreatedOn = CreatedOn,
				IsResolved = IsResolved
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

	public override void Save()
	{
		var dbitem = FMDB.Context.AIStorytellerSituations.Find(Id);
		dbitem.IsResolved = IsResolved;
		dbitem.SituationText = SituationText;
		dbitem.Name = Name;
		Changed = false;
	}

	public override string FrameworkItemType => "AIStorytellerSituation";
}
