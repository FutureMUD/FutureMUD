namespace MudSharp.Communication.Language;

public interface IGraffitiWriting : IWriting
{
	DrawingSize DrawingSize { get; }
	string ShortDescription { get; }
	double DrawingSkill { get; }
}