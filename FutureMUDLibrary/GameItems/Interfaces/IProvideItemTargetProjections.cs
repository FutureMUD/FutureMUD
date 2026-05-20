using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IProvideItemTargetProjections : IGameItemComponent
{
	IEnumerable<IGameItem> TargetProjections { get; }
}
