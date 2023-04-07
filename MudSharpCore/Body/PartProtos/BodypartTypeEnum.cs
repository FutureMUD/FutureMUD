namespace MudSharp.Body.PartProtos;

public static class BodypartExtensions
{
	public static bool IsBone(this BodypartTypeEnum type)
	{
		switch (type)
		{
			case BodypartTypeEnum.Bone:
			case BodypartTypeEnum.NonImmobilisingBone:
			case BodypartTypeEnum.MinorBone:
			case BodypartTypeEnum.MinorNonImobilisingBone:
				return true;
			default:
				return false;
		}
	}
}