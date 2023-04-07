using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Estates;

public class Estate : SaveableItem, IEstate, ILazyLoadDuringIdleTime
{
	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "Estate";

	#endregion

	#region Overrides of SaveableItem

	public override void Save()
	{
		throw new NotImplementedException();
	}

	public void DoLoad()
	{
		LoadCharacter();
	}

	#endregion

	#region Implementation of IEstate

	private ICharacter LoadCharacter()
	{
		return _character ??= Gameworld.TryGetCharacter(_characterId, true);
	}

	public IEconomicZone EconomicZone { get; private set; }
	private long _characterId;
	private ICharacter _character;
	public ICharacter Character => LoadCharacter();
	public EstateStatus EstateStatus { get; set; }
	public MudDateTime EstateStartTime { get; set; }
	public MudDateTime FinalisationDate { get; set; }

	private readonly List<EstateClaim> _claims = new();
	public IEnumerable<EstateClaim> Claims => _claims;

	public void AddClaim(EstateClaim claim)
	{
		_claims.Add(claim);
		Changed = true;
	}

	public void RemoveClaim(EstateClaim claim)
	{
		_claims.Remove(claim);
		Changed = true;
	}

	public void UpdateClaim(EstateClaim claim)
	{
		var index = _claims.FindIndex(x => x.Claimant == claim.Claimant);
		if (index == -1)
		{
			AddClaim(claim);
			return;
		}

		_claims[index] = claim;
		Changed = true;
	}

	public bool CheckStatus()
	{
		if (EstateStatus == EstateStatus.ClaimPhase)
		{
			if (EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >= FinalisationDate)
			{
				Finalise();
			}

			return false;
		}

		if (EstateStatus == EstateStatus.Undiscovered)
		{
			if (EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >=
			    EstateStartTime + EconomicZone.EstateDefaultDiscoverTime)
			{
				EstateStatus = EstateStatus.ClaimPhase;
				FinalisationDate = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime +
				                   EconomicZone.EstateClaimPeriodLength;
				Changed = true;
			}

			return false;
		}

		return false;
	}

	public void Finalise()
	{
		// Bank Accounts

		// Properties

		// Pay out claims
		throw new NotImplementedException();
	}

	#endregion
}