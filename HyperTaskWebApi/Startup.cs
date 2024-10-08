using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
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

namespace HyperTaskWebApi
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
            var vaultMongoConnectionSecretName = Configuration["KeyVaultMongoConnectionSecretName"];
            var vaultInstrumentationKeySecretName = Configuration["InstrumentationKeySecretName"];
            var personnalAccessTokenSecretName = Configuration["AzureDevopsPersonnalTokenSecretName"];
            var azureDevopsUri = Configuration["AzureDevopsUri"];
            var azureDevopsProjectName = Configuration["AzureDevopsProjectName"];

            if (vaultName != null)
            {
                Logger.Debug("from config, KeyVaultName : " + vaultName);
                Console.WriteLine("from config, KeyVaultName : " + vaultName);
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

            // Add MongoConnector which depends on AzureVaultConnector
            services.AddSingleton(serviceProvider => {
                var avureVault = serviceProvider.GetService<AzureVaultConnector>();
                var mongoConnectionString = avureVault.GetSecretValueString(vaultMongoConnectionSecretName);
                var mongoConnector = new MongoConnector(mongoConnectionString);

                return mongoConnector;
            });

            // Add AzureDevops which depends on AzureVaultConnector
            services.AddSingleton(serviceProvider => {
                var avureVault = serviceProvider.GetService<AzureVaultConnector>();
                var personnalAccessToken = avureVault.GetSecretValueString(personnalAccessTokenSecretName);
                var azureDevopsConnector = new AzureDevopsConnector(azureDevopsUri, personnalAccessToken, azureDevopsProjectName);

                return azureDevopsConnector;
            });

            // Add CalendarTaskService
            services.AddSingleton<FireCalendarTaskService>(serviceProvider => {
                var firebaseConnector = serviceProvider.GetService<FirebaseConnector>();
                var calendarTaskService= new FireCalendarTaskService(firebaseConnector);
                return calendarTaskService;
            });

            // Add CalendarTaskService
            services.AddSingleton<MongoCalendarTaskService>(serviceProvider => {
                var mongoConnector = serviceProvider.GetService<MongoConnector>();
                var calendarTaskService = new MongoCalendarTaskService(mongoConnector);
                return calendarTaskService;
            });

            // Add GroupService
            services.AddSingleton<FireTaskGroupService>(serviceProvider => {
                var firebaseConnector = serviceProvider.GetService<FirebaseConnector>();
                var groupService = new FireTaskGroupService(firebaseConnector);
                return groupService;
            });

            // Add GroupService
            services.AddSingleton<MongoTaskGroupService>(serviceProvider => {
                var mongoConnector = serviceProvider.GetService<MongoConnector>();
                var groupService = new MongoTaskGroupService(mongoConnector);
                return groupService;
            });

            // Add ReportService
            services.AddSingleton<ReportService>(serviceProvider => {
                var taskService = serviceProvider.GetService<FireCalendarTaskService>();
                var groupService = serviceProvider.GetService<FireTaskGroupService>();
                var reportService = new ReportService(taskService, groupService);
                return reportService;
            });

            // Add Action Filter
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
