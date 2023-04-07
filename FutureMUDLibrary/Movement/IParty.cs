using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Construction.Boundary;

namespace MudSharp.Movement {
    public interface IParty : IMove {
        ICharacter Leader { get; }
        IEnumerable<IMove> Members { get; }
        IEnumerable<ICharacter> CharacterMembers { get; }

        /// <summary>
        /// Active Members are people who are present with the leader, and able and willing to move
        /// </summary>
        IEnumerable<IMove> ActiveMembers { get; }

        /// <summary>
        /// Active Members are people who are present with the leader, and able and willing to move
        /// </summary>
        IEnumerable<ICharacter> ActiveCharacterMembers { get; }

        IMoveSpeed SlowestSpeed(ICellExit exit);

        void Join(IMove body);
        bool Leave(IMove body);
        void SetLeader(ICharacter leader);

        void Disband();

        string DisplayMembers(IPerceiver voyeur, int indent = 0);
    }
}