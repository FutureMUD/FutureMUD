#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Events;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.Templates;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class StockStableMountFunction(IList<IFunction> parameters, IFuturemud world)
	: BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Item; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new NullVariable(ReturnType);
		var template = world.NpcTemplates.Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0m));
		var room = ParameterFunctions[1].Result?.GetObject as IRoom;
		var stable = world.Stables.FirstOrDefault(x => x.Location == room);
		var container = ParameterFunctions[2].Result?.GetObject as IGameItem;
		var owner = ParameterFunctions[3].Result?.GetObject as ICharacter;
		var debt = (decimal)(ParameterFunctions[4].Result?.GetObject ?? 0m);
		if (template is null || template.Status != RevisionStatus.Current || stable is null ||
			container is null || container.Deleted || container.Destroyed || container.GetItemType<IContainer>() is null || debt < 0m)
			return StatementResult.Normal;

		if (StableTicketGameItemComponentProto.ItemPrototype is null)
			StableTicketGameItemComponentProto.InitialiseItemType(world);
		var ticket = Stock(world, template, stable, container, owner, debt,
			() => StableTicketGameItemComponentProto.ItemPrototype!.CreateNew());
		if (ticket is not null) Result = ticket;
		return StatementResult.Normal;
	}

	internal static IGameItem? Stock(IFuturemud world, INPCTemplate template, IStable stable,
		IGameItem destination, ICharacter? owner, decimal debt, Func<IGameItem> createTicket)
	{
		if (template.Status != RevisionStatus.Current || debt < 0m || destination.Deleted || destination.Destroyed ||
			destination.GetItemType<IContainer>() is not { } container) return null;
		var item = createTicket();
		var ticket = item.GetItemType<IStableTicket>();
		if (ticket is null || ticket.StableStayId is not null)
		{
			item.Delete();
			return null;
		}

		// Authored stock uses the same direct container placement as forceput. Confirm
		// custody before creating an animal or a stay, so refused placement leaves neither.
		container.Put(null, item, allowMerge: false);
		if (!ReferenceEquals(item.ContainedIn, destination) || !container.Contents.Any(x => ReferenceEquals(x, item)))
		{
			item.Delete();
			return null;
		}

		var npc = template.CreateNewCharacter(CharacterInstanceService.CreateDefaultSpawnLocation(stable.Location, RoomLayer.GroundLevel));
		world.Add(npc, true);
		template.ApplyTemplateLoadAdditions(npc);
		template.OnLoadProg?.Execute(npc);
		var stay = stable.RegisterStay(npc, owner, debt);
		ticket.InitialiseTicket(stay);
		item.Login();
		item.HandleEvent(EventType.ItemFinishedLoading, item);
		return item;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"stockstablemount",
		[ProgVariableTypes.Number, ProgVariableTypes.Location, ProgVariableTypes.Item, ProgVariableTypes.Character, ProgVariableTypes.Number],
		(p, w) => new StockStableMountFunction(p, w),
		["template", "stableRoom", "container", "depositor", "openingDebt"],
		["Approved NPC template ID.", "Room containing the native stable.", "Container receiving the native ticket.",
			"Original depositor, or null for an unknown depositor.", "Nonnegative existing debt; no lodging payment is invented."],
		"Creates an NPC in native stable custody and a bound native ticket in the container. Returns the ticket, or null for invalid inputs or refused placement. Does not create a recurring spawn or assign ownership.",
		"Economy", ProgVariableTypes.Item));
}

internal sealed class StabledMountFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Character; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var ticket = (ParameterFunctions[0].Result?.GetObject as IGameItem)?.GetItemType<IStableTicket>();
		Result = ticket?.IsValid == true && ticket.StableStay?.Mount is { } mount
			? mount : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"stabledmount", [ProgVariableTypes.Item], (p, _) => new StabledMountFunction(p), ["ticket"],
		["A valid native bearer ticket."], "Returns the animal in the ticket's active native stay, or null for an invalid or closed ticket.",
		"Economy", ProgVariableTypes.Character));
}
