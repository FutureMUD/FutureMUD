using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.Health;

public static class HealthExtensions
{
	public static string DescribeColour(this BodyTemperatureStatus status)
	{
		switch (status)
		{
			case BodyTemperatureStatus.CriticalHypothermia:
				return "Critical Hypothermia".Colour(Telnet.BoldWhite);
			case BodyTemperatureStatus.SevereHypothermia:
				return "Severe Hypothermia".Colour(Telnet.BoldCyan);
			case BodyTemperatureStatus.ModerateHypothermia:
				return "Moderate Hypothermia".Colour(Telnet.Cyan);
			case BodyTemperatureStatus.MildHypothermia:
				return "Mild Hypothermia".Colour(Telnet.BoldBlue);
			case BodyTemperatureStatus.VeryMildHypothermia:
				return "Very Mild Hypothermia".Colour(Telnet.Blue);
			case BodyTemperatureStatus.NormalTemperature:
				return "Normal Temperature".Colour(Telnet.BoldGreen);
			case BodyTemperatureStatus.VeryMildHyperthermia:
				return "Very Mild Hyperthermia".Colour(Telnet.Yellow);
			case BodyTemperatureStatus.MildHyperthermia:
				return "Mild Hyperthermia".Colour(Telnet.BoldYellow);
			case BodyTemperatureStatus.ModerateHyperthermia:
				return "Moderate Hyperthermia".Colour(Telnet.BoldOrange);
			case BodyTemperatureStatus.SevereHyperthermia:
				return "Severe Hyperthermia".Colour(Telnet.Red);
			case BodyTemperatureStatus.CriticalHyperthermia:
				return "Critical Hyperthermia".Colour(Telnet.BoldRed);
			default:
				throw new ArgumentOutOfRangeException(nameof(status));
		}
	}

	public static string DescribeAdjectiveColour(this BodyTemperatureStatus status)
	{
		switch (status)
		{
			case BodyTemperatureStatus.CriticalHypothermia:
				return "Critically Hypothermic".Colour(Telnet.BoldWhite);
			case BodyTemperatureStatus.SevereHypothermia:
				return "Severely Hypothermic".Colour(Telnet.BoldCyan);
			case BodyTemperatureStatus.ModerateHypothermia:
				return "Moderately Hypothermic".Colour(Telnet.Cyan);
			case BodyTemperatureStatus.MildHypothermia:
				return "Mildly Hypothermic".Colour(Telnet.BoldBlue);
			case BodyTemperatureStatus.VeryMildHypothermia:
				return "Very Mildly Hypothermic".Colour(Telnet.Blue);
			case BodyTemperatureStatus.NormalTemperature:
				return "Normally Thermic".Colour(Telnet.BoldGreen);
			case BodyTemperatureStatus.VeryMildHyperthermia:
				return "Very Mildly Hyperthermic".Colour(Telnet.Yellow);
			case BodyTemperatureStatus.MildHyperthermia:
				return "Mildly Hyperthermic".Colour(Telnet.BoldYellow);
			case BodyTemperatureStatus.ModerateHyperthermia:
				return "Moderately Hyperthermic".Colour(Telnet.BoldOrange);
			case BodyTemperatureStatus.SevereHyperthermia:
				return "Severely Hyperthermic".Colour(Telnet.Red);
			case BodyTemperatureStatus.CriticalHyperthermia:
				return "Critically Hyperthermic".Colour(Telnet.BoldRed);
			default:
				throw new ArgumentOutOfRangeException(nameof(status));
		}
	}
}