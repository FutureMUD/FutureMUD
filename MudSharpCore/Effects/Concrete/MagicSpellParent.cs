using MudSharp.Magic;
using MudSharp.NPC;
using MudSharp.RPG.Checks;
using MudSharp.Magic.Vancian;
using MudSharp.Magic.Environment;

namespace MudSharp.Effects.Concrete;

public class MagicSpellParent : Effect, IMagicSpellEffectParent
{
    private bool _removingSpellEffects;
	private bool _loaded;
	public Guid Identity { get; private set; } = Guid.NewGuid();
	public TimeSpan ResolvedDuration { get; init; }

    public static void InitialiseEffectType()
    {
        RegisterFactory("MagicSpellParent", (effect, owner) => new MagicSpellParent(effect, owner));
    }

    public MagicSpellParent(IPerceivable owner, IMagicSpell spell, ICharacter caster,
        SpellPower power = SpellPower.Standard, OpposedOutcomeDegree outcome = OpposedOutcomeDegree.None) : base(owner, null)
    {
        Spell = spell;
        _caster = caster;
        _casterId = caster?.Id ?? 0;
        _casterInstanceId = caster?.InstanceId;
        Power = power;
        Outcome = outcome;
    }

    protected MagicSpellParent(XElement root, IPerceivable owner) : base(root, owner)
    {
		_loaded = true;
        XElement trueRoot = root.Element("Effect");
		Identity = Guid.TryParse(trueRoot.Element("Identity")?.Value, out var identity) ? identity : Guid.Empty;
        Spell = trueRoot.Element("StoredSpell") is { } snapshot
            ? StoredSpellSnapshot.Load(snapshot).CreateSpell(Gameworld, false)
            : Gameworld.MagicSpells.Get(long.Parse(trueRoot.Element("Spell").Value));
        _casterId = long.Parse(trueRoot.Element("Caster").Value);
        _casterInstanceId = (long?)trueRoot.Element("CasterInstance");
        Power = (SpellPower)int.Parse(trueRoot.Element("SpellPower")?.Value ?? ((int)SpellPower.Standard).ToString());
        Outcome = (OpposedOutcomeDegree)int.Parse(trueRoot.Element("OutcomeDegree")?.Value ?? ((int)OpposedOutcomeDegree.None).ToString());
        foreach (XElement element in trueRoot.Element("Children").Elements())
        {
            IMagicSpellEffect child = (IMagicSpellEffect)LoadEffect(element, owner);
            child.ParentEffect = this;
            _spellEffects.Add(child);
            owner.AddEffect(child);
        }
    }

    #region Overrides of Effect

    public override string Describe(IPerceiver voyeur)
    {
        return Spell is null ? "Orphaned spell parent (missing source spell)." : $"Affected by the {Spell.Name.Colour(Spell.School.PowerListColour)} spell.";
    }

	public override void InitialEffect()
	{
		if (!_loaded) foreach (var treatment in _spellEffects.OfType<ILandRejuvenationEffect>().ToArray()) treatment.ActivateTreatment();
	}

	public override void Login()
	{
		foreach (var treatment in _spellEffects.OfType<ILandRejuvenationEffect>().ToArray()) treatment.ActivateTreatment();
	}

	public override void ExpireEffect()
	{
		foreach (var treatment in _spellEffects.OfType<ILandRejuvenationEffect>().ToArray()) treatment.ExpireTreatment();
		base.ExpireEffect();
	}

    protected override string SpecificEffectType => "MagicSpellParent";

    public override bool SavingEffect => true;

    protected override XElement SaveDefinition()
    {
		foreach (var treatment in _spellEffects.OfType<ILandRejuvenationEffect>().ToArray()) treatment.CheckpointTreatment();
        return new XElement("Effect",
			new XElement("Identity", Identity),
            new XElement("Spell", Spell?.Id ?? 0),
            new XElement("Caster", _casterId),
            _casterInstanceId.HasValue ? new XElement("CasterInstance", _casterInstanceId.Value) : null,
            (Spell as MagicSpell)?.StoredSnapshot?.Save(),
            new XElement("SpellPower", (int)Power),
            new XElement("OutcomeDegree", (int)Outcome),
            new XElement("Children",
                from child in _spellEffects.ToArray()
                select child.SaveToXml(new Dictionary<IEffect, TimeSpan>())
            )
        );
    }

    public override void RemovalEffect()
    {
        _removingSpellEffects = true;
        foreach (IMagicSpellEffect effect in _spellEffects.ToList())
        {
            Owner.RemoveEffect(effect, true);
        }
        _spellEffects.Clear();
        _removingSpellEffects = false;
    }

    #endregion

    #region Implementation of IMagicSpellEffectParent

    public IMagicSpell Spell { get; set; }
    public SpellPower Power { get; private set; }
    public OpposedOutcomeDegree Outcome { get; private set; }

    private long _casterId { get; set; }
    private long? _casterInstanceId;
    private ICharacter _caster;

    public ICharacter Caster
    {
        get
        {
            if (_casterId == 0) return null;
            if (_caster == null)
            {
                _caster = Gameworld.TryGetCharacter(_casterId, true);
                if (_casterInstanceId is > 0 && _caster?.Identity is { } identity)
                    _caster = identity.Instances.FirstOrDefault(x => x.InstanceId == _casterInstanceId.Value);
                if (_caster is null) return null;
                if (!Gameworld.Actors.Has(_caster))
                {
                    Gameworld.Add(_caster, _caster is INPC);
                }
            }

            return _caster;
        }
    }

    private readonly List<IMagicSpellEffect> _spellEffects = new();
    protected void RemoveOwnedChild(IMagicSpellEffect effect) => _spellEffects.Remove(effect);

    public void AddSpellEffect(IMagicSpellEffect effect)
    {
        _spellEffects.Add(effect);
    }

    public virtual void RemoveSpellEffect(IMagicSpellEffect effect)
    {
        _spellEffects.Remove(effect);
        if (!_removingSpellEffects && !_spellEffects.Any())
        {
            Owner.RemoveEffect(this);
        }
    }

    public IEnumerable<IMagicSpellEffect> SpellEffects => _spellEffects;

    #endregion
}
