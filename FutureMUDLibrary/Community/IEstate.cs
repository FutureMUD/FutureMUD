using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;

namespace MudSharp.Community
{
	public enum EstateStatus
	{
		EstateWill,
		Cancelled,
		Undiscovered,
		ClaimPhase,
		Finalised
	}

	public enum ClaimStatus
	{
		NotAssessed,
		Rejected,
		Approved
	}

	public interface IEstateClaim : IFrameworkItem, ISaveable
	{
		IEstate Estate { get; }
		IFrameworkItem Claimant { get; }
		[CanBeNull] IFrameworkItem TargetItem { get; }
		decimal Amount { get; set; }
		string Reason { get; set; }
		ClaimStatus Status { get; set; }
		[CanBeNull] string StatusReason { get; set; }
		bool IsSecured { get; }
		MudDateTime ClaimDate { get; }
	}

	public interface IEstateAsset : IFrameworkItem, ISaveable
	{
		IEstate Estate { get; }
		IFrameworkItem Asset { get; }
		bool IsPresumedOwnership { get; }
		bool IsTransferred { get; set; }
		bool IsLiquidated { get; set; }
		decimal? LiquidatedValue { get; set; }
	}

	public interface IEstate : IFrameworkItem, ISaveable
	{
		IEconomicZone EconomicZone { get; }
		ICharacter Character { get; }
		[CanBeNull] IFrameworkItem Inheritor { get; }
		EstateStatus EstateStatus { get; set; }
		MudDateTime EstateStartTime { get; }
		[CanBeNull] MudDateTime FinalisationDate { get; set; }
		IEnumerable<IEstateClaim> Claims { get; }
		IEnumerable<IEstateAsset> Assets { get; }
		void AddClaim(IEstateClaim claim);
		void RemoveClaim(IEstateClaim claim);
		void UpdateClaim(IEstateClaim claim);
		void AddAsset(IEstateAsset asset);
		void RemoveAsset(IEstateAsset asset);
		bool CheckStatus();
		void Finalise();
	}
}
