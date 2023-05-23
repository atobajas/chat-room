using System.Reflection;
using System.Text;
using Chat.Application;
using Chat.Persistence.SqlServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Chat.IntegrationTests.TestSupport;
public abstract class IntegrationTest
    : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _uniqueDatabaseName;
    protected HttpClient HttpClient { get; }
    protected IntegrationTest()
    {
        var server =
            new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseEnvironment("Test")
                    .UseCommonConfiguration()
                    .ConfigureTestServices(ConfigureTestServices));

        HttpClient = server.CreateClient();

        // Strategy to use a unique DB schema per test execution
        _serviceProvider = server.Services;
        _uniqueDatabaseName = $"Test-{Guid.NewGuid()}";
        // Apply Migrations
        using var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ChatDbContext>();
        dbContext.Database.Migrate();
    }

    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Replace INotificationService with mock
        RemoveDependencyInjectionRegisteredService<INotificationService>(services);

        var mockNotification = new Mock<INotificationService>();
        mockNotification
            .Setup(m => m.SendNotification(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        services.AddSingleton<INotificationService>(mockNotification.Object);

        // Replace EF Core's DbContext to use
        RemoveDependencyInjectionRegisteredService<DbContextOptions<ChatDbContext>>(services);

        services.AddDbContext<ChatDbContext>(
            (sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var testConnectionString = configuration.GetValue<string>("ConnectionStrings:ChatsDatabase");
                var parts = testConnectionString!.Split(";");
                var uniqueDbTestConnectionStringBuilder = new StringBuilder();
                foreach (var part in parts)
                {
                    var isDatabasePart = part.StartsWith("Database=");
                    uniqueDbTestConnectionStringBuilder.Append(isDatabasePart
                        ? $"Database={_uniqueDatabaseName};"
                        : $"{part};");
                }

                var uniqueDbTestConnectionString = uniqueDbTestConnectionStringBuilder.ToString().TrimEnd(';');
                options.UseSqlServer(uniqueDbTestConnectionString);
            });
    }

    private void RemoveDependencyInjectionRegisteredService<TService>(IServiceCollection services)
    {
        var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(TService));
        if (serviceDescriptor != null)
        {
            services.Remove(serviceDescriptor);
        }
    }

    public void Dispose()
    {
        using var dbContext = _serviceProvider.GetService<ChatDbContext>();
        dbContext?.Database.EnsureDeleted();
    }
}

internal static class WebHostBuilderExtensions
{
    internal static IWebHostBuilder UseCommonConfiguration(this IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;

            config
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                config.AddUserSecrets(appAssembly, optional: true);
            }
        });

        return builder;
    }
}