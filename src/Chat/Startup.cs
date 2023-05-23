using Chat.Application;
using Chat.Extensions;
using Chat.Persistence.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Chat;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Contenedor de dependencias (Dependency Injection Container)
    public void ConfigureServices(IServiceCollection services)
    {
        var chatsConnectionString =
            _configuration.GetValue<string>("ConnectionStrings:ChatsDatabase");

        var websocketHostUrl = _configuration.GetValue<string>("WebsocketHostUrl");

        if (websocketHostUrl is null)
        {
            throw new ApplicationException();
        }

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http://localhost:5001"; // this requires for the authority to be up and running.
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy("policy-claim-scope-required", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "chat-api");
                });
            });

        services
            .AddTransient<INotificationService>(sp =>
                new NotificationService(new NotificationServiceConfiguration { Host = websocketHostUrl }))
            .AddTransient<ChatCommandServices>()
            .AddTransient<ChatQueryService>()
            .AddDbContext<ChatDbContext>(options =>
            {
                options.UseSqlServer(chatsConnectionString);
            })
            .AddScoped<IChatDbContext, ChatDbContext>()
            .AddOpenApi()
            .AddControllers();
    }

    // Middleware pipeline
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseOpenApi();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
