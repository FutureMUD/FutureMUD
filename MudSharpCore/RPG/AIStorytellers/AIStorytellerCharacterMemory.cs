using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerCharacterMemory : SaveableItem, IAIStorytellerCharacterMemory
{
    public AIStorytellerCharacterMemory(Models.AIStorytellerCharacterMemory dbitem,
        IAIStoryteller aiStoryteller)
    {
        Gameworld = aiStoryteller.Gameworld;
        _id = dbitem.Id;
        AIStoryteller = aiStoryteller;
        Character = Gameworld.TryGetCharacter(dbitem.CharacterId, true);
        MemoryTitle = dbitem.MemoryTitle;
        MemoryText = dbitem.MemoryText;
        CreatedOn = dbitem.CreatedOn;
    }

    public AIStorytellerCharacterMemory(IAIStoryteller aiStoryteller, ICharacter character, string title,
        string text)
    {
        Gameworld = aiStoryteller.Gameworld;
        AIStoryteller = aiStoryteller;
        Character = character;
        MemoryTitle = title;
        MemoryText = text;
        CreatedOn = DateTime.UtcNow;
        using (new FMDB())
        {
            Models.AIStorytellerCharacterMemory dbitem = new()
            {
                AIStorytellerId = aiStoryteller.Id,
                CharacterId = character.Id,
                MemoryTitle = title,
                MemoryText = text,
                CreatedOn = CreatedOn
            };
            FMDB.Context.AIStorytellerCharacterMemories.Add(dbitem);
            FMDB.Context.SaveChanges();
            _id = dbitem.Id;
        }
    }
    public IAIStoryteller AIStoryteller { get; }
    public ICharacter Character { get; }
    public string MemoryTitle { get; private set; }
    public string MemoryText { get; private set; }
    public DateTime CreatedOn { get; }

    public void Forget()
    {
        Gameworld.SaveManager.Abort(this);
        if (_id != 0)
        {
            using (new FMDB())
            {
                Gameworld.SaveManager.Flush();
                Models.AIStorytellerCharacterMemory dbitem = FMDB.Context.AIStorytellerCharacterMemories.Find(Id);
                if (dbitem != null)
                {
                    FMDB.Context.AIStorytellerCharacterMemories.Remove(dbitem);
                    FMDB.Context.SaveChanges();
                }
            }
        }
    }

    public void UpdateMemory(string title, string text)
    {
        MemoryTitle = title;
        MemoryText = text;
        Changed = true;
    }

    public override void Save()
    {
        Models.AIStorytellerCharacterMemory dbitem = FMDB.Context.AIStorytellerCharacterMemories.Find(Id);
        dbitem.MemoryTitle = MemoryTitle;
        dbitem.MemoryText = MemoryText;
        Changed = false;
    }

    public override string FrameworkItemType => "AIStorytellerCharacterMemory";
}
