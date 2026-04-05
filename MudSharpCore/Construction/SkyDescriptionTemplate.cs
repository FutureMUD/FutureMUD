using MudSharp.Framework;
using MudSharp.Models;
using System.Linq;

namespace MudSharp.Construction;

public class SkyDescriptionTemplate : FrameworkItem, ISkyDescriptionTemplate
{
    public SkyDescriptionTemplate(MudSharp.Models.SkyDescriptionTemplate dbitem)
    {
        _id = dbitem.Id;
        _name = dbitem.Name;
        SkyDescriptions = new RankedRange<string>();
        foreach (SkyDescriptionTemplatesValue item in dbitem.SkyDescriptionTemplatesValues.OrderBy(x => x.LowerBound))
        {
            SkyDescriptions.Add(item.Description, item.LowerBound, item.UpperBound);
        }
    }

    #region ISkyDescriptionTemplate Members

    public RankedRange<string> SkyDescriptions { get; set; }

    #endregion

    public override string FrameworkItemType => "SkyDescriptionTemplate";
}