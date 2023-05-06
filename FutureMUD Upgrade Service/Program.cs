
using Microsoft.OpenApi.Models;

namespace FutureMUD_Upgrade_Service;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo { Title = "FutureMUD Upgrade Service", Version = "v1" });

			options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows
				{
					Implicit = new OpenApiOAuthFlow
					{
						AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
						TokenUrl = new Uri("https://accounts.google.com/o/oauth2/token"),
						Scopes = new Dictionary<string, string>
						{
							{ "openid", "OpenID" },
							{ "profile", "Profile" },
							{ "email", "Email" }
						}
					}
				}
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "oauth2"
						}
					},
					new[] { "openid", "profile", "email" }
				}
			});
		});


		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowSpecificOrigin",
				builder =>
				{
					builder.WithOrigins("https://localhost:7039")
					       .AllowAnyHeader()
					       .AllowAnyMethod();
				});
		});

		builder.Services.AddAuthentication(options =>
		       {
			       options.DefaultScheme = "Cookies";
			       options.DefaultChallengeScheme = "Google";
			       options.DefaultSignInScheme = "Cookies";
		       })
		       .AddCookie("Cookies")
		       .AddGoogle("Google", googleOptions =>
		       {
			       googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
			       googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
		       });

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "FutureMUD_Upgrade_Service v1");
				options.RoutePrefix = string.Empty;
				options.ConfigObject.DisplayRequestDuration = true;
				options.ConfigObject.DefaultModelsExpandDepth = -1;
				options.ConfigObject.DefaultModelExpandDepth = 2;
			});
		}

		app.UseCors("AllowSpecificOrigin");
		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}