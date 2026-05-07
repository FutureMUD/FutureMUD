#nullable enable annotations

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class TollkeeperMode : Effect, ITollkeeperModeEffect
{
	public TollkeeperMode(ICharacter owner, ICellExit exit)
		: base(owner)
	{
		ExitId = exit.Exit.Id;
		GuardCellId = exit.Origin.Id;
	}

	protected TollkeeperMode(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Element");
		ExitId = long.Parse(root.Element("ExitId")?.Value ?? "0");
		GuardCellId = long.Parse(root.Element("GuardCellId")?.Value ?? "0");
	}

	public long ExitId { get; }
	public long GuardCellId { get; }

	private ICell? GuardCell => Gameworld.Cells.Get(GuardCellId);

	public ICellExit? Exit
	{
		get
		{
			var cell = GuardCell;
			return cell is null ? null : Gameworld.ExitManager.GetExitByID(ExitId)?.CellExitFor(cell);
		}
	}

	protected override string SpecificEffectType => "TollkeeperMode";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Element",
			new XElement("ExitId", ExitId),
			new XElement("GuardCellId", GuardCellId)
		);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("TollkeeperMode", (effect, owner) => new TollkeeperMode(effect, owner));
	}

	public override string Describe(IPerceiver voyeur)
	{
		var exit = Exit;
		return exit is null
			? "Tollkeeper Mode for an unknown exit"
			: $"Tollkeeper Mode for the exit {exit.OutboundDirectionDescription}";
	}

	public override void RemovalEffect()
	{
		var owner = (ICharacter)Owner;
		owner.RemoveAllEffects<IGuardExitEffect>(x => x.Exit?.Exit.Id == ExitId && x.Exit?.Origin.Id == GuardCellId, true);
		owner.RemoveAllEffects<TollExitPermit>(x => x.ExitId == ExitId && x.GuardCellId == GuardCellId, true);
	}

	public override string ToString()
	{
		return "Tollkeeper Mode Effect";
	}
}
