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

	public static bool IsOrgan(this BodypartTypeEnum type)
	{
		switch (type)
		{
			case BodypartTypeEnum.Brain:
			case BodypartTypeEnum.Liver:
			case BodypartTypeEnum.Heart:
			case BodypartTypeEnum.Ear:
			case BodypartTypeEnum.Spleen:
			case BodypartTypeEnum.Intestines:
			case BodypartTypeEnum.Spine:
			case BodypartTypeEnum.Stomach:
			case BodypartTypeEnum.Lung:
			case BodypartTypeEnum.Trachea:
			case BodypartTypeEnum.Kidney:
			case BodypartTypeEnum.Esophagus:
			case BodypartTypeEnum.PositronicBrain:
			case BodypartTypeEnum.PowerCore:
			case BodypartTypeEnum.SpeechSynthesizer:
				return true;
			default:
				return false;
		}
	}
}