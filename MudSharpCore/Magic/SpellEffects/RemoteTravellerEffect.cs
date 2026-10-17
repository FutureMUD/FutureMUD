#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Magic.SpellTriggers;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Shared guarded displacement mechanics; the two concrete source routes retain distinct movement and range rules.</summary>
public abstract class RemoteTravellerEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	protected RemoteTravellerEffect(XElement xml, IMagicSpell spell, bool draw, string type)
	{ _definition = new(xml); Spell = spell; DrawsTraveller = draw; Type = type; }
	protected bool DrawsTraveller { get; }
	protected string Type { get; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	private ITag? Restricted => Gameworld.Tags.Get((long?)_definition.Element("RestrictedRoomTag") ?? 0);
	public string? DefinitionError => (string?)_definition.Attribute("type") != Type || (string?)_definition.Attribute("version") != "1" ||
		Restricted is null ? "Remote travel needs its explicit current room restriction tag." : null;
	public XElement SaveToXml() => new(_definition);
	public abstract IMagicSpellEffectTemplate Clone();
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerRecognisedCharacter;
	protected static XElement BuilderDefinition(string type) => new("Effect", new XAttribute("type", type), new XAttribute("version", 1), new XElement("RestrictedRoomTag", 0));
	private sealed record Selection(ICharacter Caster, ICharacter Target, ICharacter Mover, IRoom Destination, RoomLayer DestinationLayer,
		int Grade, IReadOnlyList<IRoomExit> Path, SpellTravelSnapshot Physical, RecognisedIdentitySnapshot Recognition,
		RecognisedIdentitySnapshot MovementRecognition, string Definition, Func<bool> Raw) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Physical.AtCapturedPositions && Recognition.Current && Raw();
	}
	private string? LiveError(ICharacter caster, ICharacter target, ICharacter mover, IRoom destination, RoomLayer layer,
		IReadOnlyList<IRoomExit> path)
	{
		if (caster.Combat is not null || caster.PositionState?.Id is not (1 or 9 or 10 or 11 or 13 or 14 or 15 or 16 or 17 or 18 or 19 or 20))
			return "The caster must use the native minimum-standing posture and be out of combat.";
		if (!CastingTriggerRecognisedCharacter.IsRecognised(caster, target) || target.IsAdministrator()) return "The exact recognised traveller is no longer eligible.";
		if (ReferenceEquals(mover.Location, destination) && mover.RoomLayer == layer) return "The traveller is already at that exact position.";
		if (!caster.SharesPlaneWith(target) || !caster.Location.SharesPlaneWith(target.Location)) return "Both travellers must be on the same plane.";
		if (target.Location.OutdoorsType(target) != RoomOutdoorsType.Outdoors || Restricted is not { } restricted ||
			caster.Location.IsA(restricted) || target.Location.IsA(restricted)) return "An outdoor or configured room restriction prevents this route.";
		if (path.Any(edge => !edge.Origin.ExitsFor(caster).Any(x => ReferenceEquals(x.Exit, edge.Exit) && ReferenceEquals(x.Destination, edge.Destination)) ||
			!SpellPermittedPath.Admitted(caster, edge))) return "The bounded source route is no longer permitted.";
		return SpellTravelEndpointPolicy.RelocationError(caster, mover, mover.Location, destination, Spell, mover.RoomLayer, layer);
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{ if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error); return existing; }
		if (DefinitionError is { } bad) throw new InvalidOperationException(bad);
		if (recipient is not ICharacter target || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("Remote travel needs one exact recognised traveller and the selected grade.");
		var grade = native.InvocationGrade.Value; var mover = DrawsTraveller ? target : caster;
		var destination = DrawsTraveller ? caster.Location : target.Location; var layer = DrawsTraveller ? caster.RoomLayer : target.RoomLayer;
		var endpoints = new SpellTravelSnapshot(Gameworld, [caster, target], [caster.Location, target.Location]);
		var recognition = new RecognisedIdentitySnapshot(caster, target);
		var refusal = LiveError(caster, target, mover, destination, layer, []);
		if (refusal is not null || !endpoints.AtCapturedPositions || !recognition.Current)
			throw new InvalidOperationException(refusal ?? "The remote identity or endpoints changed during admission.");
		var path = grade == 7 ? Array.Empty<IRoomExit>() : SpellPermittedPath.Find(caster, caster.Location, target.Location,
			20 * grade - (DrawsTraveller ? 0 : 1)) ?? throw new InvalidOperationException("No permitted native path meets this source route's grade bound.");
		var physical = new SpellTravelSnapshot(Gameworld, [caster, target], path.SelectMany(x => new[] { x.Origin, x.Destination }).Concat([caster.Location, target.Location]));
		var pathChecks = SpellPermittedPath.CaptureRawPermissions(caster, path);
		var checks = new List<Func<bool>>();
		foreach (var edge in path)
		{
			var exit = edge.Exit; var door = exit.Door; var source = edge.Origin; var end = edge.Destination;
			var ids = string.Join(",", source.CurrentOverlay.ExitIDs); var isOpen = door?.IsOpen;
			checks.Add(() => ReferenceEquals(edge.Exit, exit) && ReferenceEquals(exit.Door, door) && door?.IsOpen == isOpen &&
				ReferenceEquals(edge.Origin, source) && ReferenceEquals(edge.Destination, end) && string.Join(",", source.CurrentOverlay.ExitIDs) == ids);
		}
		var restricted = Restricted; var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		bool Raw() => endpoints.AtCapturedPositions && pathChecks() && checks.All(x => x()) && ReferenceEquals(Restricted, restricted) &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition;
		var token = new Selection(caster, target, mover, destination, layer, grade, path, physical, recognition,
			new RecognisedIdentitySnapshot(caster, target, false), definition, Raw);
		if (!token.IsCurrent || LiveError(caster, target, mover, destination, layer, path) is not null || !token.IsCurrent)
			throw new InvalidOperationException("The selected remote route changed during preparation.");
		_selections.Add(recipient, token); return token;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The frozen remote traveller, route, or endpoints changed.";
		if (selection is not Selection token || !ReferenceEquals(caster, token.Caster) || !ReferenceEquals(recipient, token.Target) ||
			Spell is not MagicSpell native || native.InvocationGrade != token.Grade || SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition ||
			!token.IsCurrent || LiveError(caster, token.Target, token.Mover, token.Destination, token.DestinationLayer, token.Path) is not null || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{ error = "This remote route was not prepared."; return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error); }
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !TryConfirmPreparedSelection(caster, target, out _)) return new(MagicEffectOperationStatus.Rejected, null);
		var token = _selections[target]; token.Attempted = true;
		var traveller = token.Physical.Travellers.Single(x => ReferenceEquals(x.Actor, token.Mover));
		var others = token.Physical.Travellers.Where(x => !ReferenceEquals(x.Actor, token.Mover)).ToArray();
		bool Raw() => token.Physical.Current && token.MovementRecognition.Current && others.All(x => x.At(x.Origin, x.Layer));
		bool Current() => Raw() && SpellTravelEndpointPolicy.InFlightError(caster, token.Mover, traveller.Origin, token.Destination,
			Spell, traveller.Layer, token.DestinationLayer) is null && Raw();
		try
		{
			using var displacement = CommandExecutionScope.EnterSoloDisplacement(token.Mover,
				Current);
			token.Mover.Teleport(token.Destination, token.DestinationLayer, false, true);
			if (Current() && traveller.At(token.Destination, token.DestinationLayer))
				return new(MagicEffectOperationStatus.Applied, null);
		}
		catch { /* Inspect exact arrival and independently compensate only this captured body. */ }
		if (traveller.Current && ReferenceEquals(Gameworld.Rooms.Get(traveller.Origin.Id), traveller.Origin) &&
			(ReferenceEquals(token.Mover.Location, traveller.Origin) || ReferenceEquals(token.Mover.Location, token.Destination)))
		{
			try
			{
				using var independent = CommandExecutionScope.EnterIndependent();
				using var repair = CommandExecutionScope.EnterSoloDisplacement(token.Mover, () => traveller.Current);
				token.Mover.Teleport(traveller.Origin, traveller.Layer, false, false);
			}
			catch { /* Exact native membership determines whether compensation succeeded. */ }
		}
		if (!traveller.At(traveller.Origin, traveller.Layer) || !others.All(x => x.At(x.Origin, x.Layer)))
			throw new InvalidOperationException("The exact remote traveller could not be restored; reconcile the paid operation without replay.");
		return new(MagicEffectOperationStatus.NoChange, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => DrawsTraveller ? "Draw one recognised traveller here, after native resistance and source route checks." : "Join one recognised traveller after source route checks.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("restricted") || Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument) is not { } tag) return false;
		_definition.SetElementValue("RestrictedRoomTag", tag.Id); Spell.Changed = true;
		actor.OutputHandler.Send("The remote route restriction is now " + tag.Name.ColourName() + "."); return true;
	}
}

public sealed class JoinTravellerEffect(XElement xml, IMagicSpell spell) : RemoteTravellerEffect(xml, spell, false, "jointraveller")
{
	public override IMagicSpellEffectTemplate Clone() => new JoinTravellerEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("jointraveller", (xml, spell) => new JoinTravellerEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("jointraveller", (_, spell) => (new JoinTravellerEffect(BuilderDefinition("jointraveller"), spell), ""),
			"Moves the caster to one exact recognised remote traveller", "restricted <tag>", true, true, ["recognisedcharacter"]);
	}
}

public sealed class DrawTravellerEffect(XElement xml, IMagicSpell spell) : RemoteTravellerEffect(xml, spell, true, "drawtraveller")
{
	public override IMagicSpellEffectTemplate Clone() => new DrawTravellerEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("drawtraveller", (xml, spell) => new DrawTravellerEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("drawtraveller", (_, spell) => (new DrawTravellerEffect(BuilderDefinition("drawtraveller"), spell), ""),
			"Draws one resisted recognised remote traveller to the caster", "restricted <tag>", true, true, ["recognisedcharacter"]);
	}
}
