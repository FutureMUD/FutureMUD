#nullable enable

using System;
using System.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private sealed record HistoricalStableClaim(Room Room, NpcTemplate Animal);

	private HistoricalStableClaim? ResolveHistoricalStableClaim(ArmWorldReset reset)
	{
		var values = ResetValues(reset);
		if (values.Length != 6 || !_npcs.TryGetValue(values[2], out var animal)) return null;
		var stable = ServiceLocations("animal_to_store", "luirs_stablehand")
			.FirstOrDefault(x => _rooms.TryGetValue(values[0], out var room) && x.Room.Id == room.Id);
		return stable.Room is null ? null : new(stable.Room, animal);
	}

	private string HistoricalStableTicketReset(ArmWorldReset reset, int index, string command)
	{
		var claim = ResolveHistoricalStableClaim(reset);
		if (claim is null)
		{
			Adapt("excluded historical stable ticket", reset.Source.Line,
				$"{reset.Source.Path}: Vnum1599 has no retained animal or native stable for its saved values; ticket omitted.");
			// An omitted receipt must not suppress the remaining sibling contents.
			return $"{command} = true";
		}

		var register = $"armw_stable_claim_{reset.Zone}_{reset.Source.Line}";
		var owner = ProgVariableTypes.Location.ToStorageString();
		if (!_db.VariableDefinitions.Local.Any(x => x.OwnerTypeDefinition == owner && x.Property == register) &&
			!_db.VariableDefinitions.Any(x => x.OwnerTypeDefinition == owner && x.Property == register))
		{
			_db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = owner, Property = register,
				ContainedTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() });
			_db.VariableDefaults.Add(new VariableDefault { OwnerTypeDefinition = owner, Property = register,
				DefaultValue = "<var>false</var>" });
		}
		Record($"stable.claim.{reset.Zone}.{reset.Source.Line}", "ArmageddonHistoricalStableClaim", null, null,
			new { Source = reset.Source, SourceValues = ResetValues(reset), StableRoomId = claim.Room.Id,
				AnimalTemplateId = claim.Animal.Id, OpeningDebt = 20, Register = register,
				Disposition = "One-time native stable stay and bearer ticket; unknown depositor, no inferred ownership or recurring replacement." });

		return $"""
			var parent{index} as item
			parent{index} = @lastobj
			var ancestor{index} as item
			ancestor{index} = @lastobj
			var depth{index} as number
			depth{index} = 0
			while (not(isnull(@ancestor{index})) and not(isnull(@ancestor{index}.container)) and @depth{index} < 1000)
			depth{index} = @depth{index} + 1
			ancestor{index} = @ancestor{index}.container
			end while
			depth{index} = @depth{index} + 1
			while (@depth{index} > {reset.Arguments[0]} and not(isnull(@parent{index})) and not(isnull(@parent{index}.container)))
			depth{index} = @depth{index} - 1
			parent{index} = @parent{index}.container
			end while
			if (not(isnull(@parent{index})) and @parent{index}.iscontainer and not(isnull(@ticketorigin)))
			if (getregister(@ticketorigin, "{register}"))
			{command} = true
			lastobj = @parent{index}
			else
			var item{index} = stockstablemount({claim.Animal.Id}, tolocation({claim.Room.Id}), @parent{index}, null("character"), 20)
			{command} = not(isnull(@item{index}))
			if (@{command})
			var mount{index} = stabledmount(@item{index})
			setregister @mount{index} "{MountTrainedRegister}" 1
			setregister @ticketorigin "{register}" true
			lastobj = @item{index}
			end if
			end if
			end if
			""";
	}
}
