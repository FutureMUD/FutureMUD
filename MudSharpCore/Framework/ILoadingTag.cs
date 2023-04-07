namespace MudSharp.Framework;

public interface ILoadingTag : IEditableTag
{
	void FinaliseLoad(MudSharp.Models.Tag tag);
}