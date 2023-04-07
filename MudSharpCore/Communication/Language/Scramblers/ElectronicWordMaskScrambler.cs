using MudSharp.Framework;

namespace MudSharp.Communication.Language.Scramblers;

public class ElectronicWordMaskScrambler : WordMaskScrambler
{
	protected ElectronicWordMaskScrambler()
	{
	}

	public new static ElectronicWordMaskScrambler Instance { get; } = new();

	#region Overrides of WordMaskScrambler

	public override string Mask
	{
		get
		{
			switch (Dice.Roll(1, 5))
			{
				case 1:
					return "...";
				case 2:
					return "*pop*";
				case 3:
					return "*whistle*";
				case 4:
					return "*indistinct*";
				case 5:
					return "*scratch*";
			}

			return string.Empty;
		}
		protected set { }
	}

	#endregion
}