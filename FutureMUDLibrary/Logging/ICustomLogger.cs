
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Logging
{
    public enum LogEntryType {
        SkillUse,
        SkillBranch,
        HealingTick,
        SkillImprovement
    }

    public interface ICustomLogger {
        void HandleLog(LogEntryType type, params object[] data);
        void SaveLog();
    }
}
