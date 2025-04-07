using MudSharp.Body;
using MudSharp.Character;
using MudSharp.FutureProg;

namespace MudSharp.GameItems.Interfaces {
	public interface IWieldable : IGameItemComponent {
		IWield PrimaryWieldedLocation { get; set; }
		bool AlwaysRequiresTwoHandsToWield { get; }
		bool CanWield(ICharacter actor);
		string WhyCannotWield(ICharacter actor);
	}
}