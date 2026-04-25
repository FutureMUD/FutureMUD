using MudSharp.Character;
using MudSharp.Economy.Property;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Economy;

public interface IHotelTax : IEditableItem, ISaveable
{
	string MerchantDescription { get; }
	bool Applies(IProperty property, ICharacter patron);
	decimal TaxValue(IProperty property, ICharacter patron, decimal rentalCharge);
	void Delete();
}
