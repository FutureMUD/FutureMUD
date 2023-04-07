using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;

namespace MudSharp.GameItems.Interfaces {
    public interface IWriteable : IGameItemComponent {
        bool HasSpareRoom { get; }

        /// <summary>
        /// Determines whether the character can write on this writeable with the given implement and text
        /// </summary>
        /// <param name="character">The character doing the writing</param>
        /// <param name="implement">The writing implement</param>
        /// <param name="writing">The proposed writing to put on the writable. Can be null if this is a check prior to having dropped into the editor</param>
        /// <returns>True if the character can write on the proposed writeable in the proposed way</returns>
        bool CanWrite(ICharacter character, IWritingImplement implement, IWriting writing);
        string WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing);
        bool Write(ICharacter character, IWritingImplement implement, IWriting writing);
        string Title { get; }
        string WhyCannotGiveTitle(ICharacter character, string title);
        bool CanGiveTitle(ICharacter character, string title);
        bool GiveTitle(ICharacter character, string title);

        bool CanAddWriting(IWriting writing);
        bool AddWriting(IWriting newWriting);

        bool CanAddDrawing(IDrawing drawing);
        bool AddDrawing(IDrawing drawing);

        /// <summary>
        /// Determines whether the character can draw on this writeable with the given implement and text
        /// </summary>
        /// <param name="character">The character doing the drawing</param>
        /// <param name="implement">The writing implement</param>
        /// <param name="drawing">The proposed drawing to put on the writeable. Can be null if this is a check prior to having dropped into the editor.</param>
        /// <returns>True if the character can draw on the proposed writeable in the proposed way</returns>
        bool CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing);
        string WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing);
        bool Draw(ICharacter character, IWritingImplement implement, IDrawing drawing);
    }
}
