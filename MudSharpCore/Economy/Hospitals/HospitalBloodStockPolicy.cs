using System;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using DbHospitalBloodStockPolicy = MudSharp.Models.HospitalBloodStockPolicy;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public class HospitalBloodStockPolicy : SaveableItem, IHospitalBloodStockPolicy
{
	private double _targetLitres;
	private decimal _pricePerLitre;

	public HospitalBloodStockPolicy(IHospital hospital, IBloodtype bloodtype)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		Bloodtype = bloodtype;
		_name = $"{hospital.Name} {bloodtype.Name} blood stock";
		_targetLitres = 0.0;
		_pricePerLitre = 0.0M;

		using (new FMDB())
		{
			var dbitem = new DbHospitalBloodStockPolicy
			{
				HospitalId = hospital.Id,
				BloodtypeId = bloodtype.Id,
				TargetLitres = 0.0,
				PricePerLitre = 0.0M
			};
			FMDB.Context.HospitalBloodStockPolicies.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public HospitalBloodStockPolicy(DbHospitalBloodStockPolicy policy, IHospital hospital)
	{
		Gameworld = hospital.Gameworld;
		Hospital = hospital;
		_id = policy.Id;
		Bloodtype = Gameworld.Bloodtypes.Get(policy.BloodtypeId)!;
		_name = $"{hospital.Name} {Bloodtype.Name} blood stock";
		_targetLitres = Math.Max(0.0, policy.TargetLitres);
		_pricePerLitre = Math.Max(0.0M, policy.PricePerLitre);
	}

	public override string FrameworkItemType => "HospitalBloodStockPolicy";
	public IHospital Hospital { get; }
	public IBloodtype Bloodtype { get; }

	public double TargetLitres
	{
		get => _targetLitres;
		set
		{
			_targetLitres = Math.Max(0.0, value);
			Changed = true;
		}
	}

	public decimal PricePerLitre
	{
		get => _pricePerLitre;
		set
		{
			_pricePerLitre = Math.Max(0.0M, value);
			Changed = true;
		}
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.HospitalBloodStockPolicies.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.TargetLitres = TargetLitres;
		dbitem.PricePerLitre = PricePerLitre;
		Changed = false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Blood Stock Policy #{Id.ToString("N0", actor)} - {Bloodtype.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Hospital: {Hospital.Name.ColourName()}");
		sb.AppendLine($"Target Stock: {TargetLitres.ToString("N2", actor).ColourValue()}L");
		sb.AppendLine($"Donation Price: {Hospital.Currency.Describe(PricePerLitre, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} per litre");
		return sb.ToString();
	}
}