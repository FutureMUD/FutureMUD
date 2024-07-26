using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MudSharp.Framework {
	public interface IFrameworkItem {
        string Name { get; }

        long Id { get; }

        string FrameworkItemType { get; }
    }
}