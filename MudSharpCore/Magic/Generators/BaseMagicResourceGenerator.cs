using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Magic.Generators;

public abstract class BaseMagicResourceGenerator : FrameworkItem, IMagicResourceRegenerator
{
	private Dictionary<IHaveMagicResource, HeartbeatManagerDelegate> _delegates =
		new();

	protected BaseMagicResourceGenerator(Models.MagicGenerator generator)
	{
		_id = generator.Id;
		_name = generator.Name;
	}

	public static IMagicResourceRegenerator LoadFromDatabase(Models.MagicGenerator generator, IFuturemud gameworld)
	{
		switch (generator.Type)
		{
			case "linear":
				return new LinearTimeBasedGenerator(generator, gameworld);
			case "state":
				return new StateGenerator(generator, gameworld);
		}

		throw new ApplicationException("Invalid MagicGenerator type: " + generator.Type);
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "MagicResourceGenerator";

	#endregion

	#region Implementation of IMagicResourceRegenerator

	public HeartbeatManagerDelegate GetOnMinuteDelegate(IHaveMagicResource thing)
	{
		if (!_delegates.ContainsKey(thing))
		{
			_delegates[thing] = InternalGetOnMinuteDelegate(thing);
		}

		return _delegates[thing];
	}

	protected abstract HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing);

	public abstract IEnumerable<IMagicResource> GeneratedResources { get; }

	#endregion
}