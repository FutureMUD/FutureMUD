using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Work.Butchering
{
	public interface IButcheryProductItem : IFrameworkItem, ISaveable
	{
		/// <summary>
		/// The prototype to load in normal cases for this item
		/// </summary>
		IGameItemProto NormalProto { get; set; }

		/// <summary>
		/// The prototype to load in cases where the damage threshold is exceeded. If null, load nothing.
		/// </summary>
		IGameItemProto DamagedProto { get; set; }

		/// <summary>
		/// The quantity of items to load in the normal case
		/// </summary>
		int NormalQuantity { get; set; }

		/// <summary>
		/// The quantity of items to load in the damaged case
		/// </summary>
		int DamagedQuantity { get; set; }

		/// <summary>
		/// The threshold in terms of damage percentage to parts beyond which this item is considered "damaged"
		/// </summary>
		double DamagedThreshold { get; set; }

		void Delete();
	}
}
