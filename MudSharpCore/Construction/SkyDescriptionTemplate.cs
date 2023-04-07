using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Construction;

public class SkyDescriptionTemplate : FrameworkItem, ISkyDescriptionTemplate
{
	public SkyDescriptionTemplate(MudSharp.Models.SkyDescriptionTemplate dbitem)
	{
		_id = dbitem.Id;
		_name = dbitem.Name;
		SkyDescriptions = new RankedRange<string>();
		foreach (var item in dbitem.SkyDescriptionTemplatesValues.OrderBy(x => x.LowerBound))
		{
			SkyDescriptions.Add(item.Description, item.LowerBound, item.UpperBound);
		}
	}

	#region ISkyDescriptionTemplate Members

	public RankedRange<string> SkyDescriptions { get; set; }

	#endregion

	public override string FrameworkItemType => "SkyDescriptionTemplate";
}