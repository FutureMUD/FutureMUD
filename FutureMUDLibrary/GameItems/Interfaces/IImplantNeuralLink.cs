using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplantNeuralLink : IImplant
    {
        void IssueCommand(string alias, string command, StringStack arguments);
        void AddLink(IImplant implant);
        void RemoveLink(IImplant implant);
        bool IsLinkedTo(IImplant implant);
        bool DNIConnected { get; }
        void DoReportStatus();
        bool PermitsAudio { get; }
        bool PermitsVisual { get; }
    }
}
