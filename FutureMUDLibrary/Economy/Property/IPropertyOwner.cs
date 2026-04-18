using MudSharp.Framework;

namespace MudSharp.Economy.Property
{
    public interface IPropertyOwner
    {
        long OwnerId { get; }
        string OwnerFrameworkItemType { get; }
        IFrameworkItem Owner { get; }
        decimal ShareOfOwnership { get; set; }
        IBankAccount RevenueAccount { get; set; }
        string Describe(IPerceiver voyeur);
        void Delete();
    }
}
