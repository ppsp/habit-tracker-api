using HabitTrackerCore.DAL;
using HabitTrackerCore.Utils;
using HabitTrackerServices.Caching;
using HabitTrackerServices.DAL;
using HabitTrackerTools;
using HabitTrackerWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;

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
            var vaultInstrumentationKeySecretName = Configuration["InstrumentationKeySecretName"];

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

            // Add ApplicationInsightsInstrumentationKey which depends on AzureVaultConnector
            services.AddSingleton(serviceProvider => {
                var avureVault = serviceProvider.GetService<AzureVaultConnector>();
                var instrumentationKey = avureVault.GetSecretValueString(vaultInstrumentationKeySecretName);
                var applicationInsightsConnector = new ApplicationInsightsConnector(instrumentationKey);
                return applicationInsightsConnector;
            });

            // Add FirebaseConnector which depends on AzureVaultConnector
            services.AddSingleton(serviceProvider => {
                var avureVault = serviceProvider.GetService<AzureVaultConnector>();
                var firebaseSecretJson = avureVault.GetSecretValueString(vaultFirebaseSecretName);
                var firebaseConnector = new FirebaseConnector(firebaseSecretJson);

                return firebaseConnector;
            });

            // Add DALTaskHistory which depends on FirebaseConnector
            services.AddSingleton<IDALTaskHistory>(serviceProvider => {
                var firebaseConnector = serviceProvider.GetService<FirebaseConnector>();
                var dalTaskHistory = new DALTaskHistory(firebaseConnector);
                return dalTaskHistory;
            });

            services.AddScoped<AuthorizeJwt>();

            services.AddSingleton(new CachingManager());

            // Add TaskHistoryCache which depends on CachingManager
            services.AddSingleton<TaskHistoryCache>(serviceProvider => {
                var cachingManager = serviceProvider.GetService<CachingManager>();
                var taskHistoryCache = new TaskHistoryCache(cachingManager);
                return taskHistoryCache;
            });

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

            app.UseCors(builder => builder.SetIsOriginAllowed(origin => {
                if (new Uri(origin).Host == "localhost")
                {
                    return true;
                }
                else
                {
                    Logger.Warn("Origin unknown : " + origin);
                    return false;
                }
            }).AllowAnyHeader().AllowAnyMethod());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
            contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
                RequestPath = "/staticfiles",
                ContentTypeProvider = contentTypes,
            });
        }
    }
}
