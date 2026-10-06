#nullable enable

using System;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal override Action CaptureCustodySaveRollback()
	{
		var restoreShared = base.CaptureCustodySaveRollback();
		var inventory = _inventoryChanged; var stamina = _staminaChanged; var merits = _meritsChanged;
		var parts = _bodypartsChanged; var drugs = _drugsChanged; var characteristics = _characteristicsChanged;
		var needs = _needsChanged;
		var prosthetics = _prostheticsChanged; var implants = _implantsChanged;
		var tattoos = _tattoosChanged; var scars = _scarsChanged; var liquid = _surfaceLiquidChanged;
		return () =>
		{
			_inventoryChanged |= inventory; _staminaChanged |= stamina; _meritsChanged |= merits;
			_bodypartsChanged |= parts; _drugsChanged |= drugs; _characteristicsChanged |= characteristics;
			_prostheticsChanged |= prosthetics; _implantsChanged |= implants;
			_tattoosChanged |= tattoos; _scarsChanged |= scars; _surfaceLiquidChanged |= liquid;
			_needsChanged |= needs;
			restoreShared();
		};
	}

	internal Action CaptureNeedsSaveRollback()
	{
		// Called immediately before the save's random batching setter, after any earlier
		// helper saves. A later live setter retains its own counter progress.
		var count = _needsChangedCount; var revision = _needsChangedRevision;
		return () =>
		{
			_needsChanged = true;
			if (_needsChangedRevision == revision + 1)
			{
				_needsChangedCount = count; _needsChangedRevision = revision;
			}
		};
	}
}
