using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

public class ThermalImbalance : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("ThermalImbalance", (effect, owner) => new ThermalImbalance(effect, owner));
	}

	public ThermalImbalance(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		ImbalanceProgress = 0.0;
		TemperatureStatus = BodyTemperatureStatus.NormalTemperature;
		((ICharacter)owner).StartHealthTick();
	}

	protected ThermalImbalance(XElement root, IPerceivable owner) : base(root, owner)
	{
		root = root.Element("Effect");
		ImbalanceProgress = double.Parse(root.Element("ImbalanceProgress").Value);
		TemperatureStatus = (BodyTemperatureStatus)int.Parse(root.Element("TemperatureStatus").Value);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Suffering from {TemperatureStatus.DescribeColour()} and {ImbalanceProgress} progress";
	}

	private double _imbalanceProgress;
	private BodyTemperatureStatus _temperatureStatus;

	public double ImbalanceProgress
	{
		get => _imbalanceProgress;
		set
		{
			_imbalanceProgress = value;
			Changed = true;
		}
	}

	public BodyTemperatureStatus TemperatureStatus
	{
		get => _temperatureStatus;
		set
		{
			_temperatureStatus = value;
			Changed = true;
		}
	}

	public void TenSecondProgress(Temperature currentExposure)
	{
		var decayFloor = BodyTemperatureStatus.NormalTemperature;
		var changeMultiplier = 1.0;
		switch (currentExposure)
		{
			case Temperature.AbysmallyHot:
				changeMultiplier = 20.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.Torrid:
				changeMultiplier = 15.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.Sweltering:
				changeMultiplier = 10.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.ExtremelyHot:
				changeMultiplier = 7.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.VeryHot:
				changeMultiplier = 5.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.Hot:
				changeMultiplier = 4.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.VeryWarm:
				changeMultiplier = 3.0;
				decayFloor = BodyTemperatureStatus.CriticalHyperthermia;
				break;
			case Temperature.Warm:
				// Decay towards Very Mildly Hyperthermic
				changeMultiplier = 2.0;
				decayFloor = BodyTemperatureStatus.VeryMildHyperthermia;
				break;
			case Temperature.Temperate:
				// Decay towards Normal
				break;
			case Temperature.Cool:
				changeMultiplier = 1.5;
				decayFloor = BodyTemperatureStatus.VeryMildHypothermia;
				// Decay towards Very Mildly Hypothermic
				break;
			case Temperature.Chilly:
				changeMultiplier = 2.25;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.Cold:
				changeMultiplier = 3.0;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.VeryCold:
				changeMultiplier = 3.75;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.ExtremelyCold:
				changeMultiplier = 5;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.Frigid:
				changeMultiplier = 7.0;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.Freezing:
				changeMultiplier = 10.0;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
			case Temperature.AbysmallyCold:
				changeMultiplier = 15.0;
				decayFloor = BodyTemperatureStatus.CriticalHypothermia;
				break;
		}

		if (TemperatureStatus.In(BodyTemperatureStatus.NormalTemperature,
			    BodyTemperatureStatus.VeryMildHyperthermia, BodyTemperatureStatus.VeryMildHypothermia))
		{
			if (TemperatureStatus > BodyTemperatureStatus.NormalTemperature)
			{
				ImbalanceProgress -= Gameworld.GetStaticDouble("TemperatureImbalanceBasePerTenSeconds") *
				                     changeMultiplier;
			}
			else
			{
				ImbalanceProgress += Gameworld.GetStaticDouble("TemperatureImbalanceBasePerTenSeconds") *
				                     changeMultiplier;
			}
		}
		else if (TemperatureStatus > decayFloor)
		{
			ImbalanceProgress -= Gameworld.GetStaticDouble("TemperatureImbalanceBasePerTenSeconds") * changeMultiplier;
		}
		else
		{
			ImbalanceProgress += Gameworld.GetStaticDouble("TemperatureImbalanceBasePerTenSeconds") *
			                     changeMultiplier;
		}

		if (TemperatureStatus < BodyTemperatureStatus.CriticalHyperthermia && ImbalanceProgress >=
		    Gameworld.GetStaticDouble("TemperatureImbalanceHotterThreshold"))
		{
			TemperatureStatus += 1;
			ImbalanceProgress = 0.0;
			DoHeatUpMessage();
		}
		else if (TemperatureStatus > BodyTemperatureStatus.CriticalHypothermia &&
		         ImbalanceProgress <= Gameworld.GetStaticDouble("TemperatureImbalanceColderThreshold"))
		{
			TemperatureStatus -= 1;
			ImbalanceProgress = 0.0;
			DoCoolDownMessage();
		}

		if (Math.Abs(ImbalanceProgress) < Gameworld.GetStaticDouble("TemperatureImbalanceBasePerTenSeconds") &&
		    currentExposure == Temperature.Temperate)
		{
			RemovalEffect();
		}
	}

	public void DoHeatUpMessage()
	{
		if (TemperatureStatus == BodyTemperatureStatus.NormalTemperature)
		{
			Owner.OutputHandler.Send("You are no longer too cold.");
			return;
		}

		if (TemperatureStatus < BodyTemperatureStatus.NormalTemperature)
		{
			Owner.OutputHandler.Send($"You feel as if you are only {TemperatureStatus.DescribeAdjectiveColour()}.");
			return;
		}

		Owner.OutputHandler.Send($"You feel as if you are becoming {TemperatureStatus.DescribeAdjectiveColour()}.");
	}

	public void DoCoolDownMessage()
	{
		if (TemperatureStatus == BodyTemperatureStatus.NormalTemperature)
		{
			Owner.OutputHandler.Send("You are no longer too hot.");
			return;
		}

		if (TemperatureStatus > BodyTemperatureStatus.NormalTemperature)
		{
			Owner.OutputHandler.Send($"You feel as if you are only {TemperatureStatus.DescribeAdjectiveColour()}.");
			return;
		}

		Owner.OutputHandler.Send($"You feel as if you are becoming {TemperatureStatus.DescribeAdjectiveColour()}.");
	}

	protected override string SpecificEffectType => "ThermalImbalance";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("TemperatureStatus", (int)TemperatureStatus),
			new XElement("ImbalanceProgress", ImbalanceProgress)
		);
	}
}