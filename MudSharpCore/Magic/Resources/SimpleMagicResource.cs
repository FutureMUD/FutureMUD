using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Magic.Resources;

public class SimpleMagicResource : BaseMagicResource
{
	public SimpleMagicResource(Models.MagicResource resource, IFuturemud gameworld) : base(resource, gameworld)
	{
		var root = XElement.Parse(resource.Definition);
		var element = root.Element("ShouldStartWithResourceCharacterProg");
		if (element != null)
		{
			ShouldStartWithResourceCharacterProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (ShouldStartWithResourceCharacterProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid ShouldStartWithResourceCharacterProg.");
			}

			if (!ShouldStartWithResourceCharacterProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceCharacterProg that returns a boolean.");
			}

			if (!ShouldStartWithResourceCharacterProg.MatchesParameters(new[] { FutureProgVariableTypes.Character })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceCharacterProg that accepts a single character parameter.");
			}
		}

		element = root.Element("StartingResourceAmountCharacterProg");
		if (element != null)
		{
			StartingResourceAmountCharacterProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (StartingResourceAmountCharacterProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid StartingResourceAmountCharacterProg.");
			}

			if (!StartingResourceAmountCharacterProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountCharacterProg that returns a number.");
			}

			if (!StartingResourceAmountCharacterProg.MatchesParameters(new[] { FutureProgVariableTypes.Character })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountCharacterProg that accepts a single character parameter.");
			}
		}

		element = root.Element("ShouldStartWithResourceItemProg");
		if (element != null)
		{
			ShouldStartWithResourceItemProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (ShouldStartWithResourceItemProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid ShouldStartWithResourceItemProg.");
			}

			if (!ShouldStartWithResourceItemProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceItemProg that returns a boolean.");
			}

			if (!ShouldStartWithResourceItemProg.MatchesParameters(new[] { FutureProgVariableTypes.Item })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceItemProg that accepts a single item parameter.");
			}
		}

		element = root.Element("StartingResourceAmountItemProg");
		if (element != null)
		{
			StartingResourceAmountItemProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (StartingResourceAmountItemProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid StartingResourceAmountItemProg.");
			}

			if (!StartingResourceAmountItemProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountItemProg that returns a number.");
			}

			if (!StartingResourceAmountItemProg.MatchesParameters(new[] { FutureProgVariableTypes.Item })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountItemProg that accepts a single item parameter.");
			}
		}

		element = root.Element("ShouldStartWithResourceLocationProg");
		if (element != null)
		{
			ShouldStartWithResourceLocationProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (ShouldStartWithResourceLocationProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid ShouldStartWithResourceLocationProg.");
			}

			if (!ShouldStartWithResourceLocationProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceLocationProg that returns a boolean.");
			}

			if (!ShouldStartWithResourceLocationProg.MatchesParameters(new[] { FutureProgVariableTypes.Location })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a ShouldStartWithResourceLocationProg that accepts a single location parameter.");
			}
		}

		element = root.Element("StartingResourceAmountLocationProg");
		if (element != null)
		{
			StartingResourceAmountLocationProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);

			if (StartingResourceAmountLocationProg == null)
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) had an invalid StartingResourceAmountLocationProg.");
			}

			if (!StartingResourceAmountLocationProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountLocationProg that returns a number.");
			}

			if (!StartingResourceAmountLocationProg.MatchesParameters(new[] { FutureProgVariableTypes.Location })
			   )
			{
				throw new ApplicationException(
					$"SimpleMagicResource ID #{Id} ({Name}) must have a StartingResourceAmountLocationProg that accepts a single location parameter.");
			}
		}

		element = root.Element("ResourceCapProg");
		if (element != null)
		{
			ResourceCapProg = long.TryParse(element.Value, out var value)
				? Gameworld.FutureProgs.Get(value)
				: Gameworld.FutureProgs.GetByName(element.Value);
		}

		if (ResourceCapProg == null)
		{
			throw new ApplicationException($"SimpleMagicResource ID #{Id} ({Name}) had an invalid ResourceCapProg.");
		}

		if (!ResourceCapProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Number))
		{
			throw new ApplicationException(
				$"SimpleMagicResource ID #{Id} ({Name}) must have a ResourceCapProg that returns a number.");
		}

		if (!ResourceCapProg.MatchesParameters(new[] { FutureProgVariableTypes.MagicResourceHaver })
		   )
		{
			throw new ApplicationException(
				$"SimpleMagicResource ID #{Id} ({Name}) must have a ResourceCapProg that accepts a single MagicResourceHaver parameter.");
		}
	}

	#region Overrides of BaseMagicResource

	public IFutureProg ShouldStartWithResourceCharacterProg { get; set; }
	public IFutureProg ShouldStartWithResourceItemProg { get; set; }
	public IFutureProg ShouldStartWithResourceLocationProg { get; set; }
	public IFutureProg StartingResourceAmountCharacterProg { get; set; }
	public IFutureProg StartingResourceAmountItemProg { get; set; }
	public IFutureProg StartingResourceAmountLocationProg { get; set; }
	public IFutureProg ResourceCapProg { get; set; }

	public override bool ShouldStartWithResource(IHaveMagicResource thing)
	{
		switch (thing)
		{
			case ICharacter ch:
				return (bool?)ShouldStartWithResourceCharacterProg?.Execute(ch) ?? false;
			case ICell cell:
				return (bool?)ShouldStartWithResourceLocationProg?.Execute(cell) ?? false;
			case IGameItem gi:
				return (bool?)ShouldStartWithResourceItemProg?.Execute(gi) ?? false;
		}

		return false;
	}

	public override double StartingResourceAmount(IHaveMagicResource thing)
	{
		switch (thing)
		{
			case ICharacter ch:
				return (double)((decimal?)StartingResourceAmountCharacterProg?.Execute(ch) ?? 0.0M);
			case ICell cell:
				return (double)((decimal?)StartingResourceAmountLocationProg?.Execute(cell) ?? 0.0M);
			case IGameItem gi:
				return (double)((decimal?)StartingResourceAmountItemProg?.Execute(gi) ?? 0.0M);
		}

		return 0.0;
	}

	public override double ResourceCap(IHaveMagicResource thing)
	{
		return ResourceCapProg.ExecuteDouble(0.0, thing);
	}

	#endregion
}