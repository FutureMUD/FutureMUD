using System.Collections.Generic;
using MudSharp.Accounts;

namespace MudSharp.Commands {
    public interface IReportCommands {
        IEnumerable<string> ReportCommands(PermissionLevel authority = PermissionLevel.Any);
    }
}