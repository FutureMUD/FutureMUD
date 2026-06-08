#nullable enable

using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class MeasuringInstrumentGameItemComponent : GameItemComponent, IMeasuringInstrument, IConditionDegradingComponent
{
	private MeasuringInstrumentGameItemComponentProto _prototype;
	private double _calibrationBias;
	private bool _calibrationBiasIsPercentage;
	private bool _hasDeliberateBias;
	private int _usesSinceCalibration;
	private int _driftDirection;

	public override IGameItemComponentProto Prototype => _prototype;
	public bool ConditionDegradesOnUse => _prototype.ConditionMaintenance.ConditionDegradesOnUse;
	public int ItemQualityStages => _prototype.ConditionMaintenance.QualityPenaltyStages(Parent);
	public MeasuringInstrumentMode Mode => _prototype.Mode;
	public UnitType UnitType => _prototype.UnitType;
	public double Precision => _prototype.Precision;
	public double Capacity => _prototype.Capacity;
	public double CalibrationBias => _calibrationBias;
	public bool CalibrationBiasIsPercentage => _calibrationBiasIsPercentage;
	public bool HasDeliberateBias => _hasDeliberateBias;
	public int UsesSinceCalibration => _usesSinceCalibration;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MeasuringInstrumentGameItemComponentProto)newProto;
	}

	public void UseCondition(ItemConditionUseContext context)
	{
		_prototype.ConditionMaintenance.UseCondition(Parent, context);
	}

	public MeasuringInstrumentGameItemComponent(MeasuringInstrumentGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		_driftDirection = StableDriftDirection();
	}

	public MeasuringInstrumentGameItemComponent(MudSharp.Models.GameItemComponent component,
		MeasuringInstrumentGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MeasuringInstrumentGameItemComponent(MeasuringInstrumentGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_calibrationBias = rhs._calibrationBias;
		_calibrationBiasIsPercentage = rhs._calibrationBiasIsPercentage;
		_hasDeliberateBias = rhs._hasDeliberateBias;
		_usesSinceCalibration = rhs._usesSinceCalibration;
		_driftDirection = rhs._driftDirection;
	}

	private int StableDriftDirection()
	{
		var basis = Parent.Id != 0 ? Parent.Id : Parent.Prototype?.Id ?? _prototype.Id;
		return basis % 2 == 0 ? 1 : -1;
	}

	private void LoadFromXml(XElement root)
	{
		_calibrationBias = double.Parse(root.Element("CalibrationBias")?.Value ?? "0.0");
		_calibrationBiasIsPercentage = bool.Parse(root.Element("CalibrationBiasIsPercentage")?.Value ?? "false");
		_hasDeliberateBias = bool.Parse(root.Element("HasDeliberateBias")?.Value ?? "false");
		_usesSinceCalibration = int.Parse(root.Element("UsesSinceCalibration")?.Value ?? "0");
		_driftDirection = int.Parse(root.Element("DriftDirection")?.Value ?? "0");
		if (_driftDirection == 0)
		{
			_driftDirection = StableDriftDirection();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MeasuringInstrumentGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("CalibrationBias", _calibrationBias),
			new XElement("CalibrationBiasIsPercentage", _calibrationBiasIsPercentage),
			new XElement("HasDeliberateBias", _hasDeliberateBias),
			new XElement("UsesSinceCalibration", _usesSinceCalibration),
			new XElement("DriftDirection", _driftDirection)).ToString();
	}

	public bool CanMeasure(IGameItem target, out string error)
	{
		var value = TrueValue(target);
		if (value is null)
		{
			error = Mode switch
			{
				MeasuringInstrumentMode.Weight => "That instrument can only weigh physical items.",
				MeasuringInstrumentMode.FluidVolume => "That instrument can only measure the contents of liquid containers.",
				_ => "That instrument cannot measure anything in that mode."
			};
			return false;
		}

		if (value.Value > Capacity)
		{
			error = $"That is beyond the instrument's capacity of {Gameworld.UnitManager.DescribeExact(Capacity, UnitType, "metric")}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public MeasurementResult Measure(ICharacter actor, IGameItem target)
	{
		if (!CanMeasure(target, out var error))
		{
			throw new InvalidOperationException(error);
		}

		var trueValue = TrueValue(target)!.Value;
		_usesSinceCalibration++;
		var drift = CurrentDrift(trueValue);
		var bias = _calibrationBiasIsPercentage ? trueValue * _calibrationBias : _calibrationBias;
		var reported = RoundToPrecision(Math.Max(0.0, trueValue + drift + bias));
		UseCondition(new ItemConditionUseContext(ItemConditionUseKind.Measurement));
		Changed = true;
		return new MeasurementResult(Mode, UnitType, trueValue, reported, drift, bias, _usesSinceCalibration);
	}

	public void Calibrate(ICharacter actor)
	{
		_calibrationBias = 0.0;
		_calibrationBiasIsPercentage = false;
		_hasDeliberateBias = false;
		_usesSinceCalibration = 0;
		_driftDirection = StableDriftDirection();
		Changed = true;
	}

	public bool CalibrateWrong(ICharacter actor, double bias, bool percentageBias, out string error)
	{
		if (percentageBias && Math.Abs(bias) > _prototype.MaximumWrongCalibration)
		{
			error = $"That bias is too large. This instrument can only be wrong-calibrated by up to {_prototype.MaximumWrongCalibration.ToString("P2", actor)}.";
			return false;
		}

		if (!percentageBias && Math.Abs(bias) > Capacity * _prototype.MaximumWrongCalibration)
		{
			error = $"That bias is too large. This instrument can only be wrong-calibrated by up to {Gameworld.UnitManager.DescribeExact(Capacity * _prototype.MaximumWrongCalibration, UnitType, actor)}.";
			return false;
		}

		_calibrationBias = bias;
		_calibrationBiasIsPercentage = percentageBias;
		_hasDeliberateBias = true;
		_usesSinceCalibration = 0;
		_driftDirection = StableDriftDirection();
		Changed = true;
		error = string.Empty;
		return true;
	}

	public string InspectCalibration(ICharacter actor)
	{
		var passed = actor.IsAdministrator() ||
		             actor.Gameworld.GetCheck(CheckType.AppraiseItemCheck)
		                  .Check(actor, _prototype.CalibrationInspectionDifficulty, Parent)
		                  .IsPass();
		if (!passed)
		{
			return $"{Parent.HowSeen(actor, true)} is a {Mode.DescribeEnum().ToLowerInvariant()} measuring instrument, but you cannot determine its calibration.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{Parent.HowSeen(actor, true)} measures {Mode.DescribeEnum().ColourName()}.");
		sb.AppendLine($"Capacity: {Gameworld.UnitManager.DescribeExact(Capacity, UnitType, actor).ColourValue()}");
		sb.AppendLine($"Precision: {Gameworld.UnitManager.DescribeExact(Precision, UnitType, actor).ColourValue()}");
		sb.AppendLine($"Uses Since Calibration: {_usesSinceCalibration.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Stable Drift Direction: {(_driftDirection >= 0 ? "high" : "low").ColourValue()}");
		sb.AppendLine($"Estimated Drift At Capacity: {Gameworld.UnitManager.DescribeExact(CurrentDrift(Capacity), UnitType, actor).ColourValue()}");
		if (_hasDeliberateBias)
		{
			sb.AppendLine(_calibrationBiasIsPercentage
				? $"Deliberate Bias: {_calibrationBias.ToString("P2", actor).ColourError()}"
				: $"Deliberate Bias: {Gameworld.UnitManager.DescribeExact(_calibrationBias, UnitType, actor).ColourError()}");
		}
		else
		{
			sb.AppendLine("Deliberate Bias: None".ColourValue());
		}

		return sb.ToString();
	}

	private double? TrueValue(IGameItem target)
	{
		return Mode switch
		{
			MeasuringInstrumentMode.Weight => target.Weight,
			MeasuringInstrumentMode.FluidVolume => target.GetItemType<ILiquidContainer>()?.LiquidVolume,
			_ => null
		};
	}

	private double CurrentDrift(double trueValue)
	{
		var qualityStepsBelowLegendary = (int)ItemQuality.Legendary - (int)Parent.Quality;
		var qualityMultiplier = Math.Clamp(1.0 + qualityStepsBelowLegendary * 0.2, 0.25, 4.0);
		var driftPercent = Math.Min(_prototype.MaximumDrift, _usesSinceCalibration * _prototype.BaseDriftPerUse * qualityMultiplier);
		return trueValue * driftPercent * _driftDirection;
	}

	private double RoundToPrecision(double value)
	{
		if (Precision <= 0.0)
		{
			return value;
		}

		return Math.Round(value / Precision, MidpointRounding.AwayFromZero) * Precision;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Full => $"{description}\n\nIt is marked for measuring {Mode.DescribeEnum().ToLowerInvariant()}.",
			DescriptionType.Evaluate when voyeur is ICharacter ch => InspectCalibration(ch),
			_ => description
		};
	}
}
