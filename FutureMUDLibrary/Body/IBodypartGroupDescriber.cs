using System.Collections.Generic;
using MudSharp.Body.Grouping;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Body {
    /// <summary>
    ///     An IBodypartGroupDescriber is used to
    /// </summary>
    public interface IBodypartGroupDescriber : IFrameworkItem {
        string Comment { get; }
        string DescribedAs { get; }
        BodypartGroupResult Match(IEnumerable<IBodypart> parts);
        void FinaliseLoad(Models.BodypartGroupDescriber describer, IFuturemud gameworld);
    }
}