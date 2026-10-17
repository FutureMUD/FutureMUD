#nullable enable

using System.Globalization;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic.Casting;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Independent local injury and bounded nearby fall paths, including an empty local injury set.</summary>
public sealed partial class GroundTremorEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public GroundTremorEffect(XElement definition, IMagicSpell spell) { _definition = new(definition); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new GroundTremorEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes == "room";
	private long Binding(string name) => (long?)_definition.Element(name) ?? 0;
	public string? DefinitionError => (string?)_definition.Attribute("version") != "1" ||
		Gameworld.MagicSpells.Get(Binding("StoneSkinSpell")) is null || Gameworld.Traits.Get(Binding("AgilityTrait")) is not IAttributeDefinition ||
		new[] { "ShadowPlane", "WindPlane", "WaterPlane", "VoidPlane" }.Any(x => Gameworld.Planes.Get(Binding(x)) is null) ||
		Gameworld.Tags.Get(Binding("SiltTag")) is null ? "Ground Tremor requires valid typed spell, agility, plane and Silt bindings." : null;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("groundtremor", (xml, spell) => new GroundTremorEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("groundtremor", (_, spell) =>
			(new GroundTremorEffect(new XElement("Effect", new XAttribute("type", "groundtremor"), new XAttribute("version", 1)), spell), ""),
			"Local earthquake injury and separate nearby falls", "Set one of the displayed native bindings by ID.", true, true, ["room"]);
	}
	private bool Protected(ICharacter caster, ICharacter actor, bool local) => actor.IsAdministrator() || actor.RoomLayer != RoomLayer.GroundLevel ||
		actor.PositionState is PositionFlying or PositionFloatingInZeroGravity or PositionFloatingInWater or PositionSwimming or PositionClimbing or PositionHanging ||
		actor.State.IsDead() || actor.GetPlanarPresence().SuspendsPhysicalContact ||
		local && actor.GetPlanarPresence().PresencePlaneIds.Contains(Binding("ShadowPlane")) ||
		actor.Effects.OfType<IMagicSpellEffectParent>().Any(x => x.Spell.Id == Binding("StoneSkinSpell") && x.SpellEffects.Any()) ||
		MagicInterdictionHelper.GetInterdiction(caster, actor, Spell.School, false) is not null;
	private bool RoomAllowed(ICharacter caster, IRoom room, bool local)
	{
		if (room.Id <= 0 || !ReferenceEquals(Gameworld.Rooms.Get(room.Id), room) || room.RouteDefinition is not null ||
			!room.Terrain(caster).TerrainLayers.Contains(RoomLayer.GroundLevel) ||
			!caster.CanInteractPlanar(room, PlanarInteractionKind.Magic) ||
			MagicInterdictionHelper.GetInterdiction(caster, room, Spell.School, false) is not null) return false;
		var planes = room.GetPlanarPresence().PresencePlaneIds;
		return !planes.Contains(Binding("WindPlane")) && (local ||
			!planes.Contains(Binding("WaterPlane")) && !planes.Contains(Binding("VoidPlane")) && !room.Terrain(caster).IsA(Gameworld.Tags.Get(Binding("SiltTag"))));
	}
	private sealed record Person(ICharacter Actor, IBody Body, ICharacterIdentity? Identity, long Instance,
		long Id, long BodyId, CharacterState State, IPositionState Position, IRoom Room, RoomLayer Layer, bool Local, bool Protected)
	{
		public bool Current => Actor.Id == Id && Body.Id == BodyId && Actor.State == State && ReferenceEquals(Actor.PositionState, Position) &&
			ReferenceEquals(Actor.Body, Body) && ReferenceEquals(Body.Actor, Actor) &&
			ReferenceEquals(Actor.Identity, Identity) && Actor.InstanceId == Instance &&
			ReferenceEquals(Actor.Location, Room) && Actor.RoomLayer == Layer && CommandExecutionAuthority.IsCurrent(Actor, false);
	}
	private sealed record Place(IRoom Room, IRoomOverlay Overlay, ITerrain Terrain, ICharacter[] Actors, long[] Exits)
	{
		public bool Current => ReferenceEquals(Room.CurrentOverlay, Overlay) && ReferenceEquals(Overlay.Terrain, Terrain) &&
			ReferenceEquals(Room.Gameworld.Rooms.Get(Room.Id), Room) && Overlay.ExitIDs.SequenceEqual(Exits) &&
			Room.Characters.SequenceEqual(Actors, ReferenceEqualityComparer.Instance);
	}
	private sealed record Selection(ICharacter Caster, IRoom Origin, int Grade, Person[] People, Place[] Places,
		Dictionary<ICharacter, DamageEffect> Damage, IMagicSpellEffectPreparedSelectionToken[] DamageTokens, Func<bool> Check)
		: IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool Attempted { get; set; }
		public bool IsCurrent => !Attempted && Check() && Places.All(x => x.Current) && People.All(x => x.Current) &&
			DamageTokens.All(x => x is IMagicSpellEffectPreparedSelectionRawToken { IsCurrent: true });
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var prior)) return prior;
		if (DefinitionError is { } error) throw new InvalidOperationException(error);
		if (recipient is not IRoom origin || !ReferenceEquals(caster.Location, origin) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native || !RoomAllowed(caster, origin, true))
			throw new InvalidOperationException("Ground Tremor requires the caster's admitted ground room and selected grade.");
		var grade = native.InvocationGrade.Value;
		var queue = new Queue<(IRoom Room, int Depth)>(); queue.Enqueue((origin, 0));
		var seen = new HashSet<IRoom>(ReferenceEqualityComparer.Instance) { origin };
		var places = new List<Place>(); var people = new List<Person>();
		var edgeChecks = new List<Func<bool>>();
		var damage = new Dictionary<ICharacter, DamageEffect>(ReferenceEqualityComparer.Instance);
		var damageTokens = new List<IMagicSpellEffectPreparedSelectionToken>();
		while (queue.TryDequeue(out var step))
		{
			var actors = step.Room.Characters.Take(257).ToArray();
			if (actors.Length + people.Count > 256) throw new InvalidOperationException("Ground Tremor exceeds its 256-body bound before payment.");
			places.Add(new(step.Room, step.Room.CurrentOverlay, step.Room.CurrentOverlay.Terrain, actors, step.Room.CurrentOverlay.ExitIDs.ToArray()));
			foreach (var actor in actors.Distinct<ICharacter>(ReferenceEqualityComparer.Instance))
			{
				if (actor.Body is not { Id: > 0 } body || !ReferenceEquals(body.Actor, actor) ||
					!caster.CanInteractPlanar(actor, PlanarInteractionKind.Magic)) continue;
				var local = ReferenceEquals(step.Room, origin);
				var person = new Person(actor, body, actor.Identity, actor.InstanceId, actor.Id, body.Id, actor.State,
					actor.PositionState, step.Room, actor.RoomLayer, local, Protected(caster, actor, local));
				people.Add(person);
				if (!local || person.Protected) continue;
				var amount = grade * (ReferenceEquals(actor, caster) ? 1 : 3);
				var template = new DamageEffect(new XElement("Effect", new XAttribute("type", "damage"),
					new XElement("DamageType", (int)DamageType.Crushing), new XElement("DamageExpression", amount.ToString(CultureInfo.InvariantCulture)),
					new XElement("PainExpression", "0"), new XElement("StunExpression", "0"), new XElement("Bodypart", 0), new XElement("Limb", -1),
					new XElement("IdentitySafeDamage", new XAttribute("schema", 1), new XAttribute("version", 1))), Spell)
					{ AdmitsGroundedRoomDamage = true };
				var selectedPower = native.GradeProfile!.Grades.Single(x => x.Grade == grade).Power;
				template.DamageExpression = CastingNumerics.Bind(template.DamageExpression, native.CastingTrait, grade, selectedPower, "groundtremor/damage", Gameworld);
				template.PainExpression = CastingNumerics.Bind(template.PainExpression!, native.CastingTrait, grade, selectedPower, "groundtremor/pain", Gameworld);
				template.StunExpression = CastingNumerics.Bind(template.StunExpression!, native.CastingTrait, grade, selectedPower, "groundtremor/stun", Gameworld);
				damage.Add(actor, template); damageTokens.Add(template.CapturePreparedSelection(caster, actor));
			}
			if (step.Depth >= grade - 1) continue;
			var exits = step.Room.ExitsFor(caster, true).Take(513).ToArray();
			if (exits.Length > 512) throw new InvalidOperationException("Ground Tremor exceeds its per-room native exit bound.");
			foreach (var exit in exits)
			{
				var from = exit.Origin; var to = exit.Destination; var nativeExit = exit.Exit;
				edgeChecks.Add(() => ReferenceEquals(exit.Origin, from) && ReferenceEquals(exit.Destination, to) && ReferenceEquals(exit.Exit, nativeExit));
				if (!ReferenceEquals(exit.Origin, step.Room) || seen.Contains(exit.Destination) || !RoomAllowed(caster, exit.Destination, false)) continue;
				if (seen.Count >= 512) throw new InvalidOperationException("Ground Tremor exceeds its 512-room bound before payment.");
				seen.Add(exit.Destination); queue.Enqueue((exit.Destination, step.Depth + 1));
			}
		}
		var casterBody = caster.Body; var casterIdentity = caster.Identity; var instance = caster.InstanceId;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var policyCurrent = CaptureRawPolicy(places.Select(x => x.Room), people.Select(x => x.Actor).Append(caster));
		var result = new Selection(caster, origin, grade, people.ToArray(), places.ToArray(), damage, damageTokens.ToArray(),
			() => ReferenceEquals(caster.Location, origin) && ReferenceEquals(caster.Body, casterBody) &&
				ReferenceEquals(caster.Identity, casterIdentity) && caster.InstanceId == instance && native.InvocationGrade == grade &&
				SaveToXml().ToString(SaveOptions.DisableFormatting) == definition && policyCurrent() && edgeChecks.All(x => x()));
		if (!result.IsCurrent) throw new InvalidOperationException("Ground Tremor's room, bodies or protections changed during selection.");
		_selections.Add(recipient, result); return result;
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "Ground Tremor's captured room, bodies, protections or grade changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Origin, recipient) || !token.IsCurrent) return false;
		if (!token.Places.All(x => RoomAllowed(caster, x.Room, ReferenceEquals(x.Room, token.Origin))) ||
			!token.People.All(x => Protected(caster, x.Actor, x.Local) == x.Protected) || !token.IsCurrent) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "Ground Tremor's prepared selection is unavailable or changed.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (target is null || !_selections.TryGetValue(target, out var selection) || !selection.IsCurrent)
			return new(MagicEffectOperationStatus.Rejected, null);
		selection.Attempted = true; var applied = false;
		foreach (var person in selection.People.Where(x => x.Local && !x.Protected))
		{
			if (!person.Current || Protected(caster, person.Actor, true) || !person.Current || !CommandExecutionScope.TryContinue())
				throw new InvalidOperationException("A local earthquake recipient changed before harm.");
			applied |= selection.Damage[person.Actor].Apply(caster, person.Actor, outcome, power, parent, parameters).Status == MagicEffectOperationStatus.Applied;
		}
		foreach (var place in selection.Places.Where(x => !ReferenceEquals(x.Room, selection.Origin)))
		{
			if (!RoomAllowed(caster, place.Room, false)) continue;
			foreach (var person in selection.People.Where(x => !x.Local && ReferenceEquals(x.Room, place.Room)))
			{
				if (!person.Current || !CommandExecutionScope.TryContinue()) continue;
				person.Actor.OutputHandler.Send("The ground shakes violently beneath your feet.");
				if (!person.Current || Protected(caster, person.Actor, false) || !person.Current) continue;
				var check = Gameworld.GetCheck(CheckType.GenericAttributeCheck).Check(person.Actor, Difficulty.Normal,
					Gameworld.Traits.Get(Binding("AgilityTrait")), caster);
				if (check.IsPass() || !person.Current || Protected(caster, person.Actor, false) || !person.Current || !CommandExecutionScope.TryContinue()) continue;
				person.Actor.SetPosition(PositionSprawled.Instance, PositionModifier.None, null, null);
				person.Actor.OutputHandler.Send("You stumble and fall to the ground."); applied = true;
			}
		}
		return new(applied ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters) => Apply(caster, target, outcome, power, parent, parameters).Effect;
	public string Show(ICharacter actor) => "Local Crushing damage: 3*grade, caster grade; independent nearby Agility falls through grade-1 edges. " + SaveToXml();
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = _definition.Elements().FirstOrDefault(x => x.Name.LocalName.EqualTo(command.PeekSpeech()));
		if (field is null) return false;
		command.PopSpeech(); if (!long.TryParse(command.PopSpeech(), out var value) || value <= 0 || !command.IsFinished) return false;
		field.Value = value.ToString(CultureInfo.InvariantCulture); Spell.Changed = true;
		actor.OutputHandler.Send("The native earthquake binding has been updated."); return true;
	}
}
