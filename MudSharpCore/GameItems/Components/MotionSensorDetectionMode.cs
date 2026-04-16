#nullable enable

using MudSharp.Events;
using System;

namespace MudSharp.GameItems.Components;

public enum MotionSensorDetectionMode
{
	AnyMovement = 0,
	BeginMovement = 1,
	EnterCell = 2,
	StopMovement = 3
}

public static class MotionSensorDetectionModeExtensions
{
	public static bool MatchesEventType(this MotionSensorDetectionMode mode, EventType type)
	{
		return mode switch
		{
			MotionSensorDetectionMode.AnyMovement => type is EventType.CharacterBeginMovementWitness or
				EventType.CharacterEnterCellWitness or
				EventType.CharacterStopMovementWitness or
				EventType.CharacterStopMovementClosedDoorWitness,
			MotionSensorDetectionMode.BeginMovement => type == EventType.CharacterBeginMovementWitness,
			MotionSensorDetectionMode.EnterCell => type == EventType.CharacterEnterCellWitness,
			MotionSensorDetectionMode.StopMovement => type is EventType.CharacterStopMovementWitness or
				EventType.CharacterStopMovementClosedDoorWitness,
			_ => false
		};
	}

	public static string Describe(this MotionSensorDetectionMode mode)
	{
		return mode switch
		{
			MotionSensorDetectionMode.AnyMovement => "Any Movement",
			MotionSensorDetectionMode.BeginMovement => "Begin Movement",
			MotionSensorDetectionMode.EnterCell => "Enter Cell",
			MotionSensorDetectionMode.StopMovement => "Stop Movement",
			_ => "Unknown"
		};
	}

	public static bool TryParse(string text, out MotionSensorDetectionMode mode)
	{
		switch (text.Trim().ToLowerInvariant())
		{
			case "any":
			case "movement":
			case "anymovement":
			case "any movement":
				mode = MotionSensorDetectionMode.AnyMovement;
				return true;
			case "begin":
			case "beginmovement":
			case "begin movement":
			case "start":
				mode = MotionSensorDetectionMode.BeginMovement;
				return true;
			case "enter":
			case "entercell":
			case "enter cell":
			case "arrival":
				mode = MotionSensorDetectionMode.EnterCell;
				return true;
			case "stop":
			case "stopmovement":
			case "stop movement":
				mode = MotionSensorDetectionMode.StopMovement;
				return true;
		}

		return Enum.TryParse(text, true, out mode);
	}
}
