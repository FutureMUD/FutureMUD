using MudSharp.Character.Name;
using MudSharp.Framework;

namespace MudSharp.Economy
{
    public interface IEmployeeRecord : IXmlSavable
    {
        long EmployeeCharacterId { get; set; }
        IPersonalName Name { get; set; }
        TimeAndDate.MudDateTime EmployeeSince { get; set; }
        bool ClockedIn { get; set; }
        bool IsManager { get; set; }
        bool IsProprietor { get; set; }
    }
}