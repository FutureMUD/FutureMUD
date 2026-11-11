#nullable enable
using MudSharp.Database;
using MudSharp.Economy.Property;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	internal static bool PayRecoveryRent(IProperty property, decimal rent, Action updateLease)
	{
		if (rent <= 0 || !HasPortfolio(property.Gameworld)) return false;
		using var handle = new FMDB();
		var security = $"Property:{property.Id}";
		var loan = FMDB.Context.LendingContracts.SingleOrDefault(x => x.SecurityKey == security && x.Status == "Recovery");
		if (loan == null) return false;
		return InTransaction(property.Gameworld, () =>
		{
			RecordSettlement(property.Gameworld, loan.Id, "Recovery rent");
			updateLease();
			if (!CreditRecoveryRent(property, rent)) throw new InvalidOperationException("Recovery rent authority changed.");
			return true;
		});
	}

	internal static bool CreditRecoveryRent(IProperty property, decimal rent)
	{
		var zone = property.EconomicZone;
		if (rent <= 0 || !HasPortfolio(zone.Gameworld)) return false;
		using (new FMDB())
		{
			var security = $"Property:{property.Id}";
			var loan = FMDB.Context.LendingContracts.SingleOrDefault(x => x.SecurityKey == security && x.Status == "Recovery");
			if (loan is null) return false;
			var lender = Lender(zone.Gameworld, loan);
			var terms = Terms(loan);
			var now = zone.FinancialPeriodReferenceCalendar.CurrentDateTime;
			var debt = loan.Principal + loan.Interest;
			var paid = Math.Min(debt, rent);
			if (paid > 0)
			{
				var allocation = ApplyPayment(loan, terms, paid);
				Credit(lender, paid, $"Rent on repossessed {property.Name}");
				Post(loan, "RecoveryPayment", allocation.Principal, allocation.Interest, paid, $"Rent from {property.Name}", now);
			}
			if (paid == debt)
			{
				var house = zone.Gameworld.AuctionHouses.Get(terms.AuctionHouseId);
				var lot = house?.ActiveAuctionItems.FirstOrDefault(x => x.RecoveryContractId == loan.Id);
				if (lot != null) house!.CancelItem(lot);
				var borrower = zone.Gameworld.TryGetCharacter(loan.BorrowerId, true);
				using (AuthorizeSecurityTransfer(property.Id)) property.TransferProperty(borrower);
				loan.SecurityKey = null;
				loan.Status = "Closed";
				loan.Surplus += rent - paid;
				Post(loan, "Release", 0, 0, rent - paid, "Rent cleared the debt. Title returned to borrower.", now);
			}
			Store(loan, terms);
			FMDB.Context.SaveChanges();
			return true;
		}
	}
}
