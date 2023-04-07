using MudSharp.Framework;

namespace MudSharp.GameItems.Decorators;

public class SuffixStackDecorator : StackDecorator
{
	public SuffixStackDecorator(MudSharp.Models.StackDecorator proto)
	{
		_id = proto.Id;
		_name = proto.Name;
		Description = proto.Description;
	}

	public override string FrameworkItemType => "SuffixStackDecorator";

	#region IStackDecorator Members

	public override string Describe(string name, string description, double quantity)
	{
		var iquantity = (int)quantity;
		return $"{description.A_An()}{(iquantity == 1 ? "" : " (x" + iquantity + ")")}";
	}

	#endregion
}