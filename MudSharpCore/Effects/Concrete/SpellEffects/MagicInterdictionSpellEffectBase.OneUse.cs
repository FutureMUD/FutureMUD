#nullable enable
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract partial class MagicInterdictionSpellEffectBase
{
	private bool _removed;
	public bool OneUse { get; }
	public bool Used { get; private set; }
	public bool UsesAreLimited => OneUse;
	public IMagicInterdictionUse? CaptureUse(MagicInterdictionContext context)
	{
		if (!OneUse || Used || _removed || ParentEffect is null) return null;
		var use = new WardUse(this, context);
		return use.IsCurrent ? use : null;
	}

	public override void RemovalEffect()
	{
		_removed = true;
		base.RemovalEffect();
	}

	private sealed class WardUse : IMagicInterdictionUse
	{
		private readonly MagicInterdictionSpellEffectBase _ward;
		private readonly IMagicSpellEffectParent _parent;
		private readonly IPerceivable _owner;
		private readonly ICharacter _source;
		private readonly IBody _sourceBody;
		private readonly ICharacter? _target;
		private readonly IBody? _targetBody;
		private readonly ICharacter? _caster;
		private readonly IBody? _casterBody;
		private readonly IBody? _ownerBody;
		private readonly long _sourceInstance;
		private readonly long? _targetInstance;
		private readonly long? _casterInstance;
		private readonly long? _ownerInstance;
		private readonly IMagicSpell _spell;
		private readonly ILocation? _sourceLocation;
		private readonly ILocation? _targetLocation;
		private readonly ILocation? _casterLocation;
		private readonly ILocation? _ownerLocation;
		private bool _committed;
		public WardUse(MagicInterdictionSpellEffectBase ward, MagicInterdictionContext context)
		{
			_ward = ward; _parent = ward.ParentEffect; _owner = ward.Owner;
			_source = context.Source; _sourceBody = _source.Body;
			_target = context.Target as ICharacter; _targetBody = _target?.Body;
			_caster = _parent.Caster; _casterBody = _caster?.Body;
			_ownerBody = (_owner as ICharacter)?.Body;
			_sourceInstance = _source.InstanceId; _targetInstance = _target?.InstanceId;
			_casterInstance = _caster?.InstanceId; _ownerInstance = (_owner as ICharacter)?.InstanceId;
			_spell = _parent.Spell;
			_sourceLocation = _source.Location; _targetLocation = _target?.Location;
			_casterLocation = _caster?.Location; _ownerLocation = _owner.Location;
		}
		public bool IsCurrent => !_ward._removed && ReferenceEquals(_ward.Owner, _owner) &&
			ReferenceEquals(_ward.ParentEffect, _parent) && ReferenceEquals(_parent.Owner, _owner) &&
			ReferenceEquals(_parent.Spell, _spell) && _source.InstanceId == _sourceInstance &&
			_target?.InstanceId == _targetInstance && _caster?.InstanceId == _casterInstance &&
			(_owner as ICharacter)?.InstanceId == _ownerInstance &&
			ReferenceEquals(_source.Location, _sourceLocation) && ReferenceEquals(_target?.Location, _targetLocation) &&
			ReferenceEquals(_caster?.Location, _casterLocation) && ReferenceEquals(_owner.Location, _ownerLocation) &&
			ReferenceEquals(_source.Body, _sourceBody) && ReferenceEquals(_target?.Body, _targetBody) &&
			ReferenceEquals(_parent.Caster, _caster) && ReferenceEquals(_caster?.Body, _casterBody) &&
			ReferenceEquals((_owner as ICharacter)?.Body, _ownerBody) &&
			_owner.Effects.Any(x => ReferenceEquals(x, _ward)) &&
			_owner.Effects.Any(x => ReferenceEquals(x, _parent)) &&
			_parent.SpellEffects.Any(x => ReferenceEquals(x, _ward));
		public bool TryCommit()
		{
			if (_committed || _ward.Used || !IsCurrent) return false;
			_ward.Used = true; _committed = true;
			_owner.EffectsChanged = true;
			if (!IsCurrent) throw new InvalidOperationException("Ward authority changed after its use was persisted; retain payment and do not replay.");
			return true;
		}
	}
}
