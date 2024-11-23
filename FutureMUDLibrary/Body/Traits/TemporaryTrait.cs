using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits
{
	public class TemporaryTrait : ITrait
    {
		public required IHaveTraits Owner { get; init; }
		public required ITraitDefinition Definition { get; init; }
		public required double Value { get; set; }
        public double RawValue => Value;
        public bool Hidden => Definition.Hidden;
        public double MaxValue => Definition.MaxValue;

		public bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype, IEnumerable<Tuple<string, double>> bonuses)
		{
            return false;
		}

		public event EventHandler<TraitChangedEventArgs> TraitValueChanged;

		public void Initialise(IHaveTraits owner)
		{
			// Do nothing
		}

		public bool Changed { get; set; }

		public void Save()
		{
			// Do nothing
		}

		public IFuturemud Gameworld { get; }
	}
}