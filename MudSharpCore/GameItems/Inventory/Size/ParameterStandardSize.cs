using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Size;

internal class ParameterStandardSize : FrameworkItem, IStandardSize
{
	public override string FrameworkItemType => "ParameterStandardSize";

	public int BodyID { get; protected set; }

	public IWearableSize TemplateSize { get; protected set; }
}