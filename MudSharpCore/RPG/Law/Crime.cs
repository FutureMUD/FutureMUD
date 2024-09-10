using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;
using MudSharp.Accounts;

namespace MudSharp.RPG.Law;
#nullable enable
public class Crime : LateInitialisingItem, ICrime
{
	public override IFuturemud Gameworld
	{
		get => Law.Gameworld;
		protected set => base.Gameworld = value;
	}

	public Crime(ICharacter criminal, ICharacter? victim, IEnumerable<ICharacter> witnesses, ILaw law,
		IFrameworkItem? thirdparty = null)
	{
		Gameworld = criminal.Gameworld;
		CriminalId = criminal.Id;
		_criminal = criminal;
		VictimId = victim?.Id;
		TimeOfCrime = criminal.Location.DateTime();
		RealTimeOfCrime = DateTime.UtcNow;
		CrimeLocation = criminal.Location;
		_witnessIds.AddRange(witnesses.Select(x => x.Id));
		Law = law;
		ThirdPartyId = thirdparty?.Id;
		ThirdPartyFrameworkItemType = thirdparty?.FrameworkItemType;
		CriminalShortDescription = criminal.HowSeen(criminal,
			flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf);
		CriminalDescription = criminal.HowSeen(criminal, type: DescriptionType.Full,
			flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf);
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public Crime(MudSharp.Models.Crime dbitem, ILaw law, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Law = law;
		_id = dbitem.Id;
		IdInitialised = true;
		CriminalId = dbitem.CriminalId;
		VictimId = dbitem.VictimId;
		AccuserId = dbitem.AccuserId;
		RealTimeOfCrime = dbitem.RealTimeOfCrime;
		CrimeLocation = Gameworld.Cells.Get(dbitem.LocationId ?? 0);
		TimeOfCrime = new MudDateTime(dbitem.TimeOfCrime, Gameworld);
		TimeOfReport = string.IsNullOrEmpty(dbitem.TimeOfReport)
			? null
			: new MudDateTime(dbitem.TimeOfReport, Gameworld);
		HasBeenConvicted = dbitem.ConvictionRecorded;
		_isKnownCrime = dbitem.IsKnownCrime;
		_criminalIdentityIsKnown = dbitem.IsCriminalIdentityKnown;
		_bailPosted = dbitem.BailHasBeenPosted;
		_hasBeenEnforced = dbitem.HasBeenEnforced;
		CriminalShortDescription = dbitem.CriminalShortDescription;
		CriminalDescription = dbitem.CriminalFullDescription;
		_fineRecorded = dbitem.FineRecorded;
		_custodialSentenceLength = TimeSpan.FromSeconds(dbitem.CustodialSentenceLength);
		_calculatedBail = dbitem.CalculatedBail;
		HasBeenFinalised = dbitem.IsFinalised;
		if (dbitem.CriminalCharacteristics.Length > 0)
		{
			foreach (var item in dbitem.CriminalCharacteristics.Split('\n'))
			{
				var split = item.Split(' ');
				var definition = Gameworld.Characteristics.Get(long.Parse(split[0]));
				var value = Gameworld.CharacteristicValues.Get(long.Parse(split[1]));
				if (definition == null || value == null)
				{
					continue;
				}

				_criminalCharacteristics[definition] = value;
			}
		}

		if (!string.IsNullOrEmpty(dbitem.WitnessIds))
		{
			_witnessIds.AddRange(dbitem.WitnessIds.Split(' ').Select(x => long.Parse(x)));
		}

		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Crimes.Find(Id);
		dbitem.AccuserId = AccuserId;
		dbitem.TimeOfReport = TimeOfReport?.GetDateTimeString();
		dbitem.IsKnownCrime = _isKnownCrime;
		dbitem.IsCriminalIdentityKnown = CriminalIdentityIsKnown;
		dbitem.IsStaleCrime = Law.Authority.StaleCrimes.Contains(this);
		dbitem.IsFinalised = HasBeenFinalised;
		dbitem.ConvictionRecorded = HasBeenConvicted;
		dbitem.BailHasBeenPosted = _bailPosted;
		dbitem.HasBeenEnforced = _hasBeenEnforced;
		dbitem.LocationId = CrimeLocation?.Id;
		dbitem.CriminalShortDescription = CriminalShortDescription;
		dbitem.CriminalFullDescription = CriminalDescription;
		dbitem.CriminalCharacteristics = _criminalCharacteristics.Select(x => $"{x.Key.Id} {x.Value.Id}")
																 .ListToCommaSeparatedValues("\n");
		dbitem.WitnessIds = WitnessIds.Select(x => x.ToString()).ListToCommaSeparatedValues(" ");
		dbitem.CalculatedBail = CalculatedBail;
		dbitem.FineRecorded = FineRecorded;
		dbitem.CustodialSentenceLength = CustodialSentenceLength.TotalSeconds;
		Changed = false;
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Models.Crime
		{
			LawId = Law.Id,
			TimeOfCrime = TimeOfCrime.GetDateTimeString(),
			RealTimeOfCrime = RealTimeOfCrime,
			CriminalId = CriminalId,
			VictimId = VictimId,
			LocationId = CrimeLocation?.Id,
			AccuserId = AccuserId,
			ThirdPartyId = ThirdPartyId,
			ThirdPartyIItemType = ThirdPartyFrameworkItemType,
			TimeOfReport = TimeOfReport?.GetDateTimeString(),
			IsKnownCrime = _isKnownCrime,
			IsCriminalIdentityKnown = CriminalIdentityIsKnown,
			IsStaleCrime = Law.Authority.StaleCrimes.Contains(this),
			IsFinalised = HasBeenFinalised,
			ConvictionRecorded = HasBeenConvicted,
			BailHasBeenPosted = _bailPosted,
			HasBeenEnforced = _hasBeenEnforced,
			CriminalShortDescription = CriminalShortDescription,
			CriminalFullDescription = CriminalDescription,
			CriminalCharacteristics = _criminalCharacteristics.Select(x => $"{x.Key.Id} {x.Value.Id}")
															  .ListToCommaSeparatedValues("\n"),
			WitnessIds = WitnessIds.Select(x => x.ToString()).ListToCommaSeparatedValues(" "),
			CalculatedBail = CalculatedBail,
			FineRecorded = FineRecorded,
			CustodialSentenceLength = CustodialSentenceLength.TotalSeconds
		};
		FMDB.Context.Crimes.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Crime)dbitem).Id;
	}

	public sealed override string FrameworkItemType => "Crime";

	public ILaw Law { get; }
	public ILegalAuthority LegalAuthority => Law.Authority;
	public MudDateTime TimeOfCrime { get; }

	public MudDateTime? TimeOfReport
	{
		get => _timeOfReport;
		set
		{
			_timeOfReport = value;
			Changed = true;
		}
	}

	public DateTime RealTimeOfCrime { get; }

	public decimal CalculatedBail
	{
		get => _calculatedBail;
		set
		{
			_calculatedBail = value;
			Changed = true;
		}
	}

	public decimal FineRecorded
	{
		get => _fineRecorded;
		set
		{
			_fineRecorded = value;
			Changed = true;
		}
	}

	public TimeSpan CustodialSentenceLength
	{
		get => _custodialSentenceLength;
		set
		{
			_custodialSentenceLength = value;
			Changed = true;
		}
	}

	public long CriminalId { get; }
	private ICharacter _criminal;

	private void TryLoadCharacter()
	{
		if (_criminal is not null)
		{
			return;
		}

		_criminal = Gameworld.TryGetCharacter(CriminalId, true);
	}

	public ICharacter Criminal
	{
		get
		{
			TryLoadCharacter();
			return _criminal;
		}
	}

	public long? VictimId { get; }
	public ICharacter Victim => Gameworld.TryGetCharacter(VictimId ?? 0, true);

	public long? AccuserId
	{
		get => _accuserId;
		set
		{
			_accuserId = value;
			Changed = true;
		}
	}

	public long? ThirdPartyId { get; }
	public string? ThirdPartyFrameworkItemType { get; }

	private readonly List<long> _witnessIds = new();
	private bool _isKnownCrime;
	private bool _bailPosted;
	private bool _hasBeenEnforced;
	private bool _hasBeenConvicted;
	private bool _hasBeenFinalised;
	private bool _criminalIdentityIsKnown;
	private MudDateTime? _timeOfReport;
	private long? _accuserId;
	private decimal _calculatedBail;
	private decimal _fineRecorded;
	private TimeSpan _custodialSentenceLength;
	private ICell? _crimeLocation;

	public IEnumerable<long> WitnessIds => _witnessIds;

	public void AddWitness(long witnessId)
	{
		if (!_witnessIds.Contains(witnessId))
		{
			_witnessIds.Add(witnessId);
		}
	}

	public bool IsKnownCrime
	{
		get => _isKnownCrime;
		set
		{
			_isKnownCrime = value;
			Changed = true;
		}
	}

	public bool HasBeenEnforced
	{
		get => _hasBeenEnforced;
		set
		{
			_hasBeenEnforced = value;
			Changed = true;
		}
	}

	public bool BailPosted
	{
		get => _bailPosted;
		set
		{
			_bailPosted = value;
			Changed = true;
		}
	}

	public bool HasBeenConvicted
	{
		get => _hasBeenConvicted;
		set
		{
			_hasBeenConvicted = value;
			Changed = true;
		}
	}

	public bool HasBeenFinalised
	{
		get => _hasBeenFinalised;
		set
		{
			_hasBeenFinalised = value;
			Changed = true;
		}
	}

	public ICell? CrimeLocation
	{
		get => _crimeLocation; init
		{
			if (_crimeLocation is not null)
			{
				_crimeLocation.CellRequestsDeletion -= CrimeLocation_CellRequestsDeletion;
			}
			_crimeLocation = value;
			if (_crimeLocation is not null)
			{
				_crimeLocation.CellRequestsDeletion -= CrimeLocation_CellRequestsDeletion;
				_crimeLocation.CellRequestsDeletion += CrimeLocation_CellRequestsDeletion;
			}
		}
	}

	private void CrimeLocation_CellRequestsDeletion(object? sender, EventArgs e)
	{
		var cell = (ICell)sender;
		_crimeLocation = null;
		Changed = true;
		cell.CellRequestsDeletion -= CrimeLocation_CellRequestsDeletion;
	}

	public string? CriminalShortDescription { get; set; }
	public string? CriminalDescription { get; set; }
	private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> _criminalCharacteristics = new();

	public IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> CriminalCharacteristics =>
		_criminalCharacteristics;

	public void SetCharacteristicValue(ICharacteristicDefinition definition, ICharacteristicValue value)
	{
		_criminalCharacteristics[definition] = value;
	}

	public bool CriminalIdentityIsKnown
	{
		get => _criminalIdentityIsKnown;
		set
		{
			_criminalIdentityIsKnown = value;
			Changed = true;
		}
	}

	private string DescribeCrimeInternal(IPerceiver voyeur, string victimDesc, string locationAddendum, string thirdPartyDesc)
	{
		switch (Law.CrimeType)
		{
			case CrimeTypes.Assault:
				return $"assaulted {victimDesc}{locationAddendum}";
			case CrimeTypes.AssaultWithADeadlyWeapon:
				return $"Assaulted {victimDesc} with a deadly weapon{locationAddendum}";
			case CrimeTypes.Battery:
				return $"Battery against {victimDesc}{locationAddendum}";
			case CrimeTypes.AttemptedMurder:
				return $"Attempted murder of {victimDesc}{locationAddendum}";
			case CrimeTypes.Murder:
				return $"Murder of {victimDesc}{locationAddendum}";
			case CrimeTypes.Manslaughter:
				return $"Manslaughter of {victimDesc}{locationAddendum}";
			case CrimeTypes.Torture:
				return $"Torture of {victimDesc}{locationAddendum}";
			case CrimeTypes.GreviousBodilyHarm:
				return $"Grevious bodily harm of {victimDesc}{locationAddendum}";
			case CrimeTypes.Theft:
				return $"Theft of {thirdPartyDesc} from {victimDesc}{locationAddendum}";
			case CrimeTypes.Fraud:
				return $"Fraud against {victimDesc}{locationAddendum}";
			case CrimeTypes.Racketeering:
				return $"Racketeering against {victimDesc}{locationAddendum}";
			case CrimeTypes.CartelCollusion:
				return $"Cartel collusion against {victimDesc}{locationAddendum}";
			case CrimeTypes.PossessingStolenGoods:
				return $"Possessing stolen goods{locationAddendum}";
			case CrimeTypes.SellingContraband:
				return $"Selling {thirdPartyDesc} (contraband){locationAddendum}";
			case CrimeTypes.PossessingContraband:
				return $"Possessing {thirdPartyDesc} (contraband){locationAddendum}";
			case CrimeTypes.TrafficingContraband:
				return $"Trafficing {thirdPartyDesc} (contraband){locationAddendum}";
			case CrimeTypes.Vandalism:
				if (Victim is null)
				{
					return $"Vandalism{locationAddendum}";
				}

				return $"Vandalism of {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.BreakAndEnter:
				return $"Break and enter{locationAddendum}";
			case CrimeTypes.Loitering:
				return $"Loitering{locationAddendum}";
			case CrimeTypes.Littering:
				return $"Littering of {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.Libel:
				return $"Libel against {victimDesc}{locationAddendum}";
			case CrimeTypes.Slander:
				return $"Slander against {victimDesc}{locationAddendum}";
			case CrimeTypes.Treason:
				return $"Treason{locationAddendum}";
			case CrimeTypes.Conspiracy:
				return $"Conspiracy against {victimDesc}{locationAddendum}";
			case CrimeTypes.Intimidation:
				return $"Intimidation of {victimDesc}{locationAddendum}";
			case CrimeTypes.Blackmail:
				return $"Blackmail of {victimDesc}{locationAddendum}";
			case CrimeTypes.Trespassing:
				return $"Tresspassing{locationAddendum}";
			case CrimeTypes.ResistArrest:
				return $"Resisting Arrest{locationAddendum}";
			case CrimeTypes.DisobeyLegalInstruction:
				return $"Disobeying a legal instruction of {victimDesc}{locationAddendum}";
			case CrimeTypes.ViolateParole:
				return $"Violating parole{locationAddendum}";
			case CrimeTypes.EscapeCaptivity:
				if (Victim is not null)
				{
					return $"Escaping from the custody of {victimDesc}{locationAddendum}";
				}

				return $"Escaping captivity{locationAddendum}";
			case CrimeTypes.Indecency:
				return $"Indecency against {victimDesc}{locationAddendum}";
			case CrimeTypes.Immorality:
				return $"Immorality{locationAddendum}";
			case CrimeTypes.PublicIntoxication:
				return $"Public intoxication{locationAddendum}";
			case CrimeTypes.Blasphemy:
				return $"Blasphemy{locationAddendum}";
			case CrimeTypes.Apostacy:
				return $"Apostacy{locationAddendum}";
			case CrimeTypes.Profanity:
				return $"Profanity{locationAddendum}";
			case CrimeTypes.Vagrancy:
				return $"Vagrancy{locationAddendum}";
			case CrimeTypes.DestructionOfProperty:
				return $"Destruction of {thirdPartyDesc}, property of {victimDesc}{locationAddendum}";
			case CrimeTypes.UnauthorisedDealing:
				return $"Unauthorised dealing{locationAddendum}";
			case CrimeTypes.Embezzlement:
				return $"Embezzlement{locationAddendum}";
			case CrimeTypes.Sedition:
				return $"Sedition{locationAddendum}";
			case CrimeTypes.Mayhem:
				return $"Mayhem{locationAddendum}";
			case CrimeTypes.ContemptOfCourt:
				return $"Contempt of Court{locationAddendum}";
			case CrimeTypes.ViolateBail:
				return "Violating Bail";
			case CrimeTypes.Aiding:
				return $"Aiding{locationAddendum}";
			case CrimeTypes.Abetting:
				return $"Abetting{locationAddendum}";
			case CrimeTypes.Accessory:
				return $"Accessory{locationAddendum}";
			case CrimeTypes.Arson:
				return $"Arson{locationAddendum}";
			case CrimeTypes.Bribery:
				return $"Bribery of {victimDesc}{locationAddendum}";
			case CrimeTypes.Tyranny:
				return "Tyranny";
			case CrimeTypes.Harassment:
				return $"Harassment of {victimDesc}{locationAddendum}";
			case CrimeTypes.Extortion:
				return $"Extortion of {victimDesc}{locationAddendum}";
			case CrimeTypes.Rape:
				return $"Rape of {victimDesc}{locationAddendum}";
			case CrimeTypes.SexualAssault:
				return $"Sexual Assault of {victimDesc}{locationAddendum}";
			case CrimeTypes.Negligence:
				return $"Negligence against {victimDesc}{locationAddendum}";
			case CrimeTypes.Perjury:
				return "Perjury";
			case CrimeTypes.Desertion:
				return $"Desertion{locationAddendum}";
			case CrimeTypes.Mutiny:
				return $"Mutiny against {victimDesc}{locationAddendum}";
			case CrimeTypes.Rebellion:
				return $"Rebellion against {victimDesc}{locationAddendum}";
			case CrimeTypes.Rioting:
				return $"Rioting{locationAddendum}";
			case CrimeTypes.TaxEvasion:
				return "Tax Evasion";
			case CrimeTypes.Gambling:
				return $"Gambling{locationAddendum}";
			case CrimeTypes.ObstructionOfJustice:
				return $"Obstruction of Justice{locationAddendum}";
			case CrimeTypes.Forgery:
				return $"Forgery of {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.Adultery:
				return $"Adultery with {victimDesc}{locationAddendum}";
			case CrimeTypes.Sodomy:
				return $"Sodomy with {victimDesc}{locationAddendum}";
			case CrimeTypes.Fornication:
				return $"Fornication with {victimDesc}{locationAddendum}";
			case CrimeTypes.Prostitution:
				return $"Prostitution with {victimDesc}{locationAddendum}";
			case CrimeTypes.Kidnapping:
				return $"Kidnapping of {victimDesc}{locationAddendum}";
			case CrimeTypes.Slavery:
				return $"Slavery{locationAddendum}";
			case CrimeTypes.Smuggling:
				return $"Smuggling of {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.AnimalCruelty:
				return $"Animal Cruelty {victimDesc}{locationAddendum}";
			case CrimeTypes.UnlawfulUseOfMagic:
				return $"Unlawful Use of Magic{locationAddendum}";
			case CrimeTypes.UnlawfulUseOfPsionics:
				return $"Unlawful Use of Psionics{locationAddendum}";
			case CrimeTypes.IllegalConsumption:
				return $"Illegal Consumption of {thirdPartyDesc}{locationAddendum}";
			default:
				return "An unknown crime";
		}
	}

	public string DescribeCrimeAtTrial(IPerceiver voyeur)
	{
		var victimDesc = Victim?.PersonalName.GetName(NameStyle.FullName) ??
						 "unnamed victims".ColourCharacter();
		var locationAddendum = CrimeLocation != null ? $" at {CrimeLocation.CurrentOverlay.CellName.ColourRoom()}" : "";
		var thirdPartyDesc = ThirdPartyId.HasValue
			? Gameworld.GetPerceivable(ThirdPartyFrameworkItemType, ThirdPartyId.Value)
					   .HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee)
			: "an unidentified thing".ColourObject();
		return DescribeCrimeInternal(voyeur, victimDesc, locationAddendum, thirdPartyDesc);
	}

	public string DescribeCrime(IPerceiver voyeur)
	{
		var victimDesc = Victim?.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee) ??
						 "unnamed victims".ColourCharacter();
		var locationAddendum = CrimeLocation != null ? $" at {CrimeLocation.CurrentOverlay.CellName.ColourRoom()}" : "";
		var thirdPartyDesc = ThirdPartyId.HasValue
			? Gameworld.GetPerceivable(ThirdPartyFrameworkItemType, ThirdPartyId.Value)
					   .HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee)
			: "an unidentified thing".ColourObject();
		return DescribeCrimeInternal(voyeur, victimDesc, locationAddendum, thirdPartyDesc);
	}

	public void Forgive(ICharacter forgiver, string reason)
	{
		// Stop active enforcement
		foreach (var patrol in LegalAuthority.Patrols)
		{
			if (patrol.ActiveEnforcementCrime == this)
			{
				patrol.InvalidateActiveCrime();
			}
		}

		// TODO - log this
		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.Crimes.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.Crimes.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}

		LegalAuthority.RemoveCrime(this);
	}

	public void Convict(ICharacter? enforcer, PunishmentResult result, string reason)
	{
		FineRecorded = result.Fine;
		CustodialSentenceLength = result.CustodialSentence;
		// TODO - log
		HasBeenConvicted = true;
		HasBeenFinalised = true;
		LegalAuthority.HandleDiscordNotificationOfConviction(Criminal, this, result, enforcer);
		// TODO - echo to enforcers
	}

	public void Acquit(ICharacter enforcer, string reason)
	{
		HasBeenConvicted = false;
		HasBeenFinalised = true;
	}

	public bool EligableForAutomaticConviction()
	{
		if (!CriminalIdentityIsKnown)
		{
			return false;
		}

		if (HasBeenEnforced)
		{
			return true;
		}

		return false;
	}

	void ILazyLoadDuringIdleTime.DoLoad()
	{
		TryLoadCharacter();
	}

	public Difficulty DefenseDifficulty
	{
		get
		{
			var difficulty = Difficulty.Normal;
			// Easier if you've been accused by vNPCs
			if (_accuserId is null)
			{
				// TODO - witness profile biases
				difficulty.StageDown();
			}
			return difficulty;
		}
	}

	public Difficulty ProsecutionDifficulty
	{
		get
		{
			var difficulty = Difficulty.Normal;
			var crimes = LegalAuthority.ResolvedCrimesForIndividual(Criminal)
			                           .Where(x => x.HasBeenConvicted && x.CustodialSentenceLength > TimeSpan.Zero)
			                           .ToList();
			if (crimes.Any())
			{
				difficulty.StageDown(2);
			}
			return difficulty;
		}
	}

	#region FutureProg Implementations

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Crime;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Law.Name);
			case "law":
				return Law;
			case "authority":
				return Law.Authority;
			case "crimename":
				return new TextVariable(Law.CrimeType.DescribeEnum(true));
			case "crimetype":
				return new NumberVariable((int)Law.CrimeType);
			case "isviolentcrime":
				return new BooleanVariable(Law.CrimeType.IsViolentCrime());
			case "ismoralcrime":
				return new BooleanVariable(Law.CrimeType.IsMoralCrime());
			case "ismajorcrime":
				return new BooleanVariable(Law.CrimeType.IsMajorCrime());
			case "isarrestable":
				return new BooleanVariable(Law.EnforcementStrategy.IsArrestable());
			case "iskillable":
				return new BooleanVariable(Law.EnforcementStrategy.IsKillable());
			default:
				throw new ApplicationException($"Invalid property {property} requested in Crime.GetProperty");
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "law", FutureProgVariableTypes.Law },
			{ "authority", FutureProgVariableTypes.LegalAuthority },
			{ "crimename", FutureProgVariableTypes.Text },
			{ "crimetype", FutureProgVariableTypes.Number },
			{ "isviolentcrime", FutureProgVariableTypes.Boolean },
			{ "ismoralcrime", FutureProgVariableTypes.Boolean },
			{ "ismajorcrime", FutureProgVariableTypes.Boolean },
			{ "isarrestable", FutureProgVariableTypes.Boolean },
			{ "iskillable", FutureProgVariableTypes.Boolean }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The ID of the crime" },
			{ "name", "The name of the law which this crime is against" },
			{ "law", "The law which this crime is against" },
			{ "authority", "The legal authority that this crime belongs to" },
			{ "crimename", "The generic name of the crime" },
			{ "crimetype", "The numerical ID of the crime that this law governs" },
			{ "isviolentcrime", "Whether or not this crime type is considered a violent crime" },
			{ "ismoralcrime", "Whether or not this crime type is considered a moral crime" },
			{ "ismajorcrime", "Whether or not this crime type is considered a major crime" },
			{ "isarrestable", "Whether or not this crime can lead to the individual being arrested" },
			{ "iskillable", "Whether or not this crime attracts a lethal force response" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Crime, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
#nullable restore