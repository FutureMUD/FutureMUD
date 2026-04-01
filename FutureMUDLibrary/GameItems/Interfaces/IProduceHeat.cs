using MudSharp.Construction;

namespace MudSharp.GameItems.Interfaces
{
	public interface IProduceHeat : IGameItemComponent
	{
		double CurrentAmbientHeat { get; }
		double CurrentHeat(Proximity proximity);
	}
}
