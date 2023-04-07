using System;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.RPG.Knowledge;

public class CharacterKnowledge : SaveableItem, ICharacterKnowledge
{
	private ICharacter _character;

	private string _howAcquired;

	private IKnowledge _knowledge;

	private int _timesTaught;

	private DateTime _whenAcquired;

	public CharacterKnowledge(ICharacter character, IKnowledge knowledge, string howacquired)
	{
		Gameworld = character.Gameworld;
		Character = character;
		Knowledge = knowledge;
		HowAcquired = howacquired;
		WhenAcquired = DateTime.UtcNow;
		TimesTaught = 0;
	}

	public CharacterKnowledge(MudSharp.Models.CharacterKnowledge knowledge, ICharacter character)
	{
		_id = knowledge.Id;
		Character = character;
		Knowledge = character.Gameworld.Knowledges.Get(knowledge.KnowledgeId);
		WhenAcquired = knowledge.WhenAcquired;
		TimesTaught = knowledge.TimesTaught;
		HowAcquired = knowledge.HowAcquired;
	}

	public ICharacter Character
	{
		get => _character;
		set
		{
			_character = value;
			Changed = true;
		}
	}

	public IKnowledge Knowledge
	{
		get => _knowledge;
		set
		{
			_knowledge = value;
			Changed = true;
		}
	}

	public DateTime WhenAcquired
	{
		get => _whenAcquired;
		set
		{
			_whenAcquired = value;
			Changed = true;
		}
	}

	public string HowAcquired
	{
		get => _howAcquired;
		set
		{
			_howAcquired = value;
			Changed = true;
		}
	}

	public int TimesTaught
	{
		get => _timesTaught;
		set
		{
			_timesTaught = value;
			Changed = true;
		}
	}

	public override string FrameworkItemType => "CharacterKnowledge";

	public void SetId(long id)
	{
		_id = id;
	}

	public override void Save()
	{
		Changed = false;
		using (new FMDB())
		{
			if (Id == 0)
			{
				var dbnew = new Models.CharacterKnowledge
				{
					CharacterId = Character.Id,
					KnowledgeId = Knowledge.Id,
					HowAcquired = HowAcquired,
					TimesTaught = TimesTaught
				};
				FMDB.Context.CharacterKnowledges.Add(dbnew);
				FMDB.Context.SaveChanges();
				_id = dbnew.Id;
				return;
			}

			var dbitem = FMDB.Context.CharacterKnowledges.Find(Id);
			dbitem.HowAcquired = HowAcquired;
			dbitem.WhenAcquired = WhenAcquired;
			dbitem.TimesTaught = TimesTaught;
			FMDB.Context.SaveChanges();
		}
	}
}