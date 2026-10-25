using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MudSharp.Framework
{
    public interface IFrameworkItem
    {
        string Name { get; }

        long Id { get; }

        string FrameworkItemType { get; }

        string NameAndId(IPerceiver voyeur, bool colour = true) => colour ? $"{Name.ColourName()} (#{Id.ToStringN0(voyeur)})" : $"{Name} (#{Id.ToStringN0(voyeur)})";
    }
}