using System;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.GameItems;

namespace MudSharp.Framework {
	public interface IPerceiver : IPerceivable, IHaveDubs, IFormatProvider, IHaveAccount, ICombatant {
		int LineFormatLength { get; }

		int InnerLineFormatLength { get; }

		/// <summary>
		///     If the perceiver is returning a non-null value for this, they wish to see non-default CellOverlays as they move
		///     around (likely as they are building)
		/// </summary>
		ICellOverlayPackage CurrentOverlayPackage { get; set; }

		RoomLayer RoomLayer { get; set; }

		InRoomLocation InRoomLocation => new()
		{
			Location = Location,
			RoomLayer = RoomLayer
		};

		PerceptionTypes NaturalPerceptionTypes { get; }

		bool BriefCombatMode { get; set; }
		bool CanHear(IPerceivable thing);
		bool CanSense(IPerceivable thing, bool ignoreFuzzy = false);
		bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
		bool CanSmell(IPerceivable thing);
		double VisionPercentage { get; }
		bool IsPersonOfInterest(IPerceivable thing);
		bool ShouldFall();
		void FallToGround();
		bool FallOneLayer(ref double fallDistance);
		bool ColocatedWith(IPerceiver otherThing);
		bool CouldTransitionToLayer(RoomLayer otherLayer);
	}
}