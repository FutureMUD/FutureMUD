using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantRespondToCommands : IImplant
    {
        string AliasForCommands { get; set; }
        IEnumerable<string> Commands { get; }
        string CommandHelp { get; }
        void IssueCommand(string command, StringStack arguments);
    }
}
