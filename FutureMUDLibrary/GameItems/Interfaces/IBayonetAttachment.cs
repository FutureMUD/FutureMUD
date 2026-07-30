#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum BayonetAttachmentStyle
{
	Plug = 0,
	Socket = 1,
	Sword = 2
}

public interface IBayonetAttachment : IGameItemComponent
{
	BayonetAttachmentStyle Style { get; }
	double MinimumBore { get; }
	double MaximumBore { get; }
	bool BlocksFiring { get; }

	bool FitsBore(double bore)
	{
		return bore >= MinimumBore && bore <= MaximumBore;
	}
}
