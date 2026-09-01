#nullable enable

using MudSharp.Body;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public interface IImplantMachinePrototypeSettings
{
	bool External { get; }
	string ExternalDescription { get; }
	IBodyPrototype TargetBody { get; }
	IBodypart TargetBodypart { get; }
	double ImplantSpaceOccupied { get; }
	Difficulty InstallDifficulty { get; }
	double ImplantDamageFunctionGrace { get; }
}

public sealed class ImplantMachineRuntime
{
	private readonly IGameItem _parent;
	private readonly Func<IImplantMachinePrototypeSettings> _settings;
	private readonly Action _changed;
	private IBodypart? _overriddenBodypart;

	public ImplantMachineRuntime(IGameItem parent, Func<IImplantMachinePrototypeSettings> settings, Action changed)
	{
		_parent = parent;
		_settings = settings;
		_changed = changed;
	}

	public IBody? InstalledBody { get; private set; }
	public IBodypart TargetBodypart
	{
		get => _overriddenBodypart ?? _settings().TargetBodypart;
		set
		{
			_overriddenBodypart = value;
			_changed();
		}
	}

	public void Load(XElement root)
	{
		if (long.TryParse(root.Element("OverridenBodypart")?.Value, out var id) && id > 0)
		{
			_overriddenBodypart = _parent.Gameworld.BodypartPrototypes.Get(id);
		}
	}

	public void Save(XElement root)
	{
		root.Add(new XElement("OverridenBodypart", _overriddenBodypart?.Id ?? 0L));
	}

	public void Install(IBody body)
	{
		InstalledBody = body;
		_changed();
	}

	public void Remove()
	{
		InstalledBody = null;
		_overriddenBodypart = null;
		_changed();
	}

	public double FunctionFactor(bool powered)
	{
		if (!powered)
		{
			return 0.0;
		}
		var health = _parent.HealthStrategy.CurrentHealthPercentage(_parent);
		var grace = _settings().ImplantDamageFunctionGrace;
		return health >= 1.0 - grace ? 1.0 : health * (1.0 - grace);
	}
}
