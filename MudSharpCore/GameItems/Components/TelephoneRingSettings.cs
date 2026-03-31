#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Audio;

namespace MudSharp.GameItems.Components;

internal static class TelephoneRingSettings
{
	public const string Quiet = "quiet";
	public const string Normal = "normal";
	public const string Loud = "loud";
	public const string Silent = "silent";

	public static readonly IReadOnlyList<string> LandlineSettings = [Quiet, Normal, Loud];
	public static readonly IReadOnlyList<string> CellularSettings = [Quiet, Normal, Loud, Silent];

	public static bool TryGetVolumeForSetting(string setting, bool allowSilent, out AudioVolume volume)
	{
		switch (setting.ToLowerInvariant())
		{
			case Quiet:
				volume = AudioVolume.Quiet;
				return true;
			case Normal:
				volume = AudioVolume.Decent;
				return true;
			case Loud:
				volume = AudioVolume.Loud;
				return true;
			case Silent when allowSilent:
				volume = AudioVolume.Silent;
				return true;
			default:
				volume = AudioVolume.Decent;
				return false;
		}
	}

	public static string DescribeSetting(AudioVolume volume, bool allowSilent)
	{
		if (allowSilent && volume == AudioVolume.Silent)
		{
			return Silent;
		}

		if (volume <= AudioVolume.Quiet)
		{
			return Quiet;
		}

		return volume >= AudioVolume.Loud ? Loud : Normal;
	}

	public static bool TryParseBuilderSetting(string setting, bool allowSilent, out AudioVolume volume)
	{
		if (TryGetVolumeForSetting(setting, allowSilent, out volume))
		{
			return true;
		}

		if (string.IsNullOrWhiteSpace(setting))
		{
			return false;
		}

		if (int.TryParse(setting, out var rawValue) && Enum.IsDefined(typeof(AudioVolume), rawValue))
		{
			volume = NormaliseVolume((AudioVolume)rawValue, allowSilent);
			return true;
		}

		var normalised = new string(setting.Where(char.IsLetterOrDigit).ToArray());
		if (Enum.TryParse(normalised, true, out AudioVolume parsed))
		{
			volume = NormaliseVolume(parsed, allowSilent);
			return true;
		}

		return false;
	}

	public static AudioVolume NormaliseVolume(AudioVolume volume, bool allowSilent)
	{
		if (allowSilent && volume == AudioVolume.Silent)
		{
			return AudioVolume.Silent;
		}

		if (volume <= AudioVolume.Quiet)
		{
			return AudioVolume.Quiet;
		}

		return volume >= AudioVolume.Loud ? AudioVolume.Loud : AudioVolume.Decent;
	}
}
