using HyperTaskServices.Services;
using HyperTaskTools;
using HyperTaskWebApi.ActionFilterAttributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuthorizeJwt>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
});

// Add Application Insights telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Register AzureVaultConnector
builder.Services.AddSingleton<AzureVaultConnector>(sp =>
    new AzureVaultConnector(builder.Configuration["KeyVault:VaultName"] ?? "hypertask-vault-dev6"));

// Register ConnectorFactory
builder.Services.AddSingleton<IConnectorFactory>(sp => new ConnectorFactory(
    vaultInstrumentationKeySecretName: builder.Configuration["KeyVault:InstrumentationKeySecretName"] ?? "hypertask-insights-api-key",
    vaultFirebaseSecretName: builder.Configuration["KeyVault:FirebaseSecretName"] ?? "hypertask-firebase",
    vaultMongoConnectionSecretName: builder.Configuration["KeyVault:MongoConnectionSecretName"] ?? "hypertask-mongo-connection",
    personnalAccessTokenSecretName: builder.Configuration["KeyVault:AzureDevopsTokenSecretName"] ?? "hypertask-azure-devops-personnal-token",
    azureDevopsUri: builder.Configuration["AzureDevops:Uri"] ?? "https://dev.azure.com/your-organization",
    azureDevopsProjectName: builder.Configuration["AzureDevops:ProjectName"] ?? "your-project"
));

// Register services with async initialization using a factory
builder.Services.AddSingleton<FireCalendarTaskService>(sp =>
{
    var factory = sp.GetRequiredService<IConnectorFactory>();
    // Use Task.Run to perform async initialization synchronously for DI
    var firebaseConnector = Task.Run(() => factory.CreateFirebaseConnectorAsync(sp)).GetAwaiter().GetResult();
    return new FireCalendarTaskService(firebaseConnector);
});

builder.Services.AddSingleton<MongoCalendarTaskService>(sp =>
{
    var factory = sp.GetRequiredService<IConnectorFactory>();
    var mongoConnector = Task.Run(() => factory.CreateMongoConnectorAsync(sp)).GetAwaiter().GetResult();
    return new MongoCalendarTaskService(mongoConnector);
});

builder.Services.AddSingleton<FireTaskGroupService>(sp =>
{
    var factory = sp.GetRequiredService<IConnectorFactory>();
    var firebaseConnector = Task.Run(() => factory.CreateFirebaseConnectorAsync(sp)).GetAwaiter().GetResult();
    return new FireTaskGroupService(firebaseConnector);
});

builder.Services.AddSingleton<MongoTaskGroupService>(sp =>
{
    var factory = sp.GetRequiredService<IConnectorFactory>();
    var mongoConnector = Task.Run(() => factory.CreateMongoConnectorAsync(sp)).GetAwaiter().GetResult();
    return new MongoTaskGroupService(mongoConnector);
});

builder.Services.AddSingleton<ReportService>(sp =>
{
    var taskService = sp.GetRequiredService<FireCalendarTaskService>();
    var groupService = sp.GetRequiredService<FireTaskGroupService>();
    return new ReportService(taskService, groupService);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowLocalhost");
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Configure static files
var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".apk"] = "application/vnd.android.package-archive";

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/staticfiles",
    ContentTypeProvider = contentTypeProvider
});

app.Logger.LogDebug("Application started");
app.Run();

public partial class Program { } // make public
