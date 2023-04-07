using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Framework.Save
{
    /// <summary>
    /// An object implementing this database has some data that is lazy loaded but that is desirable to load ahead of the first time that it is actually required. It can register itself with the Gameworld's SaveManager and be loaded during server idle time. A good example of this is corpses that need to load a character.
    /// </summary>
    public interface ILazyLoadDuringIdleTime : ISaveable
    {
        void DoLoad();
        public int LazyLoadPriority => 0;
    }
}
