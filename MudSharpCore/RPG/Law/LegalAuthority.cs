using MudSharp.Framework.Save;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.Economy;
using MudSharp.TimeAndDate;
using MudSharp.Character.Name;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.RPG.Law;

public partial class LegalAuthority : SaveableItem, ILegalAuthority
{
	public LegalAuthority(string name, ICurrency currency, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_name = name;
		Currency = currency;
		using (new FMDB())
		{
			var dbitem = new Models.LegalAuthority();
			FMDB.Context.LegalAuthorities.Add(dbitem);
			dbitem.Name = name;
			dbitem.CurrencyId = Currency.Id;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		PatrolController = new PatrolController(this);
	}

	public LegalAuthority(MudSharp.Models.LegalAuthority dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Currency = gameworld.Currencies.Get(dbitem.CurrencyId);
		PlayersKnowTheirCrimes = dbitem.PlayersKnowTheirCrimes;
		AutomaticallyConvict = dbitem.AutomaticallyConvict;
		AutomaticConvictionTime = TimeSpan.FromSeconds(dbitem.AutomaticConvictionTime);

		foreach (var zone in dbitem.LegalAuthoritiesZones)
		{
			_enforcementZones.Add(Gameworld.Zones.Get(zone.ZoneId));
		}

		foreach (var item in dbitem.LegalClasses)
		{
			var legalClass = new LegalClass(item, this, gameworld);
			Gameworld.Add(legalClass);
			_legalClasses.Add(legalClass);
		}

		foreach (var item in dbitem.EnforcementAuthorities)
		{
			var authority = new EnforcementAuthority(item, this, gameworld);
			Gameworld.Add(authority);
			_enforcementAuthorities.Add(authority);
		}

		foreach (var item in dbitem.Laws)
		{
			var law = new Law(item, this, Gameworld);
			Gameworld.Add(law);
			_laws.Add(law);

			foreach (var dbcrime in item.Crimes)
			{
				var crime = new Crime(dbcrime, law, Gameworld);
				if (dbcrime.IsFinalised)
				{
					_resolvedCrimes.Add(crime);
				}
				else if (dbcrime.IsStaleCrime)
				{
					_staleCrimes.Add(crime);
				}
				else if (dbcrime.IsKnownCrime)
				{
					_knownCrimes.Add(crime);
				}
				else
				{
					_unknownCrimes.Add(crime);
				}

				Gameworld.Add(crime);
			}
		}

		PreparingLocation = Gameworld.Cells.Get(dbitem.PreparingLocationId ?? 0);
		MarshallingLocation = Gameworld.Cells.Get(dbitem.MarshallingLocationId ?? 0);
		EnforcerStowingLocation = Gameworld.Cells.Get(dbitem.EnforcerStowingLocationId ?? 0);
		PrisonLocation = Gameworld.Cells.Get(dbitem.PrisonLocationId ?? 0);
		PrisonReleaseLocation = Gameworld.Cells.Get(dbitem.PrisonReleaseLocationId ?? 0);
		PrisonerBelongingsStorageLocation = Gameworld.Cells.Get(dbitem.PrisonBelongingsLocationId ?? 0);
		JailLocation = Gameworld.Cells.Get(dbitem.JailLocationId ?? 0);
		CourtLocation = Gameworld.Cells.Get(dbitem.CourtLocationId ?? 0);
		BankAccount = Gameworld.BankAccounts.Get(dbitem.BankAccountId ?? 0);
		_cellLocations.AddRange(dbitem.LegalAuthorityCells.SelectNotNull(x => Gameworld.Cells.Get(x.CellId)));
		_jailLocations.AddRange(dbitem.LegalAuthorityJailCells.SelectNotNull(x => Gameworld.Cells.Get(x.CellId)));

		OnPrisonerReleased = Gameworld.FutureProgs.Get(dbitem.OnReleaseProgId ?? 0);
		OnPrisonerImprisoned = Gameworld.FutureProgs.Get(dbitem.OnImprisonProgId ?? 0);
		OnPrisonerHeld = Gameworld.FutureProgs.Get(dbitem.OnHoldProgId ?? 0);
		BailCalculationProg = Gameworld.FutureProgs.Get(dbitem.BailCalculationProgId ?? 0);

		DiscordChannelId = dbitem.GuardianDiscordChannel;

		foreach (var item in dbitem.PatrolRoutes)
		{
			_patrolRoutes.Add(new PatrolRoute(item, this));
		}

		foreach (var fine in dbitem.Fines)
		{
			_finesOwed.Add(fine.CharacterId, fine.FinesOwned);
			_finePaymentDueDates.Add(fine.CharacterId, new MudDateTime(fine.PaymentRequiredBy, Gameworld));
		}

		CalculateLawLookup();
		CalculateKnownCrimeLookup();
		CalculateUnknownCrimeLookup();
		CalculateResolvedCrimesLookup();

		PatrolController = new PatrolController(this);
		Gameworld.HeartbeatManager.FuzzyMinuteHeartbeat += LegalHeartbeat;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.LegalAuthorities.Find(Id);
		dbitem.CurrencyId = Currency.Id;
		dbitem.Name = Name;
		dbitem.PreparingLocationId = PreparingLocation?.Id;
		dbitem.MarshallingLocationId = MarshallingLocation?.Id;
		dbitem.EnforcerStowingLocationId = EnforcerStowingLocation?.Id;
		dbitem.PrisonLocationId = PrisonLocation?.Id;
		dbitem.PrisonReleaseLocationId = PrisonReleaseLocation?.Id;
		dbitem.PrisonBelongingsLocationId = PrisonerBelongingsStorageLocation?.Id;
		dbitem.JailLocationId = JailLocation?.Id;
		dbitem.OnImprisonProgId = OnPrisonerImprisoned?.Id;
		dbitem.OnReleaseProgId = OnPrisonerReleased?.Id;
		dbitem.OnHoldProgId = OnPrisonerHeld?.Id;
		dbitem.BailCalculationProgId = BailCalculationProg?.Id;
		dbitem.CourtLocationId = CourtLocation?.Id;
		dbitem.BankAccountId = BankAccount?.Id;
		dbitem.PlayersKnowTheirCrimes = PlayersKnowTheirCrimes;
		dbitem.AutomaticallyConvict = AutomaticallyConvict;
		dbitem.AutomaticConvictionTime = AutomaticConvictionTime.TotalSeconds;
		dbitem.GuardianDiscordChannel = DiscordChannelId;
		FMDB.Context.LegalAuthoritiyCells.RemoveRange(dbitem.LegalAuthorityCells);
		foreach (var cell in CellLocations)
		{
			dbitem.LegalAuthorityCells.Add(new LegalAuthorityCells { LegalAuthority = dbitem, CellId = cell.Id });
		}

		FMDB.Context.LegalAuthorityJailCells.RemoveRange(dbitem.LegalAuthorityJailCells);
		foreach (var cell in JailLocations)
		{
			dbitem.LegalAuthorityJailCells.Add(new LegalAuthorityJailCell
				{ LegalAuthority = dbitem, CellId = cell.Id });
		}

		FMDB.Context.LegalAuthoritiesZones.RemoveRange(dbitem.LegalAuthoritiesZones);
		foreach (var zone in _enforcementZones)
		{
			dbitem.LegalAuthoritiesZones.Add(new LegalAuthoritiesZones { LegalAuthority = dbitem, ZoneId = zone.Id });
		}

		Changed = false;
	}

	public void LoadPatrols()
	{
		var dbitem = FMDB.Context.LegalAuthorities.Find(Id);
		foreach (var patrol in dbitem.Patrols)
		{
			_patrols.Add(new Patrol(patrol, this));
		}
	}

	public void Delete()
	{
		// TODO - aborting patrols in progress
		if (Changed)
		{
			Gameworld.SaveManager.Abort(this);
		}

		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.LegalAuthorities.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.LegalAuthorities.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public override string FrameworkItemType => "LegalAuthority";
	public ICurrency Currency { get; protected set; }

	private readonly List<ILaw> _laws = new();
	public IEnumerable<ILaw> Laws => _laws;
	private readonly CollectionDictionary<CrimeTypes, ILaw> _lawLookup = new();
	private readonly CollectionDictionary<long, ICrime> _knownCrimesLookup = new();
	private readonly CollectionDictionary<long, ICrime> _unknownCrimesLookup = new();
	private readonly CollectionDictionary<long, ICrime> _resolvedCrimesLookup = new();

	private readonly DecimalCounter<long> _finesOwed = new();
	private readonly Dictionary<long, MudDateTime> _finePaymentDueDates = new();
	private readonly List<ICrime> _knownCrimes = new();
	public IEnumerable<ICrime> KnownCrimes => _knownCrimes;

	private readonly List<ICrime> _staleCrimes = new();
	public IEnumerable<ICrime> StaleCrimes => _staleCrimes;

	private readonly List<ICrime> _unknownCrimes = new();
	public IEnumerable<ICrime> UnknownCrimes => _unknownCrimes;

	private readonly List<ICrime> _resolvedCrimes = new();
	public IEnumerable<ICrime> ResolvedCrimes => _resolvedCrimes;

	public bool PlayersKnowTheirCrimes { get; set; }
	public ulong? DiscordChannelId { get; set; }
	public IBankAccount BankAccount { get; set; }
	public bool AutomaticallyConvict { get; set; }
	public TimeSpan AutomaticConvictionTime { get; set; }

	#region Calculation Helpers
	private void CalculateLawLookup()
	{
		_lawLookup.Clear();
		foreach (var law in _laws)
		{
			_lawLookup.Add(law.CrimeType, law);
		}
	}

	private void CalculateKnownCrimeLookup()
	{
		_knownCrimesLookup.Clear();
		foreach (var crime in _knownCrimes)
		{
			_knownCrimesLookup.Add(crime.CriminalId, crime);
		}
	}

	private void CalculateUnknownCrimeLookup()
	{
		_unknownCrimesLookup.Clear();
		foreach (var crime in _unknownCrimes)
		{
			_unknownCrimesLookup.Add(crime.CriminalId, crime);
		}
	}

	private void CalculateResolvedCrimesLookup()
	{
		_resolvedCrimesLookup.Clear();
		foreach (var crime in _resolvedCrimes)
		{
			_resolvedCrimesLookup.Add(crime.CriminalId, crime);
		}
	}
	#endregion

	#region Heartbeats
	public void LegalHeartbeat()
	{
		var now = EnforcementZones.FirstOrDefault()?.DateTime() ?? Gameworld.Calendars.First().CurrentDateTime;
		SentencingHeartbeat(now);
		BailHeartbeat(now);
		CustodialSentencesHeartbeat(now);
	}

	private void CustodialSentencesHeartbeat(MudDateTime now)
	{
		// Check for custodial sentences ending
		var custodial = Gameworld.Actors.Where(x => x.AffectedBy<ServingCustodialSentence>(this)).ToList();
		foreach (var prisoner in custodial)
		{
			var custodialEffect =
				prisoner.EffectsOfType<ServingCustodialSentence>(x => x.LegalAuthority == this).First();
			if (custodialEffect.ReleaseDate > now)
			{
				continue;
			}

			prisoner.RemoveEffect(custodialEffect, true);
			ReleaseCharacterToFreedom(prisoner);
			// Notify Discord
			// Notify enforcers
		}
	}

	private void BailHeartbeat(MudDateTime now)
	{
		// Check for bail skippers
		var bailed = KnownCrimes.Where(x => x.BailPosted).Select(x => x.Criminal).Distinct().ToList();
		foreach (var criminal in bailed)
		{
			var bailEffect = criminal.EffectsOfType<OnBail>(x => x.LegalAuthority == this).FirstOrDefault();
			if (bailEffect is null)
			{
				continue;
			}

			if (bailEffect.ArrestTime > now)
			{
				continue;
			}

			var crimes = CheckPossibleCrime(criminal, CrimeTypes.ViolateBail, null, null, "");
			if (!crimes.Any())
			{
				continue;
			}

			EndBail(criminal);

			// Notify Discord
			// Notify enforcers
		}
	}

	private void SentencingHeartbeat(MudDateTime now)
	{
		if (AutomaticallyConvict)
		{
			// If we have a judge on duty, use the judge
			if (Patrols.Any(x => x.PatrolStrategy.Name == "Judge"))
			{
				return;
			}

			foreach (var criminal in KnownCrimes.Where(x => x.EligableForAutomaticConviction()).Select(x => x.Criminal)
			                                    .Distinct())
			{
				var awaitingEffect = criminal.EffectsOfType<AwaitingSentencing>(x => x.LegalAuthority == this)
				                             .FirstOrDefault();
				if (awaitingEffect is null)
				{
					continue;
				}

				if (criminal.AffectedBy<OnBail>(this) || criminal.AffectedBy<OnTrial>(this))
				{
					continue;
				}

				if (now - awaitingEffect.ArrestTime < AutomaticConvictionTime)
				{
					continue;
				}

				var crimes = KnownCrimesForIndividual(criminal).ToList();
				var result = new PunishmentResult();
				foreach (var crime in crimes)
				{
					var crimeResult = crime.Law.PunishmentStrategy.GetResult(criminal, crime);
					result += crimeResult;
					crime.Convict(null, result, "Automatic conviction by the system");
					FinaliseCrime(crime);
				}

				if (result.Fine > 0)
				{
					_finesOwed[criminal.Id] += result.Fine;
					_finePaymentDueDates[criminal.Id] = now + MudTimeSpan.FromMonths(1);
					Changed = true;
				}

				if (result.CustodialSentence > MudTimeSpan.Zero)
				{
					var servingEffect = criminal.EffectsOfType<ServingCustodialSentence>(x => x.LegalAuthority == this)
					                            .FirstOrDefault();
					if (servingEffect == null)
					{
						servingEffect = new ServingCustodialSentence(criminal, this, result.CustodialSentence,
							now + result.CustodialSentence);
						criminal.AddEffect(servingEffect);
					}
					else
					{
						servingEffect.ExtendSentence(result.CustodialSentence);
					}
				}

				// Good Behaviour Bond
				if (result.GoodBehaviourBondLength > MudTimeSpan.Zero)
				{
					var bondEffect = criminal.EffectsOfType<GoodBehaviourBond>(x => x.Authority == this)
					                         .FirstOrDefault();
					if (bondEffect is null)
					{
						bondEffect = new GoodBehaviourBond(criminal, this, result.GoodBehaviourBondLength);
						criminal.AddEffect(bondEffect);
					}
					else
					{
						bondEffect.AddLengthToBond(result.GoodBehaviourBondLength);
					}
				}

				criminal.RemoveAllEffects<AwaitingSentencing>(x => x.LegalAuthority == this);
			}
		}
	}
	#endregion

	#region Helper Methods

	public void ConvictCrime(ICharacter criminal, ICrime crime, PunishmentResult result)
	{
		var now = EnforcementZones.FirstOrDefault()?.DateTime() ?? Gameworld.Calendars.First().CurrentDateTime;
		_knownCrimes.Remove(crime);
		_knownCrimesLookup.Remove(criminal.Id, crime);
		_resolvedCrimes.Add(crime);
		_resolvedCrimesLookup.Add(criminal.Id, crime);

		if (result.Fine > 0)
		{
			_finesOwed[criminal.Id] += result.Fine;
			if (!_finePaymentDueDates.ContainsKey(criminal.Id))
			{
				_finePaymentDueDates[criminal.Id] = now + MudTimeSpan.FromMonths(1);
			}

			Changed = true;
		}

		if (result.CustodialSentence > MudTimeSpan.Zero)
		{
			var servingEffect = criminal.EffectsOfType<ServingCustodialSentence>(x => x.LegalAuthority == this)
			                            .FirstOrDefault();
			if (servingEffect == null)
			{
				servingEffect = new ServingCustodialSentence(criminal, this, result.CustodialSentence,
					now + result.CustodialSentence);
				criminal.AddEffect(servingEffect);
			}
			else
			{
				servingEffect.ExtendSentence(result.CustodialSentence);
			}
		}

		// Good Behaviour Bond
		if (result.GoodBehaviourBondLength > MudTimeSpan.Zero)
		{
			var bondEffect = criminal.EffectsOfType<GoodBehaviourBond>(x => x.Authority == this).FirstOrDefault();
			if (bondEffect is null)
			{
				bondEffect = new GoodBehaviourBond(criminal, this, result.GoodBehaviourBondLength);
				criminal.AddEffect(bondEffect);
			}
			else
			{
				bondEffect.AddLengthToBond(result.GoodBehaviourBondLength);
			}
		}

		if (result.Execution)
		{
			var executionEffect = criminal.EffectsOfType<AwaitingExecution>(x => x.LegalAuthority == this).FirstOrDefault();
			if (executionEffect is null)
			{
				executionEffect = new AwaitingExecution(criminal, this, now + MudTimeSpan.FromDays(1));
				criminal.AddEffect(executionEffect);
			}
		}
	}

	public void ConvictAllKnownCrimes(ICharacter criminal, ICharacter judge)
	{
		
		var crimes = KnownCrimesForIndividual(criminal).ToList();
		var result = new PunishmentResult();
		foreach (var crime in crimes)
		{
			var crimeResult = crime.Law.PunishmentStrategy.GetResult(criminal, crime);
			result += crimeResult;
			crime.Convict(judge, crimeResult, "Automatic conviction by the system");
		}

		criminal.RemoveAllEffects<AwaitingSentencing>(x => x.LegalAuthority == this);
		criminal.RemoveAllEffects<OnTrial>(x => x.LegalAuthority == this);
	}

	public void CheckCharacterForCustodyChanges(ICharacter criminal)
	{
		var effect = criminal.EffectsOfType<AwaitingSentencing>(x => x.LegalAuthority == this).FirstOrDefault();
		if (effect is not null && !KnownCrimesForIndividual(criminal).Any())
		{
			ReleaseCharacterToFreedom(criminal);
		}
	}

	public void IncarcerateCriminal(ICharacter criminal)
	{
		foreach (var crime in KnownCrimesForIndividual(criminal))
		{
			crime.HasBeenEnforced = true;
		}

		var items = new List<IGameItem>();
		foreach (var item in criminal.Body.WornItems.ToArray())
		{
			items.Add(item);
			criminal.Body.Take(item);
		}

		foreach (var item in criminal.Body.HeldOrWieldedItems.ToArray())
		{
			items.Add(item);
			criminal.Body.Take(item);
		}

		if (items.Any())
		{
			var bundle =
				PrisonerBelongingsStorageLocation.GameItems.FirstOrDefault(x =>
					x.AffectedBy<PrisonerBelongings>(criminal));
			if (bundle is null)
			{
				bundle = GameItems.Prototypes.PileGameItemComponentProto.CreateNewBundle(items);
				Gameworld.Add(bundle);
				bundle.AddEffect(new PrisonerBelongings(bundle, criminal));
				PrisonerBelongingsStorageLocation.Insert(bundle, true);
				bundle.SetEmote(new Emote(
					$"marked as the property of {criminal.CurrentName.GetName(NameStyle.FullWithNickname)}", bundle));
			}
			else
			{
				var container = bundle.GetItemType<IContainer>();
				foreach (var item in items)
				{
					container.Put(criminal, item);
				}
			}
		}

		var cell = CellLocations.GetRandomElement();
		criminal.Movement?.CancelForMoverOnly(criminal);
		criminal.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		criminal.Location.Leave(criminal);
		criminal.RoomLayer = RoomLayer.GroundLevel;
		cell.Enter(criminal);
		OnPrisonerHeld?.Execute(criminal);
		criminal.Body.Look(true);
		EndBail(criminal);
		if (!criminal.AffectedBy<AwaitingSentencing>(this))
		{
			criminal.AddEffect(new AwaitingSentencing(criminal, this, criminal.Location.DateTime()));
		}

		HandleDiscordNotificationOfIncarceration(criminal);
		// Notify enforcers
	}

	public void SendCharacterToHoldingCell(ICharacter criminal)
	{
		criminal.Movement?.CancelForMoverOnly(criminal);
		criminal.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		criminal.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("SendCharacterToHoldingCellEmoteOrigin"), criminal, criminal),
			flags: OutputFlags.SuppressSource));
		criminal.OutputHandler.Send(
			new EmoteOutput(new Emote(Gameworld.GetStaticString("SendCharacterToHoldingCellEmoteSelf"), criminal,
				criminal)));
		criminal.Location.Leave(criminal);
		criminal.RoomLayer = RoomLayer.GroundLevel;
		var cell = CellLocations.OrderBy(x => x.Characters.Count()).First();
		cell.Enter(criminal);
		OnPrisonerImprisoned?.Execute(criminal);
		criminal.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("SendCharacterToHoldingCellEmoteDestination"), criminal, criminal),
			flags: OutputFlags.SuppressSource));
		criminal.Body.Look(true);
		HandleDiscordNotificationOfIncarceration(criminal);
	}

	public void SendCharacterToPrison(ICharacter criminal)
	{
		criminal.Movement?.CancelForMoverOnly(criminal);
		criminal.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		criminal.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("SendCharacterToPrisonEmoteOrigin"), criminal, criminal),
			flags: OutputFlags.SuppressSource));
		criminal.OutputHandler.Send(
			new EmoteOutput(new Emote(Gameworld.GetStaticString("SendCharacterToPrisonEmoteSelf"), criminal,
				criminal)));
		criminal.Location.Leave(criminal);
		criminal.RoomLayer = RoomLayer.GroundLevel;
		PrisonLocation.Enter(criminal);
		OnPrisonerImprisoned?.Execute(criminal);
		criminal.OutputHandler.Handle(new EmoteOutput(
			new Emote(Gameworld.GetStaticString("SendCharacterToPrisonEmoteDestination"), criminal, criminal),
			flags: OutputFlags.SuppressSource));
		criminal.Body.Look(true);
		HandleDiscordNotificationOfImprisonment(criminal,
			criminal.EffectsOfType<ServingCustodialSentence>().First().TotalTime.Describe());
	}

	public void ReleaseCharacterToFreedom(ICharacter criminal)
	{
		criminal.Movement?.CancelForMoverOnly(criminal);
		criminal.RemoveAllEffects(x => x.IsEffectType<IActionEffect>());
		criminal.Location.Leave(criminal);
		criminal.RoomLayer = RoomLayer.GroundLevel;
		PrisonReleaseLocation.Enter(criminal);
		OnPrisonerReleased?.Execute(criminal);
		var bundle =
			PrisonerBelongingsStorageLocation.GameItems.FirstOrDefault(x => x.AffectedBy<PrisonerBelongings>(criminal));
		if (bundle is not null)
		{
			if (criminal.Body.CanGet(bundle, 0))
			{
				criminal.Body.Get(bundle, silent: true);
			}
			else
			{
				bundle.RoomLayer = criminal.RoomLayer;
				PrisonReleaseLocation.Insert(bundle, true);
			}
		}

		criminal.Body.Look(true);
		HandleDiscordNotificationOfRelease(criminal);
	}

	public void CalculateAndSetBail(ICharacter criminal)
	{
		foreach (var crime in KnownCrimesForIndividual(criminal).ToArray())
		{
			crime.CalculatedBail = BailCalculationProg?.Execute<decimal?>(criminal, crime) ?? 0.0M;
			crime.BailPosted = true;
		}
	}
	
	public void EndBail(ICharacter criminal)
	{
		criminal.RemoveAllEffects<OnBail>(x => x.LegalAuthority == this, true);
		// Surrender any bail payments
		foreach (var crime in KnownCrimesForIndividual(criminal))
		{
			if (crime.BailPosted)
			{
				crime.BailPosted = false;
				crime.CalculatedBail = 0.0M;
			}
		}
	}

	public (decimal Fine, MudDateTime DueDate) FinesOwed(ICharacter criminal)
	{
		if (_finesOwed[criminal.Id] == 0.0M)
		{
			return (0.0M, MudDateTime.Never);
		}

		return (_finesOwed[criminal.Id], _finePaymentDueDates[criminal.Id]);
	}

	public void PayFine(ICharacter criminal, ICrime crime)
	{
		if (crime.FineRecorded <= 0.0M)
		{
			return;
		}

		if (BankAccount != null)
		{
			BankAccount.DepositFromTransaction(crime.FineRecorded, $"Fine paid by {criminal.PersonalName.GetName(NameStyle.FullName)}");
		}

		crime.FineHasBeenPaid = true;
		_finesOwed[criminal.Id] -= crime.FineRecorded;
	}

	public IEnumerable<ICrime> CheckPossibleCrime(ICharacter criminal, CrimeTypes crime, ICharacter victim,
		IGameItem item, string additionalInformation)
	{
		if (criminal.IsAdministrator())
		{
			return Enumerable.Empty<ICrime>();
		}

		if (!EnforcementZones.Contains(criminal.Location.Zone))
		{
			return Enumerable.Empty<ICrime>();
		}

		var crimes = new List<ICrime>();
		foreach (var law in _lawLookup[crime])
		{
			if (!law.CanBeAppliedAutomatically)
			{
				continue;
			}

			if (!law.IsCrime(criminal, victim, item))
			{
				continue;
			}

			if (law.DoNotAutomaticallyApplyRepeats && _unknownCrimesLookup[criminal.Id]
													  .Concat(_knownCrimesLookup[criminal.Id]).Any(x =>
														  x.Victim == victim &&
														  (x.ThirdPartyId == item?.Id ||
														   x.ThirdPartyFrameworkItemType != "item") &&
														  x.CrimeLocation == criminal.Location &&
														  DateTime.UtcNow - x.RealTimeOfCrime < TimeSpan.FromMinutes(10)
													  ))
			{
				continue;
			}

			var witnesses = criminal.Location.LayerCharacters(criminal.RoomLayer).Except(criminal)
									.Where(x => x.CanSee(criminal)).ToList();
			var newCrime = new Crime(criminal, victim, witnesses, law, item);
			_unknownCrimes.Add(newCrime);
			_unknownCrimesLookup.Add(criminal.Id, newCrime);
			Gameworld.Add(newCrime);
			foreach (var witness in witnesses)
			{
				witness.HandleEvent(Events.EventType.WitnessedCrime, criminal, victim, witness, newCrime);
			}

			victim?.HandleEvent(Events.EventType.VictimOfCrime, criminal, victim, newCrime);
			Changed = true;
			crimes.Add(newCrime);
		}

		return crimes;
	}

	public bool WouldBeACrime(ICharacter criminal, CrimeTypes crime, ICharacter victim, IGameItem item,
		string additionalInformation)
	{
		if (!EnforcementZones.Contains(criminal.Location.Zone))
		{
			return false;
		}

		foreach (var law in _lawLookup[crime])
		{
			if (!law.CanBeAppliedAutomatically)
			{
				continue;
			}

			if (!law.IsCrime(criminal, victim, item))
			{
				continue;
			}

			return true;
		}

		return false;
	}

	public ILegalClass GetLegalClass(ICharacter character)
	{
		return _legalClasses.First(x => x.IsMemberOfClass(character));
	}

	public IEnforcementAuthority GetEnforcementAuthority(ICharacter character)
	{
		return _enforcementAuthorities.OrderByDescending(x => x.Priority)
									  .FirstOrDefault(x => x.HasAuthority(character));
	}

	public void AccuseCrime(ICrime crime)
	{
		_knownCrimes.Add(crime);
		_knownCrimesLookup.Add(crime.CriminalId, crime);
		Gameworld.Add(crime);
		Changed = true;
	}

	public void RemoveCrime(ICrime crime)
	{
		_knownCrimes.Remove(crime);
		_unknownCrimes.Remove(crime);
		_resolvedCrimes.Remove(crime);
		_staleCrimes.Remove(crime);
		_knownCrimesLookup[crime.CriminalId].Remove(crime);
		_unknownCrimesLookup[crime.CriminalId].Remove(crime);
		_resolvedCrimesLookup[crime.CriminalId].Remove(crime);

		foreach (var patrol in Patrols)
		{
			if (patrol.ActiveEnforcementCrime == crime)
			{
				patrol.InvalidateActiveCrime();
			}
		}
	}

	public void FinaliseCrime(ICrime crime)
	{
		_knownCrimes.Remove(crime);
		_knownCrimesLookup[crime.CriminalId].Remove(crime);
		_unknownCrimes.Remove(crime);
		_unknownCrimesLookup[crime.CriminalId].Remove(crime);
		_resolvedCrimes.Add(crime);
		_resolvedCrimesLookup.Add(crime.CriminalId, crime);
		_staleCrimes.Remove(crime);
		Changed = true;
	}

	public void ReportCrime(ICrime crime, ICharacter witness, bool identityKnown, double reliability)
	{
		// TODO - should there be any kind of delay on this for fairness?

		if (_resolvedCrimes.Contains(crime))
		{
			return;
		}

		if (identityKnown)
		{
			crime.CriminalIdentityIsKnown = true;
		}
		else
		{
			var failures = RandomUtilities.ConsecutiveRoll(1.0, 1.0 - reliability,
				crime.Criminal.CharacteristicDefinitions.Count());
			var flubbedDetails = RandomUtilities.Shuffle(crime.Criminal.CharacteristicDefinitions).Take(failures)
												.ToList();
			var actualDetails =
				crime.Criminal.CharacteristicDefinitions.Select(x => crime.Criminal.GetCharacteristic(x, witness));
			foreach (var detail in actualDetails)
			{
				if (!crime.CriminalCharacteristics.ContainsKey(detail.Definition) ||
					crime.CriminalCharacteristics[detail.Definition] != detail)
				{
					if (!crime.CriminalCharacteristics.ContainsKey(detail.Definition))
					{
						if (flubbedDetails.Contains(detail.Definition))
						{
							crime.SetCharacteristicValue(detail.Definition,
								Gameworld.CharacteristicValues.Where(x => x.Definition == detail.Definition)
										 .GetRandomElement());
							continue;
						}

						crime.SetCharacteristicValue(detail.Definition, detail);
						continue;
					}

					if (flubbedDetails.Contains(detail.Definition))
					{
						continue;
					}

					crime.SetCharacteristicValue(detail.Definition, detail);
				}
			}
		}

		Changed = true;

		if (_knownCrimes.Contains(crime))
		// Only solidify identity and/or correct reliability
		{
			return;
		}

		_knownCrimes.Add(crime);
		_unknownCrimes.Remove(crime);
		_staleCrimes.Remove(crime);
		crime.TimeOfReport = crime.CrimeLocation?.DateTime() ??
							 witness.Location?.DateTime() ?? EnforcementZones.First().DateTime();
		if (crime.AccuserId == null)
		{
			crime.AccuserId = witness?.Id;
		}

		_knownCrimesLookup.Add(crime.CriminalId, crime);
		_unknownCrimesLookup.Remove(crime.CriminalId, crime);
		crime.IsKnownCrime = true;
	}

	public IEnumerable<ICrime> KnownCrimesForIndividual(ICharacter character)
	{
		// TODO - compare characteristics
		return _knownCrimesLookup[character.Id];
	}

	public IEnumerable<ICrime> UnknownCrimesForIndividual(ICharacter character)
	{
		return _unknownCrimesLookup[character.Id];
	}

	public IEnumerable<ICrime> ResolvedCrimesForIndividual(ICharacter individual)
	{
		return _resolvedCrimesLookup[individual.Id];
	}
	#endregion

	#region Laws
	public void AddLaw(ILaw law)
	{
		_laws.Add(law);
		Changed = true;
		CalculateLawLookup();
	}

	public void RemoveLaw(ILaw law)
	{
		_laws.Remove(law);
		Changed = true;
		CalculateLawLookup();
	}
	#endregion

	#region Enforcement Authorities

	private readonly List<IEnforcementAuthority> _enforcementAuthorities = new();
	public IEnumerable<IEnforcementAuthority> EnforcementAuthorities => _enforcementAuthorities;

	public void AddEnforcementAuthority(IEnforcementAuthority authority)
	{
		_enforcementAuthorities.Add(authority);
		Changed = true;
	}

	public void RemoveEnforcementAuthority(IEnforcementAuthority authority)
	{
		_enforcementAuthorities.Remove(authority);
		Changed = true;
	}

	#endregion

	#region EnforcementZones

	private readonly List<IZone> _enforcementZones = new();
	public IEnumerable<IZone> EnforcementZones => _enforcementZones;

	public void AddEnforcementZone(IZone zone)
	{
		_enforcementZones.Add(zone);
		Changed = true;
	}

	public void RemoveEnforcementZone(IZone zone)
	{
		_enforcementZones.Remove(zone);
		Changed = true;
	}
	#endregion

	#region Legal Classes
	private readonly List<ILegalClass> _legalClasses = new();
	public IEnumerable<ILegalClass> LegalClasses => _legalClasses;

	public void AddLegalClass(ILegalClass item)
	{
		_legalClasses.Add(item);
		Changed = true;
	}

	public void RemoveLegalClass(ILegalClass item)
	{
		_legalClasses.Remove(item);
		Changed = true;
	}
	#endregion

	#region Patrol Routes
	private readonly List<IPatrolRoute> _patrolRoutes = new();
	public IEnumerable<IPatrolRoute> PatrolRoutes => _patrolRoutes;

	public void AddPatrolRoute(IPatrolRoute route)
	{
		_patrolRoutes.Add(route);
		Changed = true;
	}

	public void RemovePatrolRoute(IPatrolRoute route)
	{
		_patrolRoutes.Remove(route);
		Changed = true;
	}


	#endregion

	#region Location Properties
	public ICell PreparingLocation
	{
		get
		{
			return _preparingLocation;
		}

		set
		{
			if (_preparingLocation is not null)
			{
				_preparingLocation.CellProposedForDeletion -= PreparingLocation_CellProposedForDeletion;
			}
			_preparingLocation = value;
			if (_preparingLocation is not null)
			{
				_preparingLocation.CellProposedForDeletion -= PreparingLocation_CellProposedForDeletion;
				_preparingLocation.CellProposedForDeletion += PreparingLocation_CellProposedForDeletion;
			}
		}
	}

	private void PreparingLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a preparing location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell MarshallingLocation
	{
		get
		{
			return _marshallingLocation;
		}

		set
		{
			if (_marshallingLocation is not null)
			{
				_marshallingLocation.CellProposedForDeletion -= MarshallingLocation_CellProposedForDeletion;
			}
			_marshallingLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= MarshallingLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += MarshallingLocation_CellProposedForDeletion;
			}
		}
	}


	private void MarshallingLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a marshalling location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell EnforcerStowingLocation
	{
		get
		{
			return _enforcerStowingLocation;
		}

		set
		{
			if (_enforcerStowingLocation is not null)
			{
				_enforcerStowingLocation.CellProposedForDeletion -= EnforcerStowingLocation_CellProposedForDeletion;
			}
			_enforcerStowingLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= EnforcerStowingLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += EnforcerStowingLocation_CellProposedForDeletion;
			}
		}
	}


	private void EnforcerStowingLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is an enforcer stowing location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell PrisonLocation
	{
		get
		{
			return _prisonLocation;
		}

		set
		{
			if (_prisonLocation is not null)
			{
				_prisonLocation.CellProposedForDeletion -= PrisonLocation_CellProposedForDeletion;
			}
			_prisonLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= PrisonLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += PrisonLocation_CellProposedForDeletion;
			}
		}
	}


	private void PrisonLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a prison location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell PrisonReleaseLocation
	{
		get
		{
			return _prisonReleaseLocation;
		}

		set
		{
			if (_prisonReleaseLocation is not null)
			{
				_prisonReleaseLocation.CellProposedForDeletion -= PrisonReleaseLocation_CellProposedForDeletion;
			}
			_prisonReleaseLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= PrisonReleaseLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += PrisonReleaseLocation_CellProposedForDeletion;
			}
		}
	}


	private void PrisonReleaseLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a prison release location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell PrisonerBelongingsStorageLocation
	{
		get
		{
			return _prisonerBelongingsStorageLocation;
		}

		set
		{
			if (_prisonerBelongingsStorageLocation is not null)
			{
				_prisonerBelongingsStorageLocation.CellProposedForDeletion -= PrisonerBelongingsLocation_CellProposedForDeletion;
			}
			_prisonerBelongingsStorageLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= PrisonerBelongingsLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += PrisonerBelongingsLocation_CellProposedForDeletion;
			}
		}
	}


	private void PrisonerBelongingsLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a prison belongings location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell JailLocation
	{
		get
		{
			return _jailLocation;
		}

		set
		{
			if (_jailLocation is not null)
			{
				_jailLocation.CellProposedForDeletion -= JailLocation_CellProposedForDeletion;
			}
			_jailLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= JailLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += JailLocation_CellProposedForDeletion;
			}
		}
	}


	private void JailLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a jail location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public ICell CourtLocation
	{
		get
		{
			return _courtLocation;
		}

		set
		{
			if (_courtLocation is not null)
			{
				_courtLocation.CellProposedForDeletion -= CourtLocation_CellProposedForDeletion;
			}
			_courtLocation = value;
			if (value is not null)
			{
				value.CellProposedForDeletion -= CourtLocation_CellProposedForDeletion;
				value.CellProposedForDeletion += CourtLocation_CellProposedForDeletion;
			}
		}
	}

	private void CourtLocation_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That cell is a court location for patrols in Legal Authority #{Id:N0} ({Name.ColourName()})");
	}

	public IFutureProg OnPrisonerHeld { get; set; }
	public IFutureProg OnPrisonerImprisoned { get; set; }
	public IFutureProg OnPrisonerReleased { get; set; }
	public IFutureProg BailCalculationProg { get; set; }

	private readonly List<ICell> _cellLocations = new();
	public IEnumerable<ICell> CellLocations => _cellLocations;

	private readonly List<ICell> _jailLocations = new();
	public IEnumerable<ICell> JailLocations => _jailLocations;
	private ICell _preparingLocation;
	private ICell _marshallingLocation;
	private ICell _enforcerStowingLocation;
	private ICell _prisonLocation;
	private ICell _prisonReleaseLocation;
	private ICell _prisonerBelongingsStorageLocation;
	private ICell _jailLocation;
	private ICell _courtLocation;
	#endregion

	#region Patrols
	public IPatrolController PatrolController { get; }

	private readonly List<IPatrol> _patrols = new();
	

	public IEnumerable<IPatrol> Patrols => _patrols;

	public void AddPatrol(IPatrol patrol)
	{
		_patrols.Add(patrol);
	}

	public void RemovePatrol(IPatrol patrol)
	{
		_patrols.Remove(patrol);
	}
	#endregion

	#region Discord Notifications
	public void HandleDiscordNotification(ICrime crime)
	{
		if (DiscordChannelId is null)
		{
			return;
		}
		//Gameworld.DiscordConnection.NotifyEnforcement("crime", DiscordChannelId.Value, $"");
	}

	public void HandleDiscordNotificationOfEnforcement(ICrime crime, IPatrol patrol)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("enforcement", DiscordChannelId.Value,
			$"\"{Name}\" {crime.Criminal.Id} \"{crime.Criminal.PersonalName.GetName(NameStyle.FullName)}\" \"{patrol.PatrolRoute.Name}\" \"{crime.Name}\" \"{patrol.PatrolLeader.Location.GetFriendlyReference(null)}\" \"{crime.Law.EnforcementStrategy.DescribeEnum(true)}\"");
	}

	public void HandleDiscordNotificationOfConviction(ICharacter criminal, ICrime crime, PunishmentResult result,
		ICharacter enforcer)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("conviction", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\" \"{crime.Name}\" \"{result.Describe(null, this)}\"");
	}

	public void HandleDiscordNotificationOfForgiveness(ICrime crime, ICharacter enforcer)
	{
		if (DiscordChannelId is null)
		{
			return;
		}
		//Gameworld.DiscordConnection.NotifyEnforcement("crime", DiscordChannelId.Value, $"");
	}

	public void HandleDiscordNotificationOfBail(ICharacter criminal, string bailAmountText)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("crime", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\" \"{bailAmountText}\"");
	}

	public void HandleDiscordNotificationReturnFromBail(ICharacter criminal)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("returnfrombail", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\"");
	}

	public void HandleDiscordNotificationOfRelease(ICharacter criminal)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("release", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\"");
	}

	public void HandleDiscordNotificationOfImprisonment(ICharacter criminal, string imprisonmentLengthText)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("imprisonment", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\" \"{imprisonmentLengthText}\"");
	}

	private void HandleDiscordNotificationOfIncarceration(ICharacter criminal)
	{
		if (DiscordChannelId is null)
		{
			return;
		}

		Gameworld.DiscordConnection.NotifyEnforcement("incarceration", DiscordChannelId.Value,
			$"\"{Name}\" {criminal.Id} \"{criminal.PersonalName.GetName(NameStyle.FullName)}\"");
	}
	#endregion
}