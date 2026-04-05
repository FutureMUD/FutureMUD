using MudSharp.Economy.Currency;
using MudSharp.GameItems.Components;
using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces
{
    public interface IVendingMachine : ISelectable, IListable
    {
        IList<VendingMachineSelection> Selections { get; }
        decimal InternalBalance { get; set; }
        decimal CurrentBalance { get; set; }
        ICurrency Currency { get; }
    }
}