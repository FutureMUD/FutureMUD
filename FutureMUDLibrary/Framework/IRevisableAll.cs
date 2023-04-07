using System;

namespace MudSharp.Framework
{
    public interface IRevisableAll<T> : IAll<T> where T : class, IRevisableItem {
        T Get(long id, int revision);
        bool Has(long id, int revision);
    }
}