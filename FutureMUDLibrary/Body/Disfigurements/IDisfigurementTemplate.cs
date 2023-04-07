using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;

namespace MudSharp.Body.Disfigurements
{
    public interface IDisfigurementTemplate : IEditableRevisableItem
    {
        bool CanSelectInChargen { get; }
        string ShortDescription { get; }
        string FullDescription { get; }
        string ShortDescriptionForChargen { get; }
        string FullDescriptionForChargen { get; }
        IEnumerable<IBodypartShape> BodypartShapes { get; }
        bool CanBeAppliedToBodypart(IBody body, IBodypart part);
        Counter<IChargenResource> ChargenCosts { get; }
        bool AppearInChargenList(IChargen chargen);
        IDisfigurementTemplate Clone(IAccount originator, string newName);
    }
}
