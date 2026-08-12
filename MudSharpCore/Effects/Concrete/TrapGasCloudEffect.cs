#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Health;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// A temporary, localised cloud produced by a trap. It deliberately does not rewrite the cell atmosphere:
/// existing atmospheric simulations continue to own the world's bulk gas while this effect supplies a bounded
/// inhalation hazard on one room layer.
/// </summary>
public sealed class TrapGasCloudEffect : Effect
{
	private bool _subscribed;

	public static void InitialiseEffectType()
	{
		RegisterFactory("TrapGasCloud", (effect, owner) => new TrapGasCloudEffect(effect, owner));
	}

	public TrapGasCloudEffect(ICell owner, IGas gas, double dosePerTick, RoomLayer layer, string echo)
		: base(owner)
	{
		GasId = gas.Id;
		DosePerTick = Math.Max(0.0, dosePerTick);
		Layer = layer;
		Echo = echo;
	}

	private TrapGasCloudEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		GasId = long.Parse(effect.Element("GasId")!.Value);
		DosePerTick = double.Parse(effect.Element("DosePerTick")?.Value ?? "0");
		Layer = (RoomLayer)int.Parse(effect.Element("Layer")?.Value ?? "0");
		Echo = effect.Element("Echo")?.Value ?? string.Empty;
	}

	public long GasId { get; }
	public double DosePerTick { get; }
	public RoomLayer Layer { get; }
	public string Echo { get; }
	public IGas? Gas => Gameworld.Gases.Get(GasId);

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "TrapGasCloud";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("GasId", GasId),
			new XElement("DosePerTick", DosePerTick),
			new XElement("Layer", (int)Layer),
			new XElement("Echo", new XCData(Echo)));
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"A temporary cloud of {Gas?.Name.ColourValue() ?? "an unknown gas".ColourError()} on the {Layer.DescribeEnum().ColourValue()} layer.";
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		Subscribe();
		if (Owner is ICell cell && !string.IsNullOrWhiteSpace(Echo))
		{
			foreach (ICharacter character in cell.LayerCharacters(Layer))
			{
				character.OutputHandler.Send(Echo);
			}
		}
	}

	public override void Login()
	{
		base.Login();
		Subscribe();
	}

	public override void RemovalEffect()
	{
		Unsubscribe();
		base.RemovalEffect();
	}

	private void Subscribe()
	{
		if (_subscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += ApplyDose;
		_subscribed = true;
	}

	private void Unsubscribe()
	{
		if (!_subscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= ApplyDose;
		_subscribed = false;
	}

	private void ApplyDose()
	{
		if (Owner is not ICell cell || !CanDose(Gas, DosePerTick))
		{
			return;
		}

		var drug = Gas!.Drug;

		foreach (ICharacter character in cell.LayerCharacters(Layer).Where(x => x.NeedsToBreathe && x.CanBreathe))
		{
			character.Body.Dose(drug, DrugVector.Inhaled, DosePerTick, this);
		}
	}

	/// <summary>
	/// Determines whether a cloud can deliver its configured drug through inhalation. Non-inhalable drugs must not
	/// be silently dosed using an incompatible vector.
	/// </summary>
	internal static bool CanDose(IGas? gas, double dosePerTick)
	{
		return gas?.Drug is { } drug && dosePerTick > 0.0 && drug.DrugVectors.HasFlag(DrugVector.Inhaled);
	}
}
