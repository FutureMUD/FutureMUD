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

	public record EstateClaim
	{
		public ICharacter Claimant { get; init; }
		public decimal Amount { get; init; }
		public string Reason { get; init; }
		public ClaimStatus Status { get; init; }
		[CanBeNull] public string StatusReason { get; init; }
	}

	public interface IEstate : IFrameworkItem, ISaveable
	{
		IEconomicZone EconomicZone { get; }
		ICharacter Character { get; }
		EstateStatus EstateStatus { get; set; }
		[CanBeNull] MudDateTime FinalisationDate { get; set; }
		IEnumerable<EstateClaim> Claims { get; }
		void AddClaim(EstateClaim claim);
		void RemoveClaim(EstateClaim claim);
		void UpdateClaim(EstateClaim claim);
		bool CheckStatus();
		void Finalise();
	}
}
