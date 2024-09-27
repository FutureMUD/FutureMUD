using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Law
{
#nullable enable
	public interface ICrime : IFrameworkItem, ISaveable, ILazyLoadDuringIdleTime, IFutureProgVariable
	{
		ILaw Law { get; }
		ILegalAuthority LegalAuthority { get; }
		MudDateTime TimeOfCrime { get; }
		MudDateTime? TimeOfReport { get; set; }
		DateTime RealTimeOfCrime { get; }
		ICharacter Criminal { get; }
		long CriminalId { get; }
		long? AccuserId { get; set; }
		long? VictimId { get; }
		ICharacter? Victim { get; }
		long? ThirdPartyId { get; }
		string? ThirdPartyFrameworkItemType { get; }
		IEnumerable<long> WitnessIds { get; }
		void AddWitness(long witnessId);
		bool IsKnownCrime { get; set; }
		bool HasBeenEnforced { get; set; }
		bool BailPosted { get; set; }
		bool HasBeenConvicted { get; set; }
		bool HasBeenFinalised { get; set; }
		ICell? CrimeLocation { get; }
		string? CriminalShortDescription { get; }
		string? CriminalDescription { get; }
		decimal CalculatedBail { get; set; }
		decimal FineRecorded { get; set; }
		bool ExecutionPunishment { get; set; }
		bool FineHasBeenPaid { get; set; }
		bool SentenceHasBeenServed { get; set; }
		TimeSpan GoodBehaviourBond { get; set; }
		TimeSpan CustodialSentenceLength { get; set; }
		IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> CriminalCharacteristics { get; }
		void SetCharacteristicValue(ICharacteristicDefinition definition, ICharacteristicValue value);
		bool CriminalIdentityIsKnown { get; set; }
		string DescribeCrime(IPerceiver voyeur);
		string DescribeCrimeAtTrial(IPerceiver voyeur);
		string ShowCrimeInfo(ICharacter enforcer);
		void Forgive(ICharacter enforcer, string reason);
		void Convict(ICharacter? enforcer, PunishmentResult result, string reason);
		void Acquit(ICharacter enforcer, string reason);
		bool EligableForAutomaticConviction();
		Difficulty DefenseDifficulty { get; }
		Difficulty ProsecutionDifficulty { get; }
		string DescribePunishment(ICharacter voyeur);
	}
#nullable restore
}
