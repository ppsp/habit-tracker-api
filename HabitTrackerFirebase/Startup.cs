using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace HabitTrackerWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Logger.Debug("configuring services");

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    });
            services.AddApplicationInsightsTelemetry();

            var vaultName = Configuration["KeyVaultName"];
            var vaultFirebaseSecretName = Configuration["KeyVaultFirebaseSecretName"];

            if (vaultName != null)
            {
                Logger.Debug("from config, KeyVaultName : " + vaultName);
                Logger.Debug("from config, KeyVaultFirebaseSecretName : " + vaultFirebaseSecretName);
            }
            else
            {
                Logger.Debug("KeyVaultName is null");
                Logger.Debug("Config to string : " + Configuration.ToString());
            }

            services.AddSingleton(new AzureVaultConnector(vaultName));

            // Add FirebaseConnector which depends on AzureVaultConnector
            services.AddSingleton(serviceProvider => {
                var avureVault = serviceProvider.GetService<AzureVaultConnector>();
                var firebaseSecretJson = avureVault.GetSecretValueString(vaultFirebaseSecretName);
                var firebaseConnector = new FirebaseConnector(firebaseSecretJson);
                return firebaseConnector;
            });
            services.AddScoped<AuthorizeJwt>();

            Logger.Debug("configured services");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder => builder.WithOrigins("http://localhost:4200").AllowAnyHeader());
            app.UseCors(builder => builder.WithOrigins("http://localhost").AllowAnyHeader());
            app.UseCors(builder => builder.WithOrigins("ionic://localhost").AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
