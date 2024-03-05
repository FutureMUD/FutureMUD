using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy
{
    public interface IPaymentMethod
    {
        decimal AccessibleMoneyForPayment();
        ICurrency Currency { get; }
        void TakePayment(decimal price);
        decimal AccessibleMoneyForCredit();
        void GivePayment(decimal price);
    }
}
