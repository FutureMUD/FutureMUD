#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp.GameItems.Components;

/// <summary>One-shot S3 blast output. Its SpellOwnedItemOrigin is the persisted invocation/creator/lifetime binding.</summary>
public sealed class ArmageddonEmberSeedGameItemComponent : GameItemComponent, IApply, IArmageddonEmberSeedComponent
{
	private ArmageddonEmberSeedGameItemComponentProto _prototype;
	private ArmageddonEmberSeedBinding? _binding;
	public override IGameItemComponentProto Prototype => _prototype;
	public bool Spent { get; private set; }

	public ArmageddonEmberSeedBinding? Binding
	{
		get
		{
			var origin = Parent.SpellCreationOrigin;
			return _binding is { IsValid: true } binding && origin is { } source &&
			       source.LifecycleId == binding.LifecycleId && source.Mode == binding.LifecycleMode &&
			       source.CreatorId == binding.CreatorId && source.SpellId == binding.SpellId && source.Grade == binding.Grade &&
			       source.DeadlineUtc == binding.ExpiresAtUtc
				? binding : null;
		}
	}

	public ArmageddonEmberSeedGameItemComponent(ArmageddonEmberSeedGameItemComponentProto prototype,
		IGameItem parent, bool temporary = false) : base(parent, prototype, temporary)
	{
		_prototype = prototype;
	}

