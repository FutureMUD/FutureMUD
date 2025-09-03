using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Health;

public interface IBloodtype : IEditableItem
{
    IEnumerable<IBloodtypeAntigen> Antigens { get; }

    bool IsCompatibleWithDonorBlood(IBloodtype donorBloodtype);
}

