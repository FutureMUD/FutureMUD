using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IToolItem : IGameItemComponent
    {
        bool CountAsTool(ITag toolTag);
        bool CanUseTool(ITag toolTag, TimeSpan baseUsage);
        double ToolTimeMultiplier(ITag toolTag);
        void UseTool(ITag toolTag, TimeSpan usage);
    }
}
