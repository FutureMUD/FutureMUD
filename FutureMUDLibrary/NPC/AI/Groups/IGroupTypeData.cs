using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.NPC.AI.Groups
{
    public enum GroupAction
    {
        FindWater,
        FindFood,
        Graze,
        Sleep,
        Rest,
        Alert,
        FindShelter,
        AvoidThreat,
        Flee,
        FleeToPreferredTerrain,
        FleeToDen,
        Posture,
        ControlledRetreat,
        AttackThreats,
        MoveFood,
        FindMate,
        Mate
    }

    public interface IGroupTypeData
    {
        XElement SaveToXml();
        string ShowText(ICharacter voyeur);
    }
}
