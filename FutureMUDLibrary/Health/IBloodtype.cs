using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System.Collections.Generic;

namespace MudSharp.Health;

public interface IBloodtype : IEditableItem
{
    IEnumerable<IBloodtypeAntigen> Antigens { get; }

    bool IsCompatibleWithDonorBlood(IBloodtype donorBloodtype);
}

