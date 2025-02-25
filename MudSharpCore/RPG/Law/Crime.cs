﻿using MudSharp.Character;
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
using MudSharp.Economy.Currency;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

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
		_executionPunishment = dbitem.ExecutionPunishment;
		_fineHasBeenPaid = dbitem.FineHasBeenPaid;
		_sentenceHasBeenServed = dbitem.SentenceHasBeenServed;
		_goodBehaviourBond = TimeSpan.FromSeconds(dbitem.GoodBehaviourBond);
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
		dbitem.FineHasBeenPaid = FineHasBeenPaid;
		dbitem.ExecutionPunishment = ExecutionPunishment;
		dbitem.SentenceHasBeenServed = SentenceHasBeenServed;
		dbitem.GoodBehaviourBond = GoodBehaviourBond.TotalSeconds;
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
			CustodialSentenceLength = CustodialSentenceLength.TotalSeconds,
			FineHasBeenPaid = FineHasBeenPaid,
			ExecutionPunishment = ExecutionPunishment,
			SentenceHasBeenServed = SentenceHasBeenServed,
			GoodBehaviourBond = GoodBehaviourBond.TotalSeconds
		};
		FMDB.Context.Crimes.Add(dbitem);
		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((MudSharp.Models.Crime)dbitem).Id;
	}

	public sealed override string FrameworkItemType => "Crime";

	/// <inheritdoc />
	public override string Name => Law.Name;
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

	private bool _executionPunishment;

	public bool ExecutionPunishment
	{
		get => _executionPunishment;
		set
		{
			_executionPunishment = value;
			Changed = true;
		}
	}

	private bool _fineHasBeenPaid;

	public bool FineHasBeenPaid
	{
		get => _fineHasBeenPaid;
		set
		{
			_fineHasBeenPaid = value;
			Changed = true;
		}
	}

	private bool _sentenceHasBeenServed;

	public bool SentenceHasBeenServed
	{
		get => _sentenceHasBeenServed;
		set
		{
			_sentenceHasBeenServed = value;
			Changed = true;
		}
	}

	private TimeSpan _goodBehaviourBond;

	public TimeSpan GoodBehaviourBond
	{
		get => _goodBehaviourBond;
		set
		{
			_goodBehaviourBond = value;
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
	public ICharacter? Victim => Gameworld.TryGetCharacter(VictimId ?? 0, true);

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

	public IPerceivable? ThirdParty => ThirdPartyId.HasValue
		? Gameworld.GetPerceivable(ThirdPartyFrameworkItemType, ThirdPartyId.Value)
		: null;

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
				return $"assaulted {victimDesc} with a deadly weapon{locationAddendum}";
			case CrimeTypes.Battery:
				return $"committed battery against {victimDesc}{locationAddendum}";
			case CrimeTypes.AttemptedMurder:
				return $"attempted murder of {victimDesc}{locationAddendum}";
			case CrimeTypes.Murder:
				return $"murdered {victimDesc}{locationAddendum}";
			case CrimeTypes.Manslaughter:
				return $"were responsible for the manslaughter of {victimDesc}{locationAddendum}";
			case CrimeTypes.Torture:
				return $"tortured {victimDesc}{locationAddendum}";
			case CrimeTypes.GreviousBodilyHarm:
				return $"caused grevious bodily harm to {victimDesc}{locationAddendum}";
			case CrimeTypes.Theft:
				return $"stole {thirdPartyDesc} from {victimDesc}{locationAddendum}";
			case CrimeTypes.Fraud:
				return $"committed fraud against {victimDesc}{locationAddendum}";
			case CrimeTypes.Racketeering:
				return $"committed racketeering against {victimDesc}{locationAddendum}";
			case CrimeTypes.CartelCollusion:
				return $"colluded as a cartel against {victimDesc}{locationAddendum}";
			case CrimeTypes.PossessingStolenGoods:
				return $"possessed stolen goods{locationAddendum}";
			case CrimeTypes.SellingContraband:
				return $"sold contraband{(thirdPartyDesc.StripANSIColour().EqualTo("an unidentified thing") ? "" : $", namely {thirdPartyDesc}")}{locationAddendum}";
			case CrimeTypes.PossessingContraband:
				return $"possessed contrand{(thirdPartyDesc.StripANSIColour().EqualTo("an unidentified thing") ? "" : $", namely {thirdPartyDesc}")}{locationAddendum}";
			case CrimeTypes.TrafficingContraband:
				return $"trafficked contraband{(thirdPartyDesc.StripANSIColour().EqualTo("an unidentified thing") ? "" : $", namely {thirdPartyDesc}")}{locationAddendum}";
			case CrimeTypes.Vandalism:
				if (Victim is null)
				{
					return $"committed vandalism{locationAddendum}";
				}

				return $"vandalised {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.BreakAndEnter:
				return $"committed break and enter{locationAddendum}";
			case CrimeTypes.Loitering:
				return $"loitered{locationAddendum}";
			case CrimeTypes.Littering:
				return $"littered {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.Libel:
				return $"wrote libel against {victimDesc}{locationAddendum}";
			case CrimeTypes.Slander:
				return $"spoke slander against {victimDesc}{locationAddendum}";
			case CrimeTypes.Treason:
				return $"committed treason{locationAddendum}";
			case CrimeTypes.Conspiracy:
				return $"conspired against {victimDesc}{locationAddendum}";
			case CrimeTypes.Intimidation:
				return $"intimidated {victimDesc}{locationAddendum}";
			case CrimeTypes.Blackmail:
				return $"blackmailed {victimDesc}{locationAddendum}";
			case CrimeTypes.Trespassing:
				return $"trespassed{locationAddendum}";
			case CrimeTypes.ResistArrest:
				return $"resisted arrest{locationAddendum}";
			case CrimeTypes.DisobeyLegalInstruction:
				return $"disobeyed a legal instruction of {victimDesc}{locationAddendum}";
			case CrimeTypes.ViolateParole:
				return $"violated parole{locationAddendum}";
			case CrimeTypes.EscapeCaptivity:
				if (Victim is not null)
				{
					return $"escaping from the custody of {victimDesc}{locationAddendum}";
				}

				return $"escaped captivity{locationAddendum}";
			case CrimeTypes.Indecency:
				return $"committed indecency against {victimDesc}{locationAddendum}";
			case CrimeTypes.Immorality:
				return $"acted immorally{locationAddendum}";
			case CrimeTypes.PublicIntoxication:
				return $"was publicly intoxicated{locationAddendum}";
			case CrimeTypes.Blasphemy:
				return $"committed blasphemy{locationAddendum}";
			case CrimeTypes.Apostasy:
				return $"practiced apostasy{locationAddendum}";
			case CrimeTypes.Profanity:
				return $"uttered profanity{locationAddendum}";
			case CrimeTypes.Vagrancy:
				return $"was a vagrant{locationAddendum}";
			case CrimeTypes.DestructionOfProperty:
				return $"destroyed {thirdPartyDesc}, property of {victimDesc}{locationAddendum}";
			case CrimeTypes.UnauthorisedDealing:
				return $"engaged in unauthorised dealing{locationAddendum}";
			case CrimeTypes.Embezzlement:
				return $"embezzled{locationAddendum}";
			case CrimeTypes.Sedition:
				return $"committed sedition{locationAddendum}";
			case CrimeTypes.Mayhem:
				return $"caused mayhem{locationAddendum}";
			case CrimeTypes.ContemptOfCourt:
				return $"was in contempt of court{locationAddendum}";
			case CrimeTypes.ViolateBail:
				return "violated bail";
			case CrimeTypes.Aiding:
				return $"provided aid to the committing of a crime{locationAddendum}";
			case CrimeTypes.Abetting:
				return $"abetted another to commit a crime{locationAddendum}";
			case CrimeTypes.Accessory:
				return $"was an accessory to a crime{locationAddendum}";
			case CrimeTypes.Arson:
				return $"committed arson{locationAddendum}";
			case CrimeTypes.Bribery:
				return $"bribed {victimDesc}{locationAddendum}";
			case CrimeTypes.Tyranny:
				return "acted tyrannically";
			case CrimeTypes.Harassment:
				return $"harassed {victimDesc}{locationAddendum}";
			case CrimeTypes.Extortion:
				return $"extorted {victimDesc}{locationAddendum}";
			case CrimeTypes.Rape:
				return $"raped {victimDesc}{locationAddendum}";
			case CrimeTypes.SexualAssault:
				return $"sexually assaulted {victimDesc}{locationAddendum}";
			case CrimeTypes.Negligence:
				return $"was negligent against {victimDesc}{locationAddendum}";
			case CrimeTypes.Perjury:
				return "commited perjury";
			case CrimeTypes.Desertion:
				return $"deserted their lawful duty{locationAddendum}";
			case CrimeTypes.Mutiny:
				return $"mutinied against {victimDesc}{locationAddendum}";
			case CrimeTypes.Rebellion:
				return $"rebelled against {victimDesc}{locationAddendum}";
			case CrimeTypes.Rioting:
				return $"rioted{locationAddendum}";
			case CrimeTypes.TaxEvasion:
				return "committed tax evasion";
			case CrimeTypes.Gambling:
				return $"gambled{locationAddendum}";
			case CrimeTypes.ObstructionOfJustice:
				return $"obstructed justice{locationAddendum}";
			case CrimeTypes.Forgery:
				return $"forged {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.Adultery:
				return $"committed adultery with {victimDesc}{locationAddendum}";
			case CrimeTypes.Sodomy:
				return $"committed sodomy with {victimDesc}{locationAddendum}";
			case CrimeTypes.Fornication:
				return $"fornicated with {victimDesc}{locationAddendum}";
			case CrimeTypes.Prostitution:
				return $"engaged in prostitution with {victimDesc}{locationAddendum}";
			case CrimeTypes.Kidnapping:
				return $"kidnapped {victimDesc}{locationAddendum}";
			case CrimeTypes.Slavery:
				return $"was engaged in slavery{locationAddendum}";
			case CrimeTypes.Smuggling:
				return $"smuggled {thirdPartyDesc}{locationAddendum}";
			case CrimeTypes.AnimalCruelty:
				return $"was unlawfully cruel to {victimDesc}{locationAddendum}";
			case CrimeTypes.UnlawfulUseOfMagic:
				return $"unlawfully used magic{locationAddendum}";
			case CrimeTypes.UnlawfulUseOfPsionics:
				return $"unlawfully used psionics{locationAddendum}";
			case CrimeTypes.IllegalConsumption:
				return $"illegally consumed {thirdPartyDesc}{locationAddendum}";
			default:
				return "committed an unknown crime";
		}
	}

	public string DescribeCrimeAtTrial(IPerceiver voyeur)
	{
		var victimDesc = 
			Victim?.PersonalName.GetName(NameStyle.FullName) ?? 
			"unnamed victims";
		var locationAddendum = 
			CrimeLocation != null ? 
				$" at {CrimeLocation.CurrentOverlay.CellName}" : 
				"";
		var thirdPartyDesc = ThirdParty?.HowSeen(voyeur, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee) ?? "an unidentified thing";
		return 
			DescribeCrimeInternal(voyeur, victimDesc, locationAddendum, thirdPartyDesc)
				.Replace("was ", "$1|were|was ");
	}

	public string DescribeCrime(IPerceiver voyeur)
	{
		var victimDesc = Victim?.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee) ??
						 "unnamed victims".ColourCharacter();
		var locationAddendum = CrimeLocation != null ? $" at {CrimeLocation.CurrentOverlay.CellName.ColourRoom()}" : "";
		var thirdPartyDesc = ThirdParty?.HowSeen(voyeur, flags: PerceiveIgnoreFlags.IgnoreCanSee) ?? "an unidentified thing".ColourObject();
		return DescribeCrimeInternal(voyeur, victimDesc, locationAddendum, thirdPartyDesc);
	}

	public string ShowCrimeInfo(ICharacter enforcer)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Crime #{Id.ToStringN0(enforcer)}".GetLineWithTitleInner(enforcer, Telnet.BoldOrange, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Law: {Law.Name.ColourName()}");
		sb.AppendLine($"Jurisdiction: {LegalAuthority.Name.ColourValue()}");
		sb.AppendLine($"Crime Type: {Law.CrimeType.DescribeEnum(true).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Criminal".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
		sb.AppendLine();
		if (CriminalIdentityIsKnown)
		{
			sb.AppendLine($"Criminal: {Criminal.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription)}");
			sb.AppendLine($"Criminal Name: {Criminal.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()}");
			sb.AppendLine("Criminal Description:");
			sb.AppendLine();
			sb.AppendLine(Criminal.HowSeen(enforcer, type: DescriptionType.Full, flags: PerceiveIgnoreFlags.TrueDescription).Wrap(enforcer.InnerLineFormatLength, "\t"));
		}
		else
		{
			sb.AppendLine($"Criminal: {CriminalShortDescription.ColourCharacter()}");
			sb.AppendLine("Criminal Description:");
			sb.AppendLine();
			sb.AppendLine(CriminalDescription.Wrap(enforcer.InnerLineFormatLength));
			sb.AppendLine();
			sb.AppendLine("Criminal Characteristics:");
			sb.AppendLine();
			foreach (var item in _criminalCharacteristics)
			{
				sb.AppendLine($"\t{item.Key.Name.ColourName()} = {item.Value.GetValue.ColourValue()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Victim".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
		sb.AppendLine();
		if (Victim is null)
		{
			sb.AppendLine("Victim: #6Unknown Persons#0".SubstituteANSIColour());
		}
		else
		{
			sb.AppendLine($"Victim: {Victim.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription)}");
			sb.AppendLine($"Victim Name: {Victim.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()}");
		}

		sb.AppendLine();
		sb.AppendLine("Details".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
		sb.AppendLine();
		sb.AppendLine($"Time of Crime: {TimeOfCrime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine($"Location of Crime: {CrimeLocation?.HowSeen(enforcer, flags: PerceiveIgnoreFlags.IgnoreCanSee) ?? "An Unknown Location".ColourRoom()}");
		sb.AppendLine($"Third Party / Object: {ThirdParty?.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription) ?? "None".ColourError()}");
		if (enforcer.IsAdministrator())
		{
			sb.AppendLine();
			sb.AppendLine($"Witnesses:");
			sb.AppendLine();
			foreach (var id in _witnessIds)
			{
				var witness = Gameworld.TryGetCharacter(id, true);
				if (witness is null)
				{
					continue;
				}
				sb.AppendLine($"\t{witness.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription)} ({witness.PersonalName.GetName(NameStyle.FullWithNickname)}) [#{id.ToStringN0(enforcer)}]");
			}
		}

		if (enforcer != Criminal)
		{
			sb.AppendLine();
			sb.AppendLine("Enforcement".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
			sb.AppendLine();
			if (HasBeenFinalised)
			{
				sb.AppendLine($"Status: {"Finalised".Colour(Telnet.Green)}");
			}
			else if (HasBeenEnforced)
			{
				sb.AppendLine($"Status: {"Enforced".Colour(Telnet.BoldYellow)}");
			}
			else if (IsKnownCrime)
			{
				sb.AppendLine($"Status: {"Wanted".Colour(Telnet.Red)}");
			}
			else if ((TimeOfCrime.Calendar.CurrentDateTime - TimeOfCrime) > Law.ActivePeriod)
			{
				sb.AppendLine($"Status: {"Stale".Colour(Telnet.BoldPink)}");
			}
			else
			{
				sb.AppendLine($"Status: {"Unreported".Colour(Telnet.Magenta)}");
			}

			if (!HasBeenEnforced)
			{
				sb.AppendLine($"Active For: {Law.ActivePeriod.DescribePreciseBrief(enforcer).ColourValue()}");
				sb.AppendLine($"Active Until: {(TimeOfCrime+Law.ActivePeriod).ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
			}

			if (IsKnownCrime)
			{
				sb.AppendLine($"Time of Report: {TimeOfReport?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue() ?? "Unknown".ColourError()}");
				if (AccuserId is not null && Gameworld.TryGetCharacter(AccuserId.Value, true) is { } accuser)
				{
					sb.AppendLine($"Accuser: {accuser.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription)}");
					sb.AppendLine($"Accuser Name: {accuser.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()}");
				}
				else
				{
					sb.AppendLine($"Accuser: {"Unknown Persons".ColourCharacter()}");
				}
			}
		}

		if (HasBeenEnforced || HasBeenFinalised)
		{
			sb.AppendLine();
			sb.AppendLine("Outcome".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
			sb.AppendLine();
			sb.AppendLine($"Bail: {(CalculatedBail > 0.0M).ToColouredString()}");
			if (CalculatedBail > 0.0M)
			{
				sb.AppendLine($"Bail Amount: {LegalAuthority.Currency.Describe(CalculatedBail, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				sb.AppendLine($"Bail Posted: {BailPosted.ToColouredString()}");
			}
		}

		if (HasBeenFinalised)
		{
			sb.AppendLine($"Verdict: {(HasBeenConvicted ? "Guilty".Colour(Telnet.Red) : "Not Guilty".Colour(Telnet.Green))}");
			if (HasBeenConvicted)
			{
				sb.AppendLine();
				sb.AppendLine("Sentence".GetLineWithTitleInner(enforcer, Telnet.Orange, Telnet.White));
				sb.AppendLine();
				sb.AppendLine($"Death: {ExecutionPunishment.ToColouredString()}");
				sb.AppendLine($"Prison: {(CustodialSentenceLength > TimeSpan.Zero ? CustodialSentenceLength.DescribePreciseBrief(enforcer).ColourValue() : "False".Colour(Telnet.Red))}");
				sb.AppendLine($"Fine: {LegalAuthority.Currency.Describe(FineRecorded, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				sb.AppendLine($"Good Behaviour: {(GoodBehaviourBond > TimeSpan.Zero ? GoodBehaviourBond.DescribePreciseBrief(enforcer).ColourValue() : "False".Colour(Telnet.Red))}");
				if (FineRecorded > 0.0M)
				{
					sb.AppendLine();
					sb.AppendLine($"Fine Paid: {FineHasBeenPaid.ToColouredString()}");
				}
			}
		}
		
		return sb.ToString();
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
		ExecutionPunishment = result.Execution;
		GoodBehaviourBond = result.GoodBehaviourBondLength;
		// TODO - log
		HasBeenConvicted = true;
		HasBeenFinalised = true;
		LegalAuthority.HandleDiscordNotificationOfConviction(Criminal, this, result, enforcer);
		LegalAuthority.ConvictCrime(Criminal, this, result);
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

	public string DescribePunishment(ICharacter voyeur)
	{
		var list = new List<string>();
		var sb = new StringBuilder();
		if (ExecutionPunishment)
		{
			list.Add("execution");
		}

		if (FineRecorded > 0.0M)
		{
			list.Add($"a {LegalAuthority.Currency.Describe(FineRecorded, CurrencyDescriptionPatternType.ShortDecimal)} fine");
		}

		if (CustodialSentenceLength > TimeSpan.Zero)
		{
			list.Add($"a {CustodialSentenceLength.DescribePreciseBrief(voyeur)} prison sentence");
		}

		if (GoodBehaviourBond > TimeSpan.Zero)
		{
			list.Add($"a {GoodBehaviourBond.DescribePreciseBrief(voyeur)} good behaviour bond");
		}

		if (list.Count == 0)
		{
			return "no punishment";
		}

		return list.ListToColouredString();
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

	public ProgVariableTypes Type => ProgVariableTypes.Crime;
	public object GetObject => this;

	public IProgVariable GetProperty(string property)
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

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "law", ProgVariableTypes.Law },
			{ "authority", ProgVariableTypes.LegalAuthority },
			{ "crimename", ProgVariableTypes.Text },
			{ "crimetype", ProgVariableTypes.Number },
			{ "isviolentcrime", ProgVariableTypes.Boolean },
			{ "ismoralcrime", ProgVariableTypes.Boolean },
			{ "ismajorcrime", ProgVariableTypes.Boolean },
			{ "isarrestable", ProgVariableTypes.Boolean },
			{ "iskillable", ProgVariableTypes.Boolean }
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
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Crime, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
#nullable restore