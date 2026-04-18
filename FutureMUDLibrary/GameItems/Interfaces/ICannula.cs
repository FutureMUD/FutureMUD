using MudSharp.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface ICannula : IConnectable, IImplant
    {
        void SetBodypart(IBodypart proto);
    }
}
