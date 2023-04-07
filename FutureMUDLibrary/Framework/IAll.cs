namespace MudSharp.Framework
{
    public interface IAll<in T> where T : class, IFrameworkItem {
        bool Add(T item);

        bool Has(long id);
        bool Has(T item);
        bool Has(string name);
        void Clear();
    }
}