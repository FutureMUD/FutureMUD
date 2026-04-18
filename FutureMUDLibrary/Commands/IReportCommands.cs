using MudSharp.Accounts;
using System.Collections.Generic;

namespace MudSharp.Commands
{
    public interface IReportCommands
    {
        IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any);
    }
}