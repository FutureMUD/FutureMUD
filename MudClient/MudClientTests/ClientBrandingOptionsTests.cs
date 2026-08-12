using MudClientBlazor.Services;

namespace MudClientTests;

public class ClientBrandingOptionsTests
{
	[Fact]
	public void Normalize_PreservesSafeDeploymentBranding()
	{
		var branding = ClientBrandingOptions.Normalize(new ClientBrandingOptions
		{
			Title = "  Example MUD  ",
			IconUrl = "custom/example-icon.png",
			AboutText = "  The official Example MUD client.  "
		});

		Assert.Equal("Example MUD", branding.Title);
		Assert.Equal("custom/example-icon.png", branding.IconUrl);
		Assert.Equal("The official Example MUD client.", branding.AboutText);
	}

	[Theory]
	[InlineData("https://example.com/icon.png")]
	[InlineData("//example.com/icon.png")]
	[InlineData("../icon.png")]
	[InlineData("custom\\icon.png")]
	public void Normalize_ReplacesNonLocalIconUrls(string iconUrl)
	{
		var branding = ClientBrandingOptions.Normalize(new ClientBrandingOptions { IconUrl = iconUrl });

		Assert.Equal(ClientBrandingOptions.DefaultIconUrl, branding.IconUrl);
	}

	[Fact]
	public void ProductInformation_ReportsTheMudClientProductVersion()
	{
		var information = ClientProductInformation.Current;

		Assert.Equal(ClientProductInformation.ProductName, information.Name);
		Assert.Matches(@"^\d+\.\d+\.\d+$", information.Version);
	}
}
