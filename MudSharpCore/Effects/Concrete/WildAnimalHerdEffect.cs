using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class WildAnimalHerdEffect : Effect, IEffectSubtype
{
	private WildAnimalHerdRole _role;
	private WildAnimalHerdState _state;
	private WildAnimalHerdPriority _priority;
	private readonly List<ICell> _avoidedLocations = new();
	private readonly List<ICell> _knownWater = new();

	public WildAnimalHerdRole Role
	{
		get => _role;
		set
		{
			_role = value;
			Changed = true;
		}
	}

	public WildAnimalHerdState State
	{
		get => HerdLeaderEffect?.State ?? _state;
		set
		{
			_state = value;
			Changed = true;
			LastStateChange = DateTime.UtcNow;
		}
	}

	public WildAnimalHerdPriority Priority
	{
		get => HerdLeaderEffect?.Priority ?? _priority;
		set
		{
			_priority = value;
			Changed = true;
		}
	}

	public DateTime LastStateChange { get; set; } = DateTime.MinValue;

	public DateTime EntryReactionCooldown { get; set; } = DateTime.MinValue;

	public List<ICell> AvoidedLocations => HerdLeaderEffect?.AvoidedLocations ?? _avoidedLocations;

	public List<ICell> KnownWater => HerdLeaderEffect?.KnownWater ?? _knownWater;

	public ICharacter HerdLeader { get; set; }

	public WildAnimalHerdEffect HerdLeaderEffect { get; set; }

	public WildAnimalHerdEffect HerdLeaderEffectOrSelf => HerdLeaderEffect ?? this;

	public List<WildAnimalHerdEffect> SubordinateEffects { get; } = new();

	public List<ICellExit> DesignatedExits { get; } = new();
	public DateTime LastAlphaMinuteTick { get; set; } = DateTime.MinValue;

	public List<(INPC Animal, WildAnimalHerdRole Role)> GetHerdMembers()
	{
		if (HerdLeaderEffect != null)
		{
			return HerdLeaderEffect.GetHerdMembers();
		}

		var list = new List<(INPC Animal, WildAnimalHerdRole Role)>();
		list.Add(((INPC)Owner, WildAnimalHerdRole.Alpha));
		list.AddRange(SubordinateEffects.Select(x => ((INPC)x.Owner, x.Role)));
		return list;
	}

	public override bool Applies(object target)
	{
		if (target is WildAnimalHerdRole role)
		{
			return Role == role;
		}

		return base.Applies(target);
	}

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("WildAnimalHerdEffect", (effect, owner) => new WildAnimalHerdEffect(effect, owner));
	}

	#endregion

	#region Constructors

	public WildAnimalHerdEffect(ICharacter owner, ICharacter leader, WildAnimalHerdEffect leadereffect) : base(owner,
		null)
	{
		HerdLeader = leader;
		HerdLeaderEffect = leadereffect;
		HerdLeaderEffect?.SubordinateEffects.Add(this);
	}

	protected WildAnimalHerdEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromXML(effect.Element("Effect"));
	}

	#endregion

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XElement("Role", (int)Role), new XElement("State", (int)State),
			new XElement("Priority", (int)Priority));
	}

	protected void LoadFromXML(XElement root)
	{
		_role = (WildAnimalHerdRole)int.Parse(root.Element("Role").Value);
		_state = (WildAnimalHerdState)int.Parse(root.Element("State").Value);
		_priority = (WildAnimalHerdPriority)int.Parse(root.Element("Priority").Value);
		LastStateChange = DateTime.UtcNow;
		if (_role == WildAnimalHerdRole.Alpha)
		{
			HerdLeader = (ICharacter)Owner;
			HerdLeaderEffect = this;
		}
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "WildAnimalHerdEffect";

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"WildAnimalHerdAI Role [{Role.ToString().Colour(Telnet.Cyan)}] state [{State.ToString().Colour(Telnet.Cyan)}] priority [{Priority.ToString().Colour(Telnet.Cyan)}].";
	}

	public override bool SavingEffect => Role == WildAnimalHerdRole.Alpha;

	public override void RemovalEffect()
	{
		HerdLeaderEffect?.SubordinateEffects.Remove(this);
	}

	#endregion
}