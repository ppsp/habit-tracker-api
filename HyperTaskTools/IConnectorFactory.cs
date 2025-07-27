using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HyperTaskTools
{
    public interface IConnectorFactory
    {
        Task<ApplicationInsightsConnector> CreateApplicationInsightsConnectorAsync(IServiceProvider serviceProvider);
        Task<FirebaseConnector> CreateFirebaseConnectorAsync(IServiceProvider serviceProvider);
        Task<MongoConnector> CreateMongoConnectorAsync(IServiceProvider serviceProvider);
        Task<AzureDevopsConnector> CreateAzureDevopsConnectorAsync(IServiceProvider serviceProvider);
    }

    public class ConnectorFactory : IConnectorFactory
    {
        private readonly string _vaultInstrumentationKeySecretName;
        private readonly string _vaultFirebaseSecretName;
        private readonly string _vaultMongoConnectionSecretName;
        private readonly string _personnalAccessTokenSecretName;
        private readonly string _azureDevopsUri;
        private readonly string _azureDevopsProjectName;

        public ConnectorFactory(
            string vaultInstrumentationKeySecretName,
            string vaultFirebaseSecretName,
            string vaultMongoConnectionSecretName,
            string personnalAccessTokenSecretName,
            string azureDevopsUri,
            string azureDevopsProjectName)
        {
            _vaultInstrumentationKeySecretName = vaultInstrumentationKeySecretName ?? throw new ArgumentNullException(nameof(vaultInstrumentationKeySecretName));
            _vaultFirebaseSecretName = vaultFirebaseSecretName ?? throw new ArgumentNullException(nameof(vaultFirebaseSecretName));
            _vaultMongoConnectionSecretName = vaultMongoConnectionSecretName ?? throw new ArgumentNullException(nameof(vaultMongoConnectionSecretName));
            _personnalAccessTokenSecretName = personnalAccessTokenSecretName ?? throw new ArgumentNullException(nameof(personnalAccessTokenSecretName));
            _azureDevopsUri = azureDevopsUri ?? throw new ArgumentNullException(nameof(azureDevopsUri));
            _azureDevopsProjectName = azureDevopsProjectName ?? throw new ArgumentNullException(nameof(azureDevopsProjectName));
        }

        public async Task<ApplicationInsightsConnector> CreateApplicationInsightsConnectorAsync(IServiceProvider serviceProvider)
        {
            var azureVault = serviceProvider.GetService<AzureVaultConnector>();
            var instrumentationKey = await azureVault.GetSecretValueStringAsync(_vaultInstrumentationKeySecretName);
            return new ApplicationInsightsConnector(instrumentationKey);
        }

        public async Task<FirebaseConnector> CreateFirebaseConnectorAsync(IServiceProvider serviceProvider)
        {
            var azureVault = serviceProvider.GetService<AzureVaultConnector>();
            var firebaseSecretJson = await azureVault.GetSecretValueStringAsync(_vaultFirebaseSecretName);
            return new FirebaseConnector(firebaseSecretJson);
        }

        public async Task<MongoConnector> CreateMongoConnectorAsync(IServiceProvider serviceProvider)
        {
            var azureVault = serviceProvider.GetService<AzureVaultConnector>();
            var mongoConnectionString = await azureVault.GetSecretValueStringAsync(_vaultMongoConnectionSecretName);
            return new MongoConnector(mongoConnectionString);
        }

        public async Task<AzureDevopsConnector> CreateAzureDevopsConnectorAsync(IServiceProvider serviceProvider)
        {
            var azureVault = serviceProvider.GetService<AzureVaultConnector>();
            var personnalAccessToken = await azureVault.GetSecretValueStringAsync(_personnalAccessTokenSecretName);
            return new AzureDevopsConnector(_azureDevopsUri, personnalAccessToken, _azureDevopsProjectName);
        }
    }
}