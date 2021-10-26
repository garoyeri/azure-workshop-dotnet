namespace AzureHelloWorldWeb
{
    using System;
    using Features.Values;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class Extensions
    {
        public static IServiceCollection AddDatabaseValuesService(this IServiceCollection services,
            IConfiguration configuration)
        {
            // relational database support
            services.AddDbContext<ValuesContext>(o =>
            {
                // override the connection string with values from the secrets / configuration
                var settings = configuration.GetSection("Database").Get<DatabaseSettings>();
                var connectionString =
                    new SqlConnectionStringBuilder(configuration.GetConnectionString("Database"));
                connectionString.DataSource = settings.Host ?? connectionString.DataSource;
                //connectionString.Port = settings.Port ?? connectionString.Port;
                connectionString.UserID = settings.Username ?? connectionString.UserID;
                connectionString.Password = settings.Password ?? connectionString.Password;
                connectionString.ConnectTimeout = (int)TimeSpan.FromMinutes(1).TotalSeconds;

                o.UseSqlServer(connectionString.ToString());
            });

            services.AddScoped<IValuesService, DatabaseValuesService>();

            return services;
        }

        public static void ConfigureSecrets(HostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            // // if there is a secrets ARN configured AND we're not in development mode,
            // //  pull the secrets from the secrets manager
            // var secretsArn = config.Build().GetValue<string>("Database:ConnectionSecretArn");
            // if (secretsArn != null && !hostingContext.HostingEnvironment.IsDevelopment())
            // {
            //     var arn = Arn.Parse(secretsArn);
            //     config.AddSecretsManager(configurator: options =>
            //     {
            //         options.AcceptedSecretArns = new List<string> { secretsArn };
            //         options.KeyGenerator = (entry, key) => $"Database:{key.MapSecretEntries(arn.Resource)}";
            //     });
            // }
        }

        // static string MapSecretEntries(this string key, string resource)
        // {
        //     var scrubbed = key.Split(":").LastOrDefault();
        //     return scrubbed switch
        //     {
        //         "username" => "Username",
        //         "password" => "Password",
        //         "host" => "Host",
        //         "port" => "Port",
        //         _ => key
        //     };
        // }
    }
}