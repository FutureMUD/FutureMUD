using System.Collections.Generic;

namespace MudSharp.Work.Crafts;

public sealed record CraftResourceReservation(
	long CraftId,
	int RevisionNumber,
	string CraftName,
	int FromPhase,
	int ToPhase,
	IReadOnlyCollection<CraftInputReservation> Inputs,
	IReadOnlyCollection<CraftToolReservation> Tools);

public sealed record CraftInputReservation(
	long InputId,
	string InputName,
	string InputType,
	long PerceivableId,
	string PerceivableType,
	string PerceivableDescription,
	int ConsumedPhase,
	IReadOnlyCollection<long> ItemIds);

public sealed record CraftToolReservation(
	long ToolId,
	string ToolName,
	string ToolType,
	long ItemId,
	string ItemDescription,
	int Phase);
