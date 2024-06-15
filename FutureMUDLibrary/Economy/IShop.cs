using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Economy.Currency;

namespace MudSharp.Economy;

public interface IShop : IFrameworkItem, ISaveable, IFutureProgVariable
{
	decimal CashBalance { get; set; }
	IEconomicZone EconomicZone { get; set; }
	ICurrency Currency { get; set; }
	decimal MinimumFloatToBuyItems { get; }
	bool AutoPayTaxes { get; }

	IEnumerable<ITransactionRecord> TransactionRecords { get; }
	IEnumerable<IEmployeeRecord> EmployeeRecords { get; }
	bool IsEmployee(ICharacter actor);
	bool IsManager(ICharacter actor);
	bool IsProprietor(ICharacter actor);
	bool IsClockedIn(ICharacter actor);
	IEnumerable<ICharacter> EmployeesOnDuty { get; }
	IEnumerable<IMerchandise> Merchandises { get; }
	[CanBeNull] IBankAccount BankAccount { get; set; }
	void CheckFloat();
	IEnumerable<ICell> CurrentLocations { get; }
	IEnumerable<ILineOfCreditAccount> LineOfCreditAccounts { get; }
	void AddLineOfCredit(ILineOfCreditAccount account);
	void RemoveLineOfCredit(ILineOfCreditAccount account);

	bool IsTrading { get; }
	void ToggleIsTrading();
	[CanBeNull] IMarket MarketForPricingPurposes { get; }

	IReadOnlyDictionary<ICurrencyPile, Dictionary<ICoin, int>> GetCurrencyForShop(decimal amount);
	IEnumerable<ICurrencyPile> GetCurrencyPilesForShop();
	decimal AvailableCashFromAllSources();
	void TakeCashFromAllSources(decimal amount, string paymentReference);
	void AddCurrencyToShop(IGameItem currencyPile);
	bool IsReadyToDoBusiness { get; }

	#region AI Related Properties
	/// <summary>
	/// Prog Returns True, takes Character, Number, Tags as a parameter
	/// </summary>
	IFutureProg CanShopProg { get; set; }

	/// <summary>
	/// Prog Returns Text, takes Character, Number, Tags as a parameter
	/// </summary>
	IFutureProg WhyCannotShopProg { get; set; }
	#endregion

	void AddEmployee(ICharacter actor);
	void RemoveEmployee(IEmployeeRecord employee);
	void RemoveEmployee(ICharacter actor);
	void ClearEmployees();
	void EmployeeClockIn(ICharacter actor);
	void EmployeeClockOut(ICharacter actor);
	void SetManager(ICharacter actor, bool isManager);
	void SetProprietor(ICharacter actor, bool isProprietor);
	void AddMerchandise(IMerchandise merchandise);
	void RemoveMerchandise(IMerchandise merchandise);
	(int OnFloorCount, int InStockroomCount) StocktakeMerchandise(IMerchandise whichMerchandise);
	Dictionary<IMerchandise, (int OnFloorCount, int InStockroomCount)> StocktakeAllMerchandise();
	void AddTransaction(ITransactionRecord record);

	void AddToStock(ICharacter actor, IGameItem item, IMerchandise merch);
	void DisposeFromStock(ICharacter actor, IGameItem item);
	IEnumerable<IMerchandise> StockedMerchandise { get; }
	IEnumerable<IGameItem> AllStockedItems { get; }
	(bool Truth, string Reason) CanBuy(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null);
	IEnumerable<IGameItem> Buy(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null);
	(decimal Price, IEnumerable<IGameItem> Items) PreviewBuy(ICharacter actor, IMerchandise merchandise, int quantity, IPaymentMethod method, string extraArguments = null);
	decimal PriceForMerchandise(ICharacter actor, IMerchandise merchandise, int quantity);
	(decimal TotalPrice, decimal IncludedTax, bool VolumeDealsExist) GetDetailedPriceInfo(ICharacter actor, IMerchandise merchandise);
	(bool Truth, string Reason) CanSell(ICharacter actor, IMerchandise merchandise, IPaymentMethod method, IGameItem item);
	void Sell(ICharacter actor, IMerchandise merchandise, IPaymentMethod method, IGameItem item);
	void SortItemToStorePhysicalLocation(IGameItem item, IMerchandise merchandise, IGameItem container);

	void PriceAdjustmentForMerchandise(IMerchandise merchandise, decimal oldValue, ICharacter actor);
	IEnumerable<IGameItem> DoAutoRestockForMerchandise(IMerchandise merchandise, List<(IGameItem Item, IGameItem Container)> purchasedItems = null);
	IEnumerable<IGameItem> DoAutostockForMerchandise(IMerchandise merchandise);
	IEnumerable<IGameItem> DoAutostockAllMerchandise();
	bool IsWelcomeCustomer(ICharacter customer);

	void ShowDeals(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null);
	void ShowList(ICharacter actor, ICharacter purchaser, IMerchandise merchandise = null);
	void ShowInfo(ICharacter actor);

	bool BuildingCommand(ICharacter actor, StringStack command);
	void Delete();

	void PostLoadInitialisation();
}