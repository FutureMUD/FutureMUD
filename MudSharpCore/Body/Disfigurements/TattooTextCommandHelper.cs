using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;

namespace MudSharp.Body.Disfigurements;

public static class TattooTextCommandHelper
{
	public static bool TryParseTextValues(ICharacter actor, ITattooTemplate template, StringStack command,
		bool allowCopyFromWriting, out List<ITattooTextValue> textValues, out bool hasUnreadableCopyPenalty,
		out string errorMessage)
	{
		textValues = [];
		hasUnreadableCopyPenalty = false;
		errorMessage = string.Empty;

		if (!template.TextSlots.Any())
		{
			return true;
		}

		var values = new Dictionary<string, ITattooTextValue>(StringComparer.InvariantCultureIgnoreCase);
		while (!command.IsFinished)
		{
			var raw = command.PopSpeech();
			var equalsIndex = raw.IndexOf('=');
			if (equalsIndex <= 0 || equalsIndex == raw.Length - 1)
			{
				errorMessage =
					$"The text argument {raw.ColourCommand()} is not valid. Use the form {"slot=text".ColourCommand()} or {"slot=copy:writingId".ColourCommand()}.";
				return false;
			}

			var slotName = raw[..equalsIndex];
			var slot = template.GetTextSlot(slotName);
			if (slot == null)
			{
				errorMessage = $"There is no text slot called {slotName.ColourName()} for that tattoo.";
				return false;
			}

			var valueText = raw[(equalsIndex + 1)..];
			if (allowCopyFromWriting && valueText.StartsWith("copy:", StringComparison.InvariantCultureIgnoreCase))
			{
				if (!long.TryParse(valueText["copy:".Length..], out var writingId))
				{
					errorMessage = "When copying from writing you must supply a valid writing ID after copy:.";
					return false;
				}

				var writing = actor.Gameworld.Writings.Get(writingId);
				if (writing == null)
				{
					errorMessage = "There is no such writing to copy from.";
					return false;
				}

				var copiedText = GetWritingText(writing);
				if (copiedText.Length > MaximumLengthFor(slot, writing.Script))
				{
					errorMessage =
						$"The copied writing is too long for the {slot.Name.ColourName()} slot.";
					return false;
				}

				var copiedUnreadably = !actor.CanRead(writing);
				hasUnreadableCopyPenalty |= copiedUnreadably;
				values[slot.Name] = new TattooTextValue(slot.Name, writing.Language, writing.Script, writing.Style,
					writing.WritingColour, writing.LanguageSkill, copiedText, slot.DefaultAlternateText, true,
					copiedUnreadably);
				continue;
			}

			if (valueText.Length > MaximumLengthFor(slot, slot.DefaultScript))
			{
				errorMessage =
					$"The text for the {slot.Name.ColourName()} slot is too long. The maximum allowed length is {MaximumLengthFor(slot, slot.DefaultScript).ToString("N0", actor).ColourValue()} characters.";
				return false;
			}

			values[slot.Name] = new TattooTextValue(slot.Name, slot.DefaultLanguage, slot.DefaultScript,
				slot.DefaultStyle, slot.DefaultColour, slot.DefaultMinimumSkill, valueText, slot.DefaultAlternateText);
		}

		var missingRequired = template.TextSlots
			.Where(x => x.RequiredCustomText)
			.Where(x => !values.ContainsKey(x.Name))
			.Select(x => x.Name.ColourName())
			.ToList();
		if (missingRequired.Any())
		{
			errorMessage =
				$"That tattoo requires values for the following text slots: {missingRequired.ListToString()}.";
			return false;
		}

		textValues = values.Values.ToList();
		return true;
	}

	public static int MaximumLengthFor(ITattooTemplateTextSlot slot, IScript script)
	{
		return Math.Max(1, (int)Math.Floor(slot.MaximumLength * (script?.DocumentLengthModifier ?? 1.0)));
	}

	private static string GetWritingText(IWriting writing)
	{
		return writing switch
		{
			SimpleWriting sw => sw.Text,
			CompositeWriting cw => cw.Text,
			_ => writing.GetProperty("text")?.GetObject?.ToString() ?? string.Empty
		};
	}
}
