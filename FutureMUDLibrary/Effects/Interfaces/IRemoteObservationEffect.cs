using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Interfaces
{
    public interface IRemoteObservationEffect : IEffectSubtype
    {
		/// <summary>
		/// Returns whether this observer is spatially anchored close enough to receive a local
		/// output. The safe default rejects RouteCell-local events; explicit whole-location
		/// broadcasts continue to bypass this filter.
		/// </summary>
		bool Observes(SpatialLocation source)
		{
			return source.Cell.RouteDefinition is null;
		}

        void HandleOutput(IOutput output, ILocation location);
        void HandleOutput(string text, ILocation location);
    }
}
