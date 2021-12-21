namespace AzureHelloWorldWeb.Tests
{
    using System;
    using System.Threading.Tasks;
    using Features.Values;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class IntegrationFixture : IDisposable
    {
        public IntegrationFixture()
        {
            Factory = new TestApplicationFactory();
            Configuration = Factory.Services.GetRequiredService<IConfiguration>();
            ScopeFactory = Factory.Services.GetRequiredService<IServiceScopeFactory>();

            Mode = Configuration.GetValue<PersistenceMode>("Database:PersistenceMode");
            if (Mode == PersistenceMode.Database)
            {
                using var scope = ScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ValuesContext>();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
        }

        public class TestApplicationFactory : WebApplicationFactory<LocalEntryPoint>
        {
        }

        public TestApplicationFactory Factory;
        public IConfiguration Configuration;
        public IServiceScopeFactory ScopeFactory;
        public PersistenceMode Mode { get; }

        public async Task UsingValuesServiceAsync(Func<IValuesService, Task> action)
        {
            using var scope = ScopeFactory.CreateScope();

            if (Mode == PersistenceMode.Database)
            {
                var context = scope.ServiceProvider.GetRequiredService<ValuesContext>();

                try
                {
                    await context.BeginTransactionAsync();

                    var service = scope.ServiceProvider.GetRequiredService<IValuesService>();
                    await action(service);

                    await context.CommitTransactionAsync();
                }
                catch
                {
                    context.RollbackTransaction();
                }
            }
            else
            {
                var service = scope.ServiceProvider.GetRequiredService<IValuesService>();
                await action(service);
            }
        }

        public void Dispose()
        {
            if (Mode == PersistenceMode.Database)
            {
                using var scope = ScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ValuesContext>();
                context.Database.EnsureDeleted();
            }

            Factory?.Dispose();
        }
    }
}