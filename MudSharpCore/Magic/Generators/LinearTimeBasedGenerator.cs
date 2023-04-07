using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace MudSharp.Magic.Generators;

public class LinearTimeBasedGenerator : BaseMagicResourceGenerator
{
	public LinearTimeBasedGenerator(Models.MagicGenerator generator, IFuturemud gameworld) : base(generator)
	{
		var root = XElement.Parse(generator.Definition);
		var element = root.Element("WhichResource");
		if (element == null)
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) is missing a WhichResource element.");
		}

		WhichResource = long.TryParse(element.Value, out var value)
			? gameworld.MagicResources.Get(value)
			: gameworld.MagicResources.GetByName(element.Value);
		if (WhichResource == null)
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) specified an incorrect magic resource.");
		}

		element = root.Element("AmountPerMinute");
		if (element == null)
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) is missing a AmountPerMinute element.");
		}

		if (!double.TryParse(element.Value, out var dvalue))
		{
			throw new ApplicationException(
				$"LinearTimeBasedGenerator #{Id} ({Name}) specified an AboutPerMinute element that wasn't a number.");
		}

		AmountPerMinute = dvalue;
	}

	public IMagicResource WhichResource { get; set; }
	public double AmountPerMinute { get; set; }

	#region Overrides of BaseMagicResourceGenerator

	protected override HeartbeatManagerDelegate InternalGetOnMinuteDelegate(IHaveMagicResource thing)
	{
		return () => { thing.AddResource(WhichResource, AmountPerMinute); };
	}

	/// <inheritdoc />
	public override IEnumerable<IMagicResource> GeneratedResources => new[] { WhichResource };

	#endregion
}