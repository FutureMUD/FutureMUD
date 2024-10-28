using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Help {
	public interface IHelpfile : IHelpInformation, IFrameworkItem, ISaveable {
		IEditableHelpfile GetEditableHelpfile { get; }
		void Delete();
		void DeleteExtraText(int index);
		void ReorderExtraText(int oldIndex, int newIndex);
	}
}