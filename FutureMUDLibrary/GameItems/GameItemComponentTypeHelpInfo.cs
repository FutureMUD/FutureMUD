#nullable enable

using System;

namespace MudSharp.GameItems;

[Flags]
public enum GameItemComponentTypeTechnology
{
	None = 0,
	Modern = 1 << 0,
	Futuristic = 1 << 1
}

public sealed record GameItemComponentTypeHelpInfo(
	string Name,
	string Blurb,
	string Help,
	GameItemComponentTypeTechnology Technology)
{
	public bool IsModern => Technology.HasFlag(GameItemComponentTypeTechnology.Modern);
	public bool IsFuturistic => Technology.HasFlag(GameItemComponentTypeTechnology.Futuristic);
}

public static class GameItemComponentTypeVisibility
{
	public const string ShowModernSettingName = "ShowModernItemComponentTypes";
	public const string ShowFuturisticSettingName = "ShowFuturisticItemComponentTypes";
}
