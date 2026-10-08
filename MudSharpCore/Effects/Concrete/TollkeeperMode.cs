#nullable enable annotations

using MudSharp.Construction;
using MudSharp.Construction.Boundary;

namespace MudSharp.Effects.Concrete;

public class TollkeeperMode : Effect, ITollkeeperModeEffect
{
	public TollkeeperMode(ICharacter owner, IRoomExit exit)
		: base(owner)
	{
		ExitId = exit.Exit.Id;
		GuardRoomId = exit.Origin.Id;
	}

	protected TollkeeperMode(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Element");
		ExitId = long.Parse(root.Element("ExitId")?.Value ?? "0");
		GuardRoomId = long.Parse(root.Element("GuardCellId")?.Value ?? "0");
	}

	public long ExitId { get; }
	public long GuardRoomId { get; }

	private IRoom? GuardRoom => Gameworld.Rooms.Get(GuardRoomId);

	public IRoomExit? Exit
	{
		get
		{
			var room = GuardRoom;
			return room is null ? null : Gameworld.ExitManager.GetExitByID(ExitId)?.RoomExitFor(room);
		}
	}

	protected override string SpecificEffectType => "TollkeeperMode";

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Element",
			new XElement("ExitId", ExitId),
			new XElement("GuardCellId", GuardRoomId)
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
		owner.RemoveAllEffects<IGuardExitEffect>(x => x.Exit?.Exit.Id == ExitId && x.Exit?.Origin.Id == GuardRoomId, true);
		owner.RemoveAllEffects<TollExitPermit>(x => x.ExitId == ExitId && x.GuardRoomId == GuardRoomId, true);
	}

	public override string ToString()
	{
		return "Tollkeeper Mode Effect";
	}
}
