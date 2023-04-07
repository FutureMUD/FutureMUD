using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces
{
    public interface IQualityByTag : IGameItemComponent {
        ItemQuality QualityForTag(ITag tag);
    }
}
