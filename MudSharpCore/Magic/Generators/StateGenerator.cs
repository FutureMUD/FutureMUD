using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Generators;

public class StateGenerator : BaseMagicResourceGenerator
{
	private List<(IFutureProg StateProg, IEnumerable<(IMagicResource Resource, double Amount)> Resources)> _states =
		new();

	public StateGenerator(Models.MagicGenerator generator, IFuturemud gameworld) : base(generator)
	{
		var root = XElement.Parse(generator.Definition);

		var states = root.Element("States");
		if (states == null)
		{
			throw new ApplicationException($"StateGenerator #{Id} ({Name}) is missing a States element.");
		}

		foreach (var sub in states.Elements())
		{
			var resources = new List<(IMagicResource Resource, double Amount)>();
			foreach (var element in sub.Elements("Resource"))
			{
				var whichResource = long.TryParse(element.Attribute("resource")?.Value ?? "0", out var value)
					? gameworld.MagicResources.Get(value)
					: gameworld.MagicResources.GetByName(element.Attribute("resource")?.Value ?? "0");
				if (whichResource == null)
				{
					throw new ApplicationException(
						$"StateGenerator #{Id} ({Name}) specified an incorrect magic resource.");
				}

				var apm = element.Attribute("amountperminute")?.Value ?? "";

				if (!double.TryParse(apm, out var dvalue))
				{
					throw new ApplicationException(
						$"StateGenerator #{Id} ({Name}) specified an amountperminute amount that wasn't a number.");
				}

				resources.Add((whichResource, dvalue));
			}

			var progElement = sub.Element("StateProg");
			if (progElement == null)
			{
				throw new ApplicationException($"StateGenerator #{Id} ({Name}) did not have a StateProg element.");
			}

			var whichProg = long.TryParse(progElement.Value, out var pvalue)
				? gameworld.FutureProgs.Get(pvalue)
				: gameworld.FutureProgs.GetByName(progElement.Value);
			if (whichProg == null)
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified an incorrect StateProg element.");
			}

			if (!whichProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified a StateProg that doesn't return a boolean.");
			}

			if (!whichProg.MatchesParameters(new[] { FutureProgVariableTypes.MagicResourceHaver }))
			{
				throw new ApplicationException(
					$"StateGenerator #{Id} ({Name}) specified a StateProg that doesn't accept a MagicResourceHaver argument.");
			}

			_states.Add((whichProg, resources));
		}
	}

	protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing)
	{
		return () =>
		{
			foreach (var state in _states)
			{
				if (state.StateProg.Execute<bool?>(thing) == true)
				{
					foreach (var (resource, amount) in state.Resources)
					{
						thing.AddResource(resource, amount);
					}

					return;
				}
			}
		};
	}

	#region Overrides of BaseMagicResourceGenerator

	/// <inheritdoc />
	public override IEnumerable<IMagicResource> GeneratedResources =>
		_states.SelectMany(x => x.Resources.Select(y => y.Resource)).Distinct();

	#endregion
}