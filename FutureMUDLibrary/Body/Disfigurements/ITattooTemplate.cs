using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Form.Colour;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;

namespace MudSharp.Body.Disfigurements
{
    public interface ITattooTemplate : IDisfigurementTemplate {
        SizeCategory Size { get; }
        int TicksToCompleteTattoo { get; }
        bool CanSeeTattooInList(ICharacter character);
        bool CanProduceTattoo(ICharacter character);
        ITattoo ProduceTattoo(ICharacter tatooist, ICharacter target, IBodypart bodypart);
        IEnumerable<(IColour Colour, double Amount)> InkColours { get; }
        IInventoryPlan GetInkPlan(ICharacter tattooist);
        /// <summary>
        /// Whether or not this tattoo has a special form that overrides the default "heavily-tattooed" type description, e.g. "ritually-tattooed", "facially-tattooed" etc
        /// </summary>
        bool HasSpecialTattooCharacteristicOverride { get; }

        /// <summary>
        /// Returns a special overriding description, e.g. "ritually-tattooed", "with maori facial tattoos" etc
        /// </summary>
        /// <param name="withForm">If true, it's in the form "with ...", otherwise it's a participle e.g. "tattooed"</param>
        /// <returns>The description</returns>
        string SpecialTattooCharacteristicOverride(bool withForm);
    }
}
