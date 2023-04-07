namespace MudSharp.Email;

public class EmailTemplate
{
	public EmailTemplate(MudSharp.Models.EmailTemplate template)
	{
		Content = template.Content;
		Subject = template.Subject;
		ReturnAddress = template.ReturnAddress;
	}

	public string Content { get; private set; }
	public string Subject { get; private set; }
	public string ReturnAddress { get; private set; }
}