#nullable enable

namespace MudSharp.GameItems;

public partial class GameItem
{
	private string? _overrideSdesc;
	private string? _overrideDesc;

	public string? OverrideSdesc
	{
		get => _overrideSdesc;
		set
		{
			if (_overrideSdesc == value)
			{
				return;
			}

			_overrideSdesc = value;
			Changed = true;
		}
	}

	public string? OverrideDesc
	{
		get => _overrideDesc;
		set
		{
			if (_overrideDesc == value)
			{
				return;
			}

			_overrideDesc = value;
			Changed = true;
		}
	}
}