	public ArmageddonEmberSeedGameItemComponent(MudSharp.Models.GameItemComponent component,
		ArmageddonEmberSeedGameItemComponentProto prototype, IGameItem parent) : base(component, parent)
	{
		_prototype = prototype;
		_noSave = true;
		LoadSpentFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private ArmageddonEmberSeedGameItemComponent(ArmageddonEmberSeedGameItemComponent rhs,
		IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Spent = true;
		_binding = null;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false) =>
		new ArmageddonEmberSeedGameItemComponent(this, newParent, temporary);

	public void Bind(SpellLifecycleOrigin origin, ICharacter creator, double damagePerGrade)
	{
		var binding = new ArmageddonEmberSeedBinding("arm.item.ember_seed", origin.Id, origin.SpellId, origin.Mode,
			origin.Grade, origin.CreatorId, creator.InstanceId, origin.DeadlineUtc ?? DateTime.MinValue, damagePerGrade);
		if (_binding is not null || Spent || !binding.IsValid || MudSharp.Character.CharacterInstanceIdentityComparer.IdentityId(creator) != origin.CreatorId ||
		    creator.Gameworld != Gameworld || origin.Mode != SpellLifecycleMode.TemporaryCleanup || origin.DeadlineUtc is null)
			throw new InvalidOperationException("Ember Seed binding must match its unique committed spell-created item origin.");
		_binding = binding;
		Changed = true;
	}

	public WhyCannotApply CanApply(IBody target, IBodypart part)
	{
		var binding = Binding;
		return binding?.CanActivate(Spent, RuntimeClock.UtcNow) == true && Parent.SpellCreationOrigin is { } origin &&
		       Gameworld.SpellOwnedItems?.HasActiveOrigin(Parent, origin) == true && target.Actor.Location is not null &&
		       target.Bodyparts.Contains(part)
			? WhyCannotApply.CanApply
			: WhyCannotApply.CannotApplyEmpty;
	}

	public void Apply(IBody target, IBodypart part, ICharacter applier) => Apply(target, part, 0.0, applier);

	public void Apply(IBody target, IBodypart part, double amount, ICharacter applier)
	{
		var binding = Binding;
		if (binding?.CanActivate(Spent, RuntimeClock.UtcNow) != true || target.Actor.Location is not { } location ||
		    !target.Bodyparts.Contains(part) || applier is null || !ReferenceEquals(target.Gameworld, Gameworld) ||
		    !applier.ColocatedWith(target.Actor) || !ItemManipulationGuard.CanManipulate(applier, out _, Parent) ||
		    Parent.SpellCreationOrigin is not { } origin || Gameworld.SpellOwnedItems?.HasActiveOrigin(Parent, origin) != true)
		{
			applier?.Send("This Ember Seed has no live spell binding, has expired, or has already detonated.");
			return;
		}
		var creator = Gameworld.Actors.FirstOrDefault(x => CharacterInstanceIdentityComparer.IdentityId(x) == binding.CreatorId &&
			x.InstanceId == binding.CreatorInstanceId && !x.State.IsDead() && !x.State.IsInStatis());
		if (creator is null)
		{
			applier?.Send("The Ember Seed's exact creator is no longer available; it cannot detonate safely.");
			return;
		}

		var layer = target.Actor.RoomLayer;
		var detonationLocation = target.Actor.SpatialLocation;
		var creatorBody = creator.Body;
		var targets = location.Characters
			.Where(x => ReferenceEquals(x.Location, location) && x.RoomLayer == layer && target.Actor.ColocatedWith(x) &&
			            !x.State.HasFlag(CharacterState.Dead) && !x.State.HasFlag(CharacterState.Stasis))
			.Select(x => (Character: x, Body: x.Body, Spatial: x.SpatialLocation, Bodypart: x.Body.Bodyparts.FirstOrDefault(y => y is IExternalBodypart)))
			.Where(x => x.Bodypart is not null)
			.ToArray();

		// Persist one-shot consumption before any damage. An interruption can lose the blast, never replay it.
		Spent = true;
		Changed = true;
		if (Gameworld.SpellOwnedItems?.TryCommitComponentConsumption(this, origin) != true)
		{
			applier.Send("The Ember Seed's consumption could not be confirmed; it remains inert until its saved state is reloaded.");
			return;
		}

		foreach (var (character, body, spatial, bodypart) in targets)
		{
			if (!creator.CanSee(character) || !creator.CanEngage(character) ||
				!ReferenceEquals(creator.Body, creatorBody) || creator.InstanceId != binding.CreatorInstanceId ||
				CharacterInstanceIdentityComparer.IdentityId(creator) != binding.CreatorId || creator.State.IsDead() || creator.State.IsInStatis() ||
				!Gameworld.Actors.Any(x => ReferenceEquals(x, creator)) || target.Actor.SpatialLocation != detonationLocation ||
				character.SpatialLocation != spatial || !ReferenceEquals(character.Body, body) ||
				character.State.IsDead() || character.State.IsInStatis() || !body.Bodyparts.Contains(bodypart) ||
				Gameworld.SpellOwnedItems?.HasActiveOrigin(Parent, origin) != true) continue;
			character.SufferDamage(new Damage
			{
				DamageType = DamageType.Burning,
				DamageAmount = binding.DamagePerEligibleTarget,
				Bodypart = bodypart,
				ActorOrigin = creator,
				ToolOrigin = Parent
			});
		}
		Parent.Delete();
	}

	private void LoadSpentFromXml(XElement root)
	{
		Spent = bool.TryParse(root.Attribute("Spent")?.Value, out var spent) && spent;
		if (Guid.TryParse((string?)root.Attribute("LifecycleId"), out var invocation) &&
		    Enum.TryParse((string?)root.Attribute("Mode"), out SpellLifecycleMode mode) &&
		    int.TryParse((string?)root.Attribute("Grade"), out var grade) &&
			long.TryParse((string?)root.Attribute("CreatorId"), out var creator) &&
			long.TryParse((string?)root.Attribute("CreatorInstanceId"), out var creatorInstance) &&
			long.TryParse((string?)root.Attribute("SpellId"), out var spellId) &&
			double.TryParse((string?)root.Attribute("DamagePerGrade"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var damagePerGrade) &&
		    DateTime.TryParse((string?)root.Attribute("ExpiresUtc"), null, System.Globalization.DateTimeStyles.RoundtripKind, out var expires))
		{
			var binding = new ArmageddonEmberSeedBinding("arm.item.ember_seed", invocation, spellId, mode, grade, creator, creatorInstance, expires, damagePerGrade);
			_binding = binding.IsValid ? binding : null;
		}
	}

	protected override string SaveToXml() => new XElement("Definition", new XAttribute("Spent", Spent),
		_binding is null ? null : new XAttribute("LifecycleId", _binding.LifecycleId),
		_binding is null ? null : new XAttribute("SpellId", _binding.SpellId),
		_binding is null ? null : new XAttribute("Mode", _binding.LifecycleMode),
		_binding is null ? null : new XAttribute("Grade", _binding.Grade),
		_binding is null ? null : new XAttribute("CreatorId", _binding.CreatorId),
		_binding is null ? null : new XAttribute("CreatorInstanceId", _binding.CreatorInstanceId),
		_binding is null ? null : new XAttribute("DamagePerGrade", _binding.DamagePerGrade),
		_binding is null ? null : new XAttribute("ExpiresUtc", _binding.ExpiresAtUtc.ToString("O"))).ToString();

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ArmageddonEmberSeedGameItemComponentProto)newProto;
	}
}
